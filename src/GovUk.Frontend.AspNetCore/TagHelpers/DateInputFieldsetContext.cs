using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class DateInputFieldsetContext(string? describedBy, AttributeCollection attributes, ModelExpression? @for) : FormGroupFieldsetContext2(DateInputFieldsetTagHelper.TagName, DateInputFieldsetLegendTagHelper.TagName, describedBy, legendClass: null, attributes, @for)
{
}
