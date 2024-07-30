using AutoMapper;
using P2PLoan.DTOs;
using P2PLoan.Models;

namespace P2PLoan.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, UserDto>();
            CreateMap<MonnifyApiResponse<MonnifyCreateWalletResponseBody>, CreateWalletResponseDto>().ForMember(m => m, d => d.MapFrom(s => s.ResponseBody));
            CreateMap<MonnifyApiResponse<MonnifyGetBalanceResponseBody>, GetBalanceResponseDto>().ForMember(m => m, d => d.MapFrom(s => s.ResponseBody));
            CreateMap<MonnifyApiResponse<MonnifyGetTransactionsResponseBody>, GetTransactionsResponseDto>().ForMember(m => m, d => d.MapFrom(s => s.ResponseBody));

        }
    }
}