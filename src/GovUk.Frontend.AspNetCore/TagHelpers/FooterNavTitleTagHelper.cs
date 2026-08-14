using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the title of a navigation section of a GDS footer component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = FooterNavTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = FooterNavTagHelper.ShortTagName)]
public class FooterNavTitleTagHelper : TagHelper
{
    internal const string TagName = "govuk-footer-nav-title";
    internal const string ShortTagName = ShortTagNames.Title;

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];

    /// <inheritdoc />
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var navContext = context.GetContextItem<FooterNavContext>();

        if (navContext.Title is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(AllTagNames, navContext.TagName!);
        }

        if (navContext.Items?.TagName is string itemsTagName)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(context.TagName, itemsTagName);
        }

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);

        navContext.Title = (content.Snapshot(), attributes, context.TagName);

        output.SuppressOutput();
    }
}
