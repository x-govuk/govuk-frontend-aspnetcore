using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class CharacterCountContext : FormGroupContext3
{
    protected override IReadOnlyCollection<string> ErrorMessageTagNames { get; } =
        [/*CharacterCountErrorMessageTagHelper.ShortTagName, */CharacterCountErrorMessageTagHelper.TagName];

    protected override IReadOnlyCollection<string> HintTagNames { get; } =
        [/*CharacterCountHintTagHelper.ShortTagName, */CharacterCountHintTagHelper.TagName];

    protected override IReadOnlyCollection<string> LabelTagNames { get; } =
        [/*CharacterCountLabelTagHelper.ShortTagName, */CharacterCountLabelTagHelper.TagName];

    protected override string RootTagName { get; } = CharacterCountTagHelper.TagName;

    public TemplateString? Value { get; private set; }

    public override void SetErrorMessage(
        TemplateString? visuallyHiddenText,
        AttributeCollection attributes,
        TemplateString? html,
        string tagName)
    {
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
        if (Value is not null)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                CharacterCountValueTagHelper.TagName);
        }

        base.SetLabel(isPageHeading, attributes, html, tagName);
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
