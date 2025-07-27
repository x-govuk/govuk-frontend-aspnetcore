using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the error message in a GDS file upload component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = FileUploadTagHelper.TagName)]
//[HtmlTargetElement(ShortTagName, ParentTag = FileUploadTagHelper.TagName)]
public class FileUploadErrorMessageTagHelper : FormGroupErrorMessageTagHelperBase
{
    internal const string TagName = "govuk-file-upload-error-message";
}
