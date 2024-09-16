using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using P2PLoan.Constants;
using P2PLoan.DTOs;
using P2PLoan.DTOs.SearchParams;
using P2PLoan.Helpers;
using P2PLoan.Interfaces;
using P2PLoan.Models;
using P2PLoan.Utils;

namespace P2PLoan.Services;

public class LoanRequestService : ILoanRequestService
{
    private readonly ILoanRequestRepository loanRequestRepository;
    private readonly IWalletTopUpDetailRepository walletTopUpDetailRepository;
    private readonly ILoanOfferRepository loanOfferRepository;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;
    private readonly IWalletRepository walletRepository;
    private readonly IWalletService walletService;
    private readonly IPaymentReferenceRepository paymentReferenceRepository;
    private readonly ILoanRepository loanRepository;

    public LoanRequestService(ILoanRequestRepository loanRequestRepository, IWalletTopUpDetailRepository walletTopUpDetailRepository, ILoanOfferRepository loanOfferRepository, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IMapper mapper, IWalletRepository walletRepository, IWalletService walletService, IPaymentReferenceRepository paymentReferenceRepository, ILoanRepository loanRepository)
    {
        this.loanRequestRepository = loanRequestRepository;
        this.walletTopUpDetailRepository = walletTopUpDetailRepository;
        this.loanOfferRepository = loanOfferRepository;
        this.httpContextAccessor = httpContextAccessor;
        this.userRepository = userRepository;
        this.mapper = mapper;
        this.walletRepository = walletRepository;
        this.walletService = walletService;
        this.paymentReferenceRepository = paymentReferenceRepository;
        this.loanRepository = loanRepository;
    }
    public async Task<ServiceResponse<object>> Create(CreateLoanRequestRequestDto createLoanRequestDto)
    {
        var userId = httpContextAccessor.HttpContext.User.GetLoggedInUserId();

        var user = await userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "User does not exist", null);
        }

        var wallet = await walletRepository.FindById(createLoanRequestDto.WalletId);

        if (wallet is null || wallet.UserId != userId)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Invalid wallet", null);
        }


        var loanOffer = await loanOfferRepository.FindById(createLoanRequestDto.LoanOfferId);
        if (loanOffer is null || loanOffer.UserId == userId || !loanOffer.Active)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Loan offer does not exist", null);
        }

        var isUserAllowedToSendRequest = user.UserType == UserType.borrower && loanOffer.Type == LoanOfferType.lender || user.UserType == UserType.lender && loanOffer.Type == LoanOfferType.borrower;

        if (!isUserAllowedToSendRequest)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.InvalidOperation, "You are not allowed to send a loan request for this loan offer", null);
        }

        var existingLoanRequest = await loanRequestRepository.FindByLoanOfferIdForAUser(createLoanRequestDto.LoanOfferId, user.Id);

        if (existingLoanRequest != null && existingLoanRequest.Status != LoanRequestStatus.approved && existingLoanRequest.Status != LoanRequestStatus.declined)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.AlreadyExists, "You already have a loan request for this loan offer", null);
        }

        var uncompletedLoan = await loanRepository.GetUserActiveLoan(userId);

        if (uncompletedLoan != null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.AlreadyExists, "You have an active loan", null);
        }

        var loanRequest = mapper.Map<LoanRequest>(createLoanRequestDto);

        loanRequest.UserId = userId;
        loanRequest.CreatedById = userId;
        loanRequest.ModifiedById = userId;

        loanRequestRepository.Add(loanRequest);

        await loanRequestRepository.SaveChangesAsync();

        return new ServiceResponse<object>(ResponseStatus.Created, AppStatusCodes.Success, "Loan request created successfully", null);

    }

    public async Task<ServiceResponse<object>> GetAll(LoanRequestSearchParams searchParams)
    {
        var loanRequests = await loanRequestRepository.GetAllAsync(searchParams);

        return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Loan requests fetched successfully", loanRequests);
    }

    public async Task<ServiceResponse<object>> GetAllForLoggedInUser(LoanRequestSearchParams searchParams)
    {
        var userId = httpContextAccessor.HttpContext.User.GetLoggedInUserId();

        var user = await userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "User does not exist", null);
        }

        var loanRequests = await loanRequestRepository.GetAllAsync(searchParams, userId);

        return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Loan requests fetched successfully", loanRequests);

    }

    public async Task<ServiceResponse<object>> GetOne(Guid loanRequestId)
    {
        var loanRequest = await loanRequestRepository.FindById(loanRequestId);

        if (loanRequest is null || loanRequest.UserId != httpContextAccessor.HttpContext.User.GetLoggedInUserId())
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Loan request does not exist", null);
        }

        return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Loan request fetched successfully", loanRequest);
    }
    public async Task<ServiceResponse<object>> GetOneAdmin(Guid loanRequestId)
    {
        var loanRequest = await loanRequestRepository.FindById(loanRequestId);

        if (loanRequest is null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Loan request does not exist", null);
        }

        return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Loan request fetched successfully", loanRequest);
    }
    public async Task<ServiceResponse<object>> Delete(Guid loanRequestId)
    {
        var loanRequest = await loanRequestRepository.FindById(loanRequestId);

        if (loanRequest is null || loanRequest.UserId != httpContextAccessor.HttpContext.User.GetLoggedInUserId())
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Loan request does not exist", null);
        }

        if (loanRequest.Status != LoanRequestStatus.pending && loanRequest.Status != LoanRequestStatus.declined && loanRequest.Status != LoanRequestStatus.failed)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Loan request cannot be deleted", null);

        }

        loanRequestRepository.Remove(loanRequest);

        var saveResult = await loanRequestRepository.SaveChangesAsync();

        if (!saveResult)
        {
            throw new Exception();
        }

        return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Loan request deleted successfully", loanRequest);
    }
    public async Task<ServiceResponse<object>> DeleteAdmin(Guid loanRequestId)
    {
        var loanRequest = await loanRequestRepository.FindById(loanRequestId);

        if (loanRequest is null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Loan request does not exist", null);
        }

        if (loanRequest.Status != LoanRequestStatus.pending && loanRequest.Status != LoanRequestStatus.declined && loanRequest.Status != LoanRequestStatus.failed)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Loan request cannot be deleted", null);

        }

        loanRequestRepository.Remove(loanRequest);

        var saveResult = await loanRequestRepository.SaveChangesAsync();

        if (!saveResult)
        {
            throw new Exception();
        }

        return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Loan request deleted successfully", loanRequest);
    }

    public async Task<ServiceResponse<object>> Accept(Guid loanRequestId, AcceptLoanRequestDto acceptLoanRequestDto)
    {
        var userId = httpContextAccessor.HttpContext.User.GetLoggedInUserId();

        var user = await userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "User does not exist", null);
        }

        if (!Utilities.VerifyPassword(acceptLoanRequestDto.PIN, user.PIN))
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.InvalidOperation, "Invalid PIN", null);
        }

        var loanRequest = await loanRequestRepository.FindById(loanRequestId);

        if (loanRequest is null || loanRequest.LoanOffer.UserId != userId)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Loan request does not exist", null);
        }

        if (loanRequest.Status != LoanRequestStatus.pending & loanRequest.Status != LoanRequestStatus.failed)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Loan request cannot be accepted", null);
        }

        loanRequest.Status = LoanRequestStatus.processing;

        loanRequest.ProcessingStartTime = DateTime.UtcNow;

        //Check if the lender has enough balance to fund the loan request
        Wallet lenderWallet;

        if (loanRequest.LoanOffer.Type == LoanOfferType.borrower)
        {
            lenderWallet = await walletRepository.FindById(loanRequest.WalletId);
        }
        else
        {
            lenderWallet = await walletRepository.FindById(loanRequest.LoanOffer.WalletId);
        }

        if (lenderWallet is null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Lender wallet does not exist", null);
        }

        var lenderWalletBalance = await walletService.GetBalance(lenderWallet.Id);

        if (lenderWalletBalance.AvailableBalance < loanRequest.LoanOffer.Amount)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.InsufficientFunds, "Lender does not have enough balance to fund the loan request at the moment", null);
        }

        // Get system user
        var systemUser = await userRepository.GetSystemUser();

        // Start a transaction
        using (var transaction = await loanRequestRepository.BeginTransactionAsync())
        {
            try
            {
                //Create new payment reference to track the loan request
                var paymentReferenceId = Guid.NewGuid();
                var paymentReference = new PaymentReference
                {
                    Id = paymentReferenceId,
                    ResourceId = loanRequest.Id,
                    paymentReferenceType = PaymentReferenceType.LoanRequest,
                    Reference = paymentReferenceId.ToString(),
                    CreatedBy = systemUser,
                    ModifiedBy = systemUser
                };

                paymentReferenceRepository.Add(paymentReference);

                Console.WriteLine("Payment Reference Created");

                await paymentReferenceRepository.SaveChangesAsync();
                Console.WriteLine("Payment Reference Saved");
                var topUpDetails = await walletTopUpDetailRepository.GetAllForAWallet(loanRequest.Wallet.Id);
                Console.WriteLine("Top Up Details Fetched");

                var topUpDetailsJson = JsonConvert.SerializeObject(topUpDetails);
                Console.WriteLine(topUpDetailsJson);

                var topUpAccountDetail = loanRequest.Wallet.TopUpDetails.ToList()[0];
                Console.WriteLine("Top Up Account Detail Fetched");
                var topUpDetailJson = JsonConvert.SerializeObject(topUpDetails);
                Console.WriteLine(topUpDetailJson);

                // Try debiting the lender's wallet
                var transferDto = new TransferDto
                {
                    Amount = loanRequest.LoanOffer.Amount,
                    Reference = paymentReference.Id.ToString(),
                    Narration = $"Loan request {loanRequest.Id} approval",
                    DestinationBankCode = topUpAccountDetail.BankCode,
                    DestinationAccountNumber = topUpAccountDetail.AccountNumber,
                    SourceAccountNumber = loanRequest.LoanOffer.Wallet.AccountNumber,
                };

                Console.WriteLine("Transfer DTO Created");
                var transferDtoJson = JsonConvert.SerializeObject(transferDto);
                Console.WriteLine(transferDtoJson);

                try
                {
                    var transferResponse = await walletService.Transfer(transferDto, loanRequest.LoanOffer.Wallet);
                    Console.WriteLine("Transfer Response Received");
                    var transferResponseJson = JsonConvert.SerializeObject(transferResponse);
                    Console.WriteLine(transferResponseJson);
                }
                catch
                {
                    Console.WriteLine("Failed to debit lender's wallet");
                    transaction.Rollback();
                    Console.WriteLine("Transaction Rolled Back");

                    return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.InternalServerError, "Failed to debit lender's wallet", null);
                }

                // var transferResponse = await Transfer
                loanRequestRepository.MarkAsModified(loanRequest);
                Console.WriteLine("Loan Request Status Updated");

                var saveResult = await loanRequestRepository.SaveChangesAsync();
                Console.WriteLine("Loan Request Saved");

                if (!saveResult)
                {
                    throw new Exception();
                }

                await transaction.CommitAsync();
                Console.WriteLine("Transaction Committed");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new ServiceResponse<object>(ResponseStatus.Error, AppStatusCodes.InternalServerError, "Failed to accept loan request", null);
            }
        }

        return new ServiceResponse<object>(ResponseStatus.Accepted, AppStatusCodes.Success, "Your transaction is being processed, you'll be notified when the transaction goes through", null);
    }

    public async Task<ServiceResponse<object>> Decline(Guid loanRequestId)
    {
        var userId = httpContextAccessor.HttpContext.User.GetLoggedInUserId();

        var loanRequest = await loanRequestRepository.FindById(loanRequestId);

        if (loanRequest is null || loanRequest.LoanOffer.UserId != userId)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Loan request does not exist", null);
        }

        if (loanRequest.Status != LoanRequestStatus.pending && loanRequest.Status != LoanRequestStatus.failed)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Loan request cannot be declined", null);
        }

        loanRequest.Status = LoanRequestStatus.declined;

        loanRequestRepository.MarkAsModified(loanRequest);

        var saveResult = await loanRequestRepository.SaveChangesAsync();

        if (!saveResult)
        {
            throw new Exception();
        }

        return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Loan request has been declined successfully", null);
    }
}
