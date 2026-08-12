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
    private readonly Type _formActionTagHelper;

    public TagHelperApiProvider(TemplatePublishOptions publishOptions)
    {
        ArgumentNullException.ThrowIfNull(publishOptions);

        _publishOptions = publishOptions;
        _docs = LoadDocs();

        _anchorTagHelper = typeof(GovUkFrontendOptions).Assembly.GetType($"{TagHelperNamespace}.AnchorTagHelper")!;
        _formActionTagHelper = typeof(GovUkFrontendOptions).Assembly.GetType($"{TagHelperNamespace}.FormActionTagHelper")!;
    }

    /// <param name="forShortTagName">
    /// The short name that goes with <paramref name="forTagName"/>, for tag helpers that target more than one
    /// element; it targets a different parent element so it cannot be deduced from <paramref name="forTagName"/>.
    /// </param>
    public TagHelperApi GetTagHelperApi(string tagHelperName, string? forTagName = null, string? forShortTagName = null)
    {
        ArgumentNullException.ThrowIfNull(tagHelperName);

        var tagHelperClassName = $"{TagHelperNamespace}.{tagHelperName}";
        var tagHelperType = typeof(GovUkFrontendOptions).Assembly.GetType(tagHelperClassName) ??
            throw new ArgumentException($"Could not find '{tagHelperClassName}'.", nameof(tagHelperName));
        var htmlTargetElementAttrs = tagHelperType.GetCustomAttributes<HtmlTargetElementAttribute>().ToArray();
        if (htmlTargetElementAttrs.Length == 0)
        {
            throw new ArgumentException($"Could not find HtmlTargetElementAttribute on '{tagHelperClassName}'.", nameof(tagHelperName));
        }

        // Tag helpers that target several elements are documented an element at a time
        if (forTagName is not null)
        {
            var forTagNameAttrs = htmlTargetElementAttrs.Where(e => e.Tag == forTagName).ToArray();

            if (forTagNameAttrs.Length == 0)
            {
                throw new ArgumentException($"Could not find HtmlTargetElementAttribute for '{forTagName}' on '{tagHelperClassName}'.", nameof(forTagName));
            }

            // Components share their short names, so the short name targets several elements too;
            // keep the ones for the component being documented. A govuk- prefixed parent has to be
            // one this element is already inside; a short-named parent is that component's element
            // whichever component it belongs to, since only one of them is in scope in a view.
            var forParentTags = forTagNameAttrs.Select(e => e.ParentTag).OfType<string>().ToArray();

            var forShortTagNameAttrs = forShortTagName is not null ?
                htmlTargetElementAttrs
                    .Where(e => e.Tag == forShortTagName)
                    .Where(e => e.ParentTag is not { } parentTag ||
                        !parentTag.StartsWith("govuk-") ||
                        forParentTags.Contains(parentTag))
                    .ToArray() :
                [];

            if (forShortTagName is not null && forShortTagNameAttrs.Length == 0)
            {
                throw new ArgumentException($"Could not find HtmlTargetElementAttribute for '{forShortTagName}' on '{tagHelperClassName}'.", nameof(forShortTagName));
            }

            htmlTargetElementAttrs = [.. forTagNameAttrs, .. forShortTagNameAttrs];
        }

        var tagName = htmlTargetElementAttrs.Select(e => e.Tag).Distinct().Single(t => t.StartsWith("govuk"));
        var shortTagName = htmlTargetElementAttrs.Select(e => e.Tag).Distinct().SingleOrDefault(t => !t.StartsWith("govuk-"));
        var tagStructure = htmlTargetElementAttrs.Select(e => e.TagStructure).Distinct().Single();

        var documentationAttrs = tagHelperType.GetCustomAttributes<TagHelperDocumentationAttribute>().ToArray();
        var documentationAttr =
            documentationAttrs.SingleOrDefault(a => a.TagName == tagName) ??
            documentationAttrs.SingleOrDefault(a => a.TagName is null);

        var parentTagNames = htmlTargetElementAttrs
            .Select(a => a.ParentTag)
            .OfType<string>()
            .Where(t => t.Length > 0)
            .Distinct()
            .ToArray();

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
                var member = tagHelperType.GetProperty(memberName);
                if (member is null)
                {
                    // Documented non-public member (e.g. an abstract helper property); not an HTML attribute.
                    continue;
                }

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

                var htmlAttributeName = member.GetCustomAttribute<HtmlAttributeNameAttribute>();
                if (htmlAttributeName is null)
                {
                    // Not an HTML attribute (e.g. a convention-bound property without an explicit name).
                    continue;
                }

                var typeName = GetNormalizedTypeName(member.PropertyType);

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

        var canGenerateLinks =
            _anchorTagHelper.GetCustomAttributes<HtmlTargetElementAttribute>().Any(e => e.Tag == tagName) ||
            _formActionTagHelper.GetCustomAttributes<HtmlTargetElementAttribute>().Any(e => e.Tag == tagName);
        if (canGenerateLinks)
        {
            attributes.Add(new("(link attributes)", "", "See [documentation on links](../links.md) for more information."));
        }

        return new TagHelperApi(tagName, shortTagName, attributes, tagStructure, parentTagNames, documentationAttr?.ContentDescription);
    }

    private static XDocument LoadDocs()
    {
        using var fs = File.OpenRead(XmlDocFileName);
        return XDocument.Load(fs);
    }

    private static string GetNormalizedTypeName(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return GetNormalizedTypeName(type.GetGenericArguments()[0]) + "?";
        }

        if (type.IsGenericType)
        {
            var name = type.Name[..type.Name.IndexOf('`')];
            var typeArguments = type.GetGenericArguments().Select(GetNormalizedTypeName);
            return $"{name}<{string.Join(", ", typeArguments)}>";
        }

        return GetNormalizedNonGenericTypeName(type);
    }

    private static string GetNormalizedNonGenericTypeName(Type type)
    {
        return type.FullName switch
        {
            "System.Boolean" => "bool",
            "System.String" => "string",
            "System.Int32" => "int",
            "System.Int64" => "long",
            "System.Int16" => "short",
            "System.Byte" => "byte",
            "System.Double" => "double",
            "System.Single" => "float",
            "System.Decimal" => "decimal",
            "System.Char" => "char",
            "System.Object" => "object",
            _ => type.FullName ?? ""
        };
    }
}

public record TagHelperApi(
    string TagName,
    string? ShortTagName,
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
