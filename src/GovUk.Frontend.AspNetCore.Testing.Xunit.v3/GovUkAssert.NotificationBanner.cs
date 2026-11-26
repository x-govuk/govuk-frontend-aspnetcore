using System.Diagnostics;
using AngleSharp.Dom;
using Xunit;

namespace GovUk.Frontend.AspNetCore.Testing.Xunit;

public static partial class GovUkAssert
{
    extension(Assert)
    {
        /// <summary>
        /// Verifies that a notification banner contains some text.
        /// </summary>
        /// <remarks>
        /// The text has to be a whole line of the banner's content, such as its heading or a paragraph (see
        /// <see cref="NotificationBanner.ContentLines"/>), or all of its content. A part of a line doesn't match.
        /// </remarks>
        /// <param name="node">The document, or an element to search within.</param>
        /// <param name="expectedText">The text.</param>
        /// <returns>The first notification banner that contains <paramref name="expectedText"/>.</returns>
        /// <exception cref="GovUkAssertException">No notification banner contains <paramref name="expectedText"/>.</exception>
        [StackTraceHidden]
        public static NotificationBanner NotificationBannerContains(IParentNode node, string expectedText)
        {
            ArgumentNullException.ThrowIfNull(node);
            ArgumentNullException.ThrowIfNull(expectedText);

            return FindNotificationBanner(nameof(NotificationBannerContains), node, expectedText, requireSuccess: false);
        }

        /// <summary>
        /// Verifies that a success notification banner contains some text.
        /// </summary>
        /// <remarks>
        /// The text has to be a whole line of the banner's content, such as its heading or a paragraph (see
        /// <see cref="NotificationBanner.ContentLines"/>), or all of its content. A part of a line doesn't match.
        /// </remarks>
        /// <param name="node">The document, or an element to search within.</param>
        /// <param name="expectedText">The text.</param>
        /// <returns>The first success notification banner that contains <paramref name="expectedText"/>.</returns>
        /// <exception cref="GovUkAssertException">
        /// No notification banner contains <paramref name="expectedText"/>, or the ones that do aren't the success
        /// variant.
        /// </exception>
        [StackTraceHidden]
        public static NotificationBanner SuccessNotificationBannerContains(IParentNode node, string expectedText)
        {
            ArgumentNullException.ThrowIfNull(node);
            ArgumentNullException.ThrowIfNull(expectedText);

            return FindNotificationBanner(nameof(SuccessNotificationBannerContains), node, expectedText, requireSuccess: true);
        }
    }

    private static NotificationBanner FindNotificationBanner(string assertion, IParentNode node, string expectedText, bool requireSuccess)
    {
        var kind = requireSuccess ? "a success notification banner" : "a notification banner";
        var expected = $"{kind} containing {Failure.Quote(expectedText)}";
        var banners = node.NotificationBanners;

        if (banners.Count == 0)
        {
            throw Failure.Create(
                assertion,
                "Notification banner not found",
                [("Expected", expected), ("Actual", "no notification banner")],
                node as INode);
        }

        var containing = banners
            .Where(b => b.Content == expectedText || b.ContentLines.Contains(expectedText, StringComparer.Ordinal))
            .ToArray();

        if (containing.Length == 0)
        {
            throw Failure.Create(
                assertion,
                "Text not found",
                [("Expected", Failure.Quote(expectedText)), ("Actual", Failure.List(banners.SelectMany(b => b.ContentLines)))],
                banners.Count == 1 ? banners[0].Element : node as INode);
        }

        if (!requireSuccess)
        {
            return containing[0];
        }

        return containing.FirstOrDefault(b => b.IsSuccess) ?? throw Failure.Create(
            assertion,
            "Notification banner is not the success variant",
            [("Expected", expected), ("Actual", $"a notification banner with the title {Failure.Quote(containing[0].Title)}")],
            containing[0].Element);
    }
}
