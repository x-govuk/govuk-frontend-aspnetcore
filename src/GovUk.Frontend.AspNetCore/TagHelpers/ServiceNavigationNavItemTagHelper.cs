using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
    /// By default, this is determined by comparing the current URL to this item's generated <c>href</c> attribute.
    /// </remarks>
    [HtmlAttributeName(CurrentAttributeName)]
    public bool? Current { get; set; }

    /// <summary>
    /// Gets the <see cref="ViewContext"/> of the executing view.
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    [DisallowNull]
    public ViewContext? ViewContext { get; set; }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

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

        var current = Current ?? ItemIsCurrentPage();

        var item = new ServiceNavigationOptionsNavigationItem()
        {
            Current = current,
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

        bool ItemIsCurrentPage()
        {
            var currentUrl = ViewContext!.HttpContext.Request.GetEncodedPathAndQuery();
            return href?.ToHtmlString(HtmlEncoder.Default) == currentUrl;
        }
    }
}
