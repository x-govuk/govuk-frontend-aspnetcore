using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the copyright information in a GDS footer component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = FooterTagHelper.TagName)]
public class FooterCopyrightTagHelper : TagHelper
{
    internal const string TagName = "govuk-footer-copyright";

    /// <inheritdoc />
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var footerContext = context.GetContextItem<FooterContext>();

        if (footerContext.Copyright is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(output.TagName, FooterTagHelper.TagName);
        }

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);

        footerContext.Copyright = new(
            new FooterOptionsCopyright
            {
                Text = null,
                Html = content.ToTemplateString(),
                Attributes = attributes
            },
            output.TagName);

        output.SuppressOutput();
    }
}
