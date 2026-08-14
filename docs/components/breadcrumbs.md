<!-- Generated from src/GovUk.Frontend.AspNetCore.Docs/Templates/components/breadcrumbs.liquid -->
# Breadcrumbs

[GOV.UK Design System breadcrumbs component](https://design-system.service.gov.uk/components/breadcrumbs/)


## Tag helpers

### Example
<img alt="Breadcrumbs example" src="../images/breadcrumbs-example.png" />

```razor
<govuk-breadcrumbs collapse-on-mobile="true">
    <breadcrumbs-item asp-controller="Home" asp-action="Index">Home</breadcrumbs-item>
    <breadcrumbs-item href="#" link-target="_blank">Passports, travel and living abroad</breadcrumbs-item>
    <breadcrumbs-item>Travel abroad</breadcrumbs-item>
</govuk-breadcrumbs>
```


### Example with long tag names
<img alt="Breadcrumbs with long tag names example" src="../images/breadcrumbs-with-long-tag-names-example.png" />

```razor
<govuk-breadcrumbs collapse-on-mobile="true">
    <govuk-breadcrumbs-item asp-controller="Home" asp-action="Index">Home</govuk-breadcrumbs-item>
    <govuk-breadcrumbs-item href="#" link-target="_blank">Passports, travel and living abroad</govuk-breadcrumbs-item>
    <govuk-breadcrumbs-item>Travel abroad</govuk-breadcrumbs-item>
</govuk-breadcrumbs>
```


### API

#### `<govuk-breadcrumbs>`

| Attribute | Type | Description |
| --- | --- | --- |
| `collapse-on-mobile` | `bool?` | Whether to collapse to the first and last item only on tablet breakpoint and below. If not specified, `false` will be used. |
| `label-text` | `string` | The plain text label identifying the landmark to screen readers. Defaults to `Breadcrumb`. |


#### `<breadcrumbs-item>` / `<govuk-breadcrumbs-item>`

The content is the HTML to use within the breadcrumbs item.

Must be inside a `<govuk-breadcrumbs>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `link-*` |  | Additional attributes for the generated `a` element. |
| (link attributes) |  | See [documentation on links](../links.md) for more information. |

