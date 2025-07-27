namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class FileUploadContext : FormGroupContext3
{
    protected override IReadOnlyCollection<string> ErrorMessageTagNames { get; } =
        [/*FileUploadErrorMessageTagHelper.ShortTagName, */FileUploadErrorMessageTagHelper.TagName];

    protected override IReadOnlyCollection<string> HintTagNames { get; } =
        [/*FileUploadHintTagHelper.ShortTagName, */FileUploadHintTagHelper.TagName];

    protected override IReadOnlyCollection<string> LabelTagNames { get; } =
        [/*FileUploadLabelTagHelper.ShortTagName, */FileUploadLabelTagHelper.TagName];

    protected override string RootTagName => FileUploadTagHelper.TagName;
}
