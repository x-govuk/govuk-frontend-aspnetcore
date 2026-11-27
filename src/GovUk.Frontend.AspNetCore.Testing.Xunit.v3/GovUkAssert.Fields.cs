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

            return FieldHasError(field, expectedError);
        }

        /// <summary>
        /// Verifies that a field shows an error message.
        /// </summary>
        /// <remarks>
        /// Use this with a field found some other way than by its label or legend, e.g. by its name with
        /// <see cref="ComponentQueryExtensions.GetFieldByName"/>:
        /// <c>Assert.FieldHasError(doc.GetFieldByName("Email"), "Enter an email address")</c>.
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

            if (field.ErrorMessage != expectedError)
            {
                throw Failure.Create(
                    nameof(FieldHasError),
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

            return FieldHasNoError(field);
        }

        /// <summary>
        /// Verifies that a field doesn't show an error message.
        /// </summary>
        /// <remarks>
        /// Use this with a field found some other way than by its label or legend, e.g. by its name with
        /// <see cref="ComponentQueryExtensions.GetFieldByName"/>: <c>Assert.FieldHasNoError(doc.GetFieldByName("Email"))</c>.
        /// </remarks>
        /// <param name="field">The field.</param>
        /// <returns>The field.</returns>
        /// <exception cref="GovUkAssertException">The field has an error message.</exception>
        [StackTraceHidden]
        public static FormField FieldHasNoError(FormField field)
        {
            ArgumentNullException.ThrowIfNull(field);

            if (field.ErrorMessage is not null)
            {
                throw Failure.Create(
                    nameof(FieldHasNoError),
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
}
