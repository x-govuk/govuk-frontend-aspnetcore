# Error pages

The library includes the pages from these GOV.UK Design System patterns:

- [Page not found](https://design-system.service.gov.uk/patterns/page-not-found-pages/), for `404` responses
- [There is a problem with the service](https://design-system.service.gov.uk/patterns/problem-with-the-service-pages/), for unhandled exceptions (`500`)

## Getting started

The pages are Razor views, so your app needs MVC's view services. `AddControllersWithViews()`, `AddRazorPages()` and `AddMvc()` all add them.

```csharp
builder.Services.AddControllersWithViews();
builder.Services.AddGovUkFrontend();

var app = builder.Build();

app.UseGovUkFrontendErrorPages();

// ...
```

### Where it goes in the pipeline

The middleware only handles `404` responses and exceptions from the middleware and endpoints that come after it, so call `UseGovUkFrontendErrorPages()` early, before anything whose errors it should handle.

It can go before or after `UseRouting()`.
The middleware only looks at the matched endpoint, to recognise API controllers, after the rest of the pipeline has run, and routing has matched it by then wherever `UseRouting()` is.

### Which requests get a page

The pages are only rendered for requests that accept `text/html`.
Requests that don't, and requests for an `[ApiController]`, are left as they are: a `404` keeps whatever the endpoint wrote, and an unhandled exception gives an empty `500`.

A `404` is only replaced when the response has no body, as with ASP.NET Core's status code pages.
To keep the `404` from a particular endpoint, add `[SkipStatusCodePages]` to it.

## Configuration

Configure the pages with `GovUkFrontendErrorPageOptions`:

```csharp
builder.Services.Configure<GovUkFrontendErrorPageOptions>(options =>
{
    // The exception handler hides the developer exception page.
    options.HandleExceptions = !builder.Environment.IsDevelopment();
});
```

| Option | Default | Description |
| --- | --- | --- |
| `HandlePageNotFound` | `true` | Render the Page not found page for `404` responses. |
| `HandleExceptions` | `true` | Render the There is a problem with the service page for unhandled exceptions. |
| `Layout` | `null` | The layout for the pages. See [Layout](#layout). |

The options are read at startup, so changes after that have no effect.

## Layout

By default the pages use the layout set by your `Views/_ViewStart.cshtml`, so they get the same header, footer and service navigation as the rest of your app.
If there's no `Views/_ViewStart.cshtml`, or it doesn't set a layout, the pages use `_GovUkPageTemplate`.

Razor Pages apps usually have `Pages/_ViewStart.cshtml` instead, which doesn't apply to the pages, so set the layout yourself:

```csharp
builder.Services.Configure<GovUkFrontendErrorPageOptions>(options => options.Layout = "_Layout");
```

The pages set `ViewData["Title"]` to the page heading and add `govuk-main-wrapper--l` to the `<main>` element, as the patterns do.
If your layout appends the service name to the title, the titles match the patterns' format, e.g. "Page not found – Service name – GOV.UK".

## Adding content

The patterns ask for service-specific content, such as contact details, what happened to the user's answers, or a link to another service.
To add it, create a partial view in `Views/Shared` (or `Pages/Shared`) with the right name, and it's rendered below the standard content:

| Page | Partial view |
| --- | --- |
| Page not found | `_GovUkPageNotFoundContent.cshtml` |
| There is a problem with the service | `_GovUkProblemWithTheServiceContent.cshtml` |

For example, `Views/Shared/_GovUkProblemWithTheServiceContent.cshtml`:

```razor
<p class="govuk-body">We saved your answers. They will be available for 30 days.</p>
<p class="govuk-body">
    <a href="/contact" class="govuk-link">Contact the Tax Credits Helpline</a> if you need to make changes to your claim or speak to someone about your tax credits.
</p>
```

The partials' model is `GovUkErrorPageModel`, which has the response's `StatusCode`.

## Using the pages for other responses

The middleware only handles `404` responses and exceptions. To show one of the pages for another response, such as a `403`,
return it from a controller or a Razor Page with `GovUkPageNotFound()` or `GovUkProblemWithTheService()`:

```csharp
public class ReportsController : Controller
{
    public IActionResult Show(int id)
    {
        if (!CanView(id))
        {
            return this.GovUkProblemWithTheService(StatusCodes.Status403Forbidden);
        }

        // ...
    }
}
```

```csharp
public class ReportModel : PageModel
{
    public IActionResult OnGet(int id)
    {
        if (!CanView(id))
        {
            // Show a 404 page rather than revealing that the report exists
            return this.GovUkPageNotFound();
        }

        // ...
    }
}
```

`GovUkPageNotFound()` defaults to a `404` status code and `GovUkProblemWithTheService()` to a `500`; pass a status code to use another.
Both set the response's status code and render the page as the middleware does, with the same [layout](#layout),
[content partials](#adding-content), [replacement views](#replacing-a-page) and [localization](#localization).
The content partials get the status code on `GovUkErrorPageModel.StatusCode`, so they can show content for a particular status.
They work whether or not you call `UseGovUkFrontendErrorPages()`.

Both methods return a `GovUkErrorPageResult`, which you can also create directly, for example in a filter.

## Replacing a page

To replace a page altogether, create a view in `Views/Shared` (or `Pages/Shared`) with the same name:
`GovUkPageNotFound.cshtml` or `GovUkProblemWithTheService.cshtml`.
The names are also on `GovUkErrorPageModel.ViewNames`.

## Localization

The pages' content is localized through `IGovUkFrontendLocalizer`, using the `PageNotFoundPage.*` and `ProblemWithTheServicePage.*` names on `GovUkFrontendResourceNames`.
See [Localization](localization.md).
