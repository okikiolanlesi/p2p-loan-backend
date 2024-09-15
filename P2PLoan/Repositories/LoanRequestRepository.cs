using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using P2PLoan.Data;
using P2PLoan.DTOs;
using P2PLoan.DTOs.SearchParams;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Repositories;

public class LoanRequestRepository : ILoanRequestRepository
{
    private readonly P2PLoanDbContext dbContext;
    private readonly IMapper mapper;

    public LoanRequestRepository(P2PLoanDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }
    public void Add(LoanRequest loanRequest)
    {
        dbContext.LoanRequests.Add(loanRequest);
    }

    public void AddRange(IEnumerable<LoanRequest> loanRequests)
    {
        dbContext.LoanRequests.AddRange(loanRequests);
    }

    public async Task<LoanRequestDto> FindByIdPublic(Guid loanRequestId)
    {
        return await dbContext.LoanRequests.Include(lr => lr.LoanOffer).Include(lr => lr.Wallet).Include(lr => lr.User).ProjectTo<LoanRequestDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x.Id == loanRequestId);
    }

    public Task<LoanRequest> FindByIdForAUser(Guid LoanRequestId, Guid UserId)
    {
        return dbContext.LoanRequests.Include(lr => lr.LoanOffer).Include(lr => lr.Wallet).Include(lr => lr.User).FirstOrDefaultAsync(x => x.Id == LoanRequestId && x.UserId == UserId);
    }
    public Task<LoanRequest> FindByLoanOfferIdForAUser(Guid loanOfferId, Guid UserId)
    {
        return dbContext.LoanRequests.Include(lr => lr.LoanOffer).Include(lr => lr.Wallet).Include(lr => lr.User).FirstOrDefaultAsync(x => x.LoanOfferId == loanOfferId && x.UserId == UserId);
    }

    public async Task<PagedResponse<IEnumerable<LoanRequestDto>>> GetAllAsync(LoanRequestSearchParams searchParams, Guid? userId = null)
    {
        IQueryable<LoanRequest> query = dbContext.LoanRequests;

        // Apply filtering
        if (userId.HasValue)
        {
            query = query.Where(lr => lr.UserId == userId || lr.LoanOffer.UserId == userId);
        }

        if (searchParams.TrafficType != null && searchParams.TrafficType == TrafficType.received && userId != null)
        {
            query = query.Where(lr => lr.LoanOffer.UserId == userId);
        }

        if (searchParams.TrafficType != null && searchParams.TrafficType == TrafficType.sent && userId != null)
        {
            query = query.Where(lr => lr.UserId == userId);
        }

        if (userId == null && searchParams.UserId.HasValue)
        {
            query = query.Where(lr => lr.UserId == searchParams.UserId);
        }

        if (searchParams.LoanOfferId.HasValue)
        {
            query = query.Where(lr => lr.LoanOfferId == searchParams.LoanOfferId);
        }

        if (!string.IsNullOrEmpty(searchParams.SearchTerm))
        {
            query = query.Where(lr => lr.User.FirstName.ToLower().Contains(searchParams.SearchTerm.ToLower()) || lr.User.LastName.ToLower().Contains(searchParams.SearchTerm.ToLower()) || lr.User.Email.ToLower().Contains(searchParams.SearchTerm.ToLower()) || lr.AdditionalInformation.ToLower().Contains(searchParams.SearchTerm.ToLower()) || lr.LoanOffer.Amount.ToString().ToLower().Contains(searchParams.SearchTerm.ToLower()));
        }


        // Apply ordering
        if (searchParams.OrderBy != null && searchParams.OrderBy.Any())
        {
            var orderByClauses = searchParams.OrderBy
                .Select(o => $"{o.Field} {o.Direction}")
                .ToArray();
            var orderByString = string.Join(",", orderByClauses);
            query = query.OrderBy(orderByString);
        }

        // Apply pagination
        var totalItems = query.Count();
        var items = await query
            .Skip((searchParams.PageNumber - 1) * searchParams.PageSize)
            .Take(searchParams.PageSize).Include(lo => lo.User).Include(lo => lo.Wallet).ProjectTo<LoanRequestDto>(mapper.ConfigurationProvider)
            .ToListAsync();

        var result = new PagedResponse<IEnumerable<LoanRequestDto>>
        {
            TotalItems = totalItems,
            PageNumber = searchParams.PageNumber,
            PageSize = searchParams.PageSize,
            Items = items
        };

        return result;
    }

    public void MarkAsModified(LoanRequest LoanRequest)
    {
        dbContext.Entry(LoanRequest).State = EntityState.Modified;
    }
    public void Remove(LoanRequest loanRequest)
    {
        dbContext.LoanRequests.Remove(loanRequest);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await dbContext.SaveChangesAsync() > 0;
    }

    public async Task<LoanRequest> FindById(Guid loanRequestId)
    {
        return await dbContext.LoanRequests.Include(lr => lr.LoanOffer).Include(lr => lr.Wallet).Include(lr => lr.User).FirstOrDefaultAsync(x => x.Id == loanRequestId);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return dbContext.Database.BeginTransactionAsync();
    }
}
