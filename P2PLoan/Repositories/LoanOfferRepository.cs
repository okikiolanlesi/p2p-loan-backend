using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using P2PLoan.Data;
using P2PLoan.Helpers;
using P2PLoan.Interfaces;
using P2PLoan.Models;
using System.Linq.Dynamic.Core;
using AutoMapper.QueryableExtensions;
using AutoMapper;

namespace P2PLoan.Repositories;

public class LoanOfferRepository : ILoanOfferRepository
{
    private readonly P2PLoanDbContext dbContext;
    private readonly IMapper mapper;

    public LoanOfferRepository(P2PLoanDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }
    public void Add(LoanOffer loanOffer)
    {
        dbContext.LoanOffers.Add(loanOffer);
    }

    public void AddRange(IEnumerable<LoanOffer> loanOffers)
    {
        dbContext.LoanOffers.AddRange(loanOffers);
    }

    public async Task<LoanOffer> FindById(Guid loanOfferId)
    {
        return await dbContext.LoanOffers.FirstOrDefaultAsync(x => x.Id == loanOfferId);
    }

    public async Task<PagedResponse<IEnumerable<LoanOfferDto>>> GetAllAsync(LoanOfferSearchParams searchParams, Guid? userId = null)
    {
        IQueryable<LoanOffer> query = dbContext.LoanOffers;

        // Apply filtering
        if (userId != null && userId.HasValue)
        {
            query = query.Where(lo => lo.UserId == userId);
        }

        if (searchParams.Active == true)
        {
            query = query.Where(lo => lo.Active);
        }

        if (searchParams.LoanOfferType.HasValue)
        {
            query = query.Where(lo => lo.Type == searchParams.LoanOfferType.Value);
        }

        if (searchParams.MinAmount.HasValue)
        {
            query = query.Where(lo => lo.Amount >= searchParams.MinAmount.Value);
        }

        if (searchParams.MaxAmount.HasValue)
        {
            query = query.Where(lo => lo.Amount <= searchParams.MaxAmount.Value);
        }

        if (searchParams.MinInterestRate.HasValue)
        {
            query = query.Where(lo => lo.InterestRate >= searchParams.MinInterestRate.Value);
        }

        if (searchParams.MaxInterestRate.HasValue)
        {
            query = query.Where(lo => lo.InterestRate <= searchParams.MaxInterestRate.Value);
        }

        if (searchParams.MinAccruingInterestRate.HasValue)
        {
            query = query.Where(lo => lo.AccruingInterestRate >= searchParams.MinAccruingInterestRate.Value);
        }

        if (searchParams.MaxAccruingInterestRate.HasValue)
        {
            query = query.Where(lo => lo.AccruingInterestRate <= searchParams.MaxAccruingInterestRate.Value);
        }

        if (searchParams.MinGracePeriodDays.HasValue)
        {
            query = query.Where(lo => lo.GracePeriodDays >= searchParams.MinGracePeriodDays.Value);
        }

        if (searchParams.MaxGracePeriodDays.HasValue)
        {
            query = query.Where(lo => lo.GracePeriodDays <= searchParams.MaxGracePeriodDays.Value);
        }

        if (searchParams.MinLoanDurationDays.HasValue)
        {
            query = query.Where(lo => lo.LoanDurationDays >= searchParams.MinLoanDurationDays.Value);
        }

        if (searchParams.MaxLoanDurationDays.HasValue)
        {
            query = query.Where(lo => lo.LoanDurationDays <= searchParams.MaxLoanDurationDays.Value);
        }

        if (searchParams.PaymentFrequencies != null && searchParams.PaymentFrequencies.Any())
        {
            query = query.Where(lo => searchParams.PaymentFrequencies.Contains(lo.RepaymentFrequency));
        }

        if (!string.IsNullOrEmpty(searchParams.SearchTerm))
        {
            query = query.Where(lo => lo.User.FirstName.ToLower().Contains(searchParams.SearchTerm.ToLower()) || lo.User.LastName.ToLower().Contains(searchParams.SearchTerm.ToLower()) || lo.User.Email.ToLower().Contains(searchParams.SearchTerm.ToLower()) || lo.AdditionalInformation.ToLower().Contains(searchParams.SearchTerm.ToLower()) || lo.Amount.ToString().ToLower().Contains(searchParams.SearchTerm.ToLower()));
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
            .Take(searchParams.PageSize).Include(lo => lo.User).Include(lo => lo.Wallet).ProjectTo<LoanOfferDto>(mapper.ConfigurationProvider)
            .ToListAsync();

        var result = new PagedResponse<IEnumerable<LoanOfferDto>>
        {
            TotalItems = totalItems,
            PageNumber = searchParams.PageNumber,
            PageSize = searchParams.PageSize,
            Items = items
        };

        return result;
    }

    public void MarkAsModified(LoanOffer loanOffer)
    {
        dbContext.Entry(loanOffer).State = EntityState.Modified;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await dbContext.SaveChangesAsync() > 0;
    }
}
