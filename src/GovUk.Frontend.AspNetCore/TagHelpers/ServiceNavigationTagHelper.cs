using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS service navigation component.
/// </summary>
[HtmlTargetElement(TagName)]
[RestrictChildren(
    ServiceNavigationStartTagHelper.TagName,
    //ServiceNavigationStartTagHelper.ShortTagName,
    ServiceNavigationNavTagHelper.TagName,
    //ServiceNavigationNavTagHelper.ShortTagName,
    ServiceNavigationEndTagHelper.TagName/*,
    ServiceNavigationEndTagHelper.ShortTagName*/)]
public class ServiceNavigationTagHelper : TagHelper
{
    internal const string TagName = "govuk-service-navigation";

    private const string ServiceNameAttributeName = "service-name";
    private const string ServiceUrlAttributeName = "service-url";

    private readonly IComponentGenerator _componentGenerator;

    /// <summary>
    /// Creates a new <see cref="ServiceNavigationTagHelper"/>.
    /// </summary>
    public ServiceNavigationTagHelper(IComponentGenerator componentGenerator)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);

        _componentGenerator = componentGenerator;
    }

    /// <summary>
    /// The name of your service.
    /// </summary>
    [HtmlAttributeName(ServiceNameAttributeName)]
    public string? ServiceName { get; set; }

    /// <summary>
    /// The homepage of your service.
    /// </summary>
    [HtmlAttributeName(ServiceUrlAttributeName)]
#pragma warning disable CA1056
    public string? ServiceUrl { get; set; }
#pragma warning restore CA1056

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        context.SetContextItem(new ServiceNavigationContext());
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var serviceNavigationContext = context.GetContextItem<ServiceNavigationContext>();

        _ = await output.GetChildContentAsync();

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        var navigationAttributes = serviceNavigationContext.Nav?.Attributes?.Clone() ?? [];
        navigationAttributes.Remove("class", out var navigationClasses);

        var component = await _componentGenerator.GenerateServiceNavigationAsync(new ServiceNavigationOptions
        {
            Classes = classes,
            Attributes = attributes,
            AriaLabel = serviceNavigationContext.Nav?.AriaLabel,
            MenuButtonText = serviceNavigationContext.Nav?.MenuButtonText,
            MenuButtonLabel = serviceNavigationContext.Nav?.MenuButtonLabel,
            NavigationLabel = serviceNavigationContext.Nav?.Label,
            NavigationId = serviceNavigationContext.Nav?.Id,
            NavigationClasses = navigationClasses,
            CollapseNavigationOnMobile = serviceNavigationContext.Nav?.CollapseNavigationOnMobile,
            ServiceName = ServiceName,
            ServiceUrl = ServiceUrl,
            Navigation = serviceNavigationContext.Nav?.Items,
            Slots = new ServiceNavigationOptionsSlots
            {
                Start = serviceNavigationContext.StartSlot?.Html,
                End = serviceNavigationContext.EndSlot?.Html,
                NavigationStart = serviceNavigationContext.Nav?.NavigationStartSlot?.Html,
                NavigationEnd = serviceNavigationContext.Nav?.NavigationEndSlot?.Html
            },
            NavigationAttributes = navigationAttributes
        });

        component.ApplyToTagHelper(output);
    }
}
