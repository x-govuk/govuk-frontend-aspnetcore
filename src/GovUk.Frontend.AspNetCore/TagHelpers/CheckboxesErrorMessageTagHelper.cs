using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <inheritdoc/>
[HtmlTargetElement(TagName, ParentTag = CheckboxesTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(ShortTagName, ParentTag = CheckboxesTagHelper.TagName)]
#endif
[HtmlTargetElement(TagName, ParentTag = CheckboxesFieldsetTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(ShortTagName, ParentTag = CheckboxesFieldsetTagHelper.TagName)]
[HtmlTargetElement(TagName, ParentTag = CheckboxesFieldsetTagHelper.ShortTagName)]
[HtmlTargetElement(ShortTagName, ParentTag = CheckboxesFieldsetTagHelper.ShortTagName)]
#endif
public class CheckboxesErrorMessageTagHelper : FormGroupErrorMessageTagHelperBase
{
    internal const string TagName = "govuk-checkboxes-error-message";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [
        TagName
#if SHORT_TAG_NAMES
        ,
        ShortTagName
#endif
    ];

    private protected override void SetErrorMessage(TagHelperContent? content, TagHelperContext context, TagHelperOutput output)
    {
        var checkboxesContext = context.GetContextItem<CheckboxesContext>();

        var attributes = new AttributeCollection(output.Attributes);

        checkboxesContext.SetErrorMessage(
            VisuallyHiddenText is not null ? new TemplateString(VisuallyHiddenText) : null,
            attributes,
            content?.ToTemplateString(),
            context.TagName);
    }
}
