using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the content after the input in a GDS file upload component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = FileUploadTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(ShortTagName, ParentTag = FileUploadTagHelper.TagName)]
#endif
public class FileUploadAfterInputTagHelper : TagHelper
{
    private readonly ILogger<FileUploadAfterInputTagHelper> _logger;

    internal const string TagName = "govuk-file-upload-after-input";
#if SHORT_TAG_NAMES
    internal const string ShortTagName = ShortTagNames.AfterInput;
#endif

    internal static IReadOnlyCollection<string> AllTagNames { get; } = new[]
    {
        TagName
#if SHORT_TAG_NAMES
        ,
        ShortTagName
#endif
    };

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
            _logger.AttributesAreNotSupportedOnTagNameAndWillBeIgnored(output.TagName);
        }

        fileUploadContext.SetAfterInput(content.ToTemplateString(), output.TagName);

        output.SuppressOutput();
    }
}
