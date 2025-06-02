using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class CookieBannerContext
{
    public List<CookieBannerOptionsMessage> Messages { get; } = new();
}
