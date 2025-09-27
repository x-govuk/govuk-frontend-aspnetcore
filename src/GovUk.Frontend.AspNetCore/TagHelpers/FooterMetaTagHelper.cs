using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the meta section of a GDS footer component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = FooterTagHelper.TagName)]
[RestrictChildren(FooterMetaItemsTagHelper.TagName, FooterMetaContentTagHelper.TagName)]
public class FooterMetaTagHelper : TagHelper
{
    internal const string TagName = "govuk-footer-meta";

    private const string VisuallyHiddenTitleAttributeName = "visually-hidden-title";

    /// <summary>
    /// The title of the meta item section.
    /// </summary>
    /// <remarks>
    /// If not specified, <c>&quot;Support links&quot;</c> will be used.
    /// </remarks>
    [HtmlAttributeName(VisuallyHiddenTitleAttributeName)]
    public string? VisuallyHiddenTitle { get; set; }

    /// <inheritdoc />
    public override void Init(TagHelperContext context)
    {
        context.SetContextItem(new FooterMetaContext());
    }

    /// <inheritdoc />
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var footerContext = context.GetContextItem<FooterContext>();
        var metaContext = context.GetContextItem<FooterMetaContext>();

        if (footerContext.Meta is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(output.TagName, FooterTagHelper.TagName);
        }

        if (footerContext.ContentLicence?.TagName is string contentLicenceTagName)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(output.TagName, contentLicenceTagName);
        }

        if (footerContext.Copyright?.TagName is string copyrightTagName)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(output.TagName, copyrightTagName);
        }

        await output.GetChildContentAsync();

        var attributes = new AttributeCollection(output.Attributes);

        footerContext.Meta = (
            new FooterOptionsMeta()
            {
                VisuallyHiddenTitle = VisuallyHiddenTitle,
                Html = metaContext.Content?.Html,
                Text = null,
                Items = metaContext.Items?.Items,
                ItemsAttributes = metaContext.Items?.Attributes,
                Attributes = attributes,
                ContentAttributes = metaContext.Content?.Attributes
            },
            output.TagName);

        output.SuppressOutput();
    }
}
