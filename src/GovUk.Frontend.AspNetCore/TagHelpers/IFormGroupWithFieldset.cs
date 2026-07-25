using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal interface IFormGroupWithFieldset
{
    ModelExpression? For { get; }

    void OpenFieldset(FormGroupFieldsetContext2 fieldsetContext, AttributeCollection attributes);

    void CloseFieldset();
}
