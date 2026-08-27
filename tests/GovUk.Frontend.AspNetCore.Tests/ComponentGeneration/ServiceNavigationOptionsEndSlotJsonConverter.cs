using System.Text.Json;
using System.Text.Json.Serialization;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.Tests.ComponentGeneration;

/// <summary>
/// Reads the service navigation's end slot from the reference fixtures, where the slot is either
/// the HTML to insert or an object carrying that HTML alongside its alignment;
/// <see cref="ServiceNavigationOptionsEndSlot"/> is always the object.
/// </summary>
public class ServiceNavigationOptionsEndSlotJsonConverter : JsonConverter<ServiceNavigationOptionsEndSlot>
{
    public override ServiceNavigationOptionsEndSlot? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var element = document.RootElement;

        if (element.ValueKind is JsonValueKind.String)
        {
            return new ServiceNavigationOptionsEndSlot { Html = new HtmlString(element.GetString()) };
        }

        return new ServiceNavigationOptionsEndSlot
        {
            Html = element.TryGetProperty("html", out var html) ? new HtmlString(html.GetString()) : null,
            Align = element.TryGetProperty("align", out var align) ? align.GetString() : null
        };
    }

    public override void Write(
        Utf8JsonWriter writer,
        ServiceNavigationOptionsEndSlot value,
        JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }
}
