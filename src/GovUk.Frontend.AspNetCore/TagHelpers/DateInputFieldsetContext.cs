using GovUk.Frontend.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class DateInputFieldsetContext : FormGroupFieldsetContext2
{
    public DateInputFieldsetContext(string? describedBy, AttributeCollection attributes, ModelExpression? @for) :
        base(DateInputFieldsetTagHelper.TagName, DateInputFieldsetLegendTagHelper.TagName, describedBy, legendClass: null, attributes, @for)
    {
    }
}
