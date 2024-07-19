﻿using System;

namespace P2PLoan;

public enum StatusCodes
{
    Success = 0000,
    BvnNotVerified = 0001,
    EmailNotVerified = 0002,
    NoLinkedWallet = 0003,
    InvalidCredentials = 0004,
    InvalidVerificationToken = 0005,
    NoWalletsLinked = 0006,
    NoPinCreated = 0007,
    AlreadyExists = 0008,
    InternalServerError = 9999,
}
