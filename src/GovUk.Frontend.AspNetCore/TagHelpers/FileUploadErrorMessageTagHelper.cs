using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the error message in a GDS file upload component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = FileUploadTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(ShortTagName, ParentTag = FileUploadTagHelper.TagName)]
#endif
public class FileUploadErrorMessageTagHelper : FormGroupErrorMessageTagHelperBase
{
    internal const string TagName = "govuk-file-upload-error-message";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [
        TagName
#if SHORT_TAG_NAMES
        ,
        ShortTagName
#endif
    ];
}
