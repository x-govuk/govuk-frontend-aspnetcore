using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;
using Xunit.Sdk;
using Xunit.v3;

namespace GovUk.Frontend.AspNetCore.Tests.ComponentGeneration;

public class ComponentFixtureData(
    string componentName,
    Type optionsType,
    string? only = null,
    params string[] exclude) : DataAttribute
{
    private static readonly JsonSerializerOptions _serializerOptions;

    static ComponentFixtureData()
    {
        _serializerOptions = new JsonSerializerOptions(ComponentOptionsJsonSerializerOptions.Instance);
        _serializerOptions.Converters.Insert(0, new PermissiveStringJsonConverter());
        _serializerOptions.Converters.Add(new StringHtmlContentJsonConverter());
        _serializerOptions.Converters.Add(new AttributeCollectionJsonConverter());
        _serializerOptions.Converters.Add(new TemplateStringJsonConverter());
    }

    private readonly HashSet<string> _exclude = new(exclude);

    public override ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(MethodInfo testMethod, DisposalTracker disposalTracker)
    {
        return new ValueTask<IReadOnlyCollection<ITheoryDataRow>>(Impl().ToArray());

        IEnumerable<ITheoryDataRow> Impl()
        {
            var fixturesFile = Path.Combine("ComponentGeneration", "Fixtures", $"{componentName}.json");

            if (!File.Exists(fixturesFile))
            {
                throw new FileNotFoundException(
                    $"Could not find fixtures file at: '{fixturesFile}'.",
                    fixturesFile);
            }

            var fixturesJson = File.ReadAllText(fixturesFile);
            var fixtures = JsonNode.Parse(fixturesJson)!["fixtures"]?.AsArray() ??
                throw new InvalidOperationException($"Couldn't find fixtures in '{fixturesFile}'.");
            var testCaseDataType = typeof(ComponentTestCaseData<>).MakeGenericType(optionsType);

            foreach (var fixture in fixtures)
            {
                var name = fixture!["name"]!.ToString();

                if (_exclude.Contains(name) || (only is not null && name != only))
                {
                    continue;
                }

                object options;
                try
                {
                    options = fixture["options"]!.Deserialize(optionsType, _serializerOptions)!;
                }
                catch (JsonException e)
                {
                    throw new Exception($"Failed deserializing fixture options for '{name}'.", e);
                }

                var html = fixture["html"]!.ToString();

                var testCaseData = Activator.CreateInstance(testCaseDataType, name, options, html)!;

                yield return new TheoryDataRow(testCaseData);
            }
        }
    }

    public override bool SupportsDiscoveryEnumeration() => true;

    private class PermissiveStringJsonConverter : JsonConverter<string>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.String => reader.GetString(),
                JsonTokenType.Number => reader.GetInt64().ToString(),
                JsonTokenType.Null => null,
                JsonTokenType.False => string.Empty,
                JsonTokenType.None => throw new NotImplementedException(),
                JsonTokenType.StartObject => throw new NotImplementedException(),
                JsonTokenType.EndObject => throw new NotImplementedException(),
                JsonTokenType.StartArray => throw new NotImplementedException(),
                JsonTokenType.EndArray => throw new NotImplementedException(),
                JsonTokenType.PropertyName => throw new NotImplementedException(),
                JsonTokenType.Comment => throw new NotImplementedException(),
                JsonTokenType.True => throw new NotImplementedException(),
                _ => throw new JsonException($"Cannot convert {reader.TokenType} tokens to a string."),
            };
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }
    }

    private class StringHtmlContentJsonConverter : JsonConverter<IHtmlContent>
    {
        public override IHtmlContent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = JsonSerializer.Deserialize<string>(ref reader, options);
            return new HtmlString(str);
        }

        public override void Write(Utf8JsonWriter writer, IHtmlContent value, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }
    }
}
