using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class CharacterCountContext : FormGroupContext3
{
    private (TemplateString Content, string TagName)? _beforeInput;
    private (TemplateString Content, string TagName)? _afterInput;

    protected override IReadOnlyCollection<string> ErrorMessageTagNames { get; } =
        [/*CharacterCountErrorMessageTagHelper.ShortTagName, */CharacterCountErrorMessageTagHelper.TagName];

    protected override IReadOnlyCollection<string> HintTagNames { get; } =
        [/*CharacterCountHintTagHelper.ShortTagName, */CharacterCountHintTagHelper.TagName];

    protected override IReadOnlyCollection<string> LabelTagNames { get; } =
        [/*CharacterCountLabelTagHelper.ShortTagName, */CharacterCountLabelTagHelper.TagName];

    protected override string RootTagName { get; } = CharacterCountTagHelper.TagName;

    public TemplateString? BeforeInput => _beforeInput?.Content;

    public TemplateString? AfterInput => _afterInput?.Content;

    public TemplateString? Value { get; private set; }

    private IReadOnlyCollection<string> BeforeInputTagNames => CharacterCountBeforeInputTagHelper.AllTagNames;

    private IReadOnlyCollection<string> AfterInputTagNames => CharacterCountAfterInputTagHelper.AllTagNames;

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

        if (Value is not null)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                CharacterCountValueTagHelper.TagName);
        }

        base.SetErrorMessage(visuallyHiddenText, attributes, html, tagName);
    }

    public override void SetHint(AttributeCollection attributes, TemplateString? html, string tagName)
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

        if (Value is not null)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                CharacterCountValueTagHelper.TagName);
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

        if (Value is not null)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                CharacterCountValueTagHelper.TagName);
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

        if (Value is not null)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                CharacterCountValueTagHelper.TagName);
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

        if (Value is not null)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                CharacterCountValueTagHelper.TagName);
        }

        _afterInput = (content, tagName);
    }

    public void SetValue(TemplateString html, string tagName)
    {
        if (Value is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(tagName, RootTagName);
        }

        Value = html;
    }
}
