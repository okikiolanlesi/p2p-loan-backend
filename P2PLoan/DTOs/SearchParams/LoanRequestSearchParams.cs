using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using P2PLoan.ModelBinders;

namespace P2PLoan.DTOs.SearchParams;

public class LoanRequestSearchParams : SearchParams
{
    public Guid? UserId { get; set; }
    public Guid? LoanOfferId { get; set; }

    [ModelBinder(BinderType = typeof(LoanRequestOrderByModelBinder))]
    public List<(LoanRequestOrderByField Field, SortDirection Direction)> OrderBy { get; set; }

    public TrafficType? TrafficType { get; set; }
}

public enum LoanRequestOrderByField
{
    CreatedAt,
    ModifiedAt,
}

public enum TrafficType
{
    received, sent
}