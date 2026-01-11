using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;
using AttributeCollection = GovUk.Frontend.AspNetCore.ComponentGeneration.AttributeCollection;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS panel component.
/// </summary>
[HtmlTargetElement(TagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.Panel)]
[RestrictChildren(PanelTitleTagHelper.TagName, PanelBodyTagHelper.TagName)]
public class PanelTagHelper : TagHelper
{
    internal const string TagName = "govuk-panel";

    private const string HeadingLevelAttributeName = "heading-level";
    private const int DefaultHeadingLevel = 1;
    private const int MinHeadingLevel = 1;
    private const int MaxHeadingLevel = 6;

    private readonly IComponentGenerator _componentGenerator;
    private int? _headingLevel;

    /// <summary>
    /// Creates a new <see cref="PanelTagHelper"/>.
    /// </summary>
    public PanelTagHelper(IComponentGenerator componentGenerator)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);
        _componentGenerator = componentGenerator;
    }

    /// <summary>
    /// The heading level.
    /// </summary>
    /// <remarks>
    /// Must be between <c>1</c> and <c>6</c> (inclusive). The default is <c>1</c>.
    /// </remarks>
    [HtmlAttributeName(HeadingLevelAttributeName)]
    public int? HeadingLevel
    {
        get => _headingLevel;
        set
        {
            if (value is < MinHeadingLevel or > MaxHeadingLevel)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    $"{nameof(HeadingLevel)} must be between {MinHeadingLevel} and {MaxHeadingLevel}.");
            }

            _headingLevel = value;
        }
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var panelContext = new PanelContext();

        using (context.SetScopedContextItem(panelContext))
        {
            _ = await output.GetChildContentAsync();
        }

        panelContext.ThrowIfNotComplete();

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        var component = await _componentGenerator.GeneratePanelAsync(new PanelOptions()
        {
            TitleHtml = panelContext.Title?.ToTemplateString(),
            Html = panelContext.Body?.ToTemplateString(),
            HeadingLevel = HeadingLevel ?? DefaultHeadingLevel,
            Classes = classes,
            Attributes = attributes
        });

        component.ApplyToTagHelper(output);
    }
}
