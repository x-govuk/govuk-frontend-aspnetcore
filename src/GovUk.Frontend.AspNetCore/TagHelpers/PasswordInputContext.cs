using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class PasswordInputContext : FormGroupContext3
{
    private (TemplateString Content, string TagName)? _beforeInput;
    private (TemplateString Content, string TagName)? _afterInput;

    public TemplateString? BeforeInput => _beforeInput?.Content;

    public TemplateString? AfterInput => _afterInput?.Content;

    protected override IReadOnlyCollection<string> ErrorMessageTagNames => PasswordInputErrorMessageTagHelper.AllTagNames;

    protected override IReadOnlyCollection<string> HintTagNames => PasswordInputHintTagHelper.AllTagNames;

    protected override IReadOnlyCollection<string> LabelTagNames => PasswordInputLabelTagHelper.AllTagNames;

    private IReadOnlyCollection<string> BeforeInputTagNames => PasswordInputBeforeInputTagHelper.AllTagNames;

    private IReadOnlyCollection<string> AfterInputTagNames => PasswordInputAfterInputTagHelper.AllTagNames;

    protected override string RootTagName => PasswordInputTagHelper.TagName;

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

        _afterInput = (content, tagName);
    }
}
