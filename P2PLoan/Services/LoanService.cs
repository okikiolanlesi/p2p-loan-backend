using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using P2PLoan.Constants;
using P2PLoan.DTOs;
using P2PLoan.DTOs.SearchParams;
using P2PLoan.Helpers;
using P2PLoan.Interfaces;
using P2PLoan.Models;
using P2PLoan.Utils;

namespace P2PLoan.Services;

public class LoanService : ILoanService
{
    private readonly ILoanRepository loanRepository;
    private readonly IRepaymentRepository repaymentRepository;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IUserRepository userRepository;
    private readonly ILoanOfferRepository loanOfferRepository;
    private readonly IWalletService walletService;
    private readonly IWalletRepository walletRepository;
    private readonly ILoanRequestRepository loanRequestRepository;
    private readonly IPaymentReferenceRepository paymentReferenceRepository;
    private readonly IWalletTopUpDetailRepository walletTopUpDetailRepository;

    public LoanService(ILoanRepository loanRepository, IRepaymentRepository repaymentRepository, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, ILoanOfferRepository loanOfferRepository, IWalletService walletService, IWalletRepository walletRepository, ILoanRequestRepository loanRequestRepository, IPaymentReferenceRepository paymentReferenceRepository, IWalletTopUpDetailRepository walletTopUpDetailRepository)
    {
        this.loanRepository = loanRepository;
        this.repaymentRepository = repaymentRepository;
        this.httpContextAccessor = httpContextAccessor;
        this.userRepository = userRepository;
        this.loanOfferRepository = loanOfferRepository;
        this.walletService = walletService;
        this.walletRepository = walletRepository;
        this.loanRequestRepository = loanRequestRepository;
        this.paymentReferenceRepository = paymentReferenceRepository;
        this.walletTopUpDetailRepository = walletTopUpDetailRepository;
    }
    public async Task<ServiceResponse<object>> GetActiveLoan()
    {
        var userId = httpContextAccessor.HttpContext.User.GetLoggedInUserId();

        var user = await userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "User does not exist", null);
        }
        var activeLoan = await loanRepository.GetUserActiveLoan(userId);

        if (activeLoan is null)
        {
            return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "No active loan found", null);
        }

        return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Active loan found", activeLoan);
    }

    public async Task<ServiceResponse<object>> GetALoan(Guid loanId)
    {
        var userId = httpContextAccessor.HttpContext.User.GetLoggedInUserId();

        var user = await userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "User does not exist", null);
        }

        var loan = await loanRepository.FindById(loanId);

        if (loan is null || loan.BorrowerId != userId || loan.LenderId != userId)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Loan does not exist", null);
        }

        return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Loan found", loan);
    }

    public async Task<ServiceResponse<object>> GetALoanRepayment(Guid repaymentId)
    {
        var userId = httpContextAccessor.HttpContext.User.GetLoggedInUserId();

        var user = await userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "User does not exist", null);
        }

        var repayment = await repaymentRepository.FindById(repaymentId);

        if (repayment is null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Repayment does not exist", null);
        }

        var loan = await loanRepository.FindById(repayment.LoanId);

        if (loan.BorrowerId != userId && loan.LenderId != userId)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Repayment does not exist", null);
        }

        return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Repayment found", repayment);
    }

    public async Task<ServiceResponse<object>> GetLoanRepayments(Guid loanId, RepaymentSearchParams repaymentSearchParams)
    {
        var userId = httpContextAccessor.HttpContext.User.GetLoggedInUserId();

        var user = await userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "User does not exist", null);
        }

        var loan = await loanRepository.FindById(loanId);

        if (loan is null || (loan.BorrowerId != userId && loan.LenderId != userId))
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Loan does not exist", null);
        }

        // Set the loanId in the search params so that we can filter repayments by loanId
        repaymentSearchParams.LoanId = loanId;

        var repayments = await repaymentRepository.GetAllAsync(repaymentSearchParams);

        return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Repayments fetched successfully", repayments);
    }

    public async Task<ServiceResponse<object>> GetMyLoans(LoanSearchParams loanSearchParams)
    {
        var userId = httpContextAccessor.HttpContext.User.GetLoggedInUserId();

        var user = await userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "User does not exist", null);
        }

        var loans = await loanRepository.GetAllAsync(loanSearchParams, userId);

        return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Loans fetched successfully", loans);
    }

    public async Task<ServiceResponse<object>> RepayLoan(RepayLoanRequestDto repayDto)
    {
        var userId = httpContextAccessor.HttpContext.User.GetLoggedInUserId();

        var user = await userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "User does not exist", null);
        }

        if (!Utilities.VerifyPassword(repayDto.PIN, user.PIN))
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.InvalidOperation, "Invalid PIN", null);
        }

        var loan = await loanRepository.FindById(repayDto.LoanId);

        if (loan is null || loan.BorrowerId != userId)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Loan does not exist", null);
        }

        if (loan.Status == LoanStatus.Completed)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Loan is not active", null);
        }

        if (repayDto.Amount <= 0)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.InvalidData, "Invalid repayment amount", null);
        }

        if (repayDto.Amount > loan.AmountLeft)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.InvalidData, "Repayment amount exceeds loan amount left", null);
        }

        var pendingRepayments = await repaymentRepository.GetPendingRepaymentsForALoan(loan.Id);

        if (pendingRepayments.Count > 0)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.InvalidOperation, "You have pending repayments", null);
        }

        using (var transaction = await loanRepository.BeginTransactionAsync())
        {
            try
            {

                // Get the system user
                var systemUser = await userRepository.GetSystemUser();

                var repaymentId = Guid.NewGuid();

                //Create new payment reference to track the loan repayment
                var paymentReferenceId = Guid.NewGuid();
                var paymentReference = new PaymentReference
                {
                    Id = paymentReferenceId,
                    ResourceId = repaymentId,
                    paymentReferenceType = PaymentReferenceType.Repayment,
                    Reference = paymentReferenceId.ToString(),
                    CreatedBy = systemUser,
                    ModifiedBy = systemUser
                };

                paymentReferenceRepository.Add(paymentReference);

                await paymentReferenceRepository.SaveChangesAsync();

                // Get the wallet of the borrower and lender depending on the loan offer type
                Wallet borrowerWallet;
                Wallet lenderWallet;

                var loanOffer = await loanOfferRepository.FindById(loan.LoanOfferId);
                var loanRequest = await loanRequestRepository.FindById(loan.LoanRequestId);

                if (loanOffer.Type == LoanOfferType.borrower)
                {
                    // The borrower made the offer and the lender sent a request
                    lenderWallet = await walletRepository.FindById(loanRequest.WalletId);
                    borrowerWallet = await walletRepository.FindById(loanOffer.WalletId);
                }
                else
                {
                    // The lender made the offer and the borrower sent a request
                    lenderWallet = await walletRepository.FindById(loanOffer.WalletId);
                    borrowerWallet = await walletRepository.FindById(loanRequest.WalletId);
                }

                if (borrowerWallet is null)
                {
                    return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Wallet not found", null);
                }

                if (lenderWallet is null)
                {
                    return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Lender wallet not found", null);
                }

                // Check if the borrower has enough funds in their wallet
                var borrowerWalletBalance = await walletService.GetBalance(borrowerWallet.Id);

                if (borrowerWalletBalance.AvailableBalance < repayDto.Amount)
                {
                    return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.InsufficientFunds, "You do not have enough funds to make the repayment", null);
                }

                //Get the lender's top up account details
                var lenderTopUpAccountDetails = await walletTopUpDetailRepository.GetAllForAWallet(lenderWallet.Id);

                if (lenderTopUpAccountDetails is null || lenderTopUpAccountDetails.Count == 0)
                {
                    return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Lender Top up account details not found", null);
                }

                var lenderTopUpAccountDetail = lenderTopUpAccountDetails.FirstOrDefault();

                // Try to transfer the funds from the  wallet
                var transferDto = new TransferDto
                {
                    Amount = repayDto.Amount,
                    Reference = paymentReference.Id.ToString(),
                    Narration = $"Loan {loan.Id} repayment with reference {paymentReference.Id} and id {repaymentId}",
                    DestinationBankCode = lenderTopUpAccountDetail.BankCode,
                    DestinationAccountNumber = lenderTopUpAccountDetail.AccountNumber,
                    SourceAccountNumber = borrowerWallet.AccountNumber,
                };

                try
                {
                    var transferResponse = await walletService.Transfer(transferDto, borrowerWallet);
                }
                catch
                {
                    return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.InternalServerError, "Failed to debit your  wallet", null);
                }

                // Create a new repayment
                var repayment = new Repayment
                {
                    Id = repaymentId,
                    UserId = userId,
                    LoanId = repayDto.LoanId,
                    Amount = repayDto.Amount,
                    FinancialTransactionId = null,
                    InterestRate = loan.CurrentInterestRate,
                    Status = RepaymentStatus.pending,
                    CreatedBy = user,
                    ModifiedBy = user
                };

                repaymentRepository.Add(repayment);

                var saveResult = await repaymentRepository.SaveChangesAsync();

                if (!saveResult)
                {
                    return new ServiceResponse<object>(ResponseStatus.Error, AppStatusCodes.InternalServerError, "Error saving repayment", null);
                }

                await transaction.CommitAsync();
                return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Repayment saved successfully", repayment);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }


}
