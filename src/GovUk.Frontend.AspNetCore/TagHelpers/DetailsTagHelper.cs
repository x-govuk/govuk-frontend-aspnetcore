using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS details component.
/// </summary>
[HtmlTargetElement(TagName)]
[RestrictChildren(DetailsSummaryTagHelper.TagName, DetailsTextTagHelper.TagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.Details)]
public class DetailsTagHelper : TagHelper
{
    internal const string TagName = "govuk-details";

    private const string OpenAttributeName = "open";

    private readonly IComponentGenerator _componentGenerator;

    /// <summary>
    /// Creates a new <see cref="DetailsTagHelper"/>.
    /// </summary>
    public DetailsTagHelper(IComponentGenerator componentGenerator)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);

        _componentGenerator = componentGenerator;
    }

    /// <summary>
    /// Whether the details element should be expanded.
    /// </summary>
    /// <remarks>
    /// The default is <c>false</c>.
    /// </remarks>
    [HtmlAttributeName(OpenAttributeName)]
    public bool? Open { get; set; }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var detailsContext = new DetailsContext();

        using (context.SetScopedContextItem(detailsContext))
        {
            _ = await output.GetChildContentAsync();
        }

        detailsContext.ThrowIfNotComplete();

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        var component = await _componentGenerator.GenerateDetailsAsync(new DetailsOptions()
        {
            Open = Open,
            SummaryHtml = detailsContext.Summary?.Content.ToTemplateString(),
            SummaryAttributes = detailsContext.Summary?.Attributes,
            Html = detailsContext.Text?.Content.ToTemplateString(),
            TextAttributes = detailsContext.Text?.Attributes,
            Classes = classes,
            Attributes = attributes
        });

        component.ApplyToTagHelper(output);
    }
}
