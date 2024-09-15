using Newtonsoft.Json;
using System;
using System.Globalization;

namespace P2PLoan.Converters;

public class MultiFormatDateTimeConverter : JsonConverter
{
    private readonly string[] formats = new[]
    {
        "dd/MM/yyyy hh:mm:ss tt",  // Custom format
        "yyyy-MM-ddTHH:mm:ss",     // ISO 8601
        "MM/dd/yyyy hh:mm:ss tt",  // US format
        // Add more formats as needed
    };

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(DateTime);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        string dateString = reader.Value.ToString();
        foreach (var format in formats)
        {
            if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date;
            }
        }

        return DateTime.Parse(dateString, CultureInfo.InvariantCulture);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        DateTime date = (DateTime)value;
        writer.WriteValue(date.ToString("yyyy-MM-ddTHH:mm:ss")); // Output in standard ISO 8601 format
    }
}
