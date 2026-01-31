using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class RadiosFieldsetContext(string? describedBy, ModelExpression? @for) :
    FormGroupFieldsetContext2(describedBy, legendClass: null, @for)
{
    private protected override string FieldsetTagName => RadiosFieldsetTagHelper.TagName;
}
