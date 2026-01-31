using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class RadiosContext(string? name, ModelExpression? @for) : FormGroupContext3
{
    private bool _fieldsetIsOpen;
    private readonly List<RadiosOptionsItem> _items = [];

    public string? Name { get; } = name;

    public ModelExpression? For { get; } = @for;

    public IReadOnlyCollection<RadiosOptionsItem> Items => _items;

    public AttributeCollection? Attributes { get; private set; }

    public RadiosFieldsetContext? Fieldset { get; private set; }

    protected override IReadOnlyCollection<string> ErrorMessageTagNames => RadiosErrorMessageTagHelper.AllTagNames;

    protected string FieldsetTagName => RadiosFieldsetTagHelper.TagName;

    protected string ItemTagName => RadiosItemTagHelper.TagName;

    protected override IReadOnlyCollection<string> HintTagNames => RadiosHintTagHelper.AllTagNames;

    protected override IReadOnlyCollection<string> LabelTagNames => throw new NotSupportedException();

    protected override string RootTagName => RadiosTagHelper.TagName;

    public void AddItem(RadiosOptionsItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (Fieldset is not null && !_fieldsetIsOpen)
        {
            throw new InvalidOperationException($"<{ItemTagName}> must be inside <{FieldsetTagName}>.");
        }

        _items.Add(item);
    }

    public FieldsetOptions? GetFieldsetOptions(IModelHelper modelHelper) => Fieldset?.GetFieldsetOptions(modelHelper, Attributes!);

    public void OpenFieldset(RadiosFieldsetContext fieldsetContext, AttributeCollection attributes)
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

        if (Items.Count > 0)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, ItemTagName);
        }

        base.SetErrorMessage(visuallyHiddenText, attributes, html, tagName);
    }

    public override void SetHint(AttributeCollection attributes, TemplateString? html, string tagName)
    {
        if (Fieldset is not null && !_fieldsetIsOpen)
        {
            throw new InvalidOperationException($"<{tagName}> must be inside <{FieldsetTagName}>.");
        }

        if (Items.Count > 0)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, ItemTagName);
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
