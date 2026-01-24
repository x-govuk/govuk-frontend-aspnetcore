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
[HtmlTargetElement(TextAreaHintTagHelper.TagName, ParentTag = TextAreaTagHelper.TagName)]
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
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

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
