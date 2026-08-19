using System.Text.Json;
using System.Text.Json.Serialization;
using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.Tests.ComponentGeneration;

/// <summary>
/// Reads a row from the reference fixtures, where a row is a bare array of cells rather than an
/// object; <see cref="TableOptionsRow"/> wraps that array so the row can carry its own classes and
/// attributes.
/// </summary>
public class TableOptionsRowJsonConverter : JsonConverter<TableOptionsRow>
{
    public override TableOptionsRow Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        new()
        {
            Cells = JsonSerializer.Deserialize<IReadOnlyCollection<TableOptionsColumn?>>(ref reader, options)
        };

    public override void Write(Utf8JsonWriter writer, TableOptionsRow value, JsonSerializerOptions options) =>
        JsonSerializer.Serialize(writer, value.Cells, options);
}
