using System;
using System.Collections.Generic;

namespace P2PLoan.DTOs.SearchParams;

public class LoanSearchParams : SearchParams
{
    public Guid? LoanOfferId { get; set; }
    public Guid? LoanRequestId { get; set; }
    public double MinAmount { get; set; }
    public double MaxAmount { get; set; }
    public List<(LoanOrderByField Field, SortDirection Direction)> OrderBy { get; set; }
}

public enum LoanOrderByField
{
    CreatedAt,
    ModifiedAt,
    DueDate,
}


