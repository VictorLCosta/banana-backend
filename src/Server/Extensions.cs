using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Server;

public static class Extensions
{
    public static WebApplicationBuilder AddServer(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddControllers(mvcOptions => mvcOptions
            .AddResultConvention(resultStatusMap => resultStatusMap
                .AddDefaultMap()
                .For(ResultStatus.Ok, HttpStatusCode.OK, resultStatusOptions => resultStatusOptions
                    .For("DELETE", HttpStatusCode.NoContent))
                .For(ResultStatus.Created, HttpStatusCode.Created, resultStatusOptions => resultStatusOptions
                    .For("POST", HttpStatusCode.Created))
            ))
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter()));

        return builder;
    }

    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private const string Format = "yyyy-MM-dd";

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (string.IsNullOrWhiteSpace(value))
                throw new JsonException("Date value is null or empty");

            // Interpreta o ISO 8601 como DateTime
            if (DateTime.TryParse(value, null, System.Globalization.DateTimeStyles.RoundtripKind, out var dateTime))
            {
                return DateOnly.FromDateTime(dateTime);
            }

            // Fallback: tenta interpretar no formato yyyy-MM-dd
            return DateOnly.ParseExact(value, "yyyy-MM-dd");
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString(Format));
    }
}
