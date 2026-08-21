using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Views;

/// <summary>
/// <see cref="ITagHelper"/> implementation that copies the attributes in an <see cref="AttributeDictionary"/>
/// onto the element it's applied to.
/// </summary>
/// <remarks>
/// <para>
/// The page template gets these attributes from <c>ViewData</c>, so their names aren't known until the view runs.
/// Writing them with an expression in the element's attribute declaration area isn't allowed on an element that any
/// tag helper targets, and escaping the element name to opt out of tag helpers stops those tag helpers running at
/// all. Binding the entire dictionary to a tag helper attribute keeps the element a regular element that other tag
/// helpers still see.
/// </para>
/// <para>
/// This exists for _GovUkPageTemplate rather than for applications to use. Razor only discovers public tag helpers,
/// so it can't be internal; it's hidden from IntelliSense instead, its attribute is named with a leading underscore to
/// mark it private, and it does nothing unless an element carries that attribute.
/// </para>
/// </remarks>
[EditorBrowsable(EditorBrowsableState.Never)]
[HtmlTargetElement(Attributes = AdditionalAttributesAttributeName)]
public class AdditionalAttributesTagHelper : TagHelper
{
    internal const string AdditionalAttributesAttributeName = "_govuk-additional-attributes";

    /// <inheritdoc/>
    /// <remarks>
    /// Runs before any other tag helper on the element so that they see the attributes that were added here.
    /// </remarks>
    public override int Order => int.MinValue;

    /// <summary>
    /// The attributes to add to the element.
    /// </summary>
    [HtmlAttributeName(AdditionalAttributesAttributeName)]
    public AttributeDictionary? AdditionalAttributes { get; set; }

    /// <inheritdoc/>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(output);

        if (AdditionalAttributes is null)
        {
            return;
        }

        foreach (var attribute in AdditionalAttributes)
        {
            output.Attributes.SetAttribute(attribute.Value is null ?
                new TagHelperAttribute(attribute.Key, value: null, HtmlAttributeValueStyle.Minimized) :
                new TagHelperAttribute(attribute.Key, attribute.Value));
        }
    }
}
