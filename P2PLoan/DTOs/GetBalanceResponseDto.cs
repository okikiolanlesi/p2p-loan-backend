using System;

namespace P2PLoan;

public class GetBalanceResponseDto
{
    public int AvailableBalance { get; set; }
    public int LedgerBalance { get; set; }

}
