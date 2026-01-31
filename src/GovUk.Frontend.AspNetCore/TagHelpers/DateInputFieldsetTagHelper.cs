using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the fieldset in a GDS date input component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = DateInputTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(ShortTagName, ParentTag = DateInputTagHelper.TagName)]
#endif
[RestrictChildren(
    DateInputFieldsetLegendTagHelper.TagName,
    DateInputHintTagHelper.TagName,
    DateInputErrorMessageTagHelper.TagName,
    DateInputDayTagHelper.TagName,
    DateInputMonthTagHelper.TagName,
    DateInputYearTagHelper.TagName
#if SHORT_TAG_NAMES
    ,
    FormGroupHintTagHelperBase.ShortTagName,
    FormGroupErrorMessageTagHelperBase.ShortTagName
#endif
    )]
public class DateInputFieldsetTagHelper : TagHelper
{
    internal const string TagName = "govuk-date-input-fieldset";
#if SHORT_TAG_NAMES
    internal const string ShortTagName = ShortTagNames.Fieldset;
#endif

    private const string DescribedByAttributeName = "described-by";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [
        TagName
#if SHORT_TAG_NAMES
        ,
        ShortTagName
#endif
    ];

    /// <summary>
    /// Creates a <see cref="DateInputFieldsetTagHelper"/>.
    /// </summary>
    public DateInputFieldsetTagHelper()
    {
    }

    /// <summary>
    /// One or more element IDs to add to the <c>aria-describedby</c> attribute.
    /// </summary>
    [HtmlAttributeName(DescribedByAttributeName)]
    public string? DescribedBy { get; set; }

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        var dateInputContext = context.GetContextItem<DateInputContext>();

        var fieldsetContext = new DateInputFieldsetContext(
            DescribedBy,
            dateInputContext.For);

        context.SetContextItem(fieldsetContext);
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var dateInputContext = context.GetContextItem<DateInputContext>();
        var fieldsetContext = context.GetContextItem<DateInputFieldsetContext>();

        dateInputContext.OpenFieldset(fieldsetContext, new AttributeCollection(output.Attributes));

        _ = await output.GetChildContentAsync();

        fieldsetContext.ThrowIfNotComplete(DateInputFieldsetLegendTagHelper.TagName);
        dateInputContext.CloseFieldset();

        output.SuppressOutput();
    }
}
