using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal interface IFormGroupWithFieldset
{
    ModelExpression? For { get; }

    /// <summary>
    /// The context for the fieldset created by an explicit fieldset element, if there is one.
    /// </summary>
    FormGroupFieldsetContext2? Fieldset { get; }

    /// <summary>
    /// The attributes specified on the explicit fieldset element, if there is one.
    /// </summary>
    AttributeCollection? Attributes { get; }

    /// <summary>
    /// The context for the fieldset that's generated when no explicit fieldset element is used.
    /// </summary>
    FormGroupFieldsetContext2 ImplicitFieldset { get; }

    string RootTagName { get; }

    string FieldsetTagName { get; }

    string LegendTagName { get; }

    void OpenFieldset(FormGroupFieldsetContext2 fieldsetContext, AttributeCollection attributes);

    void CloseFieldset();
}
