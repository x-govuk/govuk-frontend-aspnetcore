using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class FooterMetaContext
{
    public (TemplateString Html, AttributeCollection Attributes, string TagName)? Content { get; set; }
    public (IReadOnlyCollection<FooterOptionsMetaItem> Items, AttributeCollection? Attributes, string TagName)? Items { get; set; }
}
