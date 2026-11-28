using System.Diagnostics;
using AngleSharp.Dom;
using Xunit;

namespace GovUk.Frontend.AspNetCore.Testing.Xunit;

public static partial class GovUkAssert
{
    extension(Assert)
    {
        /// <summary>
        /// Verifies that the field with a name shows an error message.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The field can be of any kind. It's found as <see cref="ComponentQueryExtensions.GetFieldByName"/> finds it,
        /// so <paramref name="name"/> is the name the field is submitted with and its model state error is keyed by,
        /// e.g. <c>DateOfBirth</c> for a date input, or the name of any of its inputs.
        /// </para>
        /// <para>
        /// To find the field by its label or legend, pass it to the other overload:
        /// <c>Assert.FieldHasError(doc.GetFieldByLabelOrLegend("Email address"), "Enter an email address")</c>.
        /// The error message is compared without its visually hidden <c>Error:</c> prefix.
        /// </para>
        /// <para>
        /// By default this also verifies that the page's error summary lists the error with a link to the field, as
        /// the error summary generated from model state does. Pass <c>inErrorSummary: false</c> for an error that
        /// isn't in the error summary, or a page without one.
        /// </para>
        /// </remarks>
        /// <param name="node">The document, or an element to search within.</param>
        /// <param name="name">The field's name, e.g. <c>Email</c>.</param>
        /// <param name="expectedError">The error message, e.g. <c>Enter an email address</c>.</param>
        /// <param name="inErrorSummary">
        /// Whether to verify that the error summary lists <paramref name="expectedError"/> with a link to an element in
        /// the field. The error summary is looked for in the whole document, not only within <paramref name="node"/>.
        /// </param>
        /// <returns>The field.</returns>
        /// <exception cref="GovUkAssertException">
        /// There's no such field, there's more than one, its error message isn't <paramref name="expectedError"/>, or
        /// <paramref name="inErrorSummary"/> is <see langword="true"/> and the error summary doesn't list the error
        /// with a link to the field.
        /// </exception>
        [StackTraceHidden]
        public static FormField FieldHasError(IParentNode node, string name, string expectedError, bool inErrorSummary = true)
        {
            ArgumentNullException.ThrowIfNull(node);
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(expectedError);

            var field = GetFieldByName(nameof(FieldHasError), node, name);

            return CheckFieldHasError(nameof(FieldHasError), field, expectedError, inErrorSummary);
        }

        /// <summary>
        /// Verifies that a field shows an error message.
        /// </summary>
        /// <remarks>
        /// Use this with a field you've already found, e.g. by its label or legend with
        /// <see cref="ComponentQueryExtensions.GetFieldByLabelOrLegend"/>.
        /// The error message is compared without its visually hidden <c>Error:</c> prefix.
        /// By default this also verifies that the page's error summary lists the error with a link to the field; pass
        /// <c>inErrorSummary: false</c> to skip that.
        /// </remarks>
        /// <param name="field">The field.</param>
        /// <param name="expectedError">The error message, e.g. <c>Enter an email address</c>.</param>
        /// <param name="inErrorSummary">
        /// Whether to verify that the field's document has an error summary listing <paramref name="expectedError"/>
        /// with a link to an element in the field.
        /// </param>
        /// <returns>The field.</returns>
        /// <exception cref="GovUkAssertException">
        /// The field's error message isn't <paramref name="expectedError"/>, or <paramref name="inErrorSummary"/> is
        /// <see langword="true"/> and the error summary doesn't list the error with a link to the field.
        /// </exception>
        [StackTraceHidden]
        public static FormField FieldHasError(FormField field, string expectedError, bool inErrorSummary = true)
        {
            ArgumentNullException.ThrowIfNull(field);
            ArgumentNullException.ThrowIfNull(expectedError);

            return CheckFieldHasError(nameof(FieldHasError), field, expectedError, inErrorSummary);
        }

        /// <summary>
        /// Verifies that the field with a name doesn't show an error message.
        /// </summary>
        /// <remarks>
        /// The field can be of any kind. It's found as <see cref="ComponentQueryExtensions.GetFieldByName"/> finds it,
        /// so <paramref name="name"/> is the name the field is submitted with and its model state error is keyed by,
        /// e.g. <c>DateOfBirth</c> for a date input, or the name of any of its inputs.
        /// To find the field by its label or legend, pass it to the other overload:
        /// <c>Assert.FieldHasNoError(doc.GetFieldByLabelOrLegend("Email address"))</c>.
        /// </remarks>
        /// <param name="node">The document, or an element to search within.</param>
        /// <param name="name">The field's name, e.g. <c>Email</c>.</param>
        /// <returns>The field.</returns>
        /// <exception cref="GovUkAssertException">
        /// There's no such field, there's more than one, or it has an error message.
        /// </exception>
        [StackTraceHidden]
        public static FormField FieldHasNoError(IParentNode node, string name)
        {
            ArgumentNullException.ThrowIfNull(node);
            ArgumentNullException.ThrowIfNull(name);

            var field = GetFieldByName(nameof(FieldHasNoError), node, name);

            return CheckFieldHasNoError(nameof(FieldHasNoError), field);
        }

        /// <summary>
        /// Verifies that a field doesn't show an error message.
        /// </summary>
        /// <remarks>
        /// Use this with a field you've already found, e.g. by its label or legend with
        /// <see cref="ComponentQueryExtensions.GetFieldByLabelOrLegend"/>.
        /// </remarks>
        /// <param name="field">The field.</param>
        /// <returns>The field.</returns>
        /// <exception cref="GovUkAssertException">The field has an error message.</exception>
        [StackTraceHidden]
        public static FormField FieldHasNoError(FormField field)
        {
            ArgumentNullException.ThrowIfNull(field);

            return CheckFieldHasNoError(nameof(FieldHasNoError), field);
        }

    }

    /// <summary>
    /// Gets the field with the name <paramref name="name"/>. If there isn't one but there's a field labelled
    /// <paramref name="name"/>, the failure says how to find it by its label instead.
    /// </summary>
    [StackTraceHidden]
    private static FormField GetFieldByName(string assertion, IParentNode node, string name)
    {
        try
        {
            return node.GetFieldByName(name);
        }
        catch (ComponentLookupException ex)
        {
            var hint = !node.Fields.Any(f => f.Names.Contains(name)) && node.Fields.Any(f => f.Label == name)
                ? $"there's a field labelled {Failure.Quote(name)}; to find it by its label or legend, pass " +
                    $"GetFieldByLabelOrLegend({Failure.Quote(name)}) to {assertion}"
                : null;

            throw Failure.FromLookup(assertion, ex.Failure, hint);
        }
    }

    [StackTraceHidden]
    private static FormField CheckFieldHasError(string assertion, FormField field, string expectedError, bool inErrorSummary)
    {
        if (field.ErrorMessage != expectedError)
        {
            throw Failure.Create(
                assertion,
                field.ErrorMessage is null ? "Field has no error" : "Error message differs",
                [
                    ("Field", field.ToString()),
                    ("Expected", Failure.Quote(expectedError)),
                    ("Actual", field.ErrorMessage is null ? "no error message" : Failure.Quote(field.ErrorMessage)),
                ],
                field.Element);
        }

        if (inErrorSummary)
        {
            CheckErrorSummaryLinksToField(assertion, field, expectedError);
        }

        return field;
    }

    private const string InErrorSummaryHint =
        "if the error shouldn't be in the error summary, pass inErrorSummary: false to FieldHasError";

    /// <summary>
    /// Checks that the error summary in <paramref name="field"/>'s document lists <paramref name="expectedError"/>
    /// with a link to an element in the field.
    /// </summary>
    [StackTraceHidden]
    private static void CheckErrorSummaryLinksToField(string assertion, FormField field, string expectedError)
    {
        // The summary is usually outside the element the field was looked for in, so look in the whole document.
        IParentNode scope = (IParentNode?)field.Element.Owner ?? field.Element;

        var summary = GetErrorSummary(
            assertion,
            scope,
            $"an error summary containing {Failure.Quote(expectedError)}",
            InErrorSummaryHint);

        var items = summary.Items.Where(i => i.Text == expectedError).ToArray();

        if (items.Length == 0)
        {
            throw Failure.Create(
                assertion,
                "Error not in error summary",
                [
                    ("Field", field.ToString()),
                    ("Expected", $"an error summary containing {Failure.Quote(expectedError)}"),
                    ("Actual", Failure.List(summary.Items.Select(i => i.Text))),
                    ("Hint", InErrorSummaryHint),
                ],
                summary.Element);
        }

        if (!items.Any(i => LinksTo(i, field)))
        {
            throw Failure.Create(
                assertion,
                "Error summary item doesn't link to the field",
                [
                    ("Field", field.ToString()),
                    ("Expected", $"{Failure.Quote(expectedError)} linking to an element in the field"),
                    ("Actual", string.Join(", ", items.Select(DescribeLink))),
                ],
                summary.Element);
        }
    }

    private static string DescribeLink(ErrorSummaryItem item) =>
        item.Href is null ? "no link" : $"a link to {Failure.Quote(item.Href)}";

    private static bool LinksTo(ErrorSummaryItem item, FormField field)
    {
        if (item.Href is not { Length: > 1 } href || href[0] != '#')
        {
            return false;
        }

        var target = field.Element.Owner?.GetElementById(Uri.UnescapeDataString(href[1..]));

        return target is not null && field.Element.Contains(target);
    }

    [StackTraceHidden]
    private static FormField CheckFieldHasNoError(string assertion, FormField field)
    {
        if (field.ErrorMessage is not null)
        {
            throw Failure.Create(
                assertion,
                "Field has an error",
                [
                    ("Field", field.ToString()),
                    ("Expected", "no error message"),
                    ("Actual", Failure.Quote(field.ErrorMessage)),
                ],
                field.Element);
        }

        return field;
    }
}
