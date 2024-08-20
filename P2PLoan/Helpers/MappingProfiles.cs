using AutoMapper;
using P2PLoan.DTOs;
using P2PLoan.Interfaces;
using P2PLoan.Models;
using P2PLoan.Repositories;
using P2PLoan.Services;

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
            CreateMap<CreateModuleRequestDto, Module>();
            CreateMap<UpdateRoleRequestDto, Role>();
            CreateMap<CreateRoleRequestDto, Role>();
            CreateMap<CreatePermissionRequestDto, Permission>().ReverseMap();
            CreateMap<RolePermissionRequestDto, RolePermission>().ReverseMap();


            // Map MonnifyApiResponse<T> to the appropriate DTOs
            // Assuming ResponseBody matches the properties of the destination DTOs
            CreateMap<MonnifyApiResponse<MonnifyCreateWalletResponseBody>, CreateWalletResponseDto>()
                .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.ResponseBody.AccountNumber))
                .ForMember(dest => dest.WalletReference, opt => opt.MapFrom(src => src.ResponseBody.WalletReference))
                .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.ResponseBody.AccountName))
                .ForMember(dest => dest.FeeBearer, opt => opt.MapFrom(src => src.ResponseBody.FeeBearer))
                .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.ResponseBody.CustomerEmail))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.ResponseBody.CustomerName))
                .ForMember(dest => dest.BVN, opt => opt.MapFrom(src => src.ResponseBody.BvnDetails.Bvn))
                .ForMember(dest => dest.BVNDateOfBirth, opt => opt.MapFrom(src => src.ResponseBody.BvnDetails.BvnDateOfBirth))
                .ForMember(dest => dest.TopUpAccountName, opt => opt.MapFrom(src => src.ResponseBody.TopUpAccountDetails.AccountName))
                .ForMember(dest => dest.TopUpAccountNumber, opt => opt.MapFrom(src => src.ResponseBody.TopUpAccountDetails.AccountNumber))
                .ForMember(dest => dest.TopUpBankName, opt => opt.MapFrom(src => src.ResponseBody.TopUpAccountDetails.BankName))
                .ForMember(dest => dest.TopUpBankCode, opt => opt.MapFrom(src => src.ResponseBody.TopUpAccountDetails.BankCode));

            CreateMap<MonnifyApiResponse<MonnifyGetBalanceResponseBody>, GetBalanceResponseDto>()
                .ForMember(dest => dest.AvailableBalance, opt => opt.MapFrom(src => src.ResponseBody.AvailableBalance))
                .ForMember(dest => dest.LedgerBalance, opt => opt.MapFrom(src => src.ResponseBody.LedgerBalance));

            CreateMap<MonnifyApiResponse<MonnifyGetTransactionsResponseBody>, GetTransactionsResponseDto>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.ResponseBody.Content))
                .ForMember(dest => dest.Last, opt => opt.MapFrom(src => src.ResponseBody.Last))
                .ForMember(dest => dest.TotalPages, opt => opt.MapFrom(src => src.ResponseBody.TotalPages))
                .ForMember(dest => dest.TotalElements, opt => opt.MapFrom(src => src.ResponseBody.TotalElements))
                .ForMember(dest => dest.First, opt => opt.MapFrom(src => src.ResponseBody.First))
                .ForMember(dest => dest.NumberOfElements, opt => opt.MapFrom(src => src.ResponseBody.NumberOfElements))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.ResponseBody.Size))
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.ResponseBody.Number))
                .ForMember(dest => dest.Empty, opt => opt.MapFrom(src => src.ResponseBody.Empty));


            // Mapping for individual response body types to DTOs
            CreateMap<MonnifyCreateWalletResponseBody, CreateWalletResponseDto>();
            CreateMap<MonnifyGetBalanceResponseBody, GetBalanceResponseDto>();
            CreateMap<MonnifyGetTransactionsResponseBody, GetTransactionsResponseDto>();
        }
    }
}