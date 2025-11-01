using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GOV.UK summary card component.
/// </summary>
[HtmlTargetElement(TagName)]
[RestrictChildren(SummaryCardTitleTagHelper.TagName, SummaryCardActionsTagHelper.TagName, SummaryListTagHelper.TagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.SummaryCard)]
public class SummaryCardTagHelper : TagHelper
{
    internal const string TagName = "govuk-summary-card";

    private readonly IComponentGenerator _componentGenerator;

    /// <summary>
    /// Creates a new <see cref="SummaryCardTagHelper"/>.
    /// </summary>
    public SummaryCardTagHelper(IComponentGenerator componentGenerator)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);

        _componentGenerator = componentGenerator;
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var cardContext = new SummaryCardContext();

        using (context.SetScopedContextItem(cardContext))
        {
            _ = await output.GetChildContentAsync();
        }

        cardContext.ThrowIfNotComplete();

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        var component = await _componentGenerator.GenerateSummaryListAsync(new SummaryListOptions()
        {
            Rows = cardContext.SummaryList?.Rows,
            Card = new SummaryListOptionsCard()
            {
                Title = cardContext.Title,
                Actions = cardContext.Actions,
                Classes = classes,
                Attributes = attributes
            },
            Classes = cardContext.SummaryList?.Classes,
            Attributes = cardContext.SummaryList?.Attributes
        });

        output.ApplyComponentHtml(component, HtmlEncoder.Default);
    }
}
