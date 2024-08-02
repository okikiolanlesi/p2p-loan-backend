using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace P2PLoan.Utils;

public static class ComputeHash
{
    public static string Compute(string requestBody, string clientSecret)
    {
        using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(clientSecret)))
        {
            byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(requestBody));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
