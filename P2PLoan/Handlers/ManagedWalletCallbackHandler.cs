using System;
using P2PLoan.DTOs;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Handlers;

public class ManagedWalletCallbackHandler : IManagedWalletCallbackHandler
{
    private readonly IPaymentReferenceRepository paymentReferenceRepository;
    private readonly IWalletRepository walletRepository;
    private readonly IEmailService emailService;
    private readonly IUserRepository userRepository;
    private readonly ILoanRequestRepository loanRequestRepository;
    private readonly ILoanRepository loanRepository;
    private readonly ILoanOfferRepository loanOfferRepository;

    public ManagedWalletCallbackHandler(IPaymentReferenceRepository paymentReferenceRepository, IWalletRepository walletRepository, IEmailService emailService, IUserRepository userRepository, ILoanRequestRepository loanRequestRepository, ILoanRepository loanRepository, ILoanOfferRepository loanOfferRepository)
    {
        this.paymentReferenceRepository = paymentReferenceRepository;
        this.walletRepository = walletRepository;
        this.emailService = emailService;
        this.userRepository = userRepository;
        this.loanRequestRepository = loanRequestRepository;
        this.loanRepository = loanRepository;
        this.loanOfferRepository = loanOfferRepository;
    }
    public void Subscribe(IMonnifyService processor)
    {
        processor.CollectionProcessingCompleted += OnCollectionProcessingCompleted;
        processor.DisbursementProcessingCompleted += OnDisbursementProcessingCompleted;
    }

    private async void OnCollectionProcessingCompleted(object sender, ManagedWalletCallbackDto<ManagedWalletCollectionCallbackData> e)
    {
        // Check if the transaction is a reserved account transaction, if not return because we are only interested in reserved account transactions
        if (e.EventData.Product.Type != "RESERVED_ACCOUNT")
        {
            return;
        }

        // Get the managed wallet associated with the transaction
        var wallet = await walletRepository.FindByAccountNumber(e.EventData.Product.Reference);

        if (wallet == null)
        {
            throw new Exception("Wallet not found");
        }

        var user = await userRepository.GetByIdAsync(wallet.UserId);


        var paymentReference = await paymentReferenceRepository.FindByReferenceAsync(e.EventData.PaymentReference);
        if (paymentReference == null)
        {
            handleDeposit(wallet, user, e.EventData);
        }
        else
        {
            switch (paymentReference.paymentReferenceType)
            {
                case PaymentReferenceType.LoanRequest:
                    handleLoanCollected(paymentReference, e.EventData.TransactionReference);
                    break;
                case PaymentReferenceType.Repayment:
                    handleRepaymentCollected(paymentReference);
                    break;
            }
        }
    }

    private async void OnDisbursementProcessingCompleted(object sender, ManagedWalletCallbackDto<ManagedWalletDisbursementCallbackData> e)
    {
        var paymentReference = await paymentReferenceRepository.FindByReferenceAsync(e.EventData.Reference);

        if (paymentReference == null)
        {
            // Return because we are only interested in transactions with payment references, others were not initiated by us
            return;
        }

        User user;
        switch (paymentReference.paymentReferenceType)
        {
            case PaymentReferenceType.LoanRequest:
                var loanRequest = await loanRequestRepository.FindById(paymentReference.ResourceId);
                user = await userRepository.GetByIdAsync(loanRequest.UserId);
                break;
            case PaymentReferenceType.Repayment:
                var loan = await loanRepository.FindById(paymentReference.ResourceId);
                user = await userRepository.GetByIdAsync(loan.BorrowerId);
                break;
            case PaymentReferenceType.Withdrawal:
                var wallet = await walletRepository.FindById(paymentReference.ResourceId);
                user = await userRepository.GetByIdAsync(wallet.UserId);
                break;
            default:
                user = null;
                break;
        }

        if (e.EventType == "FAILED_DISBURSEMENT")
        {
            switch (paymentReference.paymentReferenceType)
            {
                case PaymentReferenceType.LoanRequest:
                    handleFailedLoanDisbursal(paymentReference, e.EventData);
                    break;
                case PaymentReferenceType.Repayment:
                    handleFailedRepaymentDisbursal(paymentReference, e.EventData);
                    break;
            }
        }
        else if (e.EventType == "REVERSED_DISBURSEMENT")
        {
            handleDisbursementReversal(paymentReference, e.EventData, user);
        }
        else
        {
            switch (paymentReference.paymentReferenceType)
            {
                case PaymentReferenceType.Withdrawal:
                    handleWithdrawal(paymentReference, e.EventData);
                    break;
                case PaymentReferenceType.LoanRequest:
                    handleLoanDisbursed(paymentReference);
                    break;
                case PaymentReferenceType.Repayment:
                    handleRepaymentDisbursed(paymentReference);
                    break;
            }
        }
        throw new NotImplementedException();
    }

    private void handleRepaymentDisbursed(PaymentReference paymentReference)
    {
        throw new NotImplementedException();
    }

    private void handleRepaymentCollected(PaymentReference paymentReference)
    {
        throw new NotImplementedException();
    }

    private async void handleLoanDisbursed(PaymentReference paymentReference)
    {
        var loanRequest = await loanRequestRepository.FindById(paymentReference.ResourceId);

        await emailService.SendHtmlEmailAsync(loanRequest.LoanOffer.User.Email, "Transaction Notification", "LoanAcceptedAndDisbursedLender", new { Amount = loanRequest.LoanOffer.Amount, TransactionReference = paymentReference.Reference });
    }

    private async void handleLoanCollected(PaymentReference paymentReference, string financialTransactionId)
    {
        var loanRequest = await loanRequestRepository.FindById(paymentReference.ResourceId);

        // Get system user
        var systemUser = await userRepository.GetSystemUser();

        // Create a loan on successful collection
        var loan = new Loan
        {
            Id = Guid.NewGuid(),
            BorrowerId = loanRequest.UserId,
            LenderId = loanRequest.LoanOffer.UserId,
            LoanOfferId = loanRequest.LoanOfferId,
            LoanRequestId = loanRequest.Id,
            AmountLeft = loanRequest.LoanOffer.Amount,
            DueDate = DateTime.UtcNow.AddDays(loanRequest.LoanOffer.LoanDurationDays),
            InitialInterestRate = loanRequest.LoanOffer.InterestRate,
            Status = LoanStatus.Active,
            RepaymentFrequency = loanRequest.LoanOffer.RepaymentFrequency,
            LoanDurationDays = loanRequest.LoanOffer.LoanDurationDays,
            Defaulted = false,
            PrincipalAmount = loanRequest.LoanOffer.Amount,
            AccruingInterestRate = loanRequest.LoanOffer.InterestRate,
            FinancialTransactionId = financialTransactionId,
            CreatedBy = systemUser,
            ModifiedBy = systemUser
        };

        using (var transaction = await loanRepository.BeginTransactionAsync())
        {
            try
            {
                loanRepository.Add(loan);

                var saveResult = await loanRepository.SaveChangesAsync();

                if (!saveResult)
                {
                    throw new Exception("Failed to save loan");
                }

                // Update the loan request status to approved
                loanRequest.Status = LoanRequestStatus.approved;
                loanRequestRepository.MarkAsModified(loanRequest);

                await loanRequestRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine(ex);
                throw new Exception("Failed to save loan and update loan request");
            }
        }
        await emailService.SendHtmlEmailAsync(loanRequest.LoanOffer.User.Email, "Transaction Notification", "LoanAcceptedAndDisbursedBorrower", new { loanRequest.LoanOffer.Amount, TransactionReference = paymentReference.Reference });
        throw new NotImplementedException();
    }

    private async void handleDeposit(Wallet wallet, User user, ManagedWalletCollectionCallbackData e)
    {
        await emailService.SendHtmlEmailAsync(user.Email, "Transaction Notification", "DepositSucceeded", new { Amount = e.SettlementAmount, TransactionReference = e.PaymentReference });
    }

    private async void handleWithdrawal(PaymentReference paymentReference, ManagedWalletDisbursementCallbackData e)
    {
        var wallet = await walletRepository.FindById(paymentReference.ResourceId);
        var user = await userRepository.GetByIdAsync(wallet.UserId);
        await emailService.SendHtmlEmailAsync(user.Email, "Transaction Notification", "WithdrawalSucceeded", new { e.Amount, e.TransactionReference });
    }

    private async void handleFailedLoanDisbursal(PaymentReference paymentReference, ManagedWalletDisbursementCallbackData e)
    {
        var loanRequest = await loanRequestRepository.FindById(paymentReference.ResourceId);
        var user = await userRepository.GetByIdAsync(loanRequest.LoanOffer.Type == LoanOfferType.borrower ? loanRequest.UserId : loanRequest.LoanOffer.UserId);

        await emailService.SendHtmlEmailAsync(user.Email, "Transaction Notification", "WithdrawalSucceeded", new { e.Amount, e.TransactionReference });
    }

    private async void handleFailedRepaymentDisbursal(PaymentReference paymentReference, ManagedWalletDisbursementCallbackData e)
    {
        var loan = await loanRepository.FindById(paymentReference.ResourceId);
        var user = await userRepository.GetByIdAsync(loan.BorrowerId);

        await emailService.SendHtmlEmailAsync(user.Email, "Transaction Notification", "FailedRepaymentDisbursal", new { e.Amount, e.TransactionReference });
    }

    private async void handleDisbursementReversal(PaymentReference paymentReference, ManagedWalletDisbursementCallbackData e, User user)
    {

        await emailService.SendHtmlEmailAsync(user.Email, "Transaction Notification", "FailedRepaymentDisbursal", new { e.Amount, e.TransactionReference });
    }
}
