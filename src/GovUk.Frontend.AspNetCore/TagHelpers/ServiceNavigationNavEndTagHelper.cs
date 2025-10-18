using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the content after the last list item in the navigation list in a GDS service navigation component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = ServiceNavigationNavTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(TagName, ParentTag = ServiceNavigationNavTagHelper.ShortTagName)]
[HtmlTargetElement(ShortTagName, ParentTag = ServiceNavigationNavTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = ServiceNavigationNavTagHelper.ShortTagName)]
#endif
[TagHelperDocumentation(ContentDescription = "The content is the HTML after the last list item in the navigation list.")]
public class ServiceNavigationNavEndTagHelper : TagHelper
{
    internal const string TagName = "govuk-service-navigation-nav-end";
#if SHORT_TAG_NAMES
    internal const string ShortTagName = ShortTagNames.End;
#endif

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [
        TagName
#if SHORT_TAG_NAMES
        ,
        ShortTagName
#endif
    ];

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var navContext = context.GetContextItem<ServiceNavigationNavContext>();

        if (navContext.NavigationEndSlot is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(AllTagNames, [ServiceNavigationNavTagHelper.TagName]);
        }

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        if (output.Attributes.Any())
        {
            throw ExceptionHelper.AttributesNotSupported();
        }

        navContext.NavigationEndSlot = (content.ToTemplateString(), output.TagName);

        output.SuppressOutput();
    }
}
