using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class CookieBannerMessageActionsContext
{
    public AttributeCollection? Attributes { get; set; }
    public List<CookieBannerOptionsMessageAction> Actions { get; } = [];
}
