using System;
using System.Globalization;

namespace P2PLoan.Utils;

public static class DateConverter
{
    public static string ConvertIsoToDate(string isoDateTime)
    {
        if (DateTime.TryParse(isoDateTime, null, DateTimeStyles.RoundtripKind, out DateTime dateTime))
        {
            return dateTime.ToString("yyyy-MM-dd");
        }
        else
        {
            throw new ArgumentException("Invalid ISO 8601 date-time string.", nameof(isoDateTime));
        }
    }
}