using System.Collections;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

/// <summary>
/// Represents a collection of HTML attributes.
/// </summary>
public sealed class AttributeCollection : IEnumerable<KeyValuePair<string, TemplateString?>>
{
    private readonly Dictionary<string, Attribute> _attributes;

    /// <summary>
    /// Initializes a new empty instance of the <see cref="AttributeCollection"/> class.
    /// </summary>
    public AttributeCollection()
    {
        _attributes = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeCollection"/> class
    /// with the attributes in the specified <paramref name="attributes"/>.
    /// </summary>
    /// <param name="attributes">The existing attributes.</param>
    public AttributeCollection(IDictionary<string, string?>? attributes)
    {
        if (attributes is null)
        {
            _attributes = [];
            return;
        }

        _attributes = attributes.ToDictionary(a => a.Key, a => new Attribute(a.Key, a.Value, Optional: false));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeCollection"/> class
    /// with the attributes in the specified <paramref name="tagHelperAttributes"/>.
    /// </summary>
    /// <param name="tagHelperAttributes">The <see cref="TagHelperAttribute" />s to copy attributes from.</param>
    public AttributeCollection(IEnumerable<TagHelperAttribute> tagHelperAttributes)
    {
        ArgumentNullException.ThrowIfNull(tagHelperAttributes);

        _attributes = [];

        foreach (var tagHelperAttribute in tagHelperAttributes)
        {
            var attribute = new Attribute(
                tagHelperAttribute.Name,
                tagHelperAttribute.Value,
                Optional: tagHelperAttribute.ValueStyle is HtmlAttributeValueStyle.Minimized);

            _attributes.Add(attribute.Name, attribute);
        }
    }

    internal TemplateString? this[string name]
    {
        get
        {
            if (_attributes.TryGetValue(name, out var attribute))
            {
                return attribute.Value as TemplateString? ?? attribute.Value?.ToString();
            }

            return null;
        }
    }

    internal AttributeCollection(IEnumerable<Attribute> attributes)
    {
        ArgumentNullException.ThrowIfNull(attributes);

        _attributes = attributes.ToDictionary(a => a.Name, a => a);
    }

    internal AttributeCollection(params Attribute[] attributes) : this(attributes.AsEnumerable())
    {
    }

    internal void Add(Attribute attribute)
    {
        ArgumentNullException.ThrowIfNull(attribute);

        _attributes.Add(attribute.Name, attribute);
    }

    internal void Add(string name, TemplateString? templateString)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(templateString);

        var attribute = new Attribute(name, templateString.Value, Optional: false);
        _attributes.Add(name, attribute);
    }

    internal void AddBoolean(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        var attribute = new Attribute(name, true, Optional: true);
        _attributes.Add(name, attribute);
    }

    internal void Set(string name, TemplateString? templateString)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(templateString);

        var attribute = new Attribute(name, templateString.Value, Optional: false);
        _attributes[name] = attribute;
    }

    /// <summary>
    /// Creates a new <see cref="AttributeCollection"/> with a copy of the attributes from this instance.
    /// </summary>
    /// <returns>A new <see cref="AttributeCollection"/>.</returns>
    public AttributeCollection Clone() =>
        new(_attributes.Values.Select(a => new Attribute(a.Name, a.Value, a.Optional)));

    internal IReadOnlyCollection<Attribute> GetAttributes() => _attributes.Values;

    internal bool Remove(string name, out TemplateString? value)
    {
        if (_attributes.Remove(name, out var attribute))
        {
            value = attribute.Value as TemplateString? ?? attribute.Value?.ToString();
            return true;
        }

        value = null;
        return false;
    }

    internal IEnumerable<TagHelperAttribute> ToTagHelperAttributes()
    {
        foreach (var attribute in _attributes.Values)
        {
            if (attribute is { Optional: true, Value: true })
            {
                yield return new TagHelperAttribute(attribute.Name, null, HtmlAttributeValueStyle.Minimized);
            }
            else if (attribute.Value is not null)
            {
                yield return new TagHelperAttribute(attribute.Name, attribute.Value, HtmlAttributeValueStyle.DoubleQuotes);
            }
        }
    }

    internal sealed record Attribute(string Name, object? Value, bool Optional)
    {
        public string GetValueHtmlString(HtmlEncoder encoder)
        {
            ArgumentNullException.ThrowIfNull(encoder);

            return Value switch
            {
                TemplateString templateString => templateString.ToHtmlString(encoder),
                IHtmlContent htmlContent => htmlContent.ToHtmlString(encoder),
                true => "true",
                false => "false",
                var other => other?.ToString() ?? string.Empty
            };
        }
    }

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    public IEnumerator<KeyValuePair<string, TemplateString?>> GetEnumerator()
    {
        foreach (var attribute in _attributes.Values)
        {
            if (attribute.Optional)
            {
                if (attribute.Value is true)
                {
                    yield return KeyValuePair.Create(attribute.Name, (TemplateString?)null);
                }

                continue;
            }

            yield return KeyValuePair.Create(
                attribute.Name,
                attribute.Value as TemplateString? ?? attribute.Value?.ToString());
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
