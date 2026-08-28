using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS language navigation component.
/// </summary>
[HtmlTargetElement(TagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.LanguageNavigation)]
[RestrictChildren(LanguageNavigationItemTagHelper.TagName, LanguageNavigationItemTagHelper.ShortTagName)]
public class LanguageNavigationTagHelper : TagHelper
{
    internal const string TagName = "govuk-language-navigation";

    private const string AriaLabelAttributeName = "aria-label";

    private readonly IComponentGenerator _componentGenerator;

    /// <summary>
    /// Creates a new <see cref="LanguageNavigationTagHelper"/>.
    /// </summary>
    public LanguageNavigationTagHelper(IComponentGenerator componentGenerator)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);

        _componentGenerator = componentGenerator;
    }

    /// <summary>
    /// The plain text label identifying the landmark to screen readers, written in the language of the current page.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>Language</c>.
    /// </remarks>
    [HtmlAttributeName(AriaLabelAttributeName)]
    public string? AriaLabel { get; set; }

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        context.SetContextItem(new LanguageNavigationContext());
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var languageNavigationContext = context.GetContextItem<LanguageNavigationContext>();

        _ = await output.GetChildContentAsync();

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        var component = await _componentGenerator.GenerateLanguageNavigationAsync(new LanguageNavigationOptions
        {
            AriaLabel = AriaLabel,
            Classes = classes,
            Attributes = attributes,
            Items = languageNavigationContext.Items
        });

        component.ApplyToTagHelper(output);
    }
}
