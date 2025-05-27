using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class DateInputItemContext
{
    private readonly string _itemTagName;
    private readonly string _labelTagName;

    public DateInputItemContext(string itemTagName, string labelTagName)
    {
        ArgumentNullException.ThrowIfNull(itemTagName);
        ArgumentNullException.ThrowIfNull(labelTagName);
        _itemTagName = itemTagName;
        _labelTagName = labelTagName;
    }

    public (TemplateString Html, AttributeCollection Attributes, string TagName)? Label { get; private set; }

    public void SetLabel(TemplateString html, AttributeCollection attributes, string tagName)
    {
        ArgumentNullException.ThrowIfNull(html);
        ArgumentNullException.ThrowIfNull(attributes);
        ArgumentNullException.ThrowIfNull(tagName);

        if (Label is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(_labelTagName, _itemTagName);
        }

        Label = (html, attributes, tagName);
    }
}
