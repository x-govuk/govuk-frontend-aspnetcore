using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class FooterContext
{
    public (FooterOptionsMeta Options, string TagName)? Meta { get; set; }
    public List<FooterOptionsNavigation> Navigation { get; } = new();
    public (FooterOptionsContentLicence Options, string TagName)? ContentLicence { get; set; }
    public (FooterOptionsCopyright Options, string TagName)? Copyright { get; set; }
}
