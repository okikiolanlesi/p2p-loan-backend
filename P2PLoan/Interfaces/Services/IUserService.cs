using System;
using System.Threading.Tasks;
using P2PLoan.Helpers;

namespace P2PLoan.Interfaces;

public interface IUserService
{
   Task<ServiceResponse<object>> GetUserById(Guid userId);

}
