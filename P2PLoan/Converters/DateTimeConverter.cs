using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace P2PLoan.Converters;

public class DateTimeConverter : JsonConverter<DateTime>
{
    private readonly string[] formats = new[]
    {
        "dd/MM/yyyy hh:mm:ss tt",  // Custom format
        "yyyy-MM-ddTHH:mm:ss",     // ISO 8601
        "MM/dd/yyyy hh:mm:ss tt",  // US format
        // Add more formats as needed
    };

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string dateString = reader.GetString();
        foreach (var format in formats)
        {
            if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date;
            }
        }

        // Fallback to default DateTime.Parse if none of the formats match
        return DateTime.Parse(dateString, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss")); // Output in standard ISO 8601 format
    }
}
