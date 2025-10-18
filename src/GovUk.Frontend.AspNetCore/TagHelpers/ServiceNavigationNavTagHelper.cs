using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the navigation list in a GDS service navigation component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = ServiceNavigationTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(ShortTagName, ParentTag = ServiceNavigationTagHelper.TagName)]
#endif
[RestrictChildren(
    ServiceNavigationNavStartTagHelper.TagName,
    ServiceNavigationNavItemTagHelper.TagName,
    ServiceNavigationNavEndTagHelper.TagName
#if SHORT_TAG_NAMES
    ,
    ServiceNavigationNavStartTagHelper.ShortTagName,
    ServiceNavigationNavItemTagHelper.ShortTagName,
    ServiceNavigationNavEndTagHelper.ShortTagName
#endif
    )]
public class ServiceNavigationNavTagHelper : TagHelper
{
    internal const string TagName = "govuk-service-navigation-nav";
#if SHORT_TAG_NAMES
    internal const string ShortTagName = ShortTagNames.Nav;
#endif

    private const string AriaLabelAttributeName = "aria-label";
    private const string CollapseNavigationOnMobileAttributeName = "collapse-navigation-on-mobile";
    private const string MenuButtonTextAttributeName = "menu-button-text";
    private const string MenuButtonLabelAttributeName = "menu-button-label";
    private const string LabelAttributeName = "label";
    private const string IdAttributeName = "id";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [
        TagName
#if SHORT_TAG_NAMES
        ,
        ShortTagName
#endif
    ];

    /// <summary>
    /// The text for the <c>aria-label</c> which labels the service navigation container when a service name is included.
    /// </summary>
    /// <remarks>
    /// If not specified, &quot;Service information&quot; will be used.
    /// </remarks>
    [HtmlAttributeName(AriaLabelAttributeName)]
    public string? AriaLabel { get; set; }

    /// <summary>
    /// Whether the navigation should be collapsed inside a menu on mobile.
    /// </summary>
    /// <remarks>
    /// If not specified, the navigation will be collapsed on mobile if there is more than one navigation item.
    /// </remarks>
    [HtmlAttributeName(CollapseNavigationOnMobileAttributeName)]
    public bool? CollapseNavigationOnMobile { get; set; }

    /// <summary>
    /// The text of the mobile navigation menu toggle.
    /// </summary>
    [HtmlAttributeName(MenuButtonTextAttributeName)]
    public string? MenuButtonText { get; set; }

    /// <summary>
    /// The screen reader label for the mobile navigation menu toggle.
    /// </summary>
    /// <remarks>
    /// If not specified, the value of the <c>menu-button-text</c> attribute will be used.
    /// </remarks>
    [HtmlAttributeName(MenuButtonLabelAttributeName)]
    public string? MenuButtonLabel { get; set; }

    /// <summary>
    /// The screen reader label for the mobile navigation menu.
    /// </summary>
    /// <remarks>
    /// If not specified, the value of the <c>menu-button-text</c> attribute will be used.
    /// </remarks>
    [HtmlAttributeName(LabelAttributeName)]
    public string? Label { get; set; }

    /// <summary>
    /// The ID used to associate the mobile navigation toggle with the navigation menu.
    /// </summary>
    /// <remarks>
    /// If not specified, <c>navigation</c> will be used.
    /// </remarks>
    [HtmlAttributeName(IdAttributeName)]
    public string? Id { get; set; }

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        context.SetContextItem(new ServiceNavigationNavContext());
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var serviceNavigationContext = context.GetContextItem<ServiceNavigationContext>();
        var navContext = context.GetContextItem<ServiceNavigationNavContext>();

        if (serviceNavigationContext.Nav is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(AllTagNames, [ServiceNavigationTagHelper.TagName]);
        }

        if (serviceNavigationContext.EndSlot is var (_, endSlotTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(output.TagName, endSlotTagName);
        }

        navContext.TagName = output.TagName;

        await output.GetChildContentAsync();

        var attributes = new AttributeCollection(output.Attributes);

        navContext.AriaLabel = AriaLabel;
        navContext.MenuButtonText = MenuButtonText;
        navContext.MenuButtonLabel = MenuButtonLabel;
        navContext.Label = Label;
        navContext.Id = Id;
        navContext.CollapseNavigationOnMobile = CollapseNavigationOnMobile;
        navContext.Attributes = attributes;

        serviceNavigationContext.Nav = navContext;

        output.SuppressOutput();
    }
}
