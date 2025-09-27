using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class RadiosFieldsetContext(AttributeDictionary? attributes, ModelExpression? aspFor) : FormGroupFieldsetContext(RadiosFieldsetTagHelper.TagName, RadiosFieldsetLegendTagHelper.TagName, attributes, aspFor)
{
}
