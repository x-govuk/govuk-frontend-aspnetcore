using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class ServiceNavigationContext
{
    public ServiceNavigationNavContext? Nav { get; set; }
    public (IHtmlContent Html, string TagName)? StartSlot { get; set; }
    public (IHtmlContent Html, ServiceNavigationEndSlotAlign? Align, string TagName)? EndSlot { get; set; }

    /// <summary>
    /// Checks that <paramref name="tagName"/> is spelled the same way as the children specified so far.
    /// </summary>
    /// <remarks>
    /// The children all have &lt;govuk-service-navigation&gt; for their parent so, unlike the navigation's
    /// own children, their spelling cannot be paired up through <c>ParentTag</c>.
    /// </remarks>
    public void CheckChildTagNameSpelling(string tagName)
    {
        var siblingTagName = StartSlot?.TagName ?? Nav?.TagName ?? EndSlot?.TagName;

        if (siblingTagName is not null && UsesShortTagName(tagName) != UsesShortTagName(siblingTagName))
        {
            throw ExceptionHelper.ShortAndGovUkPrefixedTagNamesCannotBeMixed(tagName, siblingTagName);
        }

        static bool UsesShortTagName(string tagName) => tagName is
            ServiceNavigationStartTagHelper.ShortTagName or
            ServiceNavigationNavTagHelper.ShortTagName or
            ServiceNavigationEndTagHelper.ShortTagName;
    }
}
