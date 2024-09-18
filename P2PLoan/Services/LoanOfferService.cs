
using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using P2PLoan.Constants;
using P2PLoan.DTOs.SearchParams;
using P2PLoan.Helpers;
using P2PLoan.Interfaces;
using P2PLoan.Models;
using P2PLoan.Repositories;

namespace P2PLoan.Services;

public class LoanOfferService : ILoanOfferService
{
    private readonly ILoanOfferRepository loanOfferRepository;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;
    private readonly IWalletRepository walletRepository;
    private readonly IConstants constants;

    public LoanOfferService(ILoanOfferRepository loanOfferRepository, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IMapper mapper, IWalletRepository walletRepository, IConstants constants)
    {
        this.loanOfferRepository = loanOfferRepository;
        this.httpContextAccessor = httpContextAccessor;
        this.userRepository = userRepository;
        this.mapper = mapper;
        this.walletRepository = walletRepository;
        this.constants = constants;
    }

    public async Task<ServiceResponse<object>> Create(CreateLoanOfferRequestDto createLoanOfferRequestDto)
    {
        var userId = httpContextAccessor.HttpContext.User.GetLoggedInUserId();

        var user = await userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "User does not exist", null);
        }

        if (createLoanOfferRequestDto.Amount > constants.MAX_LOAN_AMOUNT)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ValidationError, $"Loan amount cannot exceed {constants.MAX_LOAN_AMOUNT}", null);
        }

        if (createLoanOfferRequestDto.Amount < constants.MIN_LOAN_AMOUNT)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ValidationError, $"Loan amount cannot be less than {constants.MIN_LOAN_AMOUNT}", null);
        }

        var wallet = await walletRepository.FindById(createLoanOfferRequestDto.WalletId);

        if (wallet is null || wallet.UserId != userId)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Invalid wallet", null);
        }

        var loanOffer = mapper.Map<LoanOffer>(createLoanOfferRequestDto);

        var loanOfferType = GetLoanOfferTypeBasedOnUserType(user.UserType);

        if (loanOfferType is null)
        {
            return new ServiceResponse<object>(ResponseStatus.Forbidden, AppStatusCodes.Unauthorized, "You are not allowed to create loan offers", null);
        }

        loanOffer.Type = (LoanOfferType)loanOfferType;

        loanOffer.UserId = userId;

        loanOffer.Active = true;

        loanOffer.CreatedBy = user;
        loanOffer.ModifiedBy = user;

        loanOfferRepository.Add(loanOffer);

        var result = await loanOfferRepository.SaveChangesAsync();

        if (!result)
        {
            throw new Exception();
        }

        return new ServiceResponse<object>(ResponseStatus.Created, AppStatusCodes.Success, "Loan offer created successfully", null);

    }

    public async Task<ServiceResponse<object>> GetOne(Guid loanOfferId)
    {
        var loanOffer = await loanOfferRepository.FindById(loanOfferId);

        if (loanOffer is null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Loan offer does not exist", null);
        }

        return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Loan offer fetched successfully", loanOffer);
    }

    public async Task<ServiceResponse<object>> GetAll(LoanOfferSearchParams searchParams)
    {
        var result = await loanOfferRepository.GetAllAsync(searchParams);

        return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Loan offers fetched successfully", result);
    }

    public async Task<ServiceResponse<object>> GetAllForLoggedInUser(LoanOfferSearchParams searchParams)
    {
        var userId = httpContextAccessor.HttpContext.User.GetLoggedInUserId();

        var user = await userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "User does not exist", null);
        }

        var result = await loanOfferRepository.GetAllAsync(searchParams, userId);

        return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Loan offers fetched successfully", result);
    }

    private LoanOfferType? GetLoanOfferTypeBasedOnUserType(UserType userType)
    {
        switch (userType)
        {
            case UserType.borrower:
                return LoanOfferType.borrower;
            case UserType.lender:
                return LoanOfferType.lender;
            default:
                return null;
        }
    }

    public async Task<ServiceResponse<object>> DisableLoanOffer(Guid loanOfferId)
    {
        var userId =  httpContextAccessor.HttpContext.User.GetLoggedInUserId();
        var user = await userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "User does not exist", null);
        }

        var result = await loanOfferRepository.FindById(loanOfferId);

        if (result is null || result.UserId != userId)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Loan offer does not exist", null);
        }

        result.Active = false;

        loanOfferRepository.MarkAsModified(result);

        var saveResult = await loanOfferRepository.SaveChangesAsync();

        if (!saveResult)
        {
            throw new Exception();
        }

        return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Loan disabled successfully", result);
    }
}
