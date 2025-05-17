using GovUk.Frontend.AspNetCore.Components;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class DateInputContextItem
{
    public string? TagName { get; set; }
    public TemplateString? Id { get; set; }
    public TemplateString? Name { get; set; }
    public TemplateString? LabelHtml { get; set; }
    public AttributeCollection? LabelAttributes { get; set; }
    public TemplateString? Value { get; set; }
    public bool ValueSpecified { get; set; }
    public string? AutoComplete { get; set; }
    public TemplateString? InputMode { get; set; }
    public TemplateString? Pattern { get; set; }
    public AttributeCollection? Attributes { get; set; }
}
