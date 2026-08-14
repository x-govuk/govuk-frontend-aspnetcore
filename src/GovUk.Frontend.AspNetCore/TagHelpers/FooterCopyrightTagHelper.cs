using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the copyright information in a GDS footer component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = FooterTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = FooterTagHelper.TagName)]
public class FooterCopyrightTagHelper : TagHelper
{
    internal const string TagName = "govuk-footer-copyright";
    internal const string ShortTagName = ShortTagNames.Copyright;

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];

    /// <inheritdoc />
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var footerContext = context.GetContextItem<FooterContext>();

        footerContext.CheckChildTagNameSpelling(context.TagName);

        if (footerContext.Copyright is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(AllTagNames, FooterTagHelper.TagName);
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

        footerContext.Copyright = new(
            new FooterOptionsCopyright
            {
                Text = null,
                Html = resolvedContent,
                Attributes = attributes
            },
            context.TagName);

        output.SuppressOutput();
    }
}
