using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

/// <summary>
/// Represents a GDS component.
/// </summary>
public abstract class GovUkComponent
{
    /// <summary>
    /// Gets the HTML for the generated component.
    /// </summary>
    public abstract string GetHtml();

    /// <summary>
    /// Applies the generated component to the specified <see cref="TagHelperOutput"/>.
    /// </summary>
    public virtual void ApplyToTagHelper(TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(output);

        TagHelperAdapter.ApplyComponentHtml(output, GetHtml());
    }
}
