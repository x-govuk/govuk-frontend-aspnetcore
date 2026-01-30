using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal abstract class FormGroupFieldsetContext2
{
    public record LegendInfo(bool? IsPageHeading, AttributeCollection Attributes, TemplateString? Html);

    private readonly string? _describedBy;
    private readonly string? _legendClass;
    private readonly ModelExpression? _for;
    private AttributeCollection? _attributes;

    protected FormGroupFieldsetContext2(
        string? describedBy,
        string? legendClass,
        ModelExpression? @for)
    {
        _describedBy = describedBy;
        _legendClass = legendClass;
        _for = @for;
    }

    public LegendInfo? Legend { get; private set; }

    public void SetAttributes(AttributeCollection attributes)
    {
        ArgumentNullException.ThrowIfNull(attributes);
        _attributes = attributes;
    }

    public FieldsetOptions GetFieldsetOptions(IModelHelper modelHelper)
    {
        ArgumentNullException.ThrowIfNull(modelHelper);

        if (_attributes is null)
        {
            throw new InvalidOperationException("Attributes must be set before calling GetFieldsetOptions.");
        }

        var clonedAttributes = _attributes.Clone();
        clonedAttributes.Remove("class", out var classes);

        var legendAttributes = Legend?.Attributes.Clone() ?? [];
        legendAttributes.Remove("class", out var legendClasses);

        if (_legendClass is not null)
        {
            legendClasses = legendClasses.AppendCssClasses(_legendClass);
        }

        var html = Legend?.Html;
        if (html is null && _for is not null)
        {
            html = modelHelper.GetDisplayName(_for.ModelExplorer, _for.Name);
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
            DescribedBy = _describedBy,
            Legend = legendOptions,
            Role = "group",
            Html = null,
            Classes = classes,
            Attributes = clonedAttributes
        };
    }

    public void SetLegend(bool? isPageHeading, AttributeCollection attributes, TemplateString? html, string legendTagName, string fieldsetTagName)
    {
        ArgumentNullException.ThrowIfNull(attributes);
        ArgumentNullException.ThrowIfNull(legendTagName);
        ArgumentNullException.ThrowIfNull(fieldsetTagName);

        if (Legend is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(legendTagName, fieldsetTagName);
        }

        Legend = new(isPageHeading, attributes, html);
    }

    public void ThrowIfNotComplete(string legendTagName)
    {
        ArgumentNullException.ThrowIfNull(legendTagName);

        if (Legend is null && _for is null)
        {
            throw ExceptionHelper.AChildElementMustBeProvided(legendTagName);
        }
    }
}
