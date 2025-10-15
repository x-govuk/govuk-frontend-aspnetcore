using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class TextInputContext : FormGroupContext3
{
    private (TemplateString Content, string TagName)? _beforeInput;
    private (InputOptionsPrefix Options, string TagName)? _prefix;
    private (InputOptionsSuffix Options, string TagName)? _suffix;
    private (TemplateString Content, string TagName)? _afterInput;

    public TemplateString? BeforeInput => _beforeInput?.Content;

    public InputOptionsPrefix? Prefix => _prefix?.Options;

    public InputOptionsSuffix? Suffix => _suffix?.Options;

    public TemplateString? AfterInput => _afterInput?.Content;

    protected override IReadOnlyCollection<string> ErrorMessageTagNames { get; } =
        [/*TextInputTagHelper.ErrorMessageShortTagName, */TextInputErrorMessageTagHelper.TagName];

    protected override IReadOnlyCollection<string> HintTagNames { get; } =
        [/*TextInputTagHelper.HintShortTagName, */TextInputHintTagHelper.TagName];

    protected override IReadOnlyCollection<string> LabelTagNames { get; } =
        [/*TextInputTagHelper.LabelShortTagName, */TextInputLabelTagHelper.TagName];

    private IReadOnlyCollection<string> BeforeInputTagNames { get; } =
        [/*TextInputBeforeInputTagHelper.ShortTagName, */TextInputBeforeInputTagHelper.TagName];

    private IReadOnlyCollection<string> PrefixTagNames { get; } =
        [/*TextInputPrefixTagHelper.ShortTagName, */TextInputPrefixTagHelper.TagName];

    private IReadOnlyCollection<string> SuffixTagNames { get; } =
        [/*TextInputSuffixTagHelper.ShortTagName, */TextInputSuffixTagHelper.TagName];

    private IReadOnlyCollection<string> AfterInputTagNames { get; } =
        [/*TextInputAfterInputTagHelper.ShortTagName, */TextInputAfterInputTagHelper.TagName];

    protected override string RootTagName { get; } = TextInputTagHelper.TagName;

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

        if (_prefix is var (_, prefixTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                prefixTagName);
        }

        if (_suffix is var (_, suffixTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                suffixTagName);
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

        if (_prefix is var (_, prefixTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                prefixTagName);
        }

        if (_suffix is var (_, suffixTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                suffixTagName);
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
        bool isPageHeading,
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

        if (_prefix is var (_, prefixTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                prefixTagName);
        }

        if (_suffix is var (_, suffixTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                suffixTagName);
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

        if (_prefix is var (_, prefixTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                prefixTagName);
        }

        if (_suffix is var (_, suffixTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                suffixTagName);
        }

        if (_afterInput is var (_, afterInputTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                afterInputTagName);
        }

        _beforeInput = (content, tagName);
    }

    public void SetPrefix(InputOptionsPrefix options, string tagName)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(tagName);

        if (Prefix is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                PrefixTagNames,
                RootTagName);
        }

        if (_suffix is var (_, suffixTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                suffixTagName);
        }

        if (_afterInput is var (_, afterInputTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                afterInputTagName);
        }

        _prefix = (options, tagName);
    }

    public void SetSuffix(InputOptionsSuffix options, string tagName)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(tagName);

        if (Suffix is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                SuffixTagNames,
                RootTagName);
        }

        if (_afterInput is var (_, afterInputTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                afterInputTagName);
        }

        _suffix = (options, tagName);
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
