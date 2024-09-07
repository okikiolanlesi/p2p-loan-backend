using System;

namespace P2PLoan.DTOs;

public class ManagedWalletCallbackDto<T> : EventArgs
{
    public string EventType { get; set; }
    public T EventData { get; set; }
}
