using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class CookieBannerMessageContext
{
    public (IHtmlContent? Html, string TagName, AttributeCollection Attributes)? Heading { get; set; }
    public (IHtmlContent? Html, string TagName, AttributeCollection Attributes)? Content { get; set; }
    public CookieBannerMessageActionsContext? Actions { get; set; }
}
