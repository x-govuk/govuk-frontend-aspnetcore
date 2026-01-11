using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;
using AttributeCollection = GovUk.Frontend.AspNetCore.ComponentGeneration.AttributeCollection;

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
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var fieldsetContext = new FieldsetContext();

        using (context.SetScopedContextItem(fieldsetContext))
        {
            var content = await output.GetChildContentAsync();
            
            if (output.Content.IsModified)
            {
                content = output.Content;
            }

            fieldsetContext.ThrowIfNotComplete();

            var attributes = new AttributeCollection(output.Attributes);
            attributes.Remove("class", out var classes);

            var legend = fieldsetContext.Legend!.Value;

            var component = await _componentGenerator.GenerateFieldsetAsync(new FieldsetOptions()
            {
                DescribedBy = DescribedBy,
                Role = Role,
                Legend = new FieldsetOptionsLegend()
                {
                    Html = legend.Content.ToTemplateString(),
                    IsPageHeading = legend.IsPageHeading,
                    Attributes = legend.Attributes is not null ? new AttributeCollection(legend.Attributes) : null
                },
                Html = content.ToTemplateString(),
                Classes = classes,
                Attributes = attributes
            });

            component.ApplyToTagHelper(output);
        }
    }
}
