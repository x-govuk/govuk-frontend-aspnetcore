<!-- Generated from src/GovUk.Frontend.AspNetCore.Docs/Templates/components/fieldset.liquid -->
# Fieldset

[GOV.UK Design System fieldset component](https://design-system.service.gov.uk/components/fieldset/)


## Tag helpers

### Example
<img alt="Fieldset example" src="../images/fieldset-example.png" />

```razor
<govuk-fieldset>
    <govuk-fieldset-legend is-page-heading="true" class="govuk-fieldset__legend--l">Legend as page heading</govuk-fieldset-legend>
</govuk-fieldset>
```


### API

#### `<govuk-fieldset>`

| Attribute | Type | Description |
| --- | --- | --- |
| `described-by` | `string` | One or more element IDs to add to the `aria-describedby` attribute. |
| `role` | `string` | The `role` attribute. |


#### `<govuk-fieldset-legend>`

The content is the HTML to use within the legend.

Must be inside a `<govuk-fieldset>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `is-page-heading` | `bool?` | Whether the legend also acts as the heading for the page. |

