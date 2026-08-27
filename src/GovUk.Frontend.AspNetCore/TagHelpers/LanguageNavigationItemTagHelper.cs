using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents a language in a GDS language navigation component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = LanguageNavigationTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = LanguageNavigationTagHelper.TagName)]
[TagHelperDocumentation(
    ContentDescription = "The content is the name of the language, written in that language.")]
public class LanguageNavigationItemTagHelper : TagHelper
{
    internal const string TagName = "govuk-language-navigation-item";
    internal const string ShortTagName = ShortTagNames.LanguageNavigationItem;

    private const string CurrentAttributeName = "current";
    private const string DirAttributeName = "dir";
    private const string HrefLangAttributeName = "hreflang";
    private const string LangAttributeName = "lang";
    private const string LanguageDescriptionTextAttributeName = "language-description-text";

    /// <summary>
    /// Whether this is the language of the current page.
    /// </summary>
    /// <remarks>
    /// Defaults to <see langword="true"/> when no <c>href</c> attribute is specified.
    /// </remarks>
    [HtmlAttributeName(CurrentAttributeName)]
    public bool? Current { get; set; }

    /// <summary>
    /// The text direction of the script the language name is written in.
    /// </summary>
    /// <remarks>
    /// Specify this on every item when the navigation includes scripts written in different directions.
    /// </remarks>
    [HtmlAttributeName(DirAttributeName)]
    public string? Dir { get; set; }

    /// <summary>
    /// The language tag for the linked page, added as an <c>hreflang</c> attribute for search engines
    /// and other machine readers.
    /// </summary>
    /// <remarks>
    /// Defaults to the <c>lang</c> attribute.
    /// </remarks>
    [HtmlAttributeName(HrefLangAttributeName)]
    public string? HrefLang { get; set; }

    /// <summary>
    /// The language tag for the language name, added as a <c>lang</c> attribute so that assistive
    /// technologies pronounce it correctly.
    /// </summary>
    [HtmlAttributeName(LangAttributeName)]
    public string? Lang { get; set; }

    /// <summary>
    /// The visually hidden text after the language's link indicating what the link will do.
    /// </summary>
    /// <remarks>
    /// Write this in the language of the link.
    /// </remarks>
    [HtmlAttributeName(LanguageDescriptionTextAttributeName)]
    public string? LanguageDescriptionText { get; set; }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var languageNavigationContext = context.GetContextItem<LanguageNavigationContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);
        attributes.Remove("href", out _);
        var href = output.GetUrlAttribute("href");

        languageNavigationContext.AddItem(new LanguageNavigationOptionsItem
        {
            Html = content.Snapshot(),
            Lang = Lang,
            HrefLang = HrefLang,
            Dir = Dir,
            Href = href,
            Current = Current,
            LanguageDescriptionText = LanguageDescriptionText,
            Classes = classes,
            Attributes = attributes
        });

        output.SuppressOutput();
    }
}
