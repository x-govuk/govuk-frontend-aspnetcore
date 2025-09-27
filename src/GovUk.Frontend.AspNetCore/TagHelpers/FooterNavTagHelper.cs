using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the navigation section of a GDS footer component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = FooterTagHelper.TagName)]
[RestrictChildren(FooterNavTitleTagHelper.TagName, FooterNavItemsTagHelper.TagName)]
public class FooterNavTagHelper : TagHelper
{
    internal const string TagName = "govuk-footer-nav";

    private const string ColumnsAttributeName = "columns";
    private const string WidthAttributeName = "width";

    /// <summary>
    /// The number of columns to display items in.
    /// </summary>
    [HtmlAttributeName(ColumnsAttributeName)]
    public int? Columns { get; set; }

    /// <summary>
    /// The width of this navigation section.
    /// </summary>
    /// <remarks>
    /// <para>For example, <c>one-third</c>, <c>two-thirds</c> or <c>one-half</c>.</para>
    /// <para>If not specified, <c>full</c> will be used.</para>
    /// </remarks>
    [HtmlAttributeName(WidthAttributeName)]
    public string? Width { get; set; }

    /// <inheritdoc />
    public override void Init(TagHelperContext context)
    {
        context.SetContextItem(new FooterNavContext());
    }

    /// <inheritdoc />
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var footerContext = context.GetContextItem<FooterContext>();
        var navContext = context.GetContextItem<FooterNavContext>();

        if (footerContext.Meta?.TagName is string metaTagName)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(output.TagName, metaTagName);
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

        footerContext.Navigation.Add(new FooterOptionsNavigation()
        {
            Title = navContext.Title?.Html,
            TitleAttributes = navContext.Title?.Attributes,
            Columns = Columns,
            Width = Width,
            Items = navContext.Items?.Items,
            Attributes = attributes,
            ItemsAttributes = navContext.Items?.Attributes
        });

        output.SuppressOutput();
    }
}
