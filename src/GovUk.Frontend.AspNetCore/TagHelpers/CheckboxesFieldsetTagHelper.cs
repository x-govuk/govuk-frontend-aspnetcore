using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the fieldset in a GDS checkboxes component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CheckboxesTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(ShortTagName, ParentTag = CheckboxesTagHelper.TagName)]
#endif
[RestrictChildren(
    CheckboxesFieldsetLegendTagHelper.TagName,
    CheckboxesItemTagHelper.TagName,
    CheckboxesItemDividerTagHelper.TagName,
    CheckboxesHintTagHelper.TagName,
    CheckboxesErrorMessageTagHelper.TagName,
    CheckboxesBeforeInputsTagHelper.TagName,
    CheckboxesAfterInputsTagHelper.TagName
#if SHORT_TAG_NAMES
    ,
    FormGroupHintTagHelperBase.ShortTagName,
    FormGroupErrorMessageTagHelperBase.ShortTagName
#endif
)]
public class CheckboxesFieldsetTagHelper : TagHelper
{
    internal const string TagName = "govuk-checkboxes-fieldset";
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
    /// One or more element IDs to add to the <c>aria-describedby</c> attribute.
    /// </summary>
    [HtmlAttributeName(DescribedByAttributeName)]
    public string? DescribedBy { get; set; }

    /// <summary>
    /// Creates a <see cref="CheckboxesFieldsetTagHelper"/>.
    /// </summary>
    public CheckboxesFieldsetTagHelper()
    {
    }

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        var checkboxesContext = context.GetContextItem<CheckboxesContext>();

        var fieldsetContext = new CheckboxesFieldsetContext(
            DescribedBy,
            @for: checkboxesContext.For);

        context.SetContextItem(fieldsetContext);
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var checkboxesContext = context.GetContextItem<CheckboxesContext>();
        var fieldsetContext = context.GetContextItem<CheckboxesFieldsetContext>();

        checkboxesContext.OpenFieldset(fieldsetContext, new AttributeCollection(output.Attributes));

        _ = await output.GetChildContentAsync();

        fieldsetContext.ThrowIfNotComplete(CheckboxesFieldsetLegendTagHelper.TagName);
        checkboxesContext.CloseFieldset();

        output.SuppressOutput();
    }
}
