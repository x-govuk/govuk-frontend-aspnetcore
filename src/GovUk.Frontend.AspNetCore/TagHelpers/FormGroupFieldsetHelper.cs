using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal static class FormGroupFieldsetHelper
{
    public const string FieldsetAttributeName = "fieldset";
    public const string FieldsetAttributesPrefix = "fieldset-";
    public const string LegendAttributesPrefix = "legend-";
    public const string LegendIsPageHeadingAttributeName = LegendAttributesPrefix + IsPageHeadingAttributeName;

    private const string IsPageHeadingAttributeName = "is-page-heading";

    /// <summary>
    /// Resolves the options for the component's fieldset, or <see langword="null"/> if no fieldset should be generated.
    /// </summary>
    /// <remarks>
    /// A fieldset is generated when an explicit fieldset element is used, when a legend element is specified directly
    /// inside the root element or when any of the <c>fieldset</c>, <c>fieldset-*</c>, <c>legend-*</c> or
    /// <c>legend-is-page-heading</c> attributes are specified on the root element.
    /// </remarks>
    public static FieldsetOptions? GetFieldsetOptions(
        IFormGroupWithFieldset context,
        IModelHelper modelHelper,
        bool generateFieldset,
        AttributeCollection fieldsetAttributes,
        AttributeCollection legendAttributes,
        bool? legendIsPageHeading)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(modelHelper);
        ArgumentNullException.ThrowIfNull(fieldsetAttributes);
        ArgumentNullException.ThrowIfNull(legendAttributes);

        // 'legend-is-page-heading' matches both the LegendIsPageHeading property and the 'legend-*' prefix; Razor
        // binds it to whichever the tag helper declares first, so take the value from the attributes when it
        // ended up there rather than emitting it onto the generated legend.
        var resolvedLegendAttributes = legendAttributes.Clone();
        var resolvedLegendIsPageHeading = legendIsPageHeading;

        if (resolvedLegendAttributes.Remove(IsPageHeadingAttributeName, out var isPageHeadingValue))
        {
            resolvedLegendIsPageHeading = bool.TryParse(isPageHeadingValue?.ToHtmlString(), out var parsed)
                ? parsed
                : throw new InvalidOperationException(
                    $"The '{LegendIsPageHeadingAttributeName}' attribute must be 'true' or 'false'.");
        }

        var haveFieldsetAttributes = generateFieldset || fieldsetAttributes.Any();
        var haveLegendAttributes = resolvedLegendAttributes.Any() || resolvedLegendIsPageHeading is not null;
        var haveLegendElement = context.ImplicitFieldset.Legend is not null;

        if (context.Fieldset is FormGroupFieldsetContext2 explicitFieldset)
        {
            if (haveLegendElement)
            {
                throw new InvalidOperationException(
                    $"<{context.LegendTagName}> must be inside <{context.FieldsetTagName}>.");
            }

            if (haveFieldsetAttributes || haveLegendAttributes)
            {
                throw new InvalidOperationException(
                    $"'{FieldsetAttributeName}' and '{LegendAttributesPrefix}*' attributes cannot be specified on " +
                    $"<{context.RootTagName}> when a <{context.FieldsetTagName}> element is used.");
            }

            return explicitFieldset.GetFieldsetOptions(context.For, modelHelper, context.Attributes!);
        }

        if (!haveFieldsetAttributes && !haveLegendAttributes && !haveLegendElement)
        {
            return null;
        }

        context.ImplicitFieldset.ThrowIfNotComplete(context.For, context.LegendTagName);

        return context.ImplicitFieldset.GetFieldsetOptions(
            context.For,
            modelHelper,
            fieldsetAttributes,
            resolvedLegendAttributes,
            resolvedLegendIsPageHeading);
    }
}
