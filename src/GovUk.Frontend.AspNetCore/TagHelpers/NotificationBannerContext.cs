using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class NotificationBannerContext
{
    public (string? Id, int? HeadingLevel, TemplateString? Content)? Title { get; private set; }

    public void SetTitle(string? id, int? headingLevel, TemplateString? content)
    {
        if (Title is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                NotificationBannerTitleTagHelper.TagName,
                NotificationBannerTagHelper.TagName);
        }

        Title = (id, headingLevel, content);
    }
}
