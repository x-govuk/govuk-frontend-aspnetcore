using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the items with the meta section of a GDS footer component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = FooterMetaTagHelper.TagName)]
[RestrictChildren(FooterMetaItemTagHelper.TagName)]
public class FooterMetaItemsTagHelper : TagHelper
{
    internal const string TagName = "govuk-footer-meta-items";

    /// <inheritdoc />
    public override void Init(TagHelperContext context)
    {
        context.SetContextItem(new FooterMetaItemsContext());
    }

    /// <inheritdoc />
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var metaContext = context.GetContextItem<FooterMetaContext>();
        var itemsContext = context.GetContextItem<FooterMetaItemsContext>();

        if (metaContext.Items is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(output.TagName, FooterMetaTagHelper.TagName);
        }

        if (metaContext.Content?.TagName is string contentTagName)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(output.TagName, contentTagName);
        }

        await output.GetChildContentAsync();

        var attributes = new AttributeCollection(output.Attributes);

        metaContext.Items = (itemsContext.Items, attributes, output.TagName);

        output.SuppressOutput();
    }
}
