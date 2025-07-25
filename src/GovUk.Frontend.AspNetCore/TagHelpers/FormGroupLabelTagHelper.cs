using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.HtmlGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the label in a GDS form group component.
/// </summary>
[HtmlTargetElement(SelectTagHelper.LabelTagName, ParentTag = SelectTagHelper.TagName)]
[HtmlTargetElement(TextAreaTagHelper.LabelTagName, ParentTag = TextAreaTagHelper.TagName)]
public class FormGroupLabelTagHelper : TagHelper
{
    private const string IsPageHeadingAttributeName = "is-page-heading";

    /// <summary>
    /// Creates a <see cref="FormGroupLabelTagHelper"/>.
    /// </summary>
    public FormGroupLabelTagHelper()
    {
    }

    /// <summary>
    /// Whether the label also acts as the heading for the page.
    /// </summary>
    /// <remarks>
    /// The default is <c>false</c>.
    /// </remarks>
    [HtmlAttributeName(IsPageHeadingAttributeName)]
    public bool IsPageHeading { get; set; } = ComponentGenerator.LabelDefaultIsPageHeading;

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var content = output.TagMode == TagMode.StartTagAndEndTag ?
            await output.GetChildContentAsync() :
            null;

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var formGroupContext = context.GetContextItem<FormGroupContext>();

        formGroupContext.SetLabel(
            IsPageHeading,
            output.Attributes.ToAttributeDictionary(),
            content?.Snapshot());

        output.SuppressOutput();
    }
}

/// <summary>
/// Represents the label in a GDS form group component.
/// </summary>
[HtmlTargetElement(CharacterCountTagHelper.LabelTagName, ParentTag = CharacterCountTagHelper.TagName)]
[HtmlTargetElement(FileUploadTagHelper.LabelTagName, ParentTag = FileUploadTagHelper.TagName)]
[HtmlTargetElement(TextInputTagHelper.LabelTagName, ParentTag = TextInputTagHelper.TagName)]
public class FormGroupLabelTagHelper3 : TagHelper
{
    private const string IsPageHeadingAttributeName = "is-page-heading";

    /// <summary>
    /// Creates a <see cref="FormGroupLabelTagHelper3"/>.
    /// </summary>
    public FormGroupLabelTagHelper3()
    {
    }

    /// <summary>
    /// Whether the label also acts as the heading for the page.
    /// </summary>
    /// <remarks>
    /// The default is <c>false</c>.
    /// </remarks>
    [HtmlAttributeName(IsPageHeadingAttributeName)]
    public bool IsPageHeading { get; set; } = ComponentGenerator.LabelDefaultIsPageHeading;

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var content = output.TagMode == TagMode.StartTagAndEndTag ?
            await output.GetChildContentAsync() :
            null;

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var formGroupContext = context.GetContextItem<FormGroupContext3>();

        formGroupContext.SetLabel(
            IsPageHeading,
            new AttributeCollection(output.Attributes),
            content?.ToTemplateString(),
            output.TagName);

        output.SuppressOutput();
    }
}

