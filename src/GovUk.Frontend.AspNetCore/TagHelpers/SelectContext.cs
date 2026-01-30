using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class SelectContext(ModelExpression? @for) : FormGroupContext3
{
    private readonly List<SelectOptionsItem> _items = [];

    public ModelExpression? For { get; } = @for;

    public bool HaveModelExpression => For is not null;

    public IReadOnlyCollection<SelectOptionsItem> Items => _items;

    protected override IReadOnlyCollection<string> ErrorMessageTagNames => SelectErrorMessageTagHelper.AllTagNames;

    protected override IReadOnlyCollection<string> HintTagNames => SelectHintTagHelper.AllTagNames;

    protected override IReadOnlyCollection<string> LabelTagNames => SelectLabelTagHelper.AllTagNames;

    protected override string RootTagName => SelectTagHelper.TagName;

    public void AddItem(SelectOptionsItem item)
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
