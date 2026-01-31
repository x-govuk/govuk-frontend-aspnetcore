using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class CheckboxesContext(string? name, ModelExpression? @for) : FormGroupContext3
{
    private bool _fieldsetIsOpen;
    private readonly List<CheckboxesOptionsItem> _items = [];
    private (TemplateString Content, string TagName)? _beforeInputs;
    private (TemplateString Content, string TagName)? _afterInputs;

    public string? Name { get; } = name;

    public ModelExpression? For { get; } = @for;

    public IReadOnlyCollection<CheckboxesOptionsItem> Items => _items;

    public TemplateString? BeforeInputs => _beforeInputs?.Content;

    public TemplateString? AfterInputs => _afterInputs?.Content;

    public AttributeCollection? Attributes { get; private set; }

    public CheckboxesFieldsetContext? Fieldset { get; private set; }

    protected override IReadOnlyCollection<string> ErrorMessageTagNames => CheckboxesErrorMessageTagHelper.AllTagNames;

    protected string FieldsetTagName => CheckboxesFieldsetTagHelper.TagName;

    protected string ItemTagName => CheckboxesItemTagHelper.TagName;

    protected override IReadOnlyCollection<string> HintTagNames => CheckboxesHintTagHelper.AllTagNames;

    protected override IReadOnlyCollection<string> LabelTagNames => throw new NotSupportedException();

    protected override string RootTagName => CheckboxesTagHelper.TagName;

    private IReadOnlyCollection<string> BeforeInputsTagNames => CheckboxesBeforeInputsTagHelper.AllTagNames;

    private IReadOnlyCollection<string> AfterInputsTagNames => CheckboxesAfterInputsTagHelper.AllTagNames;

    public void AddItem(CheckboxesOptionsItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (Fieldset is not null && !_fieldsetIsOpen)
        {
            throw new InvalidOperationException($"<{ItemTagName}> must be inside <{FieldsetTagName}>.");
        }

        _items.Add(item);
    }

    public FieldsetOptions? GetFieldsetOptions(IModelHelper modelHelper) => Fieldset?.GetFieldsetOptions(modelHelper, Attributes!);

    public void OpenFieldset(CheckboxesFieldsetContext fieldsetContext, AttributeCollection attributes)
    {
        ArgumentNullException.ThrowIfNull(fieldsetContext);
        ArgumentNullException.ThrowIfNull(attributes);

        if (_fieldsetIsOpen)
        {
            throw new InvalidOperationException($"<{FieldsetTagName}> cannot be nested inside another <{FieldsetTagName}>.");
        }

        if (Fieldset is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(FieldsetTagName, RootTagName);
        }

        if (Items.Count > 0 || Hint is not null || ErrorMessage is not null)
        {
            throw new InvalidOperationException($"<{FieldsetTagName}> must be the only direct child of the <{RootTagName}>.");
        }

        _fieldsetIsOpen = true;
        Fieldset = fieldsetContext;
        Attributes = attributes;
    }

    public void CloseFieldset()
    {
        if (!_fieldsetIsOpen)
        {
            throw new InvalidOperationException("Fieldset has not been opened.");
        }

        _fieldsetIsOpen = false;
    }

    public void SetBeforeInputs(TemplateString content, string tagName)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(tagName);

        if (BeforeInputs is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                BeforeInputsTagNames,
                RootTagName);
        }

        if (Items.Count > 0)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                ItemTagName);
        }

        if (_afterInputs is var (_, afterInputsTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                afterInputsTagName);
        }

        _beforeInputs = (content, tagName);
    }

    public void SetAfterInputs(TemplateString content, string tagName)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(tagName);

        if (AfterInputs is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                AfterInputsTagNames,
                RootTagName);
        }

        _afterInputs = (content, tagName);
    }

    public override void SetErrorMessage(
        TemplateString? visuallyHiddenText,
        AttributeCollection attributes,
        TemplateString? html,
        string tagName)
    {
        if (Fieldset is not null)
        {
            throw new InvalidOperationException($"<{tagName}> must be inside <{FieldsetTagName}>.");
        }

        if (_beforeInputs is var (_, beforeInputsTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, beforeInputsTagName);
        }

        if (Items.Count > 0)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, ItemTagName);
        }

        if (_afterInputs is var (_, afterInputsTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, afterInputsTagName);
        }

        base.SetErrorMessage(visuallyHiddenText, attributes, html, tagName);
    }

    public override void SetHint(AttributeCollection attributes, TemplateString? html, string tagName)
    {
        if (Fieldset is not null && !_fieldsetIsOpen)
        {
            throw new InvalidOperationException($"<{tagName}> must be inside <{FieldsetTagName}>.");
        }

        if (_beforeInputs is var (_, beforeInputsTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, beforeInputsTagName);
        }

        if (Items.Count > 0)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, ItemTagName);
        }

        if (_afterInputs is var (_, afterInputsTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, afterInputsTagName);
        }

        base.SetHint(attributes, html, tagName);
    }

    public override void SetLabel(
        bool? isPageHeading,
        AttributeCollection attributes,
        TemplateString? html,
        string tagName)
    {
        throw new NotSupportedException();
    }
}
