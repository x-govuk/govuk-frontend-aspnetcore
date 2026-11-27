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
        /// </remarks>
        /// <param name="node">The document, or an element to search within.</param>
        /// <param name="name">The field's name, e.g. <c>Email</c>.</param>
        /// <param name="expectedError">The error message, e.g. <c>Enter an email address</c>.</param>
        /// <returns>The field.</returns>
        /// <exception cref="GovUkAssertException">
        /// There's no such field, there's more than one, or its error message isn't <paramref name="expectedError"/>.
        /// </exception>
        [StackTraceHidden]
        public static FormField FieldHasError(IParentNode node, string name, string expectedError)
        {
            ArgumentNullException.ThrowIfNull(node);
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(expectedError);

            var field = GetFieldByName(nameof(FieldHasError), node, name);

            return CheckFieldHasError(nameof(FieldHasError), field, expectedError);
        }

        /// <summary>
        /// Verifies that a field shows an error message.
        /// </summary>
        /// <remarks>
        /// Use this with a field you've already found, e.g. by its label or legend with
        /// <see cref="ComponentQueryExtensions.GetFieldByLabelOrLegend"/>.
        /// The error message is compared without its visually hidden <c>Error:</c> prefix.
        /// </remarks>
        /// <param name="field">The field.</param>
        /// <param name="expectedError">The error message, e.g. <c>Enter an email address</c>.</param>
        /// <returns>The field.</returns>
        /// <exception cref="GovUkAssertException">The field's error message isn't <paramref name="expectedError"/>.</exception>
        [StackTraceHidden]
        public static FormField FieldHasError(FormField field, string expectedError)
        {
            ArgumentNullException.ThrowIfNull(field);
            ArgumentNullException.ThrowIfNull(expectedError);

            return CheckFieldHasError(nameof(FieldHasError), field, expectedError);
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
    private static FormField CheckFieldHasError(string assertion, FormField field, string expectedError)
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

        return field;
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
