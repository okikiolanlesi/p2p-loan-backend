using System.Threading.Tasks;
using P2PLoan.DTOs;
using P2PLoan.Helpers;

namespace P2PLoan.Interfaces
{
    public interface IBankService
    {
        Task<ServiceResponse<object>> VerifyAccountDetails(VerifyAccountDetailsDto verifyAccountDetailsDto);
        Task<ServiceResponse<object>> GetBanks();

    }
}