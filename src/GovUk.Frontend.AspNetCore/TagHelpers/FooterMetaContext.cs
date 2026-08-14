using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class FooterMetaContext
{
    /// <summary>
    /// The spelling of the element the children are written in, for the messages that name it.
    /// </summary>
    public string? TagName { get; set; }

    public (IHtmlContent Html, AttributeCollection Attributes, string TagName)? Content { get; set; }
    public (IReadOnlyCollection<FooterOptionsMetaItem> Items, AttributeCollection? Attributes, string TagName)? Items { get; set; }
}
