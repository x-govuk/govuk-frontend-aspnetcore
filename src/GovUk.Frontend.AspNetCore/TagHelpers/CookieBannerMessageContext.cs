using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <param name="tagName">The spelling the message was written with, for the messages that name it.</param>
internal class CookieBannerMessageContext(string tagName)
{
    public string TagName { get; } = tagName;

    public (IHtmlContent? Html, string TagName, AttributeCollection Attributes)? Heading { get; set; }
    public (IHtmlContent? Html, string TagName, AttributeCollection Attributes)? Content { get; set; }
    public CookieBannerMessageActionsContext? Actions { get; set; }
}
