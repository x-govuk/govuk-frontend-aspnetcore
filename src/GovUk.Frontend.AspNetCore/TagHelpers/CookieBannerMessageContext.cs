using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class CookieBannerMessageContext
{
    public (TemplateString? Html, string TagName, AttributeCollection Attributes)? Heading { get; set; }
    public (TemplateString? Html, string TagName, AttributeCollection Attributes)? Content { get; set; }
    public CookieBannerMessageActionsContext? Actions { get; set; }
}
