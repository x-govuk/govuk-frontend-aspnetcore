using System.Reflection;
using System.Xml.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Docs;

public class TagHelperApiProvider
{
    private const string XmlDocFileName = "GovUk.Frontend.AspNetCore.xml";
    private const string TagHelperNamespace = "GovUk.Frontend.AspNetCore.TagHelpers";

    private readonly TemplatePublishOptions _publishOptions;
    private readonly XDocument _docs;

    public TagHelperApiProvider(TemplatePublishOptions publishOptions)
    {
        ArgumentNullException.ThrowIfNull(publishOptions);

        _publishOptions = publishOptions;
        _docs = LoadDocs();
    }

    public TagHelperApi GetTagHelperApi(string tagHelperName)
    {
        ArgumentNullException.ThrowIfNull(tagHelperName);

        var tagHelperClassName = $"{TagHelperNamespace}.{tagHelperName}";
        var tagHelperType = typeof(GovUkFrontendOptions).Assembly.GetType(tagHelperClassName)!;

        var tagName = tagHelperType.GetCustomAttribute<HtmlTargetElementAttribute>()!.Tag;

        var tagHelperMembers = _docs.Root!
            .Element("members")!
            .Elements("member")
            .Where(m => m.Attribute("name")!.Value.StartsWith($"P:{tagHelperClassName}"));

        var properties = tagHelperMembers
            .Select(m =>
            {
                var memberName = m.Attribute("name")!.Value[(tagHelperClassName.Length + 3)..];
                var member = tagHelperType.GetProperty(memberName)!;

                var typeName = GetNormalizedTypeName(member.PropertyType);

                var htmlAttributeName = member.GetCustomAttribute<HtmlAttributeNameAttribute>()!;
                var attributeName = htmlAttributeName.Name;
                if (attributeName is null)
                {
                    attributeName = htmlAttributeName.DictionaryAttributePrefix + "*";
                    typeName = "";
                }

                var description = m.Element("summary")?.Value.Trim() ?? "";
                // TODO format and add remarks

                return new TagHelperApiAttribute(attributeName, typeName, description);
            })
            .OrderBy(m => m.Name)
            .ToArray();

        return new TagHelperApi(tagName, properties);
    }

    private static XDocument LoadDocs()
    {
        using var fs = File.OpenRead(XmlDocFileName);
        return XDocument.Load(fs);
    }

    private static string GetNormalizedTypeName(Type type) =>
        type.FullName switch
        {
            "System.String" => "string",
            _ => type.FullName ?? ""
        };
}

public record TagHelperApi(string TagName, IReadOnlyCollection<TagHelperApiAttribute> Attributes);

public record TagHelperApiAttribute(string Name, string Type, string Description);
