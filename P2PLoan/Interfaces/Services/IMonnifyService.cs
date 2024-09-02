using System;
using System.Threading.Tasks;
using P2PLoan.Helpers;

namespace P2PLoan.Interfaces.Services;

public interface IMonnifyService
{
    Task<ServiceResponse<object>> HandleCallback();
}
