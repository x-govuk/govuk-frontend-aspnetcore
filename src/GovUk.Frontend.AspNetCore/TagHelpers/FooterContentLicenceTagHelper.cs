using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the content licence information of a GDS footer component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = FooterTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = FooterTagHelper.TagName)]
public class FooterContentLicenceTagHelper : TagHelper
{
    internal const string TagName = "govuk-footer-content-licence";
    internal const string ShortTagName = ShortTagNames.ContentLicence;

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];

    /// <inheritdoc />
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var footerContext = context.GetContextItem<FooterContext>();

        footerContext.CheckChildTagNameSpelling(context.TagName);

        if (footerContext.ContentLicence is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(AllTagNames, FooterTagHelper.TagName);
        }

        if (footerContext.Copyright?.TagName is string copyrightTagName)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(context.TagName, copyrightTagName);
        }

        IHtmlContent? resolvedContent = null;

        if (output.TagMode == TagMode.StartTagAndEndTag)
        {
            var content = await output.GetChildContentAsync();

            if (output.Content.IsModified)
            {
                content = output.Content;
            }

            resolvedContent = content.Snapshot();
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
