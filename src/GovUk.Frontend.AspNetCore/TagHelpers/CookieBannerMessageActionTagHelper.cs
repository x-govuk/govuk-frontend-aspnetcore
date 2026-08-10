using System.ComponentModel;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents a button action in the message in a GDS cookie banner component.
/// </summary>
/// <remarks>
/// This element has been replaced by <c>govuk-cookie-banner-message-action-button</c>.
/// </remarks>
[HtmlTargetElement(TagName, ParentTag = CookieBannerMessageActionsTagHelper.TagName, TagStructure = TagStructure.WithoutEndTag)]
[EditorBrowsable(EditorBrowsableState.Never)]
[Obsolete(
    "Use the <" + CookieBannerMessageActionButtonTagHelper.TagName + "> element instead.",
    DiagnosticId = DiagnosticIds.UseCookieBannerMessageActionButtonElementInstead)]
public class CookieBannerMessageActionTagHelper : CookieBannerMessageActionButtonTagHelper
{
    internal new const string TagName = "govuk-cookie-banner-message-action";
}
