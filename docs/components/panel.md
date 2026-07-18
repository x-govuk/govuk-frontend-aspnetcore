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


### Interruption example
<img alt="Panel interruption example" src="../images/panel-interruption-example.png" />

```razor
<govuk-panel class="govuk-panel--interruption">
    <govuk-panel-title>Is your age correct?</govuk-panel-title>
    <govuk-panel-body>
        <p class="govuk-body">You entered your age as <strong>109</strong>.</p>
    </govuk-panel-body>
    <govuk-panel-actions>
        <govuk-panel-action type="button">Yes, this is correct</govuk-panel-action>
        <govuk-panel-action-link href="#">No, change my age</govuk-panel-action-link>
    </govuk-panel-actions>
</govuk-panel>
```


### Interruption example with generated actions
The `type="submit"` action generates a `formaction` attribute and the link action generates an `href` attribute from the routing (`asp-*`) attributes.
<img alt="Panel interruption with generated actions example" src="../images/panel-interruption-with-generated-actions-example.png" />

```razor
<govuk-panel class="govuk-panel--interruption">
    <govuk-panel-title>Is your age correct?</govuk-panel-title>
    <govuk-panel-body>
        <p class="govuk-body">You entered your age as <strong>109</strong>.</p>
    </govuk-panel-body>
    <govuk-panel-actions>
        <govuk-panel-action type="submit" asp-controller="Home" asp-action="Confirm">Yes, this is correct</govuk-panel-action>
        <govuk-panel-action-link asp-controller="Home" asp-action="Confirm">No, change my age</govuk-panel-action-link>
    </govuk-panel-actions>
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


#### `<govuk-panel-actions>`

Must be inside a `<govuk-panel>` element.


#### `<govuk-panel-action>`

The content is the HTML to use within the button.

Must be inside a `<govuk-panel-actions>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `type` | `string` | The `type` attribute for the generated `button` element. The default is `button`. |
| (link attributes) |  | See [documentation on links](../links.md) for more information. |


#### `<govuk-panel-action-link>`

The content is the HTML to use within the link.

Must be inside a `<govuk-panel-actions>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| (link attributes) |  | See [documentation on links](../links.md) for more information. |

