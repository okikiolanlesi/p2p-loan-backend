using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Helpers;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface IWalletProviderService
{
    Task<ServiceResponse<object>> GetAllAsync();

}
