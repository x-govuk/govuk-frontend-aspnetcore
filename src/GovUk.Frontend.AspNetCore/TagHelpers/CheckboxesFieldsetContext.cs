using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class CheckboxesFieldsetContext(AttributeDictionary? attributes, ModelExpression? aspFor) : FormGroupFieldsetContext(CheckboxesFieldsetTagHelper.TagName, CheckboxesFieldsetLegendTagHelper.TagName, attributes, aspFor)
{
}
