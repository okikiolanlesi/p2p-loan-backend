using System;

namespace P2PLoan.DTOs;

public class GetBalanceResponseDto
{
    public double AvailableBalance { get; set; }
    public double LedgerBalance { get; set; }

}
