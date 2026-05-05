using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Http;

namespace GovUk.Frontend.AspNetCore;

/// <summary>
/// GOV.UK Frontend extensions for <see cref="HttpContext"/>.
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// Adds a page error to the specified <see cref="HttpContext"/>.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/>.</param>
    /// <param name="html">The error content.</param>
    /// <param name="href">A link to the field the error relates to.</param>
    public static void AddPageError(this HttpContext context, TemplateString html, TemplateString? href)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(html);

        var pageErrorContext = GetPageErrorContext(context);
        pageErrorContext.AddError(html, href);
    }

    internal static PageErrorContext GetPageErrorContext(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var itemsKey = typeof(PageErrorContext);

        if (context.Items.TryGetValue(itemsKey, out var containerErrorContextObj) &&
            containerErrorContextObj is PageErrorContext pageErrorContext)
        {
            return pageErrorContext;
        }

        pageErrorContext = new PageErrorContext();
        context.Items[itemsKey] = pageErrorContext;
        return pageErrorContext;
    }
}
