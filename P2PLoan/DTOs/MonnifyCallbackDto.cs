using System;

namespace P2PLoan.DTOs;

public class MonnifyCallbackDto<T>
{
    public string EventType { get; set; }
    public T EventData { get; set; }
}