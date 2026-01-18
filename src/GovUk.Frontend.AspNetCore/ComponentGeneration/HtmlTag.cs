using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

[DebuggerDisplay("{DebuggerToString()}")]
internal class HtmlTag : IHtmlContent, IEnumerable
{
    private static readonly Func<HtmlContentBuilder, IEnumerable> _getHtmlContentBuilderEntries =
        typeof(HtmlContentBuilder)
            .GetProperty("Entries", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetMethod!
            .CreateDelegate<Func<HtmlContentBuilder, IList<object>>>();

    private readonly HtmlContentBuilder _innerHtml = new();

    public HtmlTag(string tagName) : this(tagName, _ => { })
    {
    }

    public HtmlTag(string tagName, Action<AttributeBuilder> configureAttributes)
    {
        ArgumentNullException.ThrowIfNull(tagName);
        ArgumentNullException.ThrowIfNull(configureAttributes);

        TagName = tagName;
        configureAttributes(new AttributeBuilder(Attributes));
    }

    public AttributeCollection Attributes { get; } = new();

    public IHtmlContentBuilder InnerHtml =>
        TagRenderMode is TagRenderMode.Normal ?
            _innerHtml :
            throw new InvalidOperationException($"{nameof(InnerHtml)} can only be used when TagRenderMode is Normal.");

    public bool HasInnerHtml => _innerHtml.Count > 0;

    public string TagName { get; }

    public TagRenderMode TagRenderMode { get; set; } = TagRenderMode.Normal;

    public void Add(IHtmlContent content)
    {
        ArgumentNullException.ThrowIfNull(content);

        InnerHtml.AppendHtml(content);
    }

    public void Add(string content)
    {
        ArgumentNullException.ThrowIfNull(content);

        InnerHtml.Append(content);
    }

    public void WriteTo(TextWriter writer, HtmlEncoder encoder)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(encoder);

        switch (TagRenderMode)
        {
            case TagRenderMode.StartTag:
                writer.Write("<");
                writer.Write(TagName);
                AppendAttributes(writer, encoder);
                writer.Write(">");
                break;
            case TagRenderMode.EndTag:
                writer.Write("</");
                writer.Write(TagName);
                writer.Write(">");
                break;
            case TagRenderMode.SelfClosing:
                writer.Write("<");
                writer.Write(TagName);
                AppendAttributes(writer, encoder);
                writer.Write(" />");
                break;
            default:
                writer.Write("<");
                writer.Write(TagName);
                AppendAttributes(writer, encoder);
                writer.Write(">");
                _innerHtml?.WriteTo(writer, encoder);
                writer.Write("</");
                writer.Write(TagName);
                writer.Write(">");
                break;
        }
    }

    private void AppendAttributes(TextWriter writer, HtmlEncoder encoder)
    {
        foreach (var attribute in Attributes.GetAttributes())
        {
            if (attribute is { Optional: true, Value: false or null })
            {
                continue;
            }

            writer.Write(' ');
            writer.Write(encoder.Encode(attribute.Name));

            if (!attribute.Optional || attribute.Value is not true)
            {
                writer.Write('=');
                writer.Write('"');
                writer.Write(attribute.GetValueHtmlString(encoder));
                writer.Write('"');
            }
        }
    }

    private string DebuggerToString()
    {
        using var writer = new StringWriter();
        WriteTo(writer, HtmlEncoder.Default);
        return writer.ToString();
    }

    IEnumerator IEnumerable.GetEnumerator() => _getHtmlContentBuilderEntries(_innerHtml).GetEnumerator();

    public class AttributeBuilder
    {
        public AttributeBuilder(AttributeCollection attributes)
        {
            ArgumentNullException.ThrowIfNull(attributes);

            Attributes = attributes;
        }

        public AttributeCollection Attributes { get; }

        public AttributeBuilder With(string name, TemplateString? value)
        {
            ArgumentNullException.ThrowIfNull(name);

            if (value is not null)
            {
                Attributes.Add(name, value.Value);
            }

            return this;
        }

        public AttributeBuilder With(AttributeCollection? other)
        {
            if (other is not null)
            {
                foreach (var a in other.GetAttributes())
                {
                    Attributes.Add(a);
                }
            }

            return this;
        }

        public AttributeBuilder WithBoolean(string name)
        {
            ArgumentNullException.ThrowIfNull(name);

            Attributes.AddBoolean(name);
            return this;
        }

        public AttributeBuilder WithClasses(params TemplateString?[] classes)
        {
            var nonNullClasses = classes.Where(c => c is not null).Select(c => c!.Value).ToArray();
            Attributes.Set(
                "class",
                Attributes["class"].AppendCssClasses(nonNullClasses));

            return this;
        }
    }
}
