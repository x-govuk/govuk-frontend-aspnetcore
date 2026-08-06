using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS panel component.
/// </summary>
[HtmlTargetElement(TagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.Panel)]
[RestrictChildren(
    PanelTitleTagHelper.TagName,
    PanelTitleTagHelper.ShortTagName,
    PanelBodyTagHelper.TagName,
    PanelBodyTagHelper.ShortTagName,
    PanelActionsTagHelper.TagName,
    PanelActionsTagHelper.ShortTagName)]
public class PanelTagHelper : TagHelper
{
    internal const string TagName = "govuk-panel";

    private const string HeadingLevelAttributeName = "heading-level";
    private const string InterruptionClassName = "govuk-panel--interruption";

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
            if (value is < 1 or > 6)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    $"{nameof(HeadingLevel)} must be between 1 and 6.");
            }

            _headingLevel = value;
        }
    }

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        context.SetContextItem(new PanelContext());
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var panelContext = context.GetContextItem<PanelContext>();

        TagHelperContent content;

        content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        panelContext.ThrowIfNotComplete();

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        // Actions are only rendered for the interruption variant, so reject them otherwise
        // rather than silently dropping them.
        var isInterruption = !classes.IsEmpty() &&
            classes.ToText().Contains(InterruptionClassName, StringComparison.Ordinal);

        if (panelContext.Actions is not null && !isInterruption)
        {
            throw new InvalidOperationException(
                $"The <{panelContext.ActionsTagName}> element can only be used when the " +
                $"'{InterruptionClassName}' class is specified on the <{TagName}> element.");
        }

        var options = new PanelOptions
        {
            HeadingLevel = HeadingLevel ?? 1,
            TitleHtml = panelContext.Title?.Content,
            TitleAttributes = panelContext.Title?.Attributes,
            Html = panelContext.Body?.Content,
            BodyAttributes = panelContext.Body?.Attributes,
            Actions = panelContext.Actions,
            Classes = classes,
            Attributes = attributes
        };

        var component = await _componentGenerator.GeneratePanelAsync(options);
        component.ApplyToTagHelper(output);
    }
}
