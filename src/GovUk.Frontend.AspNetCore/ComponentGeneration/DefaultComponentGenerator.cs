using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator : IComponentGenerator
{
    internal const string DefaultErrorSummaryTitleHtml = "There is a problem";

    private readonly IRazorViewEngine _viewEngine;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DefaultComponentGenerator(IRazorViewEngine viewEngine, IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(viewEngine);
        ArgumentNullException.ThrowIfNull(httpContextAccessor);

        _viewEngine = viewEngine;
        _httpContextAccessor = httpContextAccessor;
    }

    public virtual Task<IHtmlContent> GenerateAccordionAsync(AccordionOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("Accordion", options);
    }

    public virtual Task<IHtmlContent> GenerateBackLinkAsync(BackLinkOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("BackLink", options);
    }

    public virtual Task<IHtmlContent> GenerateBreadcrumbsAsync(BreadcrumbsOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("Breadcrumbs", options);
    }

    public virtual Task<IHtmlContent> GenerateButtonAsync(ButtonOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("Button", options);
    }

    public virtual Task<IHtmlContent> GenerateCharacterCountAsync(CharacterCountOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("CharacterCount", options);
    }

    public virtual Task<IHtmlContent> GenerateCheckboxesAsync(CheckboxesOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("Checkboxes", options);
    }

    public virtual Task<IHtmlContent> GenerateCookieBannerAsync(CookieBannerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("CookieBanner", options);
    }

    public virtual Task<IHtmlContent> GenerateDateInputAsync(DateInputOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("DateInput", options);
    }

    public virtual Task<IHtmlContent> GenerateDetailsAsync(DetailsOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("Details", options);
    }

    public virtual Task<IHtmlContent> GenerateErrorMessageAsync(ErrorMessageOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("ErrorMessage", options);
    }

    public virtual Task<IHtmlContent> GenerateErrorSummaryAsync(ErrorSummaryOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("ErrorSummary", options);
    }

    public virtual Task<IHtmlContent> GenerateExitThisPageAsync(ExitThisPageOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("ExitThisPage", options);
    }

    public virtual Task<IHtmlContent> GenerateFieldsetAsync(FieldsetOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("Fieldset", options);
    }

    public virtual Task<IHtmlContent> GenerateFileUploadAsync(FileUploadOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("FileUpload", options);
    }

    public virtual Task<IHtmlContent> GenerateFooterAsync(FooterOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("Footer", options);
    }

    public virtual Task<IHtmlContent> GenerateHeaderAsync(HeaderOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("Header", options);
    }

    public virtual Task<IHtmlContent> GenerateHintAsync(HintOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("Hint", options);
    }

    public virtual Task<IHtmlContent> GenerateInsetTextAsync(InsetTextOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("InsetText", options);
    }

    public virtual Task<IHtmlContent> GenerateInputAsync(InputOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("Input", options);
    }

    public virtual Task<IHtmlContent> GenerateLabelAsync(LabelOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("Label", options);
    }

    public virtual Task<IHtmlContent> GenerateNotificationBannerAsync(NotificationBannerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("NotificationBanner", options);
    }

    public virtual Task<IHtmlContent> GenerateRadiosAsync(RadiosOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("Radios", options);
    }

    public virtual Task<IHtmlContent> GenerateSelectAsync(SelectOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("Select", options);
    }

    public virtual Task<IHtmlContent> GenerateServiceNavigationAsync(ServiceNavigationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("ServiceNavigation", options);
    }

    public virtual Task<IHtmlContent> GeneratePanelAsync(PanelOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("Panel", options);
    }

    public virtual Task<IHtmlContent> GeneratePhaseBannerAsync(PhaseBannerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("PhaseBanner", options);
    }

    public virtual Task<IHtmlContent> GeneratePaginationAsync(PaginationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("Pagination", options);
    }

    public virtual Task<IHtmlContent> GeneratePasswordInputAsync(PasswordInputOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("PasswordInput", options);
    }

    public virtual Task<IHtmlContent> GenerateSkipLinkAsync(SkipLinkOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("SkipLink", options);
    }

    public virtual Task<IHtmlContent> GenerateSummaryListAsync(SummaryListOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("SummaryList", options);
    }

    public virtual Task<IHtmlContent> GenerateTableAsync(TableOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("Table", options);
    }

    public virtual Task<IHtmlContent> GenerateTabsAsync(TabsOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("Tabs", options);
    }

    public virtual Task<IHtmlContent> GenerateTagAsync(TagOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("Tag", options);
    }

    public virtual Task<IHtmlContent> GenerateTaskListAsync(TaskListOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("TaskList", options);
    }

    public virtual Task<IHtmlContent> GenerateTextareaAsync(TextareaOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("Textarea", options);
    }

    public virtual Task<IHtmlContent> GenerateWarningTextAsync(WarningTextOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderViewAsync("WarningText", options);
    }

    private async Task<IHtmlContent> RenderViewAsync(string viewName, object model)
    {
        var httpContext = _httpContextAccessor.HttpContext ??
            throw new InvalidOperationException($"No {nameof(HttpContext)}.");

        var endpoint = httpContext.GetEndpoint() ??
            throw new InvalidOperationException($"No {nameof(Endpoint)}.");

        var actionDescriptor = endpoint.Metadata.GetMetadata<ActionDescriptor>() ??
            throw new InvalidOperationException($"No {nameof(ActionDescriptor)} in endpoint metadata.");

        var actionContext = new ActionContext(httpContext, httpContext.GetRouteData(), actionDescriptor);

        var fullViewName = $"/ComponentGeneration/Templates/{viewName}.cshtml";
        var viewEngineResult = _viewEngine.GetView(executingFilePath: null, fullViewName, isMainPage: false);
        if (!viewEngineResult.Success)
        {
            throw new InvalidOperationException($"Couldn't find view '{fullViewName}'.");
        }

        var view = viewEngineResult.View;
        using (view as IDisposable)
        {
            await using var output = new StringWriter();
            var viewContext = new ViewContext(
                actionContext,
                viewEngineResult.View,
                new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                },
                new TempDataDictionary(httpContext, new DummyTempDataProvider()),
                output,
                new HtmlHelperOptions());

            await viewEngineResult.View.RenderAsync(viewContext);
            return new HtmlString(output.ToString().Trim());
        }
    }

    private class DummyTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) =>
            throw new NotSupportedException();

        public void SaveTempData(HttpContext context, IDictionary<string, object> values) =>
            throw new NotSupportedException();
    }
}
