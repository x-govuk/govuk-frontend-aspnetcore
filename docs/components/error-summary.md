<!-- Generated from src/GovUk.Frontend.AspNetCore.Docs/Templates/components/error-summary.liquid -->
# Error summary

[GOV.UK Design System error summary component](https://design-system.service.gov.uk/components/error-summary/)

> [!NOTE]
> By default an error summary will be added to the top of your `<main>` element containing links to all the errors on the page.
> You can disable this by setting the `GenerateErrorSummaries` property on `GovUkFrontendOptions` to `GenerateErrorSummariesOptions.None`.


## Tag helpers

### Example
<img alt="Error summary example" src="../images/error-summary-example.png" />

```razor
<govuk-error-summary>
    <error-summary-item href="#passport-issued-day">The date your passport was issued must be in the past</error-summary-item>
    <error-summary-item href="#postcode-input">Enter a postcode, like AA1 1AA</error-summary-item>
</govuk-error-summary>
```


### Example with overridden title
<img alt="Error summary with overridden title example" src="../images/error-summary-with-title-example.png" />

```razor
<govuk-error-summary>
    <error-summary-title>There is a problem</error-summary-title>
    <error-summary-item href="#passport-issued-day">The date your passport was issued must be in the past</error-summary-item>
    <error-summary-item href="#postcode-input">Enter a postcode, like AA1 1AA</error-summary-item>
</govuk-error-summary>
```


### Example with model state error
<img alt="Error summary with model state error example" src="../images/error-summary-with-model-state-error-example.png" />

```razor
<govuk-error-summary>
    <error-summary-item for="FullName" />
</govuk-error-summary>
```


### Example with long tag names
<img alt="Error summary with long tag names example" src="../images/error-summary-with-long-tag-names-example.png" />

```razor
<govuk-error-summary>
    <govuk-error-summary-title>There is a problem</govuk-error-summary-title>
    <govuk-error-summary-description>Check the following before continuing.</govuk-error-summary-description>
    <govuk-error-summary-item href="#passport-issued-day">The date your passport was issued must be in the past</govuk-error-summary-item>
    <govuk-error-summary-item asp-controller="Home" asp-action="Index" asp-fragment="postcode-input">Enter a postcode, like AA1 1AA</govuk-error-summary-item>
</govuk-error-summary>
```


### API

#### `<govuk-error-summary>`

| Attribute | Type | Description |
| --- | --- | --- |
| `disable-auto-focus` | `bool?` | Whether to disable the behavior that focuses the error summary when the page loads. |


#### `<error-summary-title>` / `<govuk-error-summary-title>`

The content is the HTML to use within the title for the error summary. If this element is not specified then the content is 'There is a problem'.

Must be inside a `<govuk-error-summary>` element.


#### `<description>` / `<govuk-error-summary-description>`

The content is the HTML to use within the description for the error summary.

Must be inside a `<govuk-error-summary>` element.


#### `<error-summary-item>` / `<govuk-error-summary-item>`

The content is the HTML to use within the error link item. Content is required if the 'for' attribute is not specified. If 'for' is specified and there are no errors in the model state then the item will not be rendered.

Must be inside a `<govuk-error-summary>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `for` | `Microsoft.AspNetCore.Mvc.ViewFeatures.ModelExpression` | An expression to be evaluated against the current model. |
| `link-*` |  | Additional attributes to add to the generated `a` element. |
| (link attributes) |  | See [documentation on links](../links.md) for more information. |

