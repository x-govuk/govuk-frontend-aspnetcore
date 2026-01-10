using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS summary list component.
/// </summary>
[HtmlTargetElement(TagName)]
[RestrictChildren(SummaryListRowTagHelper.TagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.SummaryList)]
public class SummaryListTagHelper : TagHelper
{
    internal const string TagName = "govuk-summary-list";

    private readonly IComponentGenerator _componentGenerator;

    /// <summary>
    /// Creates a new <see cref="SummaryListTagHelper"/>.
    /// </summary>
    public SummaryListTagHelper(IComponentGenerator componentGenerator)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);

        _componentGenerator = componentGenerator;
    }

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.SetContextItem(new SummaryListContext());
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var summaryListContext = context.GetContextItem<SummaryListContext>();

        _ = await output.GetChildContentAsync();

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        var componentOptions = new SummaryListOptions()
        {
            Rows = summaryListContext.Rows,
            Card = null,
            Classes = classes,
            Attributes = attributes
        };

        if (summaryListContext.HaveCard)
        {
            var cardContext = context.GetContextItem<SummaryCardContext>();
            cardContext.SetSummaryList(componentOptions);
            output.SuppressOutput();
            return;
        }

        var component = await _componentGenerator.GenerateSummaryListAsync(componentOptions);

        component.ApplyToTagHelper(output);
    }
}
