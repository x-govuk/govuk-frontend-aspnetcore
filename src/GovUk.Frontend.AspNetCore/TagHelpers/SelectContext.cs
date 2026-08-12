using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class SelectContext(ModelExpression? @for) : FormGroupContext3
{
    private readonly List<SelectOptionsItem> _items = [];
    private string? _firstItemTagName;
    private (IHtmlContent Content, string TagName)? _beforeInput;
    private (IHtmlContent Content, string TagName)? _afterInput;

    public ModelExpression? For { get; } = @for;

    public IReadOnlyCollection<SelectOptionsItem> Items => _items;

    public IHtmlContent? BeforeInput => _beforeInput?.Content;

    public IHtmlContent? AfterInput => _afterInput?.Content;

    protected override IReadOnlyCollection<string> ErrorMessageTagNames => SelectErrorMessageTagHelper.AllTagNames;

    protected override IReadOnlyCollection<string> HintTagNames => SelectHintTagHelper.AllTagNames;

    protected override IReadOnlyCollection<string> LabelTagNames => SelectLabelTagHelper.AllTagNames;

    private IReadOnlyCollection<string> BeforeInputTagNames => SelectBeforeInputTagHelper.AllTagNames;

    private IReadOnlyCollection<string> AfterInputTagNames => SelectAfterInputTagHelper.AllTagNames;

    protected override string RootTagName => SelectTagHelper.TagName;

    // Messages about an item that has already been added name it as it was written in the view,
    // which may be either the govuk- prefixed name or the short one
    private string ItemTagName => _firstItemTagName ?? SelectItemTagHelper.TagName;

    public void AddItem(SelectOptionsItem item, string tagName)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(tagName);

        _firstItemTagName ??= tagName;
        _items.Add(item);
    }

    public override void SetErrorMessage(
        string? visuallyHiddenText,
        AttributeCollection attributes,
        IHtmlContent? html,
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
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, ItemTagName);
        }

        base.SetErrorMessage(visuallyHiddenText, attributes, html, tagName);
    }

    public override void SetHint(
        AttributeCollection attributes,
        IHtmlContent? html,
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
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, ItemTagName);
        }

        base.SetHint(attributes, html, tagName);
    }

    public override void SetLabel(
        bool? isPageHeading,
        AttributeCollection attributes,
        IHtmlContent? html,
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
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, ItemTagName);
        }

        base.SetLabel(isPageHeading, attributes, html, tagName);
    }

    public void SetBeforeInput(IHtmlContent content, string tagName)
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
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, ItemTagName);
        }

        _beforeInput = (content, tagName);
    }

    public void SetAfterInput(IHtmlContent content, string tagName)
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
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, ItemTagName);
        }

        _afterInput = (content, tagName);
    }
}
