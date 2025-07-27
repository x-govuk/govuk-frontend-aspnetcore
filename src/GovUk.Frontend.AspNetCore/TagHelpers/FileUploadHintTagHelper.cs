using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the hint in a GDS file upload component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = FileUploadTagHelper.TagName)]
//[HtmlTargetElement(ShortTagName, ParentTag = FileUploadTagHelper.TagName)]
public class FileUploadHintTagHelper : FormGroupHintTagHelperBase
{
    internal const string TagName = "govuk-file-upload-hint";
}
