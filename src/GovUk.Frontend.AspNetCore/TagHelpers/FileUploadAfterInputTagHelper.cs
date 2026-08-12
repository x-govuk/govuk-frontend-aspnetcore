using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the content after the input in a GDS file upload component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = FileUploadTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = FileUploadTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use after the generated input element.")]
public class FileUploadAfterInputTagHelper : TagHelper
{
    private readonly ILogger<FileUploadAfterInputTagHelper> _logger;

    internal const string TagName = "govuk-file-upload-after-input";
    internal const string ShortTagName = ShortTagNames.AfterInput;

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];

    /// <summary>
    /// Creates a new <see cref="FileUploadAfterInputTagHelper"/>.
    /// </summary>
    public FileUploadAfterInputTagHelper(ILogger<FileUploadAfterInputTagHelper> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var fileUploadContext = context.GetContextItem<FileUploadContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        if (output.Attributes.Count > 0)
        {
            _logger.AttributesAreNotSupportedOnTagNameAndWillBeIgnored(context.TagName);
        }

        fileUploadContext.SetAfterInput(content.Snapshot(), context.TagName);

        output.SuppressOutput();
    }
}
