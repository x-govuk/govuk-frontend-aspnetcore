using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class PasswordInputContext : FormGroupContext3
{
    private (IHtmlContent Content, string TagName)? _beforeInput;
    private (IHtmlContent Content, string TagName)? _afterInput;

    public IHtmlContent? BeforeInput => _beforeInput?.Content;

    public IHtmlContent? AfterInput => _afterInput?.Content;

    protected override IReadOnlyCollection<string> ErrorMessageTagNames => PasswordInputErrorMessageTagHelper.AllTagNames;

    protected override IReadOnlyCollection<string> HintTagNames => PasswordInputHintTagHelper.AllTagNames;

    protected override IReadOnlyCollection<string> LabelTagNames => PasswordInputLabelTagHelper.AllTagNames;

    private IReadOnlyCollection<string> BeforeInputTagNames => PasswordInputBeforeInputTagHelper.AllTagNames;

    private IReadOnlyCollection<string> AfterInputTagNames => PasswordInputAfterInputTagHelper.AllTagNames;

    protected override string RootTagName => PasswordInputTagHelper.TagName;

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

        _afterInput = (content, tagName);
    }
}
