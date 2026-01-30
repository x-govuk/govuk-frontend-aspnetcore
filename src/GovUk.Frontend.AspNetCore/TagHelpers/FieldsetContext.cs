using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class FieldsetContext
{
    public (bool? IsPageHeading, AttributeCollection Attributes, TemplateString Content)? Legend { get; private set; }

    public void SetLegend(
        bool? isPageHeading,
        AttributeCollection attributes,
        TemplateString content)
    {
        ArgumentNullException.ThrowIfNull(attributes);
        ArgumentNullException.ThrowIfNull(content);

        if (Legend is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                FieldsetLegendTagHelper.TagName,
                FieldsetTagHelper.TagName);
        }

        Legend = (isPageHeading, attributes, content);
    }

    public void ThrowIfNotComplete()
    {
        if (Legend is null)
        {
            throw ExceptionHelper.AChildElementMustBeProvided(FieldsetLegendTagHelper.TagName);
        }
    }
}
