using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the fieldset in a GDS checkboxes component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CheckboxesTagHelper.TagName)]
[RestrictChildren(CheckboxesFieldsetLegendTagHelper.TagName, CheckboxesItemTagHelper.TagName, CheckboxesItemDividerTagHelper.TagName, CheckboxesTagHelper.HintTagName, CheckboxesTagHelper.ErrorMessageTagName)]
[OutputElementHint("fieldset")]
public class CheckboxesFieldsetTagHelper : TagHelper
{
    internal const string TagName = "govuk-checkboxes-fieldset";

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var checkboxesContext = context.GetContextItem<CheckboxesContext>();
        checkboxesContext.OpenFieldset();

        var attributes = new AttributeCollection(output.Attributes);
        var fieldsetContext = new CheckboxesFieldsetContext(
            describedBy: null,
            legendClass: null,
            attributes: attributes,
            @for: checkboxesContext.AspFor);

        using (context.SetScopedContextItem(fieldsetContext))
        {
            _ = await output.GetChildContentAsync();
        }

        fieldsetContext.ThrowIfNotComplete();
        checkboxesContext.CloseFieldset(fieldsetContext);

        output.SuppressOutput();
    }
}
