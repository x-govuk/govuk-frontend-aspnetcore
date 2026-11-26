using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS exit this page component.
/// </summary>
[HtmlTargetElement(TagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.ExitThisPage)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the exit this page button. If there is no content, the button says \"Exit this page\" with a visually hidden \"Emergency\" prefix.")]
public class ExitThisPageTagHelper : TagHelper
{
    internal const string TagName = "govuk-exit-this-page";

    private const string ActivatedTextAttributeName = "activated-text";
    private const string PressOneMoreTimeTextAttributeName = "press-one-more-time-text";
    private const string PressTwoMoreTimesTextAttributeName = "press-two-more-times-text";
    private const string RedirectUrlAttributeName = "redirect-url";
    private const string TimedOutTextAttributeName = "timed-out-text";

    private readonly IComponentGenerator _componentGenerator;

    /// <summary>
    /// Creates a new <see cref="ExitThisPageTagHelper"/>.
    /// </summary>
    public ExitThisPageTagHelper(IComponentGenerator componentGenerator)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);

        _componentGenerator = componentGenerator;
    }

    /// <summary>
    /// The URL that the exit this page button redirects to.
    /// </summary>
    /// <remarks>
    /// The default is <c>&quot;https://www.bbc.co.uk/weather&quot;</c>.
    /// </remarks>
    [HtmlAttributeName(RedirectUrlAttributeName)]
#pragma warning disable CA1056
    public string? RedirectUrl { get; set; }
#pragma warning restore CA1056

    /// <summary>
    /// The text announced by screen readers when the exit this page keyboard shortcut has been activated.
    /// </summary>
    /// <remarks>
    /// If not specified, the govuk-frontend JavaScript default is used.
    /// </remarks>
    [HtmlAttributeName(ActivatedTextAttributeName)]
    public string? ActivatedText { get; set; }

    /// <summary>
    /// The text announced by screen readers when the keyboard shortcut has timed out without successful activation.
    /// </summary>
    /// <remarks>
    /// If not specified, the govuk-frontend JavaScript default is used.
    /// </remarks>
    [HtmlAttributeName(TimedOutTextAttributeName)]
    public string? TimedOutText { get; set; }

    /// <summary>
    /// The text announced by screen readers when the user must press <c>Shift</c> two more times to activate the button.
    /// </summary>
    /// <remarks>
    /// If not specified, the govuk-frontend JavaScript default is used.
    /// </remarks>
    [HtmlAttributeName(PressTwoMoreTimesTextAttributeName)]
    public string? PressTwoMoreTimesText { get; set; }

    /// <summary>
    /// The text announced by screen readers when the user must press <c>Shift</c> one more time to activate the button.
    /// </summary>
    /// <remarks>
    /// If not specified, the govuk-frontend JavaScript default is used.
    /// </remarks>
    [HtmlAttributeName(PressOneMoreTimeTextAttributeName)]
    public string? PressOneMoreTimeText { get; set; }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);
        attributes.Remove("id", out var id);

        var component = await _componentGenerator.GenerateExitThisPageAsync(new ExitThisPageOptions
        {
            Text = null,
            Html = content.IsEmptyOrWhiteSpace ? null : content.Snapshot(),
            RedirectUrl = RedirectUrl,
            Id = id,
            Classes = classes,
            Attributes = attributes,
            ActivatedText = ActivatedText,
            TimedOutText = TimedOutText,
            PressTwoMoreTimesText = PressTwoMoreTimesText,
            PressOneMoreTimeText = PressOneMoreTimeText
        });

        component.ApplyToTagHelper(output);
    }
}
