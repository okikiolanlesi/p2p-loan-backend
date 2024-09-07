using System;
using System.Collections.Generic;

namespace P2PLoan.DTOs;

public class GetTransactionsResponseDto
{
    public List<Transaction> Content { get; set; }
    public int TotalPages { get; set; }
    public int TotalElements { get; set; }
    public int NumberOfElements { get; set; }
    public int Size { get; set; }
    public int Number { get; set; }
    public bool Empty { get; set; }
}

public class Transaction
{
    public Guid Id { get; set; }
    public double Amount { get; set; }
    public bool IsCredit { get; set; }
    public string Narration { get; set; }
    public DateTime TransactionDate { get; set; }
    public string TransactionReference { get; set; }
}