#nullable disable
using GovUk.Frontend.AspNetCore.HtmlGeneration;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore;

internal interface IGovUkHtmlGenerator
{
    TagBuilder GenerateErrorMessage(
        string visuallyHiddenText,
        IHtmlContent content,
        AttributeDictionary attributes);

    TagBuilder GenerateFieldset(
        string describedBy,
        string role,
        bool? legendIsPageHeading,
        IHtmlContent legendContent,
        AttributeDictionary legendAttributes,
        IHtmlContent content,
        AttributeDictionary attributes);

    TagBuilder GenerateFormGroup(bool haveError, IHtmlContent content, AttributeDictionary attributes);

    TagBuilder GenerateHint(string id, IHtmlContent content, AttributeDictionary attributes);

    TagBuilder GenerateLabel(
        string @for,
        bool isPageHeading,
        IHtmlContent content,
        AttributeDictionary attributes);

    TagBuilder GenerateRadios(
        string idPrefix,
        string name,
        IEnumerable<RadiosItemBase> items,
        AttributeDictionary attributes);
}
