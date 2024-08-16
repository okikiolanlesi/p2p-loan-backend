using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using P2PLoan.Constants;
using P2PLoan.DTOs;
using P2PLoan.DTOs.SearchParams;
using P2PLoan.Helpers;
using P2PLoan.Interfaces;
using P2PLoan.Interfaces.Services;
using P2PLoan.Models;

namespace P2PLoan.Services;

public class LoanRequestService : ILoanRequestService
{
    private readonly ILoanRequestRepository loanRequestRepository;
    private readonly ILoanOfferRepository loanOfferRepository;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;
    private readonly IWalletRepository walletRepository;

    public LoanRequestService(ILoanRequestRepository loanRequestRepository, ILoanOfferRepository loanOfferRepository, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IMapper mapper, IWalletRepository walletRepository)
    {
        this.loanRequestRepository = loanRequestRepository;
        this.loanOfferRepository = loanOfferRepository;
        this.httpContextAccessor = httpContextAccessor;
        this.userRepository = userRepository;
        this.mapper = mapper;
        this.walletRepository = walletRepository;
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

    public async Task<ServiceResponse<object>> Accept(Guid loanRequestId)
    {
        var userId = httpContextAccessor.HttpContext.User.GetLoggedInUserId();

        var loanRequest = await loanRequestRepository.FindById(loanRequestId);

        if (loanRequest is null || loanRequest.LoanOffer.UserId != userId)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Loan request does not exist", null);
        }

        if (loanRequest.Status != LoanRequestStatus.pending)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Loan request cannot be accepted", null);
        }

        loanRequest.Status = LoanRequestStatus.processing;

        loanRequest.ProcessingStartTime = DateTime.UtcNow;

        // Try debiting the lender's wallet
        // var debitResult = await walletRepository.Debit(loanRequest.LoanOffer.UserId, loanRequest.Amount);

        loanRequestRepository.MarkAsModified(loanRequest);

        var saveResult = await loanRequestRepository.SaveChangesAsync();

        if (!saveResult)
        {
            throw new Exception();
        }

        return new ServiceResponse<object>(ResponseStatus.Processing, AppStatusCodes.Success, "Your transaction is being processed, you'll be notified when the transaction goes through", null);

    }

    public Task<ServiceResponse<object>> Decline(Guid loanRequestId)
    {
        throw new NotImplementedException();
    }
}
