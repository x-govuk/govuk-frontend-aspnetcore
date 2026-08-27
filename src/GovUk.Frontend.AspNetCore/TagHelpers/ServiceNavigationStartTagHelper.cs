using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the content at the start of the service header container in a GDS service navigation component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = ServiceNavigationTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = ServiceNavigationTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML at the start of the service header container.")]
public class ServiceNavigationStartTagHelper : TagHelper
{
    internal const string TagName = "govuk-service-navigation-start";
    internal const string ShortTagName = ShortTagNames.Start;

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var serviceNavigationContext = context.GetContextItem<ServiceNavigationContext>();

        serviceNavigationContext.CheckChildTagNameSpelling(context.TagName);

        if (serviceNavigationContext.StartSlot is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(AllTagNames, [ServiceNavigationTagHelper.TagName]);
        }

        if (serviceNavigationContext.Nav is not null)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(context.TagName, serviceNavigationContext.Nav.TagName!);
        }

        if (serviceNavigationContext.EndSlot is { TagName: var endTagName })
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(context.TagName, endTagName);
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

        serviceNavigationContext.StartSlot = (content.Snapshot(), context.TagName);

        output.SuppressOutput();
    }
}
