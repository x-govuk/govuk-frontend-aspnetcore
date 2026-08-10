<!-- Generated from src/GovUk.Frontend.AspNetCore.Docs/Templates/components/date-input.liquid -->
# Date input

[GOV.UK Design System date input component](https://design-system.service.gov.uk/components/date-input/)


## Tag helpers

### Example
<img alt="Date input example" src="../images/date-input-example.png" />

```razor
<govuk-date-input for="PassportIssued" error-message-prefix="Your passport issue date" />
```


### Example with error message
<img alt="Date input with error message example" src="../images/date-input-with-error-message-example.png" />

```razor
<govuk-date-input id="passport-issued" name-prefix="passport-issued" error-message-prefix="Your passport issue date">
    <govuk-date-input-error-message>
        The date your passport was issued must be in the past
    </govuk-date-input-error-message>
</govuk-date-input>
```


### Example with fieldset
<img alt="Date input with fieldset example" src="../images/date-input-with-fieldset-example.png" />

```razor
<govuk-date-input id="passport-issued" name-prefix="passport-issued" error-message-prefix="Your passport issue date">
    <govuk-date-input-fieldset-legend is-page-heading="true" class="govuk-fieldset__legend--l">
        When was your passport issued?
    </govuk-date-input-fieldset-legend>
    <govuk-date-input-hint>
        For example, 27 3 2007
    </govuk-date-input-hint>
    <govuk-date-input-error-message>
        The date your passport was issued must be in the past
    </govuk-date-input-error-message>
</govuk-date-input>
```


### Example with a fieldset generated from the model metadata
<img alt="Date input with a generated fieldset example" src="../images/date-input-with-generated-fieldset-example.png" />

```razor
<govuk-date-input for="PassportIssued" error-message-prefix="Your passport issue date" fieldset legend-class="govuk-fieldset__legend--l" legend-is-page-heading="true" />
```


### Example with custom item labels
<img alt="Date input with custom item labels example" src="../images/date-input-with-custom-item-labels-example.png" />

```razor
<govuk-date-input for="PassportIssued">
    <govuk-date-input-day>
        <govuk-date-input-day-label>Dydd</govuk-date-input-day-label>
    </govuk-date-input-day>
    <govuk-date-input-month>
        <govuk-date-input-month-label>Mis</govuk-date-input-month-label>
    </govuk-date-input-month>
    <govuk-date-input-year>
        <govuk-date-input-year-label>Blwyddyn</govuk-date-input-year-label>
    </govuk-date-input-year>
</govuk-date-input>
```


### Example with custom item values
<img alt="Date input with custom item values example" src="../images/date-input-with-custom-item-values-example.png" />

```razor
<govuk-date-input for="PassportIssued" error-message-prefix="Your passport issue date">
    <govuk-date-input-day value="1" />
    <govuk-date-input-month value="4" />
    <govuk-date-input-year value="2022" />
</govuk-date-input>
```


### Example with day and month only
<img alt="Date input with day and month only example" src="../images/date-input-with-day-and-month-only-example.png" />

```razor
<govuk-date-input for="Birthday" item-types="DateInputItemTypes.DayAndMonth" error-message-prefix="Your birthday">
    <govuk-date-input-fieldset-legend>What is your birthday?</govuk-date-input-fieldset-legend>
</govuk-date-input>
```


### Example with month and year only
<img alt="Date input with month and year only example" src="../images/date-input-with-month-and-year-only-example.png" />

```razor
<govuk-date-input for="DateMovedIn" item-types="DateInputItemTypes.MonthAndYear" error-message-prefix="The date you moved into this property">
    <govuk-date-input-fieldset-legend>When did you move into this property?</govuk-date-input-fieldset-legend>
</govuk-date-input>
```


### API

#### `<govuk-date-input>`

| Attribute | Type | Description |
| --- | --- | --- |
| `date-input-*` |  | Additional attributes for the container element that wraps the items. |
| `disabled` | `bool?` | Whether the `disabled` attribute should be added to the generated `input` elements. |
| `error-message-prefix` | `string` | The prefix to use in generated error messages. This is required unless an error message prefix has been specified on the model with a `DateInputAttribute`. |
| `fieldset` | `bool` | Whether a `fieldset` should be generated to wrap the component. A `fieldset` is generated automatically when a `govuk-date-input-fieldset` element or a `govuk-date-input-fieldset-legend` element is used, or when any `fieldset-*`, `legend-*` or `legend-is-page-heading` attribute is specified; this attribute is only required when a `fieldset` is wanted but none of those are used.  The legend's content is deduced from the `For` expression's metadata. |
| `fieldset-*` |  | Additional attributes for the generated `fieldset` element. |
| `for` | `Microsoft.AspNetCore.Mvc.ViewFeatures.ModelExpression` | An expression to be evaluated against the current model. |
| `id` | `string` | The `id` attribute for the main component. Also used to generate an `id` for each item's `input` when the corresponding `Id` is not specified. |
| `ignore-modelstate-errors` | `bool?` | Whether the `Errors` for the `For` expression should be used to deduce an error message. When there are multiple errors in the `ModelErrorCollection` the first is used. |
| `item-types` | `GovUk.Frontend.AspNetCore.DateInputItemTypes?` | The `DateInputItemTypes` that this date input contains. This is required when creating a partial date input (e.g. a day and month only) and the value is a `ValueTuple`2`. |
| `legend-*` |  | Additional attributes for the generated `fieldset`'s `legend` element. These are combined with any attributes specified on a `govuk-date-input-fieldset-legend` element; where both specify the same attribute the one on the element wins, except for `class`, where the two values are combined. |
| `legend-is-page-heading` | `bool?` | Whether the generated `fieldset`'s `legend` also acts as the heading for the page. An `is-page-heading` attribute on a `govuk-date-input-fieldset-legend` element takes precedence over this. |
| `name-prefix` | `string` | Optional prefix for the `name` attribute on each item's `input`. |
| `readonly` | `bool?` | Whether the `readonly` attribute should be added to the generated `input` elements. |
| `value` | `object` | The date to populate the item values with. |


#### `<govuk-date-input-fieldset>`

A container element used when the date input should be contained within a fieldset element. When used, every other child element must be placed inside this element rather than the root date input element.

Must be inside a `<govuk-date-input>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `described-by` | `string` | One or more element IDs to add to the `aria-describedby` attribute. |


#### `<govuk-date-input-fieldset-legend>`

The content is the HTML to use within the legend. When this element is specified directly inside the root date input element a fieldset is generated automatically.

Must be inside a `<govuk-date-input>` or `<govuk-date-input-fieldset>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `is-page-heading` | `bool?` | Whether the legend also acts as the heading for the page. The default is `false`. |


#### `<govuk-date-input-hint>`

The content is the HTML to use within the component's hint.

Must be inside a `<govuk-date-input>` or `<govuk-date-input-fieldset>` element.


#### `<govuk-date-input-error-message>`

The content is the HTML to use within the component's error message.

Must be inside a `<govuk-date-input>` or `<govuk-date-input-fieldset>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `error-items` | `GovUk.Frontend.AspNetCore.DateInputItemTypes?` | The components of the date that have errors (day, month and/or year). If the value for the parent `DateInputTagHelper` was specified using `For` then `ErrorItems` will be computed from model binding errors. |
| `visually-hidden-text` | `string` | A visually hidden prefix used before the error message. The default is `"Error"`. |


#### `<govuk-date-input-before-inputs>`

The content is the HTML to use before the date input.

Must be inside a `<govuk-date-input>` or `<govuk-date-input-fieldset>` element.


#### `<govuk-date-input-day>`

Must be inside a `<govuk-date-input>` or `<govuk-date-input-fieldset>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `autocomplete` | `string` | The `autocomplete` attribute for the generated `input` element. |
| `id` | `string` | The `id` attribute for the generated `input` element. By default the value will be generated from the parent's `Id`. |
| `inputmode` | `string` | The `inputmode` attribute for the generated `input` element. The default is `numeric`. |
| `name` | `string` | The `name` attribute for the generated `input` element. By default the value will be generated from the parent's `For` and/or `NamePrefix`. |
| `pattern` | `string` | The `pattern` attribute for the generated `input` element. The default is `[0-9]*`. |
| `value` | `string` | The `value` attribute for the generated `input` element. This cannot be specified if the `Value` property on the parent is also specified. |


#### `<govuk-date-input-day-label>`

The content is the HTML to use within the item's label.

Must be inside a `<govuk-date-input-day>` element.


#### `<govuk-date-input-month>`

Must be inside a `<govuk-date-input>` or `<govuk-date-input-fieldset>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `autocomplete` | `string` | The `autocomplete` attribute for the generated `input` element. |
| `id` | `string` | The `id` attribute for the generated `input` element. By default the value will be generated from the parent's `Id`. |
| `inputmode` | `string` | The `inputmode` attribute for the generated `input` element. The default is `numeric`. |
| `name` | `string` | The `name` attribute for the generated `input` element. By default the value will be generated from the parent's `For` and/or `NamePrefix`. |
| `pattern` | `string` | The `pattern` attribute for the generated `input` element. The default is `[0-9]*`. |
| `value` | `string` | The `value` attribute for the generated `input` element. This cannot be specified if the `Value` property on the parent is also specified. |


#### `<govuk-date-input-month-label>`

The content is the HTML to use within the item's label.

Must be inside a `<govuk-date-input-month>` element.


#### `<govuk-date-input-year>`

Must be inside a `<govuk-date-input>` or `<govuk-date-input-fieldset>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `autocomplete` | `string` | The `autocomplete` attribute for the generated `input` element. |
| `id` | `string` | The `id` attribute for the generated `input` element. By default the value will be generated from the parent's `Id`. |
| `inputmode` | `string` | The `inputmode` attribute for the generated `input` element. The default is `numeric`. |
| `name` | `string` | The `name` attribute for the generated `input` element. By default the value will be generated from the parent's `For` and/or `NamePrefix`. |
| `pattern` | `string` | The `pattern` attribute for the generated `input` element. The default is `[0-9]*`. |
| `value` | `string` | The `value` attribute for the generated `input` element. This cannot be specified if the `Value` property on the parent is also specified. |


#### `<govuk-date-input-year-label>`

The content is the HTML to use within the item's label.

Must be inside a `<govuk-date-input-year>` element.


#### `<govuk-date-input-after-inputs>`

The content is the HTML to use after the date input.

Must be inside a `<govuk-date-input>` or `<govuk-date-input-fieldset>` element.


## Error message prefixes

Every date input must have an error message prefix; it's used at the start of the error messages generated by the model binder
e.g. `Your passport issue date must be a real date`.

It can be specified with the `error-message-prefix` attribute:

```razor
<govuk-date-input for="PassportIssued" error-message-prefix="Your passport issue date" />
```

or, when a `for` model expression is used, with an attribute on the bound property:

```csharp
public class MyModel
{
    [DateInput(ErrorMessagePrefix = "Your passport issue date")]
    public DateOnly? PassportIssued { get; set; }
}
```

If neither is specified an exception is thrown when the component is rendered.

## Date types

By default `System.DateTime` and `System.DateOnly` instances can be used as values for this component.
A model binder converts the three inputs into a single instance of whatever model type is required.
The model binder also tracks which components were invalid so that the correct items can be highlighted and a useful error message can be provided.

### Partial dates

For partial dates, the `value` attribute or `for` model expression should be a `ValueTuple<int, int>`.
(See [custom date types](#custom-date-types) for how to add support for other types.)

If a `for` model expression is specified, the item types can be specified by using an attribute on the bound property. For example:

```csharp
public class MyModel
{
    [DateInput(DateInputItemTypes.MonthAndYear)]
    public (int Month, int Year) Birthday { get; set; }
}
```

Otherwise, the `item-types` attribute must be specified.

### Custom date types

You can add support for additional types by implementing `GovUk.Frontend.AspNetCore.ModelBinding.DateInputModelConverter`.
See the sample at `samples/Samples.DateInput/` for a example implementations for NodaTime's `LocalDate` and `YearMonth` types.
