using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class FooterMetaContext
{
    public (IHtmlContent Html, AttributeCollection Attributes, string TagName)? Content { get; set; }
    public (IReadOnlyCollection<FooterOptionsMetaItem> Items, AttributeCollection? Attributes, string TagName)? Items { get; set; }
}
