# Links

For tag helpers that generate `<a>` elements, you can specify the same attributes as with a regular `<a>` element to generate the `href` attribute:
- `asp-action`
- `asp-controller`
- `asp-area`
- `asp-page`
- `asp-page-handler`
- `asp-fragment`
- `asp-host`
- `asp-protocol`
- `asp-route`
- `asp-all-route-data`
- `asp-route-*`

See [the MVC documentation](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/built-in/anchor-tag-helper) for more information.

The following tag helpers support generating the `href` attribute using these attributes:
- `<govuk-back-link>`
- `<govuk-breadcrumbs-item>`
- `<govuk-cookie-banner-message-action-link>`
- `<govuk-button-link>`
- `<govuk-error-summary-item>`
- `<govuk-footer-meta-item>`
- `<govuk-footer-nav-item>`
- `<govuk-pagination-item>`
- `<govuk-pagination-next>`
- `<govuk-pagination-previous>`
- `<govuk-service-navigation-nav-item>`
- `<govuk-summary-list-row-action>`

### Example - MVC action

```razor
<govuk-back-link asp-controller="TheController" asp-action="TheAction" />
```

### Example - Razor page

```razor
<govuk-back-link asp-page="ThePage" />
```

## `formaction`

For `<govuk-button>` a similar set of `asp-` attributes listed above can be used to generate the `formaction` attribute:
- `asp-action`
- `asp-controller`
- `asp-area`
- `asp-page`
- `asp-page-handler`
- `asp-fragment`
- `asp-route`
- `asp-all-route-data`
- `asp-route-*`

### Example

```razor
<govuk-button asp-controller="TheController" asp-action="TheAction" />
```
