using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the summary list within a GDS summary card component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = SummaryCardTagHelper.TagName)]
public class SummaryCardSummaryListTagHelper : TagHelper
{
    internal const string TagName = SummaryListTagHelper.TagName;

    /// <inheritdoc/>
    public override int Order => -1;

    /// <inheritdoc/>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var summaryListContext = context.GetContextItem<SummaryListContext>();
        summaryListContext.HaveCard = true;

        output.SuppressOutput();
    }
}
