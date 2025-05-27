using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal enum DateInputItemType
{
    Day = 0,
    Month = 1,
    Year = 2
}

internal class DateInputContext : FormGroupContext3
{
    private bool _fieldsetIsOpen;
    private readonly SortedDictionary<DateInputItemType, DateInputContextItem> _items;
    private readonly bool _haveValue;

    public DateInputContext(bool haveExplicitValue, ModelExpression? @for)
    {
        _items = new SortedDictionary<DateInputItemType, DateInputContextItem>();
        _haveValue = haveExplicitValue;
        For = @for;
    }

    public ModelExpression? For { get; }

    public DateInputFieldsetContext? Fieldset { get; private set; }

    public IReadOnlyDictionary<DateInputItemType, DateInputContextItem> Items => _items;

    public DateInputItems? ErrorFields { get; private set; }

    protected override IReadOnlyCollection<string> ErrorMessageTagNames => [DateInputTagHelper.ErrorMessageTagName];

    protected string FieldsetTagName => DateInputFieldsetTagHelper.TagName;

    protected override IReadOnlyCollection<string> HintTagNames => [DateInputTagHelper.HintTagName];

    protected override IReadOnlyCollection<string> LabelTagNames => throw new NotSupportedException();

    protected override string RootTagName => DateInputTagHelper.TagName;

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

    public void CloseFieldset(DateInputFieldsetContext fieldsetContext)
    {
        if (!_fieldsetIsOpen)
        {
            throw new InvalidOperationException("Fieldset has not been opened.");
        }

        _fieldsetIsOpen = false;
        Fieldset = fieldsetContext;
    }

    public override void SetLabel(bool isPageHeading, AttributeCollection attributes, TemplateString? html, string tagName)
    {
        throw new NotSupportedException();
    }

    public override void SetHint(AttributeCollection attributes, TemplateString? html, string tagName)
    {
        if (Fieldset is not null)
        {
            throw new InvalidOperationException($"<{tagName}> must be inside <{FieldsetTagName}>.");
        }

        if (Items.Count > 0)
        {
            var firstItemTagName = _items.First().Value.TagName;
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, firstItemTagName!);
        }

        base.SetHint(attributes, html, tagName);
    }

    public void SetErrorMessage(
        DateInputItems? errorFields,
        TemplateString? visuallyHiddenText,
        AttributeCollection attributes,
        TemplateString? html,
        string tagName)
    {
        if (Fieldset is not null)
        {
            throw new InvalidOperationException($"<{tagName}> must be inside <{FieldsetTagName}>.");
        }

        if (Items.Count > 0)
        {
            var firstItemTagName = _items.First().Value.TagName!;
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, firstItemTagName);
        }

        ErrorFields = errorFields;

        base.SetErrorMessage(visuallyHiddenText, attributes, html, tagName);
    }

    public override void SetErrorMessage(TemplateString? visuallyHiddenText, AttributeCollection attributes, TemplateString? html, string tagName)
    {
        throw new NotSupportedException($"Use the overload that takes a {nameof(DateInputItems)} argument too.");
    }

    public void SetItem(DateInputItemType itemType, DateInputContextItem item)
    {
        ArgumentNullException.ThrowIfNull(itemType);
        ArgumentNullException.ThrowIfNull(item);

        if (_haveValue && item.ValueSpecified)
        {
            throw new InvalidOperationException($"Value cannot be specified for both <{item.TagName}> and the parent <{RootTagName}>.");
        }

        if (Fieldset is not null)
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
}
