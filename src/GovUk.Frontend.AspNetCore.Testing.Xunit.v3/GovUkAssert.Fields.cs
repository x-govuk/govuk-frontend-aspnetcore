using System.Diagnostics;
using AngleSharp.Dom;
using Xunit;

namespace GovUk.Frontend.AspNetCore.Testing.Xunit;

public static partial class GovUkAssert
{
    extension(Assert)
    {
        /// <summary>
        /// Verifies that a field shows an error message.
        /// </summary>
        /// <remarks>
        /// The field can be of any kind. It's found as <see cref="ComponentQueryExtensions.GetFieldByLabelOrLegend"/>
        /// finds it. The error message is compared without its visually hidden <c>Error:</c> prefix.
        /// </remarks>
        /// <param name="node">The document, or an element to search within.</param>
        /// <param name="labelOrLegend">
        /// The field's label or, for date inputs, radios and checkboxes, its fieldset's legend.
        /// </param>
        /// <param name="expectedError">The error message, e.g. <c>Enter an email address</c>.</param>
        /// <returns>The field.</returns>
        /// <exception cref="GovUkAssertException">
        /// There's no such field, there's more than one, or its error message isn't <paramref name="expectedError"/>.
        /// </exception>
        [StackTraceHidden]
        public static FormField FieldHasError(IParentNode node, string labelOrLegend, string expectedError)
        {
            ArgumentNullException.ThrowIfNull(node);
            ArgumentNullException.ThrowIfNull(labelOrLegend);
            ArgumentNullException.ThrowIfNull(expectedError);

            var field = Failure.Lookup(nameof(FieldHasError), () => node.GetFieldByLabelOrLegend(labelOrLegend));

            return CheckFieldHasError(nameof(FieldHasError), field, expectedError);
        }

        /// <summary>
        /// Verifies that a field shows an error message.
        /// </summary>
        /// <remarks>
        /// Use this with a field you've already found, e.g. one of a document's <c>Fields</c>.
        /// To find the field by its label or legend, or by its name, use the other overload or
        /// <see cref="FieldWithNameHasError"/>.
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
        /// Verifies that the field with a name shows an error message.
        /// </summary>
        /// <remarks>
        /// The field can be of any kind. It's found as <see cref="ComponentQueryExtensions.GetFieldByName"/> finds it,
        /// so it's the name model state errors are keyed by, e.g. <c>DateOfBirth</c> for a date input, or the name of
        /// any of its inputs. The error message is compared without its visually hidden <c>Error:</c> prefix.
        /// </remarks>
        /// <param name="node">The document, or an element to search within.</param>
        /// <param name="name">The field's name, e.g. <c>Email</c>.</param>
        /// <param name="expectedError">The error message, e.g. <c>Enter an email address</c>.</param>
        /// <returns>The field.</returns>
        /// <exception cref="GovUkAssertException">
        /// There's no such field, there's more than one, or its error message isn't <paramref name="expectedError"/>.
        /// </exception>
        [StackTraceHidden]
        public static FormField FieldWithNameHasError(IParentNode node, string name, string expectedError)
        {
            ArgumentNullException.ThrowIfNull(node);
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(expectedError);

            var field = Failure.Lookup(nameof(FieldWithNameHasError), () => node.GetFieldByName(name));

            return CheckFieldHasError(nameof(FieldWithNameHasError), field, expectedError);
        }

        /// <summary>
        /// Verifies that a field doesn't show an error message.
        /// </summary>
        /// <remarks>
        /// The field can be of any kind. It's found as <see cref="ComponentQueryExtensions.GetFieldByLabelOrLegend"/>
        /// finds it.
        /// </remarks>
        /// <param name="node">The document, or an element to search within.</param>
        /// <param name="labelOrLegend">
        /// The field's label or, for date inputs, radios and checkboxes, its fieldset's legend.
        /// </param>
        /// <returns>The field.</returns>
        /// <exception cref="GovUkAssertException">
        /// There's no such field, there's more than one, or it has an error message.
        /// </exception>
        [StackTraceHidden]
        public static FormField FieldHasNoError(IParentNode node, string labelOrLegend)
        {
            ArgumentNullException.ThrowIfNull(node);
            ArgumentNullException.ThrowIfNull(labelOrLegend);

            var field = Failure.Lookup(nameof(FieldHasNoError), () => node.GetFieldByLabelOrLegend(labelOrLegend));

            return CheckFieldHasNoError(nameof(FieldHasNoError), field);
        }

        /// <summary>
        /// Verifies that a field doesn't show an error message.
        /// </summary>
        /// <remarks>
        /// Use this with a field you've already found, e.g. one of a document's <c>Fields</c>.
        /// To find the field by its label or legend, or by its name, use the other overload or
        /// <see cref="FieldWithNameHasNoError"/>.
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

        /// <summary>
        /// Verifies that the field with a name doesn't show an error message.
        /// </summary>
        /// <remarks>
        /// The field can be of any kind. It's found as <see cref="ComponentQueryExtensions.GetFieldByName"/> finds it,
        /// so it's the name model state errors are keyed by, e.g. <c>DateOfBirth</c> for a date input, or the name of
        /// any of its inputs.
        /// </remarks>
        /// <param name="node">The document, or an element to search within.</param>
        /// <param name="name">The field's name, e.g. <c>Email</c>.</param>
        /// <returns>The field.</returns>
        /// <exception cref="GovUkAssertException">
        /// There's no such field, there's more than one, or it has an error message.
        /// </exception>
        [StackTraceHidden]
        public static FormField FieldWithNameHasNoError(IParentNode node, string name)
        {
            ArgumentNullException.ThrowIfNull(node);
            ArgumentNullException.ThrowIfNull(name);

            var field = Failure.Lookup(nameof(FieldWithNameHasNoError), () => node.GetFieldByName(name));

            return CheckFieldHasNoError(nameof(FieldWithNameHasNoError), field);
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
