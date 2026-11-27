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
        /// To find the field by its label or legend, use <see cref="FieldByLabelHasError"/>.
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

            var field = GetFieldByName(nameof(FieldHasError), node, name, nameof(FieldByLabelHasError));

            return CheckFieldHasError(nameof(FieldHasError), field, expectedError);
        }

        /// <summary>
        /// Verifies that a field shows an error message.
        /// </summary>
        /// <remarks>
        /// Use this with a field you've already found, e.g. one of a document's <c>Fields</c>.
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
        /// Verifies that the field with a label or legend shows an error message.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The field can be of any kind. It's found as <see cref="ComponentQueryExtensions.GetFieldByLabelOrLegend"/>
        /// finds it.
        /// </para>
        /// <para>
        /// To find the field by its name, use <see cref="FieldHasError(IParentNode, string, string)"/>.
        /// The error message is compared without its visually hidden <c>Error:</c> prefix.
        /// </para>
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
        public static FormField FieldByLabelHasError(IParentNode node, string labelOrLegend, string expectedError)
        {
            ArgumentNullException.ThrowIfNull(node);
            ArgumentNullException.ThrowIfNull(labelOrLegend);
            ArgumentNullException.ThrowIfNull(expectedError);

            var field = GetFieldByLabelOrLegend(nameof(FieldByLabelHasError), node, labelOrLegend, nameof(FieldHasError));

            return CheckFieldHasError(nameof(FieldByLabelHasError), field, expectedError);
        }

        /// <summary>
        /// Verifies that the field with a name doesn't show an error message.
        /// </summary>
        /// <remarks>
        /// The field can be of any kind. It's found as <see cref="ComponentQueryExtensions.GetFieldByName"/> finds it,
        /// so <paramref name="name"/> is the name the field is submitted with and its model state error is keyed by,
        /// e.g. <c>DateOfBirth</c> for a date input, or the name of any of its inputs.
        /// To find the field by its label or legend, use <see cref="FieldByLabelHasNoError"/>.
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

            var field = GetFieldByName(nameof(FieldHasNoError), node, name, nameof(FieldByLabelHasNoError));

            return CheckFieldHasNoError(nameof(FieldHasNoError), field);
        }

        /// <summary>
        /// Verifies that a field doesn't show an error message.
        /// </summary>
        /// <remarks>
        /// Use this with a field you've already found, e.g. one of a document's <c>Fields</c>.
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
        /// Verifies that the field with a label or legend doesn't show an error message.
        /// </summary>
        /// <remarks>
        /// The field can be of any kind. It's found as <see cref="ComponentQueryExtensions.GetFieldByLabelOrLegend"/>
        /// finds it. To find the field by its name, use <see cref="FieldHasNoError(IParentNode, string)"/>.
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
        public static FormField FieldByLabelHasNoError(IParentNode node, string labelOrLegend)
        {
            ArgumentNullException.ThrowIfNull(node);
            ArgumentNullException.ThrowIfNull(labelOrLegend);

            var field = GetFieldByLabelOrLegend(nameof(FieldByLabelHasNoError), node, labelOrLegend, nameof(FieldHasNoError));

            return CheckFieldHasNoError(nameof(FieldByLabelHasNoError), field);
        }
    }

    /// <summary>
    /// Gets the field with the name <paramref name="name"/>. If there isn't one but there's a field labelled
    /// <paramref name="name"/>, the failure suggests <paramref name="byLabelAssertion"/>.
    /// </summary>
    [StackTraceHidden]
    private static FormField GetFieldByName(string assertion, IParentNode node, string name, string byLabelAssertion)
    {
        try
        {
            return node.GetFieldByName(name);
        }
        catch (ComponentLookupException ex)
        {
            var hint = !node.Fields.Any(f => f.Names.Contains(name)) && node.Fields.Any(f => f.Label == name)
                ? $"there's a field labelled {Failure.Quote(name)}; use {byLabelAssertion} to find it by its label or legend"
                : null;

            throw Failure.FromLookup(assertion, ex.Failure, hint);
        }
    }

    /// <summary>
    /// Gets the field labelled <paramref name="labelOrLegend"/>. If there isn't one but there's a field named
    /// <paramref name="labelOrLegend"/>, the failure suggests <paramref name="byNameAssertion"/>.
    /// </summary>
    [StackTraceHidden]
    private static FormField GetFieldByLabelOrLegend(string assertion, IParentNode node, string labelOrLegend, string byNameAssertion)
    {
        try
        {
            return node.GetFieldByLabelOrLegend(labelOrLegend);
        }
        catch (ComponentLookupException ex)
        {
            var hint = !node.Fields.Any(f => f.Label == labelOrLegend) && node.Fields.Any(f => f.Names.Contains(labelOrLegend))
                ? $"there's a field named {Failure.Quote(labelOrLegend)}; use {byNameAssertion} to find it by its name"
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
