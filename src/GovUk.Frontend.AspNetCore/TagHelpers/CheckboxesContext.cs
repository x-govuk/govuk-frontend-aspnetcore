using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.HtmlGeneration;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class CheckboxesContext(string? name, ModelExpression? aspFor) : FormGroupContext
{
    private bool _fieldsetIsOpen;
    private readonly List<CheckboxesItemBase> _items = [];
    private (TemplateString Content, string TagName)? _beforeInputs;
    private (TemplateString Content, string TagName)? _afterInputs;

    public string? Name { get; } = name;

    public ModelExpression? AspFor { get; } = aspFor;

    public IReadOnlyCollection<CheckboxesItemBase> Items => _items;

    public TemplateString? BeforeInputs => _beforeInputs?.Content;

    public TemplateString? AfterInputs => _afterInputs?.Content;

    public FormGroupFieldsetContext? Fieldset { get; private set; }

    protected override string ErrorMessageTagName => CheckboxesTagHelper.ErrorMessageTagName;

    protected string FieldsetTagName => CheckboxesFieldsetTagHelper.TagName;

    protected string ItemTagName => CheckboxesItemTagHelper.TagName;

    protected override string HintTagName => CheckboxesTagHelper.HintTagName;

    protected override string LabelTagName => throw new NotSupportedException();

    protected override string RootTagName => CheckboxesTagHelper.TagName;

    private IReadOnlyCollection<string> BeforeInputsTagNames => CheckboxesBeforeInputsTagHelper.AllTagNames;

    private IReadOnlyCollection<string> AfterInputsTagNames => CheckboxesAfterInputsTagHelper.AllTagNames;

    public void AddItem(CheckboxesItemBase item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (Fieldset is not null)
        {
            throw new InvalidOperationException($"<{ItemTagName}> must be inside <{FieldsetTagName}>.");
        }

        _items.Add(item);
    }

    public void OpenFieldset()
    {
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
    }

    public void CloseFieldset(CheckboxesFieldsetContext fieldsetContext)
    {
        if (!_fieldsetIsOpen)
        {
            throw new InvalidOperationException("Fieldset has not been opened.");
        }

        _fieldsetIsOpen = false;
        Fieldset = fieldsetContext;
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
        string? visuallyHiddenText,
        AttributeDictionary? attributes,
        IHtmlContent? content)
    {
        if (Fieldset is not null)
        {
            throw new InvalidOperationException($"<{ErrorMessageTagName}> must be inside <{FieldsetTagName}>.");
        }

        if (_beforeInputs is var (_, beforeInputsTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(ErrorMessageTagName, beforeInputsTagName);
        }

        if (Items.Count > 0)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(ErrorMessageTagName, ItemTagName);
        }

        if (_afterInputs is var (_, afterInputsTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(ErrorMessageTagName, afterInputsTagName);
        }

        base.SetErrorMessage(visuallyHiddenText, attributes, content);
    }

    public override void SetHint(AttributeDictionary? attributes, IHtmlContent? content)
    {
        if (Fieldset is not null)
        {
            throw new InvalidOperationException($"<{HintTagName}> must be inside <{FieldsetTagName}>.");
        }

        if (_beforeInputs is var (_, beforeInputsTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(HintTagName, beforeInputsTagName);
        }

        if (Items.Count > 0)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(HintTagName, ItemTagName);
        }

        if (_afterInputs is var (_, afterInputsTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(HintTagName, afterInputsTagName);
        }

        base.SetHint(attributes, content);
    }

    public override void SetLabel(
        bool isPageHeading,
        AttributeDictionary? attributes,
        IHtmlContent? content)
    {
        throw new NotSupportedException();
    }
}
