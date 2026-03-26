using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class DateInputContext(bool haveExplicitValue, ModelExpression? @for) : FormGroupContext3
{
    private bool _fieldsetIsOpen;
    private readonly SortedDictionary<DateInputItemTypes, DateInputContextItem> _items = [];
    private (TemplateString Content, string TagName)? _beforeInputs;
    private (TemplateString Content, string TagName)? _afterInputs;

    public ModelExpression? For { get; } = @for;

    public DateInputFieldsetContext? Fieldset { get; private set; }

    public AttributeCollection? Attributes { get; private set; }

    public IReadOnlyDictionary<DateInputItemTypes, DateInputContextItem> Items => _items;

    public DateInputItemTypes? ErrorFields { get; private set; }

    public TemplateString? BeforeInputs => _beforeInputs?.Content;

    public TemplateString? AfterInputs => _afterInputs?.Content;

    protected override IReadOnlyCollection<string> ErrorMessageTagNames { get; } =
        [/*, DateInputErrorMessageTagHelper.ShortTagName, */DateInputErrorMessageTagHelper.TagName];

    protected string FieldsetTagName { get; } = DateInputFieldsetTagHelper.TagName;

    protected override IReadOnlyCollection<string> HintTagNames { get; } =
        [/*DateInputHintTagHelper.ShortTagName, */DateInputHintTagHelper.TagName];

    protected override IReadOnlyCollection<string> LabelTagNames => throw new NotSupportedException();

    protected override string RootTagName { get; } = DateInputTagHelper.TagName;

    private IReadOnlyCollection<string> BeforeInputsTagNames => DateInputBeforeInputsTagHelper.AllTagNames;

    private IReadOnlyCollection<string> AfterInputsTagNames => DateInputAfterInputsTagHelper.AllTagNames;

    public void OpenFieldset(DateInputFieldsetContext fieldsetContext, AttributeCollection attributes)
    {
        ArgumentNullException.ThrowIfNull(_fieldsetIsOpen);
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

    public override ErrorMessageOptions GetErrorMessageOptions(
        ModelExpression? @for,
        ViewContext viewContext,
        IModelHelper modelHelper,
        bool? ignoreModelStateErrors)
    {
        throw new NotSupportedException("Use the overload that takes an 'errorMessagePrefix' argument too.");
    }

    public ErrorMessageOptions? GetErrorMessageOptions(
        string namePrefix,
        string? errorMessagePrefix,
        ModelExpression? @for,
        ViewContext viewContext,
        IModelHelper modelHelper,
        bool? ignoreModelStateErrors)
    {
        TemplateString? html = null;

        if (ErrorMessage?.Html is { } explicitHtml)
        {
            html = explicitHtml;
        }
        else if (ignoreModelStateErrors != true)
        {
            var modelName = @for?.Name ?? namePrefix;

            var invalidDateException = viewContext.ModelState[modelName]?.Errors.FirstOrDefault(e => e.Exception is DateInputParseException)
                ?.Exception as DateInputParseException;

            if (invalidDateException is not null && errorMessagePrefix is not null)
            {
                html = invalidDateException.GetMessage(errorMessagePrefix);
            }
            else if (@for is not null)
            {
                html = modelHelper.GetValidationMessage(viewContext, @for.ModelExplorer, @for.Name);
            }
        }

        return CreateErrorMessageOptions(html);
    }

    public FieldsetOptions? GetFieldsetOptions(IModelHelper modelHelper) => Fieldset?.GetFieldsetOptions(modelHelper, Attributes!);

    public override void SetLabel(bool? isPageHeading, AttributeCollection attributes, TemplateString? html, string tagName)
    {
        throw new NotSupportedException();
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
            var firstItemTagName = _items.First().Value.TagName;
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, firstItemTagName!);
        }

        if (_afterInputs is var (_, afterInputsTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, afterInputsTagName);
        }

        base.SetHint(attributes, html, tagName);
    }

    public void SetErrorMessage(
        DateInputItemTypes? errorFields,
        TemplateString? visuallyHiddenText,
        AttributeCollection attributes,
        TemplateString? html,
        string tagName)
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
            var firstItemTagName = _items.First().Value.TagName!;
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, firstItemTagName);
        }

        if (_afterInputs is var (_, afterInputsTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, afterInputsTagName);
        }

        ErrorFields = errorFields;

        base.SetErrorMessage(visuallyHiddenText, attributes, html, tagName);
    }

    public override void SetErrorMessage(TemplateString? visuallyHiddenText, AttributeCollection attributes, TemplateString? html, string tagName)
    {
        throw new NotSupportedException($"Use the overload that takes a {nameof(DateInputItemTypes)} argument too.");
    }

    public void SetItem(DateInputItemTypes itemType, DateInputContextItem item)
    {
        ArgumentNullException.ThrowIfNull(itemType);
        ArgumentNullException.ThrowIfNull(item);

        if (haveExplicitValue && item.ValueSpecified)
        {
            throw new InvalidOperationException($"Value cannot be specified for both <{item.TagName}> and the parent <{RootTagName}>.");
        }

        if (Fieldset is not null && !_fieldsetIsOpen)
        {
            throw new InvalidOperationException($"<{item.TagName}> must be inside <{FieldsetTagName}>.");
        }

        if (_items.Count != 0)
        {
            if (_items.ContainsKey(itemType))
            {
                throw ExceptionHelper.OnlyOneElementIsPermittedIn(item.TagName!, DateInputTagHelper.TagName);
            }

            var subsequentItems = _items.Where(kvp => kvp.Key > itemType).ToArray();

            if (subsequentItems.Length != 0)
            {
                throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                    item.TagName!,
                    subsequentItems[0].Value.TagName!);
            }
        }

        _items.Add(itemType, item);
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

        if (Fieldset is not null && !_fieldsetIsOpen)
        {
            throw new InvalidOperationException($"<{tagName}> must be inside <{FieldsetTagName}>.");
        }

        if (Items.Count > 0)
        {
            var firstItemTagName = _items.First().Value.TagName;
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                firstItemTagName!);
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

        if (Fieldset is not null && !_fieldsetIsOpen)
        {
            throw new InvalidOperationException($"<{tagName}> must be inside <{FieldsetTagName}>.");
        }

        _afterInputs = (content, tagName);
    }
}
