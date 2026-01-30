using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class FieldsetContext
{
    public (bool IsPageHeading, AttributeCollection? Attributes, IHtmlContent Content)? Legend { get; private set; }

    public void SetLegend(
        bool isPageHeading,
        AttributeCollection? attributes,
        IHtmlContent content)
    {
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
