using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the content at the end of the service header container in a GDS service navigation component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = ServiceNavigationTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = ServiceNavigationTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML at the end of the service header container.")]
public class ServiceNavigationEndTagHelper : TagHelper
{
    internal const string TagName = "govuk-service-navigation-end";
    internal const string ShortTagName = ShortTagNames.End;

    private const string AlignAttributeName = "align";

    private static IReadOnlyCollection<string> AllTagNames => [TagName, ShortTagName];

    /// <summary>
    /// How the content is aligned within the service header container.
    /// </summary>
    /// <remarks>
    /// By default the content is displayed underneath the navigation items.
    /// </remarks>
    [HtmlAttributeName(AlignAttributeName)]
    public ServiceNavigationEndSlotAlign? Align { get; set; }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var serviceNavigationContext = context.GetContextItem<ServiceNavigationContext>();

        serviceNavigationContext.CheckChildTagNameSpelling(context.TagName);

        if (serviceNavigationContext.EndSlot is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(AllTagNames, [ServiceNavigationTagHelper.TagName]);
        }

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        if (output.Attributes.Any())
        {
            throw ExceptionHelper.AttributesNotSupported();
        }

        serviceNavigationContext.EndSlot = (content.Snapshot(), Align, context.TagName);

        output.SuppressOutput();
    }
}
