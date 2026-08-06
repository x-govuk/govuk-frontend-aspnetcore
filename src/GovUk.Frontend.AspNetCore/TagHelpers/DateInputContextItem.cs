using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class DateInputContextItem
{
    public string? TagName { get; set; }
    public TemplateString? Id { get; set; }
    public TemplateString? Name { get; set; }
    public IHtmlContent? LabelHtml { get; set; }
    public AttributeCollection? LabelAttributes { get; set; }
    public string? Value { get; set; }
    public bool ValueSpecified { get; set; }
    public string? AutoComplete { get; set; }
    public TemplateString? InputMode { get; set; }
    public TemplateString? Pattern { get; set; }
    public AttributeCollection? Attributes { get; set; }
}
