using AngleSharp.Dom;

namespace GovUk.Frontend.AspNetCore.Testing;

/// <summary>
/// A notification banner component.
/// </summary>
public sealed class NotificationBanner
{
    internal const string Selector = ".govuk-notification-banner";

    internal NotificationBanner(IElement element)
    {
        Element = element;

        Title = TextExtraction.GetText(element.QuerySelector(".govuk-notification-banner__title"));

        var content = element.QuerySelector(".govuk-notification-banner__content");
        Heading = TextExtraction.GetTextOrNull(content?.QuerySelector(".govuk-notification-banner__heading"));
        ContentLines = content is null ? [] : TextExtraction.GetLines(content);
        Content = string.Join(" ", ContentLines);

        IsSuccess = element.ClassList.Contains("govuk-notification-banner--success");
    }

    /// <summary>
    /// Gets the banner's root element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the title in the banner's header, e.g. <c>Important</c> or <c>Success</c>.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the text of the element with the <c>govuk-notification-banner__heading</c> class, or
    /// <see langword="null"/> if there isn't one.
    /// </summary>
    /// <remarks>
    /// A banner with only text content renders it as its heading.
    /// </remarks>
    public string? Heading { get; }

    /// <summary>
    /// Gets all the text in the banner's content, including <see cref="Heading"/>, as a single line.
    /// </summary>
    public string Content { get; }

    /// <summary>
    /// Gets the lines of the banner's content, as split by line breaks and block-level elements.
    /// </summary>
    public IReadOnlyList<string> ContentLines { get; }

    /// <summary>
    /// Gets whether this is the success variant of the banner.
    /// </summary>
    public bool IsSuccess { get; }

    /// <inheritdoc/>
    public override string ToString() => $"Notification banner: {Title}: {Content}";
}
