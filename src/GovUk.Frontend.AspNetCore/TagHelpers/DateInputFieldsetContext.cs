using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class DateInputFieldsetContext(string? describedBy, ModelExpression? @for) : FormGroupFieldsetContext2(describedBy, legendClass: null, @for)
{
    private protected override string FieldsetTagName => DateInputFieldsetTagHelper.TagName;
}
