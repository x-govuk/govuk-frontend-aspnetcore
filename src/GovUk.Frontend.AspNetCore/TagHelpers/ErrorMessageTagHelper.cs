using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using AttributeCollection = GovUk.Frontend.AspNetCore.ComponentGeneration.AttributeCollection;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS error message component.
/// </summary>
[HtmlTargetElement(TagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.ErrorMessage)]
public class ErrorMessageTagHelper : TagHelper
{
    internal const string TagName = "govuk-error-message";

    private const string AspForAttributeName = "asp-for";
    private const string ForAttributeName = "for";
    private const string VisuallyHiddenTextAttributeName = "visually-hidden-text";
    private const string DefaultVisuallyHiddenText = "Error";

    private readonly IComponentGenerator _componentGenerator;
    private readonly IModelHelper _modelHelper;

    /// <summary>
    /// Creates a <see cref="ErrorMessageTagHelper"/>.
    /// </summary>
    public ErrorMessageTagHelper(IComponentGenerator componentGenerator)
        : this(componentGenerator, modelHelper: new DefaultModelHelper())
    {
    }

    internal ErrorMessageTagHelper(
        IComponentGenerator componentGenerator,
        IModelHelper modelHelper)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);
        ArgumentNullException.ThrowIfNull(modelHelper);
        
        _componentGenerator = componentGenerator;
        _modelHelper = modelHelper;
    }

    /// <summary>
    /// An expression to be evaluated against the current model.
    /// </summary>
    [HtmlAttributeName(AspForAttributeName)]
    [Obsolete("Use the 'for' attribute instead.", DiagnosticId = DiagnosticIds.UseForAttributeInstead)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ModelExpression? AspFor
    {
        get => For;
        set => For = value;
    }

    /// <summary>
    /// An expression to be evaluated against the current model.
    /// </summary>
    [HtmlAttributeName(ForAttributeName)]
    public ModelExpression? For { get; set; }

    /// <summary>
    /// The visually hidden prefix used before the error message.
    /// </summary>
    /// <remarks>
    /// The default is <c>&quot;Error&quot;</c>.
    /// </remarks>
    [HtmlAttributeName(VisuallyHiddenTextAttributeName)]
    public string? VisuallyHiddenText { get; set; } = DefaultVisuallyHiddenText;

    /// <summary>
    /// Gets the <see cref="ViewContext"/> of the executing view.
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    [DisallowNull]
    public ViewContext? ViewContext { get; set; }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var content = output.TagMode == TagMode.StartTagAndEndTag ?
            await output.GetChildContentAsync() :
            null;

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        if (content is null && For is null)
        {
            throw new InvalidOperationException(
                $"Cannot determine content. Element must contain content if the '{AspForAttributeName}' attribute is not specified.");
        }

        string? resolvedContent = null;
        if (content is not null)
        {
            resolvedContent = content.GetContent();
        }
        else if (For is not null)
        {
            var validationMessage = _modelHelper.GetValidationMessage(
                ViewContext!,
                For.ModelExplorer,
                For.Name);

            if (validationMessage is not null)
            {
                resolvedContent = validationMessage;
            }
        }

        if (resolvedContent is not null)
        {
            var attributes = new AttributeCollection(output.Attributes);
            attributes.Remove("class", out var classes);

            var component = await _componentGenerator.GenerateErrorMessageAsync(new ErrorMessageOptions()
            {
                Text = resolvedContent,
                VisuallyHiddenText = VisuallyHiddenText,
                Classes = classes,
                Attributes = attributes
            });

            component.ApplyToTagHelper(output);
        }
        else
        {
            output.SuppressOutput();
        }
    }
}
