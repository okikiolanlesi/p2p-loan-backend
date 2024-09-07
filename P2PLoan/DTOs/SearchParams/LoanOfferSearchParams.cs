using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using P2PLoan.ModelBinders;
using P2PLoan.Models;

namespace P2PLoan.DTOs.SearchParams;

public class LoanOfferSearchParams : SearchParams
{
    public int? MinAmount { get; set; }
    public int? MaxAmount { get; set; }
    public int? MinInterestRate { get; set; }
    public int? MaxInterestRate { get; set; }
    public int? MinAccruingInterestRate { get; set; }
    public int? MaxAccruingInterestRate { get; set; }
    public int? MinGracePeriodDays { get; set; }
    public int? MaxGracePeriodDays { get; set; }
    public int? MinLoanDurationDays { get; set; }
    public int? MaxLoanDurationDays { get; set; }
    public bool? Active { get; set; }
    public LoanOfferType? LoanOfferType { get; set; }
    public List<PaymentFrequency> PaymentFrequencies { get; set; }

    [ModelBinder(BinderType = typeof(LoanOfferOrderByModelBinder))]
    public List<(LoanOfferOrderByField Field, SortDirection Direction)> OrderBy { get; set; }

}

public enum LoanOfferOrderByField
{
    Amount,
    InterestRate,
    GracePeriodDays,
    LoanDurationDays,
    AccruingInterestRate,
    CreatedAt,
    ModifiedAt,
}