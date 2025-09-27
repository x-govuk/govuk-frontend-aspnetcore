using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the content before the first list item in the navigation list in a GDS service navigation component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = ServiceNavigationNavTagHelper.TagName)]
//[HtmlTargetElement(TagName, ParentTag = ServiceNavigationNavTagHelper.ShortTagName)]
//[HtmlTargetElement(ShortTagName, ParentTag = ServiceNavigationNavTagHelper.TagName)]
//[HtmlTargetElement(ShortTagName, ParentTag = ServiceNavigationNavTagHelper.ShortTagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML before the first list item in the navigation list.")]
public class ServiceNavigationNavStartTagHelper : TagHelper
{
    internal const string TagName = "govuk-service-navigation-nav-start";
    //internal const string ShortTagName = "start";

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var navContext = context.GetContextItem<ServiceNavigationNavContext>();

        if (navContext.NavigationStartSlot is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn([TagName/*, ShortTagName*/], [ServiceNavigationNavTagHelper.TagName]);
        }

        if (navContext.Items.Count > 0)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(output.TagName, navContext.FirstItemTagName!);
        }

        if (navContext.NavigationEndSlot is var (_, endTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(output.TagName, endTagName);
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

        navContext.NavigationStartSlot = (content.ToTemplateString(), output.TagName);

        output.SuppressOutput();
    }
}
