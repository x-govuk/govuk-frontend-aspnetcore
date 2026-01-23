using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS cookie banner component.
/// </summary>
[HtmlTargetElement(TagName)]
[RestrictChildren(CookieBannerMessageTagHelper.TagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.CookieBanner)]
public class CookieBannerTagHelper : TagHelper
{
    internal const string TagName = "govuk-cookie-banner";

    private const string AriaLabelAttributeName = "aria-label";
    private const string HiddenAttributeName = "hidden";

    private readonly IComponentGenerator _componentGenerator;

    /// <summary>
    /// Creates a new <see cref="CookieBannerTagHelper"/>.
    /// </summary>
    public CookieBannerTagHelper(IComponentGenerator componentGenerator)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);

        _componentGenerator = componentGenerator;
    }

    /// <summary>
    /// The text for the <c>aria-label</c> which labels the cookie banner region.
    /// </summary>
    [HtmlAttributeName(AriaLabelAttributeName)]
    public string? AriaLabel { get; set; }

    /// <summary>
    /// Whether the cookie banner should be hidden.
    /// </summary>
    /// <remarks>
    /// If not specified, <see langword="false" /> is used.
    /// </remarks>
    [HtmlAttributeName(HiddenAttributeName)]
    public bool? Hidden { get; set; }

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        context.SetContextItem(new CookieBannerContext());
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var cookieBannerContext = context.GetContextItem<CookieBannerContext>();

        _ = await output.GetChildContentAsync();

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        var component = await _componentGenerator.GenerateCookieBannerAsync(new CookieBannerOptions
        {
            AriaLabel = AriaLabel,
            Hidden = Hidden,
            Classes = classes,
            Attributes = attributes,
            Messages = cookieBannerContext.Messages
        });

        component.ApplyToTagHelper(output);
    }
}
