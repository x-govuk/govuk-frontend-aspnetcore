using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the fieldset in a GDS form component.
/// </summary>
public abstract class FormGroupFieldsetTagHelperBase : TagHelper
{
    private const string DescribedByAttributeName = "described-by";

    internal const string ShortTagName = ShortTagNames.Fieldset;

    private protected FormGroupFieldsetTagHelperBase()
    {
    }

    /// <summary>
    /// One or more element IDs to add to the <c>aria-describedby</c> attribute.
    /// </summary>
    [HtmlAttributeName(DescribedByAttributeName)]
    public string? DescribedBy { get; set; }

    private protected abstract string LegendTagName { get; }

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.SetContextItem(new FormGroupFieldsetContext2(context.TagName));
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var formGroupContext = context.GetContextItem<FormGroupContext3>() as IFormGroupWithFieldset ??
            throw new InvalidOperationException($"{nameof(FormGroupContext3)} does not implement {nameof(IFormGroupWithFieldset)}");
        var fieldsetContext = context.GetContextItem<FormGroupFieldsetContext2>();

        fieldsetContext.DescribedBy = DescribedBy;

        formGroupContext.OpenFieldset(fieldsetContext, new AttributeCollection(output.Attributes));

        _ = await output.GetChildContentAsync();

        fieldsetContext.ThrowIfNotComplete(formGroupContext.For, [ShortTagNames.Legend, LegendTagName]);
        formGroupContext.CloseFieldset();

        output.SuppressOutput();
    }
}
