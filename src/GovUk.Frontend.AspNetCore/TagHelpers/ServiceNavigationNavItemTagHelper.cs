using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents a navigation item in a GDS service navigation component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = ServiceNavigationNavTagHelper.TagName)]
//[HtmlTargetElement(TagName, ParentTag = ServiceNavigationNavTagHelper.ShortTagName)]
//[HtmlTargetElement(ShortTagName, ParentTag = ServiceNavigationNavTagHelper.TagName)]
//[HtmlTargetElement(ShortTagName, ParentTag = ServiceNavigationNavTagHelper.ShortTagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the generated service navigation item.")]
public class ServiceNavigationNavItemTagHelper : TagHelper
{
    internal const string TagName = "govuk-service-navigation-nav-item";
    //internal const string ShortTagName = ShortTagNames.Item;

    private const string ActiveAttributeName = "active";
    private const string CurrentAttributeName = "current";

    /// <summary>
    /// Whether the user is within this group of pages in the navigation hierarchy.
    /// </summary>
    [HtmlAttributeName(ActiveAttributeName)]
    public bool? Active { get; set; }

    /// <summary>
    /// Whether the user is currently on this page.
    /// </summary>
    /// <remarks>
    /// This takes precedence over the <c>active</c> attribute.
    /// </remarks>
    [HtmlAttributeName(CurrentAttributeName)]
    public bool? Current { get; set; }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var navContext = context.GetContextItem<ServiceNavigationNavContext>();

        if (navContext.NavigationEndSlot is var (_, navigationEndTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(output.TagName, navigationEndTagName);
        }

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("href", out _);
        var href = output.GetUrlAttribute("href");

        var item = new ServiceNavigationOptionsNavigationItem()
        {
            Current = Current,
            Active = Active,
            Html = content.ToTemplateString(),
            Text = null,
            Href = href,
            Attributes = attributes
        };

        if (navContext.Items.Count == 0)
        {
            navContext.FirstItemTagName = output.TagName;
        }

        navContext.Items.Add(item);

        output.SuppressOutput();
    }
}
