using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <inheritdoc/>
[HtmlTargetElement(TagName, ParentTag = RadiosTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(ShortTagName, ParentTag = RadiosTagHelper.TagName)]
#endif
[HtmlTargetElement(TagName, ParentTag = RadiosFieldsetTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(ShortTagName, ParentTag = RadiosFieldsetTagHelper.TagName)]
#endif
public class RadiosErrorMessageTagHelper : FormGroupErrorMessageTagHelperBase
{
    internal const string TagName = "govuk-radios-error-message";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [
        TagName
#if SHORT_TAG_NAMES
        ,
        ShortTagName
#endif
    ];

    private protected override void SetErrorMessage(TagHelperContent? content, TagHelperContext context, TagHelperOutput output)
    {
        var radiosContext = context.GetContextItem<RadiosContext>();

        var attributes = new AttributeCollection(output.Attributes);

        radiosContext.SetErrorMessage(
            VisuallyHiddenText is not null ? new TemplateString(VisuallyHiddenText) : null,
            attributes,
            content?.ToTemplateString(),
            context.TagName);
    }
}
