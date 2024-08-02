using System;

namespace P2PLoan.DTOs;

public class MonnifyApiResponse<T>
{
    public bool RequestSuccessful { get; set; }
    public string ResponseMessage { get; set; }
    public string ResponseCode { get; set; }
    public T ResponseBody { get; set; }
}
