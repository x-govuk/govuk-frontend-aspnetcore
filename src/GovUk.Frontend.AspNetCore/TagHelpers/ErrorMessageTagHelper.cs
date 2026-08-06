using System.Diagnostics.CodeAnalysis;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS error message component.
/// </summary>
[HtmlTargetElement(TagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.ErrorMessage)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the error message. Content is required if the 'for' attribute is not specified. If 'for' is specified and there are no errors in the model state then no output is generated; if there are multiple errors only the first is used.")]
public class ErrorMessageTagHelper : TagHelper
{
    internal const string TagName = "govuk-error-message";

    private const string ForAttributeName = "for";
    private const string VisuallyHiddenTextAttributeName = "visually-hidden-text";

    private readonly IComponentGenerator _componentGenerator;
    private readonly IModelHelper _modelHelper;

    /// <summary>
    /// Creates a new <see cref="ErrorMessageTagHelper"/>.
    /// </summary>
    public ErrorMessageTagHelper(IComponentGenerator componentGenerator)
        : this(componentGenerator, modelHelper: null)
    {
    }

    internal ErrorMessageTagHelper(
        IComponentGenerator componentGenerator,
        IModelHelper? modelHelper)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);
        _componentGenerator = componentGenerator;
        _modelHelper = modelHelper ?? new DefaultModelHelper();
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
    public string? VisuallyHiddenText { get; set; }

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
                $"Cannot determine content. Element must contain content if the '{ForAttributeName}' attribute is not specified.");
        }

        // The content written in the view is markup; a validation message is a plain string. Keeping
        // them in separate slots is what stops a message containing markup being rendered as markup.
        var resolvedHtml = content;
        string? resolvedText = null;

        if (resolvedHtml.IsEmpty() && For is not null)
        {
            resolvedText = _modelHelper.GetValidationMessage(ViewContext!, For.ModelExplorer, For.Name);
        }

        if (!resolvedHtml.IsEmpty() || !resolvedText.IsEmpty())
        {
            var attributes = new ComponentGeneration.AttributeCollection(output.Attributes);
            attributes.Remove("class", out var classes);

            var component = await _componentGenerator.GenerateErrorMessageAsync(new ErrorMessageOptions
            {
                Html = resolvedHtml,
                Text = resolvedText,
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
