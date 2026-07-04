<!-- Generated from src/GovUk.Frontend.AspNetCore.Docs/Templates/components/panel.liquid -->
# Panel

[GOV.UK Design System panel component](https://design-system.service.gov.uk/components/panel/)


## Tag helpers

### Example
<img alt="Panel example" src="../images/panel-example.png" />

```razor
<govuk-panel heading-level="2">
    <govuk-panel-title>Application complete</govuk-panel-title>
    <govuk-panel-body>
        Your reference number<br /><strong>HDJ2123F</strong>
    </govuk-panel-body>
</govuk-panel>
```


### API

#### `<govuk-panel>`

| Attribute | Type | Description |
| --- | --- | --- |
| `heading-level` | `int?` | The heading level. Must be between `1` and `6` (inclusive). The default is `1`. |


#### `<govuk-panel-title>`

The content is the HTML to use within the panel title.

Must be inside a `<govuk-panel>` element.


#### `<govuk-panel-body>`

The content is the HTML to use within the panel body.

Must be inside a `<govuk-panel>` element.

