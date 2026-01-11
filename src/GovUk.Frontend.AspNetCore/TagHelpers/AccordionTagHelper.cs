using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS accordion component.
/// </summary>
[HtmlTargetElement(TagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.Accordion)]
[RestrictChildren(AccordionItemTagHelper.TagName)]
public class AccordionTagHelper : TagHelper
{
    internal const string TagName = "govuk-accordion";

    private const string HeadingLevelAttributeName = "heading-level";
    private const string HideAllSectionsTextAttributeName = "hide-all-sections-text";
    private const string HideSectionTextAttributeName = "hide-section-text";
    private const string HideSectionAriaLabelTextAttributeName = "hide-section-aria-label-text";
    private const string IdAttributeName = "id";
    private const string RememberExpandedAttributeName = "remember-expanded";
    private const string ShowAllSectionsTextAttributeName = "show-all-sections-text";
    private const string ShowSectionTextAttributeName = "show-section-text";
    private const string ShowSectionAriaLabelTextAttributeName = "show-section-aria-label-text";

    private readonly IComponentGenerator _componentGenerator;

    /// <summary>
    /// Creates a new <see cref="AccordionTagHelper"/>.
    /// </summary>
    public AccordionTagHelper(IComponentGenerator componentGenerator)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);
        _componentGenerator = componentGenerator;
    }

    /// <summary>
    /// The heading level.
    /// </summary>
    /// <remarks>
    /// Must be between <c>1</c> and <c>6</c> (inclusive). The default is <c>2</c>.
    /// </remarks>
    [HtmlAttributeName(HeadingLevelAttributeName)]
    public int? HeadingLevel { get; set; }

    /// <summary>
    /// The text content of the &quot;Hide all sections&quot; button at the top of the accordion when all sections
    /// are expanded.
    /// </summary>
    [HtmlAttributeName(HideAllSectionsTextAttributeName)]
    public string? HideAllSectionsText { get; set; }

    /// <summary>
    /// The text content of the &quot;Hide&quot; button within each section of the accordion, which is visible when the
    /// section is expanded.
    /// </summary>
    [HtmlAttributeName(HideSectionTextAttributeName)]
    public string? HideSectionText { get; set; }

    /// <summary>
    /// The text made available to assistive technologies, like screen-readers, as the final part of the toggle's
    /// accessible name when the section is expanded.
    /// </summary>
    /// <remarks>
    /// The default is <c>&quot;Hide this section&quot;</c>.
    /// </remarks>
    [HtmlAttributeName(HideSectionAriaLabelTextAttributeName)]
    public string? HideSectionAriaLabelText { get; set; }

    /// <summary>
    /// The <c>id</c> attribute for the accordion.
    /// </summary>
    /// <remarks>
    /// Must be unique across the domain of your service if <see cref="RememberExpanded"/> is <c>true</c>.
    /// Cannot be <c>null</c> or empty.
    /// </remarks>
    [HtmlAttributeName(IdAttributeName)]
    public string? Id { get; set; }

    /// <summary>
    /// Whether the expanded/collapsed state of the accordion should be saved when a user leaves the page and restored when they return.
    /// </summary>
    /// <remarks>
    /// The default is <c>true</c>.
    /// </remarks>
    [HtmlAttributeName(RememberExpandedAttributeName)]
    public bool? RememberExpanded { get; set; }

    /// <summary>
    /// The text content of the &quot;Show all sections&quot; button at the top of the accordion, which is visible when the
    /// section is collapsed.
    /// </summary>
    [HtmlAttributeName(ShowAllSectionsTextAttributeName)]
    public string? ShowAllSectionsText { get; set; }

    /// <summary>
    /// The text content of the &quot;Show&quot; button within each section of the accordion, which is visible when the
    /// section is collapsed.
    /// </summary>
    [HtmlAttributeName(ShowSectionTextAttributeName)]
    public string? ShowSectionText { get; set; }

    /// <summary>
    /// The text made available to assistive technologies, like screen-readers, as the final part of the toggle's
    /// accessible name when the section is collapsed.
    /// </summary>
    /// <remarks>
    /// The defaults is <c>&quot;Show this section&quot;</c>.
    /// </remarks>
    [HtmlAttributeName(ShowSectionAriaLabelTextAttributeName)]
    public string? ShowSectionAriaLabelText { get; set; }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        if (Id is null)
        {
            throw ExceptionHelper.TheAttributeMustBeSpecified(IdAttributeName);
        }

        if (HeadingLevel is not null and not (>= 1 and <= 6))
        {
            throw new InvalidOperationException(
                $"The '{nameof(HeadingLevelAttributeName)}' attribute must be between 1 and 6.");
        }

        var accordionContext = new AccordionContext();

        using (context.SetScopedContextItem(accordionContext))
        {
            _ = await output.GetChildContentAsync();
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        var component = await _componentGenerator.GenerateAccordionAsync(new AccordionOptions()
        {
            Id = Id,
            HeadingLevel = HeadingLevel,
            Classes = classes,
            Attributes = attributes,
            RememberExpanded = RememberExpanded,
            HideAllSectionsText = HideAllSectionsText,
            HideSectionText = HideSectionText,
            HideSectionAriaLabelText = HideSectionAriaLabelText,
            ShowAllSectionsText = ShowAllSectionsText,
            ShowSectionText = ShowSectionText,
            ShowSectionAriaLabelText = ShowSectionAriaLabelText,
            Items = accordionContext.Items
        });

        component.ApplyToTagHelper(output);
    }
}
