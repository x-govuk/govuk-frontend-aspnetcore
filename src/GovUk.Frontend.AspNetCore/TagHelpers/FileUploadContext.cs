namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class FileUploadContext : FormGroupContext3
{
    protected override IReadOnlyCollection<string> ErrorMessageTagNames => FileUploadErrorMessageTagHelper.AllTagNames;

    protected override IReadOnlyCollection<string> HintTagNames => FileUploadHintTagHelper.AllTagNames;

    protected override IReadOnlyCollection<string> LabelTagNames => FileUploadLabelTagHelper.AllTagNames;

    protected override string RootTagName => FileUploadTagHelper.TagName;
}
