using Newtonsoft.Json;
using System;
using System.Globalization;

namespace P2PLoan.Converters
{
    public class MultiFormatDateTimeConverter : JsonConverter
    {
        private readonly string[] formats =
        [
            "dd/MM/yyyy hh:mm:ss tt",  // 12-hour format with AM/PM
            "dd/MM/yyyy h:mm:ss tt",   // 12-hour format with single-digit hour
            "dd/MM/yyyy HH:mm:ss",     // 24-hour format
            "yyyy-MM-ddTHH:mm:ss",     // ISO 8601
            "MM/dd/yyyy hh:mm:ss tt",  // US format with 12-hour clock
            "MM/dd/yyyy h:mm:ss tt",   // US format with single-digit hour
            // Add more formats as needed
        ];


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

            // Fallback to default parsing if no format matches
            return DateTime.Parse(dateString, CultureInfo.InvariantCulture);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DateTime date = (DateTime)value;
            writer.WriteValue(date.ToString("yyyy-MM-ddTHH:mm:ss")); // Output in standard ISO 8601 format
        }
    }
}
