using System.Diagnostics;
using AngleSharp.Dom;
using Xunit;

namespace GovUk.Frontend.AspNetCore.Testing.Xunit;

public static partial class GovUkAssert
{
    extension(Assert)
    {
        /// <summary>
        /// Verifies that the error summary lists an error.
        /// </summary>
        /// <param name="node">The document, or an element to search within.</param>
        /// <param name="expectedError">The error's text, e.g. <c>Enter your name</c>.</param>
        /// <returns>The error summary item.</returns>
        /// <exception cref="GovUkAssertException">
        /// There's no error summary, there's more than one, or it doesn't list <paramref name="expectedError"/>.
        /// </exception>
        [StackTraceHidden]
        public static ErrorSummaryItem ErrorSummaryContains(IParentNode node, string expectedError)
        {
            ArgumentNullException.ThrowIfNull(node);
            ArgumentNullException.ThrowIfNull(expectedError);

            var summary = GetErrorSummary(nameof(ErrorSummaryContains), node, $"an error summary containing {Failure.Quote(expectedError)}");

            return summary.Items.FirstOrDefault(i => i.Text == expectedError) ?? throw Failure.Create(
                nameof(ErrorSummaryContains),
                "Error not found",
                [("Expected", Failure.Quote(expectedError)), ("Actual", Failure.List(summary.Items.Select(i => i.Text)))],
                summary.Element);
        }

        /// <summary>
        /// Verifies that the error summary lists exactly these errors, in this order.
        /// </summary>
        /// <param name="node">The document, or an element to search within.</param>
        /// <param name="expectedErrors">The errors' text, in the order they should be listed.</param>
        /// <returns>The error summary.</returns>
        /// <exception cref="ArgumentException"><paramref name="expectedErrors"/> is empty.</exception>
        /// <exception cref="GovUkAssertException">
        /// There's no error summary, there's more than one, or it lists different errors.
        /// </exception>
        [StackTraceHidden]
        public static ErrorSummary ErrorSummaryIs(IParentNode node, params string[] expectedErrors)
        {
            ArgumentNullException.ThrowIfNull(node);
            ArgumentNullException.ThrowIfNull(expectedErrors);

            if (expectedErrors.Length == 0)
            {
                throw new ArgumentException(
                    $"At least one error is required. To check there are no errors, use {nameof(NoErrorSummary)}.",
                    nameof(expectedErrors));
            }

            var summary = GetErrorSummary(nameof(ErrorSummaryIs), node, $"an error summary with {Failure.List(expectedErrors)}");
            var actual = summary.Items.Select(i => i.Text).ToArray();

            if (!actual.SequenceEqual(expectedErrors, StringComparer.Ordinal))
            {
                throw Failure.Create(
                    nameof(ErrorSummaryIs),
                    "Errors differ",
                    [("Expected", Failure.List(expectedErrors)), ("Actual", Failure.List(actual))],
                    summary.Element);
            }

            return summary;
        }

        /// <summary>
        /// Verifies that there's no error summary.
        /// </summary>
        /// <param name="node">The document, or an element to search within.</param>
        /// <exception cref="GovUkAssertException">There's an error summary.</exception>
        [StackTraceHidden]
        public static void NoErrorSummary(IParentNode node)
        {
            ArgumentNullException.ThrowIfNull(node);

            // Not the ErrorSummary property, which would report more than one summary as a lookup failure.
            if (node.QuerySelector(ErrorSummary.Selector) is { } element)
            {
                var summary = new ErrorSummary(element);

                throw Failure.Create(
                    nameof(NoErrorSummary),
                    "Error summary found",
                    [("Expected", "no error summary"), ("Actual", Failure.List(summary.Items.Select(i => i.Text)))],
                    element);
            }
        }
    }

    private static ErrorSummary GetErrorSummary(string assertion, IParentNode node, string expected) =>
        Failure.Lookup(assertion, () => node.ErrorSummary) ?? throw Failure.Create(
            assertion,
            "Error summary not found",
            [("Expected", expected), ("Actual", "no error summary")],
            node as INode);
}
