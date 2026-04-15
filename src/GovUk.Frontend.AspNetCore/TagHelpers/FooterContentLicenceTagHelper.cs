using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the content licence information of a GDS footer component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = FooterTagHelper.TagName)]
public class FooterContentLicenceTagHelper : TagHelper
{
    internal const string TagName = "govuk-footer-content-licence";

    /// <inheritdoc />
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var footerContext = context.GetContextItem<FooterContext>();

        if (footerContext.ContentLicence is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(context.TagName, FooterTagHelper.TagName);
        }

        if (footerContext.Copyright?.TagName is string copyrightTagName)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(context.TagName, copyrightTagName);
        }

        TemplateString? resolvedContent = null;

        if (output.TagMode == TagMode.StartTagAndEndTag)
        {
            var content = await output.GetChildContentAsync();

            if (output.Content.IsModified)
            {
                content = output.Content;
            }

            resolvedContent = content.ToTemplateString();
        }

        var attributes = new AttributeCollection(output.Attributes);

        footerContext.ContentLicence = new(
            new FooterOptionsContentLicence
            {
                Text = null,
                Html = resolvedContent,
                Attributes = attributes
            },
            context.TagName);

        output.SuppressOutput();
    }
}
