using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS notification banner component.
/// </summary>
[HtmlTargetElement(TagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.NotificationBanner)]
public class NotificationBannerTagHelper : TagHelper
{
    internal const string TagName = "govuk-notification-banner";

    private const string DisableAutoFocusAttributeName = "disable-auto-focus";
    private const string RoleAttributeName = "role";
    private const string TypeAttributeName = "type";

    private readonly IComponentGenerator _componentGenerator;

    /// <summary>
    /// Creates a new <see cref="NotificationBannerTagHelper"/>.
    /// </summary>
    public NotificationBannerTagHelper(IComponentGenerator componentGenerator)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);
        _componentGenerator = componentGenerator;
    }

    /// <summary>
    /// Whether to disable the behavior that focuses the notification banner when the page loads.
    /// </summary>
    /// <remarks>
    /// Only applies when <see cref="Type"/> is <see cref="NotificationBannerType.Success"/>.
    /// </remarks>
    [HtmlAttributeName(DisableAutoFocusAttributeName)]
    public bool? DisableAutoFocus { get; set; }

    /// <summary>
    /// The <c>role</c> attribute for the notification banner.
    /// </summary>
    /// <remarks>
    /// If <see cref="Type"/> is <see cref="NotificationBannerType.Success"/> then the default is
    /// <c>&quot;alert&quot;</c> otherwise <c>&quot;region&quot;</c>.
    /// </remarks>
    [HtmlAttributeName(RoleAttributeName)]
    public string? Role { get; set; }

    /// <summary>
    /// The type of notification.
    /// </summary>
    [HtmlAttributeName(TypeAttributeName)]
    public NotificationBannerType? Type { get; set; }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var notificationBannerContext = new NotificationBannerContext();

        TagHelperContent content;

        using (context.SetScopedContextItem(notificationBannerContext))
        {
            content = await output.GetChildContentAsync();
        }

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        var options = new NotificationBannerOptions
        {
            Html = content.ToTemplateString(),
#pragma warning disable CA1308 // Type string needs to be lowercase for the liquid template
            Type = Type.HasValue ? new TemplateString(Type.Value.ToString().ToLowerInvariant()) : null,
#pragma warning restore CA1308
            Role = Role != null ? new TemplateString(Role) : null,
            DisableAutoFocus = DisableAutoFocus,
            TitleId = notificationBannerContext.Title?.Id != null ? new TemplateString(notificationBannerContext.Title.Value.Id) : null,
            TitleHeadingLevel = notificationBannerContext.Title?.HeadingLevel,
            TitleHtml = notificationBannerContext.Title?.Content,
            Classes = classes,
            Attributes = attributes
        };

        var component = await _componentGenerator.GenerateNotificationBannerAsync(options);
        component.ApplyToTagHelper(output);
    }
}
