using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

/// <summary>
/// Represents a GDS component.
/// </summary>
public abstract class GovUkComponent : IHtmlContent
{
    /// <summary>
    /// Gets the content for the generated component.
    /// </summary>
    public abstract IHtmlContent GetContent();

    /// <summary>
    /// Gets the HTML for the generated component.
    /// </summary>
    public virtual string GetHtml(HtmlEncoder? encoder = null)
    {
        using var writer = new StringWriter();
        GetContent().WriteTo(writer, encoder ?? HtmlEncoder.Default);
        return writer.ToString();
    }

    /// <summary>
    /// Applies the generated component to the specified <see cref="TagHelperOutput"/>.
    /// </summary>
    public abstract void ApplyToTagHelper(TagHelperOutput output);

    /// <inheritdoc/>
    public virtual void WriteTo(TextWriter writer, HtmlEncoder encoder) => GetContent().WriteTo(writer, encoder);
}
