using System.Diagnostics;
using System.Globalization;
using AngleSharp.Dom;
using Xunit;

namespace GovUk.Frontend.AspNetCore.Testing.Xunit;

public static partial class GovUkAssert
{
    extension(Assert)
    {
        /// <summary>
        /// Verifies that a summary list row has a value.
        /// </summary>
        /// <remarks>
        /// The row is found as <see cref="ComponentQueryExtensions.GetSummaryListRowByKey"/> finds it, across every
        /// summary list in <paramref name="node"/>; to check a row in a particular card, pass the card's
        /// <see cref="SummaryCard.Element"/>. The value is compared with <see cref="SummaryListRow.Value"/>, which has
        /// its lines joined with spaces.
        /// </remarks>
        /// <param name="node">The document, or an element to search within.</param>
        /// <param name="key">The row's key, e.g. <c>Email address</c>.</param>
        /// <param name="expectedValue">
        /// The row's value, or <see langword="null"/> or an empty string if the row has no value.
        /// </param>
        /// <returns>The row.</returns>
        /// <exception cref="GovUkAssertException">
        /// There's no row with the key <paramref name="key"/>, there's more than one, or its value isn't
        /// <paramref name="expectedValue"/>.
        /// </exception>
        [StackTraceHidden]
        public static SummaryListRow SummaryListHasRow(IParentNode node, string key, string? expectedValue)
        {
            ArgumentNullException.ThrowIfNull(node);
            ArgumentNullException.ThrowIfNull(key);

            var row = Failure.Lookup(nameof(SummaryListHasRow), () => node.GetSummaryListRowByKey(key));

            if (row.Value != (expectedValue ?? ""))
            {
                throw Failure.Create(
                    nameof(SummaryListHasRow),
                    "Value differs",
                    [("Key", Failure.Quote(key)), ("Expected", FormatValue(expectedValue)), ("Actual", FormatValue(row.Value))],
                    row.Element);
            }

            return row;
        }

        /// <summary>
        /// Verifies that a summary list has exactly these rows, in this order.
        /// </summary>
        /// <remarks>
        /// <paramref name="node"/> must contain exactly one summary list; to check one of several, pass its card's
        /// <see cref="SummaryCard.Element"/> or another element that contains only that list. Values are compared with
        /// <see cref="SummaryListRow.Value"/>, which has its lines joined with spaces.
        /// </remarks>
        /// <param name="node">The document, or an element to search within.</param>
        /// <param name="expectedRows">
        /// The rows' keys and values, in order; a <see langword="null"/> or empty value is a row with no value.
        /// </param>
        /// <returns>The summary list.</returns>
        /// <exception cref="GovUkAssertException">
        /// There's no summary list, there's more than one, or its rows differ from <paramref name="expectedRows"/>.
        /// </exception>
        [StackTraceHidden]
        public static SummaryList SummaryListHasRows(IParentNode node, params (string Key, string? Value)[] expectedRows)
        {
            ArgumentNullException.ThrowIfNull(node);
            ArgumentNullException.ThrowIfNull(expectedRows);

            var lists = node.SummaryLists;

            if (lists.Count == 0)
            {
                throw Failure.Create(
                    nameof(SummaryListHasRows),
                    "Summary list not found",
                    [("Expected", "a summary list"), ("Actual", "no summary lists")],
                    node as INode);
            }

            if (lists.Count > 1)
            {
                throw Failure.Create(
                    nameof(SummaryListHasRows),
                    "More than one summary list found",
                    [
                        ("Expected", "one summary list"),
                        ("Actual", $"{lists.Count.ToString(CultureInfo.InvariantCulture)} summary lists"),
                        ("Hint", "pass a summary card's Element, or another element that contains one summary list"),
                    ],
                    node as INode);
            }

            var list = lists[0];
            var actualRows = list.Rows.Select(r => (r.Key, r.Value)).ToArray();

            if (!actualRows.SequenceEqual(expectedRows.Select(r => (r.Key, r.Value ?? ""))))
            {
                throw Failure.Create(
                    nameof(SummaryListHasRows),
                    "Rows differ",
                    [("Expected", FormatRows(expectedRows)), ("Actual", FormatRows([.. actualRows]))],
                    list.Element);
            }

            return list;
        }
    }

    private static string FormatRows(IReadOnlyCollection<(string Key, string? Value)> rows) =>
        rows.Count == 0
            ? "no rows"
            : string.Join("\n", rows.Select(r => $"{Failure.Quote(r.Key)}: {FormatValue(r.Value)}"));

    private static string FormatValue(string? value) => string.IsNullOrEmpty(value) ? "no value" : Failure.Quote(value);
}
