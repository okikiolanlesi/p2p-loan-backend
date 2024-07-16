using System;
using P2PLoan.Interfaces;

namespace P2PLoan;

public class Constants : IConstants
{
    public int MAX_LOAN_AMOUNT { get; } = 500000;
    public string SYSTEM_USER_EMAIL { get; } = "system@system.com";

    public string SYSTEM_USER_FIRST_NAME { get; } = "SYSTEM";

    public string SYSTEM_USER_LAST_NAME { get; } = "SYSTEM";
}
