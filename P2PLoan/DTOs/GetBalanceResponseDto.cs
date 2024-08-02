using System;

namespace P2PLoan.DTOs;

public class GetBalanceResponseDto
{
    public int AvailableBalance { get; set; }
    public int LedgerBalance { get; set; }

}
