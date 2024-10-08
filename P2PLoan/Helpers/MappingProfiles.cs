using AutoMapper;
using P2PLoan.DTOs;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, UserDto>();
            CreateMap<RegisterRequestDto, User>();
            CreateMap<CreateWalletResponseDto, CreateWalletResponse>();
            CreateMap<UpdateModuleRequestDto, Module>();
            // CreateMap<CreateModuleRequestDto, Module>();
            CreateMap<UpdateRoleRequestDto, Role>();
            CreateMap<CreateRoleRequestDto, Role>();
            CreateMap<LoanOffer, LoanOfferDto>();
            CreateMap<CreateLoanOfferRequestDto, LoanOffer>();
            CreateMap<UserDto, PublicUserProfileDto>().ReverseMap();
            CreateMap<User, PublicUserProfileDto>().ReverseMap();
            CreateMap<Wallet, WalletDto>().ReverseMap();
            CreateMap<LoanRequest, LoanRequestDto>().ReverseMap();
            CreateMap<MonnifyTransferResponseBody, TransferResponseDto>().ReverseMap();
            CreateMap<TransferDto, MonnifyTransferRequestBodyDto>().ReverseMap();
            CreateMap<CreateLoanRequestRequestDto, LoanRequest>().ReverseMap();
            CreateMap<MonnifyCollectionCallBackData, ManagedWalletCollectionCallbackData>().ReverseMap();
            CreateMap<MonnifyDisbursementCallbackData, ManagedWalletDisbursementCallbackData>().ReverseMap();
            CreateMap<MonnifyCallbackDto<MonnifyDisbursementCallbackData>, ManagedWalletCallbackDto<ManagedWalletDisbursementCallbackData>>().ReverseMap();
            CreateMap<MonnifyCallbackDto<MonnifyCollectionCallBackData>, ManagedWalletCallbackDto<ManagedWalletCollectionCallbackData>>().ReverseMap();

            // Map MonnifyApiResponse<T> to the appropriate DTOs
            CreateMap<MonnifyApiResponse<MonnifyCreateWalletResponseBody>, CreateWalletResponseDto>()
                .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.ResponseBody.AccountNumber))
                .ForMember(dest => dest.WalletReference, opt => opt.MapFrom(src => src.ResponseBody.WalletReference))
                .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.ResponseBody.AccountName))
                .ForMember(dest => dest.FeeBearer, opt => opt.MapFrom(src => src.ResponseBody.FeeBearer))
                .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.ResponseBody.CustomerEmail))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.ResponseBody.CustomerName))
                .ForMember(dest => dest.BVN, opt => opt.MapFrom(src => src.ResponseBody.BvnDetails.Bvn))
                .ForMember(dest => dest.BVNDateOfBirth, opt => opt.MapFrom(src => src.ResponseBody.BvnDetails.BvnDateOfBirth));


            CreateMap<MonnifyApiResponse<MonnifyGetBalanceResponseBody>, GetBalanceResponseDto>()
                .ForMember(dest => dest.AvailableBalance, opt => opt.MapFrom(src => src.ResponseBody.AvailableBalance))
                .ForMember(dest => dest.LedgerBalance, opt => opt.MapFrom(src => src.ResponseBody.LedgerBalance));

            CreateMap<MonnifyApiResponse<MonnifyGetTransactionsResponseBody>, GetTransactionsResponseDto>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.ResponseBody.Content))
                .ForMember(dest => dest.TotalPages, opt => opt.MapFrom(src => src.ResponseBody.TotalPages))
                .ForMember(dest => dest.TotalElements, opt => opt.MapFrom(src => src.ResponseBody.TotalElements))
                .ForMember(dest => dest.NumberOfElements, opt => opt.MapFrom(src => src.ResponseBody.NumberOfElements))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.ResponseBody.Size))
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.ResponseBody.Number))
                .ForMember(dest => dest.Empty, opt => opt.MapFrom(src => src.ResponseBody.Empty));

            CreateMap<MonnifyApiResponse<MonnifyGetSingleTransferResponseBody>, TransferResponseDto>()
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.ResponseBody.DateCreated))
                .ForMember(dest => dest.Reference, opt => opt.MapFrom(src => src.ResponseBody.Reference))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.ResponseBody.Status))
                .ForMember(dest => dest.TotalFee, opt => opt.MapFrom(src => src.ResponseBody.TotalFee))
                .ForMember(dest => dest.DestinationBankCode, opt => opt.MapFrom(src => src.ResponseBody.DestinationBankCode))
                .ForMember(dest => dest.DestinationBankName, opt => opt.MapFrom(src => src.ResponseBody.DestinationBankName))
                .ForMember(dest => dest.DestinationAccountNumber, opt => opt.MapFrom(src => src.ResponseBody.DestinationAccountNumber));

            // Mapping for individual response body types to DTOs
            CreateMap<MonnifyCreateWalletResponseBody, CreateWalletResponseDto>();
            CreateMap<MonnifyGetBalanceResponseBody, GetBalanceResponseDto>();
            CreateMap<MonnifyGetTransactionsResponseBody, GetTransactionsResponseDto>();

            CreateMap<VerifyBvnDto, MonnifyVerifyBVNRequestDto>();
            CreateMap<VerifyAccountDetailsDto, MonnifyVerifyAccountDetailsRequestDto>();
        }
    }
}