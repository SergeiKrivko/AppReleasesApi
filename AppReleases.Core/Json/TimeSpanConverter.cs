using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppReleases.Core.Json;

public class TimeSpanConverter : JsonConverter<TimeSpan>
{
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            // Для ISO 8601 строки
            var isoString = reader.GetString();
            return System.Xml.XmlConvert.ToTimeSpan(isoString);
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            // Для миллисекунд
            var milliseconds = reader.GetInt64();
            return TimeSpan.FromMilliseconds(milliseconds);
        }
        else if (reader.TokenType == JsonTokenType.StartObject)
        {
            // Для объектного представления
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            if (root.TryGetProperty("ticks", out var ticksElement))
            {
                return TimeSpan.FromTicks(ticksElement.GetInt64());
            }

            var days = root.TryGetProperty("days", out var d) ? d.GetInt32() : 0;
            var hours = root.TryGetProperty("hours", out var h) ? h.GetInt32() : 0;
            var minutes = root.TryGetProperty("minutes", out var m) ? m.GetInt32() : 0;
            var seconds = root.TryGetProperty("seconds", out var s) ? s.GetInt32() : 0;

            return new TimeSpan(days, hours, minutes, seconds);
        }

        throw new JsonException("Invalid TimeSpan format");
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}