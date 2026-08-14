using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class FooterContext
{
    public (FooterOptionsMeta Options, string TagName)? Meta { get; set; }
    public List<FooterOptionsNavigation> Navigation { get; } = [];

    /// <summary>
    /// The spelling the nav sections were written with. There can be several of them and they carry
    /// no tag name of their own, so the first one's is kept for <see cref="CheckChildTagNameSpelling"/>.
    /// </summary>
    public string? NavigationTagName { get; set; }

    public (FooterOptionsContentLicence Options, string TagName)? ContentLicence { get; set; }
    public (FooterOptionsCopyright Options, string TagName)? Copyright { get; set; }

    /// <summary>
    /// Checks that <paramref name="tagName"/> is spelled the same way as the children specified so far.
    /// </summary>
    /// <remarks>
    /// The children all have &lt;govuk-footer&gt; for their parent so, unlike the children of a nav or
    /// meta section, their spelling cannot be paired up through <c>ParentTag</c>.
    /// </remarks>
    public void CheckChildTagNameSpelling(string tagName)
    {
        var siblingTagName =
            NavigationTagName ?? Meta?.TagName ?? ContentLicence?.TagName ?? Copyright?.TagName;

        if (siblingTagName is not null && UsesShortTagName(tagName) != UsesShortTagName(siblingTagName))
        {
            throw ExceptionHelper.ShortAndGovUkPrefixedTagNamesCannotBeMixed(tagName, siblingTagName);
        }

        static bool UsesShortTagName(string tagName) => tagName is
            FooterNavTagHelper.ShortTagName or
            FooterMetaTagHelper.ShortTagName or
            FooterContentLicenceTagHelper.ShortTagName or
            FooterCopyrightTagHelper.ShortTagName;
    }
}
