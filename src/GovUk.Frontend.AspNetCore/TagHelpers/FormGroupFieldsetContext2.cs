using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal abstract class FormGroupFieldsetContext2(string? describedBy, string? legendClass, ModelExpression? @for)
{
    public record LegendInfo(bool? IsPageHeading, AttributeCollection Attributes, TemplateString? Html);

    public LegendInfo? Legend { get; private set; }

    private protected abstract string FieldsetTagName { get; }

    public FieldsetOptions GetFieldsetOptions(IModelHelper modelHelper, AttributeCollection attributes)
    {
        ArgumentNullException.ThrowIfNull(modelHelper);
        ArgumentNullException.ThrowIfNull(attributes);

        var clonedAttributes = attributes.Clone();
        clonedAttributes.Remove("class", out var classes);

        var legendAttributes = Legend?.Attributes.Clone() ?? [];
        legendAttributes.Remove("class", out var legendClasses);

        if (legendClass is not null)
        {
            legendClasses = legendClasses.AppendCssClasses(legendClass);
        }

        var html = Legend?.Html;
        if (html is null && @for is not null)
        {
            html = modelHelper.GetDisplayName(@for.ModelExplorer, @for.Name);
        }

        var legendOptions = new FieldsetOptionsLegend
        {
            Text = null,
            Html = html,
            IsPageHeading = Legend?.IsPageHeading,
            Classes = legendClasses,
            Attributes = legendAttributes
        };

        return new FieldsetOptions
        {
            DescribedBy = describedBy,
            Legend = legendOptions,
            Role = "group",
            Html = null,
            Classes = classes,
            Attributes = clonedAttributes
        };
    }

    public void SetLegend(bool? isPageHeading, AttributeCollection attributes, TemplateString? html, string legendTagName)
    {
        ArgumentNullException.ThrowIfNull(attributes);
        ArgumentNullException.ThrowIfNull(legendTagName);

        if (Legend is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(legendTagName, FieldsetTagName);
        }

        Legend = new(isPageHeading, attributes, html);
    }

    public void ThrowIfNotComplete(string legendTagName)
    {
        ArgumentNullException.ThrowIfNull(legendTagName);

        if (Legend is null && @for is null)
        {
            throw ExceptionHelper.AChildElementMustBeProvided(legendTagName);
        }
    }
}
