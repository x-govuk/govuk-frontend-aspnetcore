using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.HtmlGeneration;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class SelectContext(ModelExpression? aspFor) : FormGroupContext3
{
    private readonly List<SelectItem> _items = [];

    public ModelExpression? AspFor { get; } = aspFor;

    public bool HaveModelExpression => AspFor is not null;

    public IReadOnlyCollection<SelectItem> Items => _items;

    protected override IReadOnlyCollection<string> ErrorMessageTagNames => [SelectTagHelper.ErrorMessageTagName];

    protected override IReadOnlyCollection<string> HintTagNames => [SelectTagHelper.HintTagName];

    protected override IReadOnlyCollection<string> LabelTagNames => [SelectTagHelper.LabelTagName];

    protected override string RootTagName => SelectTagHelper.TagName;

    public void AddItem(SelectItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        _items.Add(item);
    }

    public override void SetErrorMessage(
        TemplateString? visuallyHiddenText,
        AttributeCollection attributes,
        TemplateString? html,
        string tagName)
    {
        if (_items.Count != 0)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, SelectItemTagHelper.TagName);
        }

        base.SetErrorMessage(visuallyHiddenText, attributes, html, tagName);
    }

    public override void SetHint(
        AttributeCollection attributes,
        TemplateString? html,
        string tagName)
    {
        if (_items.Count != 0)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, SelectItemTagHelper.TagName);
        }

        base.SetHint(attributes, html, tagName);
    }

    public override void SetLabel(
        bool? isPageHeading,
        AttributeCollection attributes,
        TemplateString? html,
        string tagName)
    {
        if (_items.Count != 0)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, SelectItemTagHelper.TagName);
        }

        base.SetLabel(isPageHeading, attributes, html, tagName);
    }
}
