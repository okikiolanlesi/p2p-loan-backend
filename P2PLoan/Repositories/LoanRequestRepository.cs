using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

    public async Task<LoanRequest> FindById(Guid loanRequestId)
    {
        return await dbContext.LoanRequests.FirstOrDefaultAsync(x => x.Id == loanRequestId);
    }

    public async Task<PagedResponse<IEnumerable<LoanRequest>>> GetAllAsync(LoanRequestSearchParams searchParams, Guid? userId = null)
    {
        IQueryable<LoanRequest> query = dbContext.LoanRequests;

        // Apply filtering
        if (userId != null && userId.HasValue)
        {
            query = query.Where(lr => lr.UserId == userId);
        }

        if (searchParams.TrafficType.HasFlag(TrafficType.received) && userId != null)
        {
            query = query.Where(lr => lr.LoanOffer.UserId == userId);
        }

        if (searchParams.TrafficType.HasFlag(TrafficType.sent) && userId != null)
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
            .Take(searchParams.PageSize).Include(lo => lo.User).Include(lo => lo.Wallet)
            .ToListAsync();

        var result = new PagedResponse<IEnumerable<LoanRequest>>
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
}
