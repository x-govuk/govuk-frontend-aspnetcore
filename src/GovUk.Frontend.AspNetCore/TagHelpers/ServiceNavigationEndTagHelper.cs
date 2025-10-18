using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the content at the end of the service header container in a GDS service navigation component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = ServiceNavigationTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(ShortTagName, ParentTag = ServiceNavigationTagHelper.TagName)]
#endif
[TagHelperDocumentation(ContentDescription = "The content is the HTML at the end of the service header container.")]
public class ServiceNavigationEndTagHelper : TagHelper
{
    internal const string TagName = "govuk-service-navigation-end";
#if SHORT_TAG_NAMES
    internal const string ShortTagName = ShortTagNames.End;
#endif

    private static IReadOnlyCollection<string> AllTagNames => [
        TagName
#if SHORT_TAG_NAMES
        ,
        ShortTagName
#endif
    ];

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var serviceNavigationContext = context.GetContextItem<ServiceNavigationContext>();

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

        serviceNavigationContext.EndSlot = (content.ToTemplateString(), output.TagName);

        output.SuppressOutput();
    }
}
