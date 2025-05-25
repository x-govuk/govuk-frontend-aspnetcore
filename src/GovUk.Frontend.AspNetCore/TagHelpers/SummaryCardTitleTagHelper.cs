using GovUk.Frontend.AspNetCore.Components;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the title in the GDS summary card component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = SummaryCardTagHelper.TagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.SummaryCardTitle)]
public class SummaryCardTitleTagHelper : TagHelper
{
    internal const string TagName = "govuk-summary-card-title";

    private const int MinHeadingLevel = 1;
    private const int MaxHeadingLevel = 6;

    private int? _headingLevel;

    /// <summary>
    /// The heading level.
    /// </summary>
    /// <remarks>
    /// Must be between <c>1</c> and <c>6</c> (inclusive). The default is <c>2</c>.
    /// </remarks>
    [HtmlAttributeName("heading-level")]
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
        var cardContext = context.GetContextItem<SummaryCardContext>();

        var childContent = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            childContent = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        cardContext.SetTitle(new SummaryListOptionsCardTitle()
        {
            Text = null,
            Html = childContent.ToTemplateString(),
            HeadingLevel = HeadingLevel,
            Classes = classes,
            Attributes = attributes
        });

        output.SuppressOutput();
    }
}
