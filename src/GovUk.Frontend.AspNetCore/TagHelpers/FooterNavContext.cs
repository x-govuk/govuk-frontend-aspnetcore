using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class FooterNavContext
{
    /// <summary>
    /// The spelling of the element the children are written in, for the messages that name it.
    /// </summary>
    public string? TagName { get; set; }

    public AttributeCollection? Attributes { get; set; }
    public (IReadOnlyCollection<FooterOptionsNavigationItem> Items, AttributeCollection? Attributes, string TagName)? Items { get; set; }
    public (IHtmlContent Html, AttributeCollection Attributes, string TagName)? Title { get; set; }
}
