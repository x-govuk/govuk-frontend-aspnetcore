using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class PanelContext
{
    public (TemplateString? Content, AttributeCollection? Attributes)? Body { get; private set; }
    public (TemplateString? Content, AttributeCollection? Attributes)? Title { get; private set; }

    public void SetBody(TemplateString? content, AttributeCollection? attributes)
    {
        if (Body is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(PanelBodyTagHelper.TagName, PanelTagHelper.TagName);
        }

        Body = (content, attributes);
    }

    public void SetTitle(TemplateString? content, AttributeCollection? attributes)
    {
        if (Title is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(PanelTitleTagHelper.TagName, PanelTagHelper.TagName);
        }

        if (Body is not null)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(PanelTitleTagHelper.TagName, PanelBodyTagHelper.TagName);
        }

        Title = (content, attributes);
    }

    public void ThrowIfNotComplete()
    {
        if (Title is null)
        {
            throw ExceptionHelper.AChildElementMustBeProvided(PanelTitleTagHelper.TagName);
        }
    }
}
