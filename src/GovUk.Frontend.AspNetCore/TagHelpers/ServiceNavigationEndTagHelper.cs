using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the content at the end of the service header container in a GDS service navigation component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = ServiceNavigationTagHelper.TagName)]
//[HtmlTargetElement(ShortTagName, ParentTag = ServiceNavigationTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML at the end of the service header container.")]
public class ServiceNavigationEndTagHelper : TagHelper
{
    internal const string TagName = "govuk-service-navigation-end";
    //internal const string ShortTagName = ShortTagNames.End;

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var serviceNavigationContext = context.GetContextItem<ServiceNavigationContext>();

        if (serviceNavigationContext.EndSlot is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn([TagName/*, ShortTagName*/], [ServiceNavigationTagHelper.TagName]);
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
