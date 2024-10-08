﻿using System;
using P2PLoan.Interfaces;

namespace P2PLoan.Constants;

public class Constants : IConstants
{
    public int MAX_LOAN_AMOUNT { get; } = 500000;
    public int MIN_LOAN_AMOUNT { get; } = 1000;
    public string SYSTEM_USER_EMAIL { get; } = "system@system.com";

    public string SYSTEM_USER_FIRST_NAME { get; } = "SYSTEM";

    public string SYSTEM_USER_LAST_NAME { get; } = "SYSTEM";
    public int PASSWORD_RESET_TOKEN_EXPIRATION_MINUTES { get; } = 60;

    public int PASSWORD_RESET_TOKEN_LENGTH { get; } = 20;

    public int EMAIL_VERIFICATION_TOKEN_LENGTH { get; } = 20;

    public int EMAIL_VERIFICATION_TOKEN_EXPIRATION_MINUTES { get; } = 1440;
    public string USER_WALLET_NAME_PREFIX { get; } = "P2PLoan Wallet";

    public double WITHDRAWAL_FEE_PERCENTAGE { get; } = 2;

    public double WITHDRAWAL_FEE_CAP { get; } = 1000;
}
