using System;
using System.Threading.Tasks;
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
    private readonly IRepaymentRepository repaymentRepository;

    public ManagedWalletCallbackHandler(IPaymentReferenceRepository paymentReferenceRepository, IWalletRepository walletRepository, IEmailService emailService, IUserRepository userRepository, ILoanRequestRepository loanRequestRepository, ILoanRepository loanRepository, ILoanOfferRepository loanOfferRepository, IRepaymentRepository repaymentRepository)
    {
        this.paymentReferenceRepository = paymentReferenceRepository;
        this.walletRepository = walletRepository;
        this.emailService = emailService;
        this.userRepository = userRepository;
        this.loanRequestRepository = loanRequestRepository;
        this.loanRepository = loanRepository;
        this.loanOfferRepository = loanOfferRepository;
        this.repaymentRepository = repaymentRepository;
    }
    public void Subscribe(IMonnifyService processor)
    {
        processor.CollectionProcessingCompleted += OnCollectionProcessingCompleted;
        processor.DisbursementProcessingCompleted += OnDisbursementProcessingCompleted;
    }

    private async Task OnCollectionProcessingCompleted(object sender, ManagedWalletCallbackDto<ManagedWalletCollectionCallbackData> e)
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
            await handleDeposit(wallet, user, e.EventData);
        }
        else
        {
            switch (paymentReference.paymentReferenceType)
            {
                case PaymentReferenceType.LoanRequest:
                    await handleLoanCollected(paymentReference, e.EventData.TransactionReference);
                    break;
                case PaymentReferenceType.Repayment:
                    await handleRepaymentCollected(paymentReference);
                    break;
            }
        }
    }

    private async Task OnDisbursementProcessingCompleted(object sender, ManagedWalletCallbackDto<ManagedWalletDisbursementCallbackData> e)
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
                var repayment = await repaymentRepository.FindById(paymentReference.ResourceId);
                user = await userRepository.GetByIdAsync(repayment.UserId);
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
                    await handleFailedLoanDisbursal(paymentReference, e.EventData);
                    break;
                case PaymentReferenceType.Repayment:
                    await handleFailedRepaymentDisbursal(paymentReference, e.EventData);
                    break;
            }
        }
        else if (e.EventType == "REVERSED_DISBURSEMENT")
        {
            await handleDisbursementReversal(e.EventData, user);
        }
        else
        {
            switch (paymentReference.paymentReferenceType)
            {
                case PaymentReferenceType.Withdrawal:
                    await handleWithdrawal(paymentReference, e.EventData);
                    break;
                case PaymentReferenceType.LoanRequest:
                    await handleLoanDisbursed(paymentReference);
                    break;
                case PaymentReferenceType.Repayment:
                    await handleRepaymentDisbursed(paymentReference);
                    break;
            }
        }
    }

    private async Task handleRepaymentDisbursed(PaymentReference paymentReference)
    {
        var repayment = await repaymentRepository.FindById(paymentReference.ResourceId);
        var user = await userRepository.GetByIdAsync(repayment.UserId);

        await emailService.SendHtmlEmailAsync(user.Email, "Transaction Notification", "RepaymentDisbursed", new { repayment.Amount, TransactionReference = paymentReference.Reference });
    }

    private async Task handleRepaymentCollected(PaymentReference paymentReference)
    {
        using (var transaction = await loanRepository.BeginTransactionAsync())
        {
            try
            {
                var repayment = await repaymentRepository.FindById(paymentReference.ResourceId);
                repayment.Status = RepaymentStatus.success;

                repaymentRepository.MarkAsModified(repayment);
                await repaymentRepository.SaveChangesAsync();

                var loan = await loanRepository.FindById(repayment.LoanId);

                loan.AmountLeft -= repayment.Amount;

                if (loan.AmountLeft <= 0)
                {
                    loan.Status = LoanStatus.Completed;
                }

                loanRepository.MarkAsModified(loan);
                await loanRepository.SaveChangesAsync();

                var user = await userRepository.GetByIdAsync(repayment.UserId);

                try { await emailService.SendHtmlEmailAsync(user.Email, "Transaction Notification", "RepaymentReceived", new { repayment.Amount, TransactionReference = paymentReference.Reference }); }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    // throw new Exception("Failed to send email");
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine(ex);
                throw new Exception("Failed to save repayment and update loan");
            }
        }
    }

    private async Task handleLoanDisbursed(PaymentReference paymentReference)
    {
        var loanRequest = await loanRequestRepository.FindById(paymentReference.ResourceId);

        await emailService.SendHtmlEmailAsync(loanRequest.LoanOffer.User.Email, "Transaction Notification", "LoanAcceptedAndDisbursedLender", new { loanRequest.LoanOffer.Amount, TransactionReference = paymentReference.Reference });
    }

    private async Task handleLoanCollected(PaymentReference paymentReference, string financialTransactionId)
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
            AmountLeft = loanRequest.LoanOffer.Amount + (loanRequest.LoanOffer.Amount * loanRequest.LoanOffer.InterestRate / 100),
            DueDate = DateTime.UtcNow.AddDays(loanRequest.LoanOffer.LoanDurationDays),
            InitialInterestRate = loanRequest.LoanOffer.InterestRate,
            CurrentInterestRate = loanRequest.LoanOffer.InterestRate,
            Status = LoanStatus.Active,
            RepaymentFrequency = loanRequest.LoanOffer.RepaymentFrequency,
            LoanDurationDays = loanRequest.LoanOffer.LoanDurationDays,
            Defaulted = false,
            PrincipalAmount = loanRequest.LoanOffer.Amount,
            AccruingInterestRate = loanRequest.LoanOffer.AccruingInterestRate,
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

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine(ex);
                throw new Exception("Failed to save loan and update loan request");
            }
        }
        await emailService.SendHtmlEmailAsync(loanRequest.LoanOffer.User.Email, "Transaction Notification", "LoanAcceptedAndDisbursedBorrower", new { loanRequest.LoanOffer.Amount, TransactionReference = paymentReference.Reference });
    }

    private async Task handleDeposit(Wallet wallet, User user, ManagedWalletCollectionCallbackData e)
    {
        await emailService.SendHtmlEmailAsync(user.Email, "Transaction Notification", "DepositSucceeded", new { Amount = e.SettlementAmount, TransactionReference = e.PaymentReference });
    }

    private async Task handleWithdrawal(PaymentReference paymentReference, ManagedWalletDisbursementCallbackData e)
    {
        var wallet = await walletRepository.FindById(paymentReference.ResourceId);
        var user = await userRepository.GetByIdAsync(wallet.UserId);
        await emailService.SendHtmlEmailAsync(user.Email, "Transaction Notification", "WithdrawalSucceeded", new { e.Amount, e.TransactionReference });
    }

    private async Task handleFailedLoanDisbursal(PaymentReference paymentReference, ManagedWalletDisbursementCallbackData e)
    {
        var loanRequest = await loanRequestRepository.FindById(paymentReference.ResourceId);
        loanRequest.Status = LoanRequestStatus.failed;
        loanRequestRepository.MarkAsModified(loanRequest);
        await loanRequestRepository.SaveChangesAsync();

        var user = await userRepository.GetByIdAsync(loanRequest.LoanOffer.Type == LoanOfferType.borrower ? loanRequest.UserId : loanRequest.LoanOffer.UserId);

        await emailService.SendHtmlEmailAsync(user.Email, "Transaction Notification", "FailedLoanDisbursal", new { e.Amount, e.TransactionReference });
    }

    private async Task handleFailedRepaymentDisbursal(PaymentReference paymentReference, ManagedWalletDisbursementCallbackData e)
    {
        var repayment = await repaymentRepository.FindById(paymentReference.ResourceId);
        repayment.Status = RepaymentStatus.failed;

        repaymentRepository.MarkAsModified(repayment);
        await repaymentRepository.SaveChangesAsync();

        var user = await userRepository.GetByIdAsync(repayment.UserId);

        await emailService.SendHtmlEmailAsync(user.Email, "Transaction Notification", "FailedRepaymentDisbursal", new { e.Amount, e.TransactionReference });
    }

    private async Task handleDisbursementReversal(ManagedWalletDisbursementCallbackData e, User user)
    {

        await emailService.SendHtmlEmailAsync(user.Email, "Transaction Notification", "FailedRepaymentDisbursal", new { e.Amount, e.TransactionReference });
    }
}
