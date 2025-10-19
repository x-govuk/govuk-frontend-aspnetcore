using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Docs;

public class TagHelperApiProvider
{
    private const string XmlDocFileName = "GovUk.Frontend.AspNetCore.xml";
    private const string TagHelperNamespace = "GovUk.Frontend.AspNetCore.TagHelpers";

    private readonly TemplatePublishOptions _publishOptions;
    private readonly XDocument _docs;
    private readonly Type _anchorTagHelper;

    public TagHelperApiProvider(TemplatePublishOptions publishOptions)
    {
        ArgumentNullException.ThrowIfNull(publishOptions);

        _publishOptions = publishOptions;
        _docs = LoadDocs();

        _anchorTagHelper = typeof(GovUkFrontendOptions).Assembly.GetType($"{TagHelperNamespace}.AnchorTagHelper")!;
    }

    public TagHelperApi GetTagHelperApi(string tagHelperName)
    {
        ArgumentNullException.ThrowIfNull(tagHelperName);

        var tagHelperClassName = $"{TagHelperNamespace}.{tagHelperName}";
        var tagHelperType = typeof(GovUkFrontendOptions).Assembly.GetType(tagHelperClassName) ??
            throw new ArgumentException($"Could not find '{tagHelperClassName}'.", nameof(tagHelperName));
        var htmlTargetElementAttr = tagHelperType.GetCustomAttribute<HtmlTargetElementAttribute>()!;
        var tagName = htmlTargetElementAttr.Tag;
        var tagStructure = htmlTargetElementAttr.TagStructure;

        var documentationAttr = tagHelperType.GetCustomAttribute<TagHelperDocumentationAttribute>();

        string[] parentTagNames = htmlTargetElementAttr.ParentTag is string parentTag ? [parentTag] : [];

        IEnumerable<TagHelperApiAttribute> GetAttributesForType(Type type)
        {
            var className = type.FullName!;

            var tagHelperMembers = _docs.Root!
                .Element("members")!
                .Elements("member")
                .Where(m => m.Attribute("name")!.Value.StartsWith($"P:{className}"));

            foreach (var m in tagHelperMembers)
            {
                var memberName = m.Attribute("name")!.Value[(className.Length + 3)..];
                var member = tagHelperType.GetProperty(memberName)!;

                if (member.GetCustomAttribute<ViewContextAttribute>() is not null)
                {
                    continue;
                }

                if (member.GetCustomAttribute<ObsoleteAttribute>() is not null)
                {
                    continue;
                }

                if (member.GetCustomAttribute<EditorBrowsableAttribute>() is { State: EditorBrowsableState.Never })
                {
                    continue;
                }

                var typeName = GetNormalizedTypeName(member.PropertyType);

                var htmlAttributeName = member.GetCustomAttribute<HtmlAttributeNameAttribute>()!;
                var attributeName = htmlAttributeName.Name;

                if (attributeName is null)
                {
                    attributeName = htmlAttributeName.DictionaryAttributePrefix + "*";
                    typeName = "";
                }

                var description = m.Element("summary")?.GetElementValueAsMarkdown() ?? "";

                if (m.Element("remarks")?.GetElementValueAsMarkdown() is string remarks)
                {
                    description += " " + remarks;
                }

                yield return new TagHelperApiAttribute(attributeName, typeName, description);
            }

            if (type.BaseType is not null)
            {
                foreach (var baseAttribute in GetAttributesForType(type.BaseType))
                {
                    yield return baseAttribute;
                }
            }
        }

        var attributes = GetAttributesForType(tagHelperType)
            .OrderBy(a => a.Name)
            .ToList();

        var canGenerateLinks = _anchorTagHelper.GetCustomAttributes<HtmlTargetElementAttribute>().Any(e => e.Tag == tagName);
        if (canGenerateLinks)
        {
            attributes.Add(new("(link attributes)", "", "See [documentation on links](../links.md) for more information."));
        }

        return new TagHelperApi(tagName, attributes, tagStructure, parentTagNames, documentationAttr?.ContentDescription);
    }

    private static XDocument LoadDocs()
    {
        using var fs = File.OpenRead(XmlDocFileName);
        return XDocument.Load(fs);
    }

    private static string GetNormalizedTypeName(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)
            ? GetNormalizedTypeName(type.GetGenericArguments()[0]) + "?"
            : type.FullName switch
            {
                "System.Boolean" => "bool",
                "System.String" => "string",
                _ => type.FullName ?? ""
            };
    }
}

public record TagHelperApi(
    string TagName,
    IReadOnlyCollection<TagHelperApiAttribute> Attributes,
    TagStructure TagStructure,
    string[] ParentTagNames,
    string? ContentDescription);

public record TagHelperApiAttribute(string Name, string Type, string Description);

file static class Extensions
{
    private static readonly Regex _whitespace = new("\\s+");

    public static string GetElementValueAsMarkdown(this XElement element)
    {
        var sb = new StringBuilder();

        foreach (var node in element.Nodes())
        {
            VisitNode(node);
        }

        return sb.ToString().Trim();

        void VisitNode(XNode node)
        {
            if (node is XText text)
            {
                sb.Append(_whitespace.Replace(text.Value, " "));
            }
            else if (node is XElement e)
            {
                if (e.Name == "c")
                {
                    sb.Append($"`{e.Value}`");
                }
                else if (e.Name == "see" && e.Attribute("langword")?.Value is string langword)
                {
                    sb.Append($"`{langword}`");
                }
                else if (e.Name == "see" && e.Attribute("cref")?.Value is string cref)
                {
                    var typeName = cref.StartsWith("T:") ? cref[2..] : cref;
                    var shortTypeName = typeName.Split('.').Last();
                    sb.Append($"`{shortTypeName}`");
                }
                else if (e.Name == "para")
                {
                    sb.Append(' ');
                    foreach (var child in e.Nodes())
                    {
                        VisitNode(child);
                    }
                }
                else
                {
                    throw new NotSupportedException($"Cannot convert a {e.Name} element into markdown.");
                }
            }
            else
            {
                throw new NotSupportedException($"Cannot convert a {node.NodeType} node into markdown.");
            }
        }
    }
}
