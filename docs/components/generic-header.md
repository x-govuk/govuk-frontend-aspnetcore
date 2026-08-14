<!-- Generated from src/GovUk.Frontend.AspNetCore.Docs/Templates/components/generic-header.liquid -->
# Generic header

[GOV.UK Design System generic header component](https://design-system.service.gov.uk/components/generic-header/)


## Tag helpers

### Example
<img alt="Generic header example" src="../images/generic-header-example.png" />

```razor
<govuk-generic-header home-page-url="https://my.service.gov.uk">
    <logo>
        My service
    </logo>
</govuk-generic-header>
```


### API

#### `<govuk-generic-header>`

The content is the HTML to use after the logo.

| Attribute | Type | Description |
| --- | --- | --- |
| `container-*` |  | Additional attributes to add to the generated container element. |
| `home-page-url` | `string` | The URL of the homepage link. If not specified, `/` will be used. |


#### `<logo>` / `<govuk-generic-header-logo>`

The content is the HTML to use within the logo's link.

Must be inside a `<govuk-generic-header>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `link-*` |  | Additional attributes to add to the generated homepage link element. |

