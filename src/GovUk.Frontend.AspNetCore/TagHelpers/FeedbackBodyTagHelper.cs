using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the body in a GDS feedback component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = FeedbackTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = FeedbackTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the feedback body.")]
public class FeedbackBodyTagHelper : TagHelper
{
    internal const string TagName = "govuk-feedback-body";
    internal const string ShortTagName = ShortTagNames.FeedbackBody;

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var feedbackContext = context.GetContextItem<FeedbackContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);

        feedbackContext.SetBody(content.Snapshot(), attributes, context.TagName);

        output.SuppressOutput();
    }
}
