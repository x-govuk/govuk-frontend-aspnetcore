using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal abstract class FormGroupFieldsetContext2
{
    public record LegendInfo(bool IsPageHeading, AttributeCollection Attributes, TemplateString? Html);

    private readonly string _fieldsetTagName;
    private readonly string _legendTagName;
    private readonly string? _describedBy;
    private readonly string? _legendClass;
    private readonly AttributeCollection _attributes;
    private readonly ModelExpression? _for;

    protected FormGroupFieldsetContext2(
        string fieldsetTagName,
        string legendTagName,
        string? describedBy,
        string? legendClass,
        AttributeCollection attributes,
        ModelExpression? @for)
    {
        ArgumentNullException.ThrowIfNull(fieldsetTagName);
        ArgumentNullException.ThrowIfNull(legendTagName);
        ArgumentNullException.ThrowIfNull(attributes);
        _fieldsetTagName = fieldsetTagName;
        _legendTagName = legendTagName;
        _describedBy = describedBy;
        _legendClass = legendClass;
        _attributes = attributes;
        _for = @for;
    }

    public LegendInfo? Legend { get; private set; }

    public FieldsetOptions GetFieldsetOptions(IModelHelper modelHelper)
    {
        ArgumentNullException.ThrowIfNull(modelHelper);

        ThrowIfNotComplete();

        var attributes = _attributes.Clone();
        attributes.Remove("class", out var classes);

        var legendAttributes = Legend?.Attributes.Clone() ?? [];
        legendAttributes.Remove("class", out var legendClasses);

        if (_legendClass is not null)
        {
            legendClasses = legendClasses.AppendCssClasses(HtmlEncoder.Default, _legendClass);
        }

        var html = Legend?.Html;
        if (html is null && _for is not null)
        {
            html = modelHelper.GetDisplayName(_for.ModelExplorer, _for.Name);
        }

        var legendOptions = new FieldsetOptionsLegend()
        {
            Text = null,
            Html = html,
            IsPageHeading = Legend?.IsPageHeading,
            Classes = legendClasses,
            Attributes = legendAttributes
        };

        return new FieldsetOptions()
        {
            DescribedBy = _describedBy,
            Legend = legendOptions,
            Role = "group",
            Html = null,
            Classes = classes,
            Attributes = attributes
        };
    }

    public void SetLegend(bool isPageHeading, AttributeCollection attributes, TemplateString? html)
    {
        ArgumentNullException.ThrowIfNull(attributes);

        if (Legend is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(_legendTagName, _fieldsetTagName);
        }

        Legend = new(isPageHeading, attributes, html);
    }

    public void ThrowIfNotComplete()
    {
        if (Legend is null && _for is null)
        {
            throw ExceptionHelper.AChildElementMustBeProvided(_legendTagName);
        }
    }
}
