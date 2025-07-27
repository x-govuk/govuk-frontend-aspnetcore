using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the label in a GDS file upload component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = FileUploadTagHelper.TagName)]
//[HtmlTargetElement(ShortTagName, ParentTag = FileUploadTagHelper.TagName)]
public class FileUploadLabelTagHelper : FormGroupLabelTagHelperBase
{
    internal const string TagName = "govuk-file-upload-label";
}
