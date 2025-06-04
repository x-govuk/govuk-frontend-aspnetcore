using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the hint in a GDS form group component.
/// </summary>
[HtmlTargetElement(CheckboxesTagHelper.HintTagName, ParentTag = CheckboxesTagHelper.TagName)]
[HtmlTargetElement(CheckboxesTagHelper.HintTagName, ParentTag = CheckboxesFieldsetTagHelper.TagName)]
[HtmlTargetElement(RadiosTagHelper.HintTagName, ParentTag = RadiosTagHelper.TagName)]
[HtmlTargetElement(RadiosTagHelper.HintTagName, ParentTag = RadiosFieldsetTagHelper.TagName)]
[HtmlTargetElement(SelectTagHelper.HintTagName, ParentTag = SelectTagHelper.TagName)]
[HtmlTargetElement(TextAreaTagHelper.HintTagName, ParentTag = TextAreaTagHelper.TagName)]
public class FormGroupHintTagHelper : TagHelper
{
    /// <summary>
    /// Creates a <see cref="FormGroupHintTagHelper"/>.
    /// </summary>
    public FormGroupHintTagHelper()
    {
    }

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

        formGroupContext.SetHint(output.Attributes.ToAttributeDictionary(), content?.Snapshot());

        output.SuppressOutput();
    }
}

/// <summary>
/// Represents the hint in a GDS form group component.
/// </summary>
[HtmlTargetElement(CharacterCountTagHelper.HintTagName, ParentTag = CharacterCountTagHelper.TagName)]
[HtmlTargetElement(DateInputTagHelper.HintTagName, ParentTag = DateInputTagHelper.TagName)]
[HtmlTargetElement(DateInputTagHelper.HintTagName, ParentTag = DateInputFieldsetTagHelper.TagName)]
[HtmlTargetElement(FileUploadTagHelper.HintTagName, ParentTag = FileUploadTagHelper.TagName)]
[HtmlTargetElement(TextInputTagHelper.HintTagName, ParentTag = TextInputTagHelper.TagName)]
public class FormGroupHintTagHelper3 : TagHelper
{
    /// <summary>
    /// Creates a <see cref="FormGroupHintTagHelper3"/>.
    /// </summary>
    public FormGroupHintTagHelper3()
    {
    }

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

        formGroupContext.SetHint(
            new AttributeCollection(output.Attributes),
            content?.ToTemplateString(),
            output.TagName);

        output.SuppressOutput();
    }
}
