using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using P2PLoan.Constants;
using P2PLoan.DTOs;
using P2PLoan.Helpers;
using P2PLoan.Interfaces;
using P2PLoan.Interfaces.Services;

namespace P2PLoan.Services
{
    public class BankService : IBankService
    {
        private readonly IMonnifyApiService monnifyApiService;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;
        public BankService(IMonnifyApiService monnifyApiService,IMapper mapper,IHttpContextAccessor httpContextAccessor)
        {
             this.monnifyApiService = monnifyApiService;
             this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;

        }

        public async Task<ServiceResponse<object>> GetBanks()
        {
           var monifyBanks = await monnifyApiService.GetBanks();
           if(monifyBanks is null)
           {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.InvalidProvider, "banks not found", null);           
           }

            return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Banks fetched successfully", monifyBanks);

        }

        public async Task<ServiceResponse<object>> VerifyAccountDetails(VerifyAccountDetailsDto verifyAccountDetailsDto)
        {
            var monnifyRequestDto = mapper.Map<MonnifyVerifyAccountDetailsRequestDto>(verifyAccountDetailsDto);
           
            var accountProvider = await monnifyApiService.VerifyAccountDetails(monnifyRequestDto);
            if(accountProvider == null)
            {
                 return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.InvalidProvider, "account details not verified", null);           
            }

            return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Account details verified successfully", accountProvider);
            
        }
    }
}