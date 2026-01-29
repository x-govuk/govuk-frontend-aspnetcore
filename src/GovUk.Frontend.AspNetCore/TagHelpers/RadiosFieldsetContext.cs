using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class RadiosFieldsetContext(string? describedBy, AttributeCollection attributes, ModelExpression? @for) :
    FormGroupFieldsetContext2(
        RadiosFieldsetTagHelper.TagName,
        RadiosFieldsetLegendTagHelper.TagName,
        describedBy,
        legendClass: null,
        attributes,
        @for);
