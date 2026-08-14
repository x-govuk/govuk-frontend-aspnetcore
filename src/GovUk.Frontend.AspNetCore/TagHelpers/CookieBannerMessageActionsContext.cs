using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <param name="tagName">The spelling the actions were written with, for the messages that name them.</param>
internal class CookieBannerMessageActionsContext(string tagName)
{
    public string TagName { get; } = tagName;

    public AttributeCollection? Attributes { get; set; }
    public List<CookieBannerOptionsMessageAction> Actions { get; } = [];
}
