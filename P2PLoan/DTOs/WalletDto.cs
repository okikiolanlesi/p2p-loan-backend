using System;
using System.Collections;
using System.Collections.Generic;
using P2PLoan.Models;

namespace P2PLoan.DTOs;

public class WalletDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid WalletProviderId { get; set; }
    public string AccountNumber { get; set; }
    public string ReferenceId { get; set; }
    public string TopUpAccountNumber { get; set; }
    public string TopUpAccountName { get; set; }
    public string TopUpBankCode { get; set; }
    public string TopUpBankName { get; set; }

    public UserDto User { get; set; }
    public WalletProvider WalletProvider { get; set; }
    public IEnumerable<WalletTopUpDetail> TopUpDetails { get; set; }
}
