using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the items with the meta section of a GDS footer component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = FooterMetaTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = FooterMetaTagHelper.ShortTagName)]
[RestrictChildren(FooterMetaItemTagHelper.TagName, FooterMetaItemTagHelper.ShortTagName)]
public class FooterMetaItemsTagHelper : TagHelper
{
    internal const string TagName = "govuk-footer-meta-items";
    internal const string ShortTagName = ShortTagNames.MetaItems;

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];

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
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(AllTagNames, metaContext.TagName!);
        }

        if (metaContext.Content?.TagName is string contentTagName)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(context.TagName, contentTagName);
        }

        _ = await output.GetChildContentAsync();

        var attributes = new AttributeCollection(output.Attributes);

        metaContext.Items = (itemsContext.Items, attributes, context.TagName);

        output.SuppressOutput();
    }
}
