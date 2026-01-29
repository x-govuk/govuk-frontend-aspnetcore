using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class CheckboxesFieldsetContext(
    string? describedBy,
    string? legendClass,
    AttributeCollection attributes,
    ModelExpression? @for) 
    : FormGroupFieldsetContext2(
        CheckboxesFieldsetTagHelper.TagName,
        CheckboxesFieldsetLegendTagHelper.TagName,
        describedBy,
        legendClass,
        attributes,
        @for)
{
}
