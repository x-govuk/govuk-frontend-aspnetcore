using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// <see cref="ITagHelper"/> implementation targeting &lt;form&gt; elements that adds a <c>novalidate</c> attribute.
/// </summary>
/// <remarks>
/// Creates a <see cref="FormNovalidateTagHelper"/>.
/// </remarks>
[HtmlTargetElement("form")]
public class FormNovalidateTagHelper : TagHelper
{
    private readonly IOptions<GovUkFrontendOptions> _optionsAccessor;

    /// <summary>
    /// Creates a new <see cref="FormNovalidateTagHelper"/>.
    /// </summary>
    public FormNovalidateTagHelper(IOptions<GovUkFrontendOptions> optionsAccessor)
    {
        ArgumentNullException.ThrowIfNull(optionsAccessor);

        _optionsAccessor = optionsAccessor;
    }

    /// <inheritdoc/>
    public override int Order => -1;

    /// <inheritdoc/>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        if (_optionsAccessor.Value.AddNovalidateAttributeToForms)
        {
            if (!output.Attributes.ContainsName("novalidate"))
            {
                output.Attributes.Add(new TagHelperAttribute("novalidate", null, HtmlAttributeValueStyle.Minimized));
            }
        }
    }
}
