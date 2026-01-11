using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the title in a GDS notification banner component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = NotificationBannerTagHelper.TagName)]
public class NotificationBannerTitleTagHelper : TagHelper
{
    internal const string TagName = "govuk-notification-banner-title";

    private const string HeadingLevelAttributeName = "heading-level";
    private const string IdAttributeName = "id";

    /// <summary>
    /// The heading level for the notification banner title.
    /// </summary>
    /// <remarks>
    /// Must be between <c>1</c> and <c>6</c> (inclusive). The default is <c>2</c>.
    /// </remarks>
    [HtmlAttributeName(HeadingLevelAttributeName)]
    public int? HeadingLevel { get; set; }

    /// <summary>
    /// The <c>id</c> attribute for the notification banner title.
    /// </summary>
    /// <remarks>
    /// The default is <c>&quot;govuk-notification-banner-title&quot;</c>.
    /// </remarks>
    [HtmlAttributeName(IdAttributeName)]
    public string? Id { get; set; }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        if (HeadingLevel is not null and not (>= Constants.NotificationBannerMinHeadingLevel and <= Constants.NotificationBannerMaxHeadingLevel))
        {
            throw new InvalidOperationException(
                $"The '{HeadingLevelAttributeName}' attribute must be between {Constants.NotificationBannerMinHeadingLevel} and {Constants.NotificationBannerMaxHeadingLevel} (inclusive).");
        }

        var notificationBannerContext = context.GetContextItem<NotificationBannerContext>();

        var content = output.TagMode == TagMode.StartTagAndEndTag ?
            await output.GetChildContentAsync() :
            null;

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        notificationBannerContext.SetTitle(
            Id ?? Constants.NotificationBannerDefaultTitleId,
            HeadingLevel ?? Constants.NotificationBannerDefaultTitleHeadingLevel,
            content.ToTemplateString());

        output.SuppressOutput();
    }
}
