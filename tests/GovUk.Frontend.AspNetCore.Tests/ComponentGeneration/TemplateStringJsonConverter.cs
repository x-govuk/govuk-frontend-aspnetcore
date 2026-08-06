using System.Text.Json;
using System.Text.Json.Serialization;
using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.Tests.ComponentGeneration;

/// <summary>
/// Deserializes a fixture value into a <see cref="TemplateString"/>.
/// </summary>
/// <param name="encoded">
/// Whether the value is already-encoded HTML. govuk-frontend's fixtures put HTML in the <c>*html</c>
/// parameters and plain text everywhere else, so the two have to be labelled differently; deserializing
/// everything as text would let raw markup through as content that only happens to render.
/// </param>
public class TemplateStringJsonConverter(bool encoded = false) : JsonConverter<TemplateString>
{
    /// <summary>
    /// A converter for fixture values that are already-encoded HTML.
    /// </summary>
    public static TemplateStringJsonConverter Encoded { get; } = new(encoded: true);

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

        return encoded && value is not null ? TemplateString.FromEncoded(value) : new TemplateString(value);
    }

    public override void Write(Utf8JsonWriter writer, TemplateString value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
