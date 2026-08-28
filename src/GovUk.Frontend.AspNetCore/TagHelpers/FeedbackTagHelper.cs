using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS feedback component.
/// </summary>
[HtmlTargetElement(TagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.Feedback)]
[RestrictChildren(
    FeedbackTitleTagHelper.TagName,
    FeedbackTitleTagHelper.ShortTagName,
    FeedbackBodyTagHelper.TagName,
    FeedbackBodyTagHelper.ShortTagName)]
public class FeedbackTagHelper : TagHelper
{
    internal const string TagName = "govuk-feedback";

    private const string HeadingLevelAttributeName = "heading-level";

    private readonly IComponentGenerator _componentGenerator;
    private int? _headingLevel;

    /// <summary>
    /// Creates a new <see cref="FeedbackTagHelper"/>.
    /// </summary>
    public FeedbackTagHelper(IComponentGenerator componentGenerator)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);

        _componentGenerator = componentGenerator;
    }

    /// <summary>
    /// The heading level of the title.
    /// </summary>
    /// <remarks>
    /// Must be between <c>1</c> and <c>6</c> (inclusive). The default is <c>2</c>.
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
        context.SetContextItem(new FeedbackContext());
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var feedbackContext = context.GetContextItem<FeedbackContext>();

        _ = await output.GetChildContentAsync();

        feedbackContext.ThrowIfNotComplete();

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        var component = await _componentGenerator.GenerateFeedbackAsync(new FeedbackOptions
        {
            HeadingLevel = HeadingLevel,
            TitleHtml = feedbackContext.Title?.Content,
            TitleAttributes = feedbackContext.Title?.Attributes,
            Html = feedbackContext.Body?.Content,
            BodyAttributes = feedbackContext.Body?.Attributes,
            Classes = classes,
            Attributes = attributes
        });

        component.ApplyToTagHelper(output);
    }
}
