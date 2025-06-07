using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class FooterNavItemsContext
{
    public List<FooterOptionsNavigationItem> Items { get; } = new();
}
