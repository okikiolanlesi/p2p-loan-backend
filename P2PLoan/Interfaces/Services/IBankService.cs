using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using P2PLoan.DTOs;
using P2PLoan.Helpers;

namespace P2PLoan.Interfaces.Services
{
    public interface IBankService
    {
        Task<ServiceResponse<object>> VerifyAccountDetails(VerifyAccountDetailsDto verifyAccountDetailsDto);
        Task<ServiceResponse<object>> GetBanks();
        
    }
}