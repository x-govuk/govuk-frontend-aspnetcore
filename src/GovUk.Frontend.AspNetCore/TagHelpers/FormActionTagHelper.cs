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
