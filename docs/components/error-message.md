<!-- Generated from src/GovUk.Frontend.AspNetCore.Docs/Templates/components/error-message.liquid -->
# Error message

[GOV.UK Design System error message component](https://design-system.service.gov.uk/components/error-message/)


## Tag helpers

### Example with specified content
<img alt="Error message with specified content example" src="../images/error-message-with-specified-content-example.png" />

```razor
<govuk-error-message>Enter your full name</govuk-error-message>
```


### Example with overridden visually hidden text
<img alt="Error message with overridden visually hidden text example" src="../images/error-message-with-overridden-visually-hidden-text-example.png" />

```razor
<govuk-error-message visually-hidden-text="Gwall">Rhowch eich enw llawn</govuk-error-message>
```


### Example with model state error
<img alt="Error message with model state error example" src="../images/error-message-with-model-state-error-example.png" />

```razor
<govuk-error-message for="FullName" />
```


### API

#### `<govuk-error-message>`

The content is the HTML to use within the error message. Content is required if the 'for' attribute is not specified. If 'for' is specified and there are no errors in the model state then no output is generated; if there are multiple errors only the first is used.

| Attribute | Type | Description |
| --- | --- | --- |
| `for` | `Microsoft.AspNetCore.Mvc.ViewFeatures.ModelExpression` | An expression to be evaluated against the current model. |
| `visually-hidden-text` | `string` | The visually hidden prefix used before the error message. The default is `"Error"`. |

