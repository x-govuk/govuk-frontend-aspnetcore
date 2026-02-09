using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class SelectContext(ModelExpression? @for) : FormGroupContext3
{
    private readonly List<SelectOptionsItem> _items = [];
    private (TemplateString Content, string TagName)? _beforeInput;
    private (TemplateString Content, string TagName)? _afterInput;

    public ModelExpression? For { get; } = @for;

    public IReadOnlyCollection<SelectOptionsItem> Items => _items;

    public TemplateString? BeforeInput => _beforeInput?.Content;

    public TemplateString? AfterInput => _afterInput?.Content;

    protected override IReadOnlyCollection<string> ErrorMessageTagNames => SelectErrorMessageTagHelper.AllTagNames;

    protected override IReadOnlyCollection<string> HintTagNames => SelectHintTagHelper.AllTagNames;

    protected override IReadOnlyCollection<string> LabelTagNames => SelectLabelTagHelper.AllTagNames;

    private IReadOnlyCollection<string> BeforeInputTagNames => SelectBeforeInputTagHelper.AllTagNames;

    private IReadOnlyCollection<string> AfterInputTagNames => SelectAfterInputTagHelper.AllTagNames;

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
        if (_beforeInput is var (_, beforeInputTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                beforeInputTagName);
        }

        if (_afterInput is var (_, afterInputTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                afterInputTagName);
        }

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
        if (_beforeInput is var (_, beforeInputTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                beforeInputTagName);
        }

        if (_afterInput is var (_, afterInputTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                afterInputTagName);
        }

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
        if (_beforeInput is var (_, beforeInputTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                beforeInputTagName);
        }

        if (_afterInput is var (_, afterInputTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                afterInputTagName);
        }

        if (_items.Count != 0)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, SelectItemTagHelper.TagName);
        }

        base.SetLabel(isPageHeading, attributes, html, tagName);
    }

    public void SetBeforeInput(TemplateString content, string tagName)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(tagName);

        if (BeforeInput is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                BeforeInputTagNames,
                RootTagName);
        }

        if (_afterInput is var (_, afterInputTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                afterInputTagName);
        }

        if (_items.Count != 0)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, SelectItemTagHelper.TagName);
        }

        _beforeInput = (content, tagName);
    }

    public void SetAfterInput(TemplateString content, string tagName)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(tagName);

        if (AfterInput is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                AfterInputTagNames,
                RootTagName);
        }

        if (_items.Count != 0)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, SelectItemTagHelper.TagName);
        }

        _afterInput = (content, tagName);
    }
}
