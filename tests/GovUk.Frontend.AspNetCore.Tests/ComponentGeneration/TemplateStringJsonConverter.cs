using System.Text.Json;
using System.Text.Json.Serialization;
using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.Tests.ComponentGeneration;

public class TemplateStringJsonConverter : JsonConverter<TemplateString>
{
    public override TemplateString? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Null => null,
            JsonTokenType.String => new TemplateString(reader.GetString()),
            JsonTokenType.Number => new TemplateString(reader.GetDecimal().ToString(System.Globalization.CultureInfo.InvariantCulture)),
            JsonTokenType.True => new TemplateString("true"),
            JsonTokenType.False => new TemplateString("false"),
            _ => throw new NotSupportedException($"Cannot create a {nameof(TemplateString)} from a {reader.TokenType}.")
        };
    }

    public override void Write(Utf8JsonWriter writer, TemplateString value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
