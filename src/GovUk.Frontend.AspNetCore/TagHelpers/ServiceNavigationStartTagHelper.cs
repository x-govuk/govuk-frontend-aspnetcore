using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the content at the start of the service header container in a GDS service navigation component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = ServiceNavigationTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(ShortTagName, ParentTag = ServiceNavigationTagHelper.TagName)]
#endif
[TagHelperDocumentation(ContentDescription = "The content is the HTML at the start of the service header container.")]
public class ServiceNavigationStartTagHelper : TagHelper
{
    internal const string TagName = "govuk-service-navigation-start";
#if SHORT_TAG_NAMES
    internal const string ShortTagName = ShortTagNames.Start;
#endif

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [
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

        if (serviceNavigationContext.StartSlot is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(AllTagNames, [ServiceNavigationTagHelper.TagName]);
        }

        if (serviceNavigationContext.Nav is not null)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(output.TagName, serviceNavigationContext.Nav.TagName!);
        }

        if (serviceNavigationContext.EndSlot is var (_, endTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(output.TagName, endTagName);
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

        serviceNavigationContext.StartSlot = (content.ToTemplateString(), output.TagName);

        output.SuppressOutput();
    }
}
