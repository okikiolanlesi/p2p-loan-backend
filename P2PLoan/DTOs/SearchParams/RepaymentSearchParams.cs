using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using P2PLoan.ModelBinders;

namespace P2PLoan.DTOs.SearchParams;

public class RepaymentSearchParams : SearchParams
{
    public Guid? LoanId { get; set; }
    public double? MinAmount { get; set; }
    public double? MaxAmount { get; set; }

    [ModelBinder(BinderType = typeof(RepaymentOrderByModelBinder))]
    public List<(RepaymentOrderByField Field, SortDirection Direction)> OrderBy { get; set; }
}

public enum RepaymentOrderByField
{
    CreatedAt,
    ModifiedAt,
}
