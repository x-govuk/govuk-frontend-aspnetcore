using System.Text.Json;
using System.Text.Json.Serialization;
using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.Tests.ComponentGeneration;

/// <summary>
/// Deserializes a fixture value into a <see cref="TemplateString"/>.
/// </summary>
public class TemplateStringJsonConverter : JsonConverter<TemplateString>
{
    public override TemplateString? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType is not (JsonTokenType.Number or JsonTokenType.String or JsonTokenType.False or JsonTokenType.True))
        {
            throw new NotSupportedException($"Cannot create a {nameof(TemplateString)} from a {reader.TokenType}.");
        }

        var value = JsonSerializer.Deserialize(ref reader, typeof(string), options) as string;

        return new TemplateString(value);
    }

    public override void Write(Utf8JsonWriter writer, TemplateString value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
