using System;
using System.Collections.Generic;

namespace P2PLoan.DTOs;

public class GetTransactionsResponseDto
{
    public List<object> Content { get; set; }
    public bool Last { get; set; }
    public int TotalPages { get; set; }
    public int TotalElements { get; set; }
    public bool First { get; set; }
    public int NumberOfElements { get; set; }
    public int Size { get; set; }
    public int Number { get; set; }
    public bool Empty { get; set; }
}
