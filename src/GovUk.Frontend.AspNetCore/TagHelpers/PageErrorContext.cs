using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class PageErrorContext
{
    private readonly List<(TemplateString Html, TemplateString? Href)> _errors = [];

    internal IReadOnlyCollection<(TemplateString Html, TemplateString? Href)> Errors => _errors;

    internal bool ErrorSummaryHasBeenRendered { get; set; }

    public void AddError(TemplateString html, TemplateString? href)
    {
        ArgumentNullException.ThrowIfNull(html);

        _errors.Add((html, href));
    }

    public IReadOnlyCollection<ErrorSummaryOptionsErrorItem> GetErrorSummaryItems() =>
        _errors
            .Select(i => new ErrorSummaryOptionsErrorItem
            {
                Href = i.Href,
                Text = null,
                Html = i.Html,
                Attributes = null,
                ItemAttributes = null
            })
            .ToArray();
}
