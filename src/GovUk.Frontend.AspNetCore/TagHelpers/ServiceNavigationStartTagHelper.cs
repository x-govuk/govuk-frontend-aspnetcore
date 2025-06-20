using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the content at the start of the service header container in a GDS service navigation component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = ServiceNavigationTagHelper.TagName)]
//[HtmlTargetElement(ShortTagName, ParentTag = ServiceNavigationTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML at the start of the service header container.")]
public class ServiceNavigationStartTagHelper : TagHelper
{
    internal const string TagName = "govuk-service-navigation-start";
    //internal const string ShortTagName = ShortTagNames.Start;

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var serviceNavigationContext = context.GetContextItem<ServiceNavigationContext>();

        if (serviceNavigationContext.StartSlot is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn([TagName/*, ShortTagName*/], [ServiceNavigationTagHelper.TagName]);
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
