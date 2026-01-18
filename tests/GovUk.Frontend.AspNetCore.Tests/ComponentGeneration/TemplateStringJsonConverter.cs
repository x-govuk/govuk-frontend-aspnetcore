using System.Text.Json;
using System.Text.Json.Serialization;
using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.Tests.ComponentGeneration;

public class TemplateStringJsonConverter : JsonConverter<TemplateString>
{
    public override TemplateString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType == JsonTokenType.Null
            ? default
            : reader.TokenType is JsonTokenType.Number or JsonTokenType.String or JsonTokenType.False or JsonTokenType.True
            ? new TemplateString(JsonSerializer.Deserialize(ref reader, typeof(string), options) as string)
            : throw new NotSupportedException($"Cannot create a {nameof(TemplateString)} from a {reader.TokenType}.");
    }

    public override void Write(Utf8JsonWriter writer, TemplateString value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
