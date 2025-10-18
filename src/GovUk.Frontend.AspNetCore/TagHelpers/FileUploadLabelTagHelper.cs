using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the label in a GDS file upload component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = FileUploadTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(ShortTagName, ParentTag = FileUploadTagHelper.TagName)]
#endif
public class FileUploadLabelTagHelper : FormGroupLabelTagHelperBase
{
    internal const string TagName = "govuk-file-upload-label";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [
        TagName
#if SHORT_TAG_NAMES
        ,
        ShortTagName
#endif
    ];
}
