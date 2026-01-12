using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.HtmlGeneration;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS error message component.
/// </summary>
[HtmlTargetElement(TagName)]
[OutputElementHint(ComponentGenerator.ErrorMessageElement)]
public class ErrorMessageTagHelper : TagHelper
{
    internal const string TagName = "govuk-error-message";

    private const string AspForAttributeName = "asp-for";
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
                $"Cannot determine content. Element must contain content if the '{AspForAttributeName}' attribute is not specified.");
        }

        IHtmlContent? resolvedContent = content;
        if (resolvedContent is null && For is not null)
        {
            var validationMessage = _modelHelper.GetValidationMessage(
                ViewContext!,
                For.ModelExplorer,
                For.Name);

            if (validationMessage is not null)
            {
                resolvedContent = validationMessage.EncodeHtml();
            }
        }

        if (resolvedContent is not null)
        {
            var attributes = new ComponentGeneration.AttributeCollection(output.Attributes);
            attributes.Remove("class", out var classes);

            var component = await _componentGenerator.GenerateErrorMessageAsync(new ErrorMessageOptions()
            {
                Html = resolvedContent.ToTemplateString(),
                VisuallyHiddenText = VisuallyHiddenText != null ? new TemplateString(VisuallyHiddenText) : null,
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
