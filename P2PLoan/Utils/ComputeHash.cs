using System;
using System.Security.Cryptography;
using System.Text;

namespace P2PLoan.Utils;

public static class ComputeHash
{
    private const string DEFAULT_MERCHANT_CLIENT_SECRET = "91MUDL9N6U3BQRXBQ2PJ9M0PW4J22M1Y";

    public static string Compute(string requestBody)
    {
        using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(DEFAULT_MERCHANT_CLIENT_SECRET)))
        {
            byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(requestBody));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
