using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS fieldset component.
/// </summary>
[HtmlTargetElement(TagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.Fieldset)]
public class FieldsetTagHelper : TagHelper
{
    internal const string TagName = "govuk-fieldset";

    private const string DescribedByAttributeName = "described-by";
    private const string RoleAttributeName = "role";

    private readonly IComponentGenerator _componentGenerator;

    /// <summary>
    /// Creates a new <see cref="FieldsetTagHelper"/>.
    /// </summary>
    public FieldsetTagHelper(IComponentGenerator componentGenerator)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);

        _componentGenerator = componentGenerator;
    }

    /// <summary>
    /// One or more element IDs to add to the <c>aria-describedby</c> attribute.
    /// </summary>
    [HtmlAttributeName(DescribedByAttributeName)]
    public string? DescribedBy { get; set; }

    /// <summary>
    /// The <c>role</c> attribute.
    /// </summary>
    [HtmlAttributeName(RoleAttributeName)]
    public string? Role { get; set; }

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        context.SetContextItem(new FieldsetContext());
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var fieldsetContext = context.GetContextItem<FieldsetContext>();

        IHtmlContent content;

        content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        fieldsetContext.ThrowIfNotComplete();

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        FieldsetOptionsLegend? legend = null;
        if (fieldsetContext.Legend != null)
        {
            var legendValue = fieldsetContext.Legend.Value;
            var legendAttributes = legendValue.Attributes;

            legend = new FieldsetOptionsLegend
            {
                IsPageHeading = legendValue.IsPageHeading,
                Html = legendValue.Content,
                Attributes = legendAttributes
            };
        }

        var component = await _componentGenerator.GenerateFieldsetAsync(new FieldsetOptions
        {
            DescribedBy = DescribedBy,
            Role = Role,
            Legend = legend,
            Html = content.ToTemplateString(),
            Classes = classes,
            Attributes = attributes
        });

        component.ApplyToTagHelper(output);
    }
}
