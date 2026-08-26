using System.ComponentModel;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the title in the GDS summary card component.
/// </summary>
/// <remarks>
/// This element has been replaced by <c>card-title</c>.
/// </remarks>
[HtmlTargetElement(ShortTagName, ParentTag = SummaryCardTagHelper.TagName)]
[EditorBrowsable(EditorBrowsableState.Never)]
[Obsolete(
    "Use the <" + SummaryCardTitleTagHelper.ShortTagName + "> element instead.",
    DiagnosticId = DiagnosticIds.UseSummaryCardTitleElementInstead)]
public class LegacySummaryCardTitleTagHelper : SummaryCardTitleTagHelper
{
    internal new const string ShortTagName = ShortTagNames.Title;
}
