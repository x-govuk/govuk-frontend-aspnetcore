<!-- Generated from src/GovUk.Frontend.AspNetCore.Docs/Templates/components/panel.liquid -->
# Panel

[GOV.UK Design System panel component](https://design-system.service.gov.uk/components/panel/)


## Tag helpers

### Example
<img alt="Panel example" src="../images/panel-example.png" />

```razor
<govuk-panel heading-level="2">
    <panel-title>Application complete</panel-title>
    <panel-body>
        Your reference number<br><strong>HDJ2123F</strong>
    </panel-body>
</govuk-panel>
```


### Interruption example
<img alt="Panel interruption example" src="../images/panel-interruption-example.png" />

```razor
<govuk-panel class="govuk-panel--interruption">
    <panel-title>Is your age correct?</panel-title>
    <panel-body>
        <p class="govuk-body">You entered your age as <strong>109</strong>.</p>
    </panel-body>
    <panel-actions>
        <action-button type="button">Yes, this is correct</action-button>
        <action-link href="#">No, change my age</action-link>
    </panel-actions>
</govuk-panel>
```


### Interruption example with generated actions
The `type="submit"` action generates a `formaction` attribute and the link action generates an `href` attribute from the routing (`asp-*`) attributes.
<img alt="Panel interruption with generated actions example" src="../images/panel-interruption-with-generated-actions-example.png" />

```razor
<govuk-panel class="govuk-panel--interruption">
    <panel-title>Is your age correct?</panel-title>
    <panel-body>
        <p class="govuk-body">You entered your age as <strong>109</strong>.</p>
    </panel-body>
    <panel-actions>
        <action-button type="submit" asp-controller="Home" asp-action="Confirm">Yes, this is correct</action-button>
        <action-link asp-controller="Home" asp-action="Confirm">No, change my age</action-link>
    </panel-actions>
</govuk-panel>
```


### API

#### `<govuk-panel>`

| Attribute | Type | Description |
| --- | --- | --- |
| `heading-level` | `int?` | The heading level. Must be between `1` and `6` (inclusive). The default is `1`. |


#### `<panel-title>` / `<govuk-panel-title>`

The content is the HTML to use within the panel title.

Must be inside a `<govuk-panel>` element.


#### `<panel-body>` / `<govuk-panel-body>`

The content is the HTML to use within the panel body.

Must be inside a `<govuk-panel>` element.


#### `<panel-actions>` / `<govuk-panel-actions>`

Must be inside a `<govuk-panel>` element.


#### `<action-button>` / `<govuk-panel-action-button>`

The content is the HTML to use within the button.

Must be inside a `<panel-actions>` or `<govuk-panel-actions>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `type` | `string` | The `type` attribute for the generated `button` element. The default is `button`. |
| (link attributes) |  | See [documentation on links](../links.md) for more information. |


#### `<action-link>` / `<govuk-panel-action-link>`

The content is the HTML to use within the link.

Must be inside a `<panel-actions>` or `<govuk-panel-actions>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| (link attributes) |  | See [documentation on links](../links.md) for more information. |

