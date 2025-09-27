using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the content after the last list item in the navigation list in a GDS service navigation component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = ServiceNavigationNavTagHelper.TagName)]
//[HtmlTargetElement(TagName, ParentTag = ServiceNavigationNavTagHelper.ShortTagName)]
//[HtmlTargetElement(ShortTagName, ParentTag = ServiceNavigationNavTagHelper.TagName)]
//[HtmlTargetElement(ShortTagName, ParentTag = ServiceNavigationNavTagHelper.ShortTagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML after the last list item in the navigation list.")]
public class ServiceNavigationNavEndTagHelper : TagHelper
{
    internal const string TagName = "govuk-service-navigation-nav-end";
    //internal const string ShortTagName = ShortTagNames.End;

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var navContext = context.GetContextItem<ServiceNavigationNavContext>();

        if (navContext.NavigationEndSlot is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn([TagName/*, ShortTagName*/], [ServiceNavigationNavTagHelper.TagName]);
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
