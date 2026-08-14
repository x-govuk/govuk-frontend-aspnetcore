using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// <see cref="ITagHelper"/> implementation targeting &lt;button&gt; elements.
/// </summary>
/// <inheritdoc />
[HtmlTargetElement(ButtonTagHelper.TagName, Attributes = "asp-action")]
[HtmlTargetElement(ButtonTagHelper.TagName, Attributes = "asp-controller")]
[HtmlTargetElement(ButtonTagHelper.TagName, Attributes = "asp-area")]
[HtmlTargetElement(ButtonTagHelper.TagName, Attributes = "asp-page")]
[HtmlTargetElement(ButtonTagHelper.TagName, Attributes = "asp-page-handler")]
[HtmlTargetElement(ButtonTagHelper.TagName, Attributes = "asp-fragment")]
[HtmlTargetElement(ButtonTagHelper.TagName, Attributes = "asp-route")]
[HtmlTargetElement(ButtonTagHelper.TagName, Attributes = "asp-all-route-data")]
[HtmlTargetElement(ButtonTagHelper.TagName, Attributes = "asp-route-*")]
[HtmlTargetElement(CookieBannerMessageActionButtonTagHelper.TagName, Attributes = "asp-action")]
[HtmlTargetElement(CookieBannerMessageActionButtonTagHelper.TagName, Attributes = "asp-controller")]
[HtmlTargetElement(CookieBannerMessageActionButtonTagHelper.TagName, Attributes = "asp-area")]
[HtmlTargetElement(CookieBannerMessageActionButtonTagHelper.TagName, Attributes = "asp-page")]
[HtmlTargetElement(CookieBannerMessageActionButtonTagHelper.TagName, Attributes = "asp-page-handler")]
[HtmlTargetElement(CookieBannerMessageActionButtonTagHelper.TagName, Attributes = "asp-fragment")]
[HtmlTargetElement(CookieBannerMessageActionButtonTagHelper.TagName, Attributes = "asp-route")]
[HtmlTargetElement(CookieBannerMessageActionButtonTagHelper.TagName, Attributes = "asp-all-route-data")]
[HtmlTargetElement(CookieBannerMessageActionButtonTagHelper.TagName, Attributes = "asp-route-*")]
[HtmlTargetElement(CookieBannerMessageActionButtonTagHelper.ShortTagName, ParentTag = CookieBannerMessageActionsTagHelper.ShortTagName, Attributes = "asp-action")]
[HtmlTargetElement(CookieBannerMessageActionButtonTagHelper.ShortTagName, ParentTag = CookieBannerMessageActionsTagHelper.ShortTagName, Attributes = "asp-controller")]
[HtmlTargetElement(CookieBannerMessageActionButtonTagHelper.ShortTagName, ParentTag = CookieBannerMessageActionsTagHelper.ShortTagName, Attributes = "asp-area")]
[HtmlTargetElement(CookieBannerMessageActionButtonTagHelper.ShortTagName, ParentTag = CookieBannerMessageActionsTagHelper.ShortTagName, Attributes = "asp-page")]
[HtmlTargetElement(CookieBannerMessageActionButtonTagHelper.ShortTagName, ParentTag = CookieBannerMessageActionsTagHelper.ShortTagName, Attributes = "asp-page-handler")]
[HtmlTargetElement(CookieBannerMessageActionButtonTagHelper.ShortTagName, ParentTag = CookieBannerMessageActionsTagHelper.ShortTagName, Attributes = "asp-fragment")]
[HtmlTargetElement(CookieBannerMessageActionButtonTagHelper.ShortTagName, ParentTag = CookieBannerMessageActionsTagHelper.ShortTagName, Attributes = "asp-route")]
[HtmlTargetElement(CookieBannerMessageActionButtonTagHelper.ShortTagName, ParentTag = CookieBannerMessageActionsTagHelper.ShortTagName, Attributes = "asp-all-route-data")]
[HtmlTargetElement(CookieBannerMessageActionButtonTagHelper.ShortTagName, ParentTag = CookieBannerMessageActionsTagHelper.ShortTagName, Attributes = "asp-route-*")]
#pragma warning disable GFA0006 // Type or member is obsolete
[HtmlTargetElement(CookieBannerMessageActionTagHelper.TagName, Attributes = "asp-action")]
[HtmlTargetElement(CookieBannerMessageActionTagHelper.TagName, Attributes = "asp-controller")]
[HtmlTargetElement(CookieBannerMessageActionTagHelper.TagName, Attributes = "asp-area")]
[HtmlTargetElement(CookieBannerMessageActionTagHelper.TagName, Attributes = "asp-page")]
[HtmlTargetElement(CookieBannerMessageActionTagHelper.TagName, Attributes = "asp-page-handler")]
[HtmlTargetElement(CookieBannerMessageActionTagHelper.TagName, Attributes = "asp-fragment")]
[HtmlTargetElement(CookieBannerMessageActionTagHelper.TagName, Attributes = "asp-route")]
[HtmlTargetElement(CookieBannerMessageActionTagHelper.TagName, Attributes = "asp-all-route-data")]
[HtmlTargetElement(CookieBannerMessageActionTagHelper.TagName, Attributes = "asp-route-*")]
#pragma warning restore GFA0006 // Type or member is obsolete
[HtmlTargetElement(PanelActionButtonTagHelper.TagName, Attributes = "asp-action")]
[HtmlTargetElement(PanelActionButtonTagHelper.TagName, Attributes = "asp-controller")]
[HtmlTargetElement(PanelActionButtonTagHelper.TagName, Attributes = "asp-area")]
[HtmlTargetElement(PanelActionButtonTagHelper.TagName, Attributes = "asp-page")]
[HtmlTargetElement(PanelActionButtonTagHelper.TagName, Attributes = "asp-page-handler")]
[HtmlTargetElement(PanelActionButtonTagHelper.TagName, Attributes = "asp-fragment")]
[HtmlTargetElement(PanelActionButtonTagHelper.TagName, Attributes = "asp-route")]
[HtmlTargetElement(PanelActionButtonTagHelper.TagName, Attributes = "asp-all-route-data")]
[HtmlTargetElement(PanelActionButtonTagHelper.TagName, Attributes = "asp-route-*")]
[HtmlTargetElement(PanelActionButtonTagHelper.ShortTagName, ParentTag = PanelActionsTagHelper.ShortTagName, Attributes = "asp-action")]
[HtmlTargetElement(PanelActionButtonTagHelper.ShortTagName, ParentTag = PanelActionsTagHelper.ShortTagName, Attributes = "asp-controller")]
[HtmlTargetElement(PanelActionButtonTagHelper.ShortTagName, ParentTag = PanelActionsTagHelper.ShortTagName, Attributes = "asp-area")]
[HtmlTargetElement(PanelActionButtonTagHelper.ShortTagName, ParentTag = PanelActionsTagHelper.ShortTagName, Attributes = "asp-page")]
[HtmlTargetElement(PanelActionButtonTagHelper.ShortTagName, ParentTag = PanelActionsTagHelper.ShortTagName, Attributes = "asp-page-handler")]
[HtmlTargetElement(PanelActionButtonTagHelper.ShortTagName, ParentTag = PanelActionsTagHelper.ShortTagName, Attributes = "asp-fragment")]
[HtmlTargetElement(PanelActionButtonTagHelper.ShortTagName, ParentTag = PanelActionsTagHelper.ShortTagName, Attributes = "asp-route")]
[HtmlTargetElement(PanelActionButtonTagHelper.ShortTagName, ParentTag = PanelActionsTagHelper.ShortTagName, Attributes = "asp-all-route-data")]
[HtmlTargetElement(PanelActionButtonTagHelper.ShortTagName, ParentTag = PanelActionsTagHelper.ShortTagName, Attributes = "asp-route-*")]
public class FormActionTagHelper(IUrlHelperFactory urlHelperFactory) : Microsoft.AspNetCore.Mvc.TagHelpers.FormActionTagHelper(urlHelperFactory)
{
}
