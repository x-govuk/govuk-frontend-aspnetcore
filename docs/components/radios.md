<!-- Generated from src/GovUk.Frontend.AspNetCore.Docs/Templates/components/radios.liquid -->
# Radios

[GOV.UK Design System radios component](https://design-system.service.gov.uk/components/radios/)


## Tag helpers

### Example
<img alt="Radios example" src="../images/radios-example.png" />

```razor
<govuk-radios for="WhereDoYouLive">
    <govuk-radios-fieldset-legend is-page-heading="true" class="govuk-fieldset__legend--l">
        Where do you live?
    </govuk-radios-fieldset-legend>

    <govuk-radios-hint>
        Select one option.
    </govuk-radios-hint>

    <govuk-radios-item value="england">England</govuk-radios-item>
    <govuk-radios-item value="scotland">Scotland</govuk-radios-item>
    <govuk-radios-item value="wales">Wales</govuk-radios-item>
    <govuk-radios-item value="northern-ireland">Northern Ireland</govuk-radios-item>
    <govuk-radios-divider>or</govuk-radios-divider>
    <govuk-radios-item value="abroad">I am a British citizen living abroad</govuk-radios-item>
</govuk-radios>
```


### Example with a fieldset generated from the model metadata
<img alt="Radios with a generated fieldset example" src="../images/radios-with-generated-fieldset-example.png" />

```razor
<govuk-radios for="WhereDoYouLive" fieldset legend-class="govuk-fieldset__legend--l" legend-is-page-heading="true">
    <govuk-radios-item value="england">England</govuk-radios-item>
    <govuk-radios-item value="scotland">Scotland</govuk-radios-item>
    <govuk-radios-item value="wales">Wales</govuk-radios-item>
    <govuk-radios-item value="northern-ireland">Northern Ireland</govuk-radios-item>
</govuk-radios>
```


### Example with conditional reveal
<img alt="Radios with conditional reveal example" src="../images/radios-with-conditional-example.png" />

```razor
<govuk-radios for="HowContacted">
    <govuk-radios-fieldset-legend is-page-heading="true" class="govuk-fieldset__legend--l">
        How would you prefer to be contacted?
    </govuk-radios-fieldset-legend>

    <govuk-radios-hint>
        Select one option.
    </govuk-radios-hint>

    <govuk-radios-item value="email">
        Email
        <govuk-radios-item-conditional>
            <govuk-input id="contact-by-email" name="contact-by-email" type="email" autocomplete="email" spellcheck="false" class="govuk-!-width-one-half">
                <govuk-input-label>Email address</govuk-input-label>
                <govuk-input-error-message>Email address cannot be blank</govuk-input-error-message>
            </govuk-input>
        </govuk-radios-item-conditional>
    </govuk-radios-item>

    <govuk-radios-item value="phone">
        Phone
        <govuk-radios-item-conditional>
            <govuk-input for="PhoneNumber" type="tel" autocomplete="tel" class="govuk-!-width-one-third">
                <govuk-input-label>Phone number</govuk-input-label>
            </govuk-input>
        </govuk-radios-item-conditional>
    </govuk-radios-item>

    <govuk-radios-item value="text">
        Text message
        <govuk-radios-item-conditional>
            <govuk-input for="MobilePhoneNumber" type="tel" autocomplete="tel" class="govuk-!-width-one-third">
                <govuk-input-label>Mobile phone number</govuk-input-label>
            </govuk-input>
        </govuk-radios-item-conditional>
    </govuk-radios-item>
</govuk-radios>
```


### Example with error message
<img alt="Radios with error message example" src="../images/radios-with-error-example.png" />

```razor
<govuk-radios name="where-do-you-live">
    <govuk-radios-fieldset-legend is-page-heading="true" class="govuk-fieldset__legend--l">
        Where do you live?
    </govuk-radios-fieldset-legend>

    <govuk-radios-hint>
        Select one option.
    </govuk-radios-hint>

    <govuk-radios-error-message>
        Select the country where you live
    </govuk-radios-error-message>

    <govuk-radios-item value="england">England</govuk-radios-item>
    <govuk-radios-item value="scotland">Scotland</govuk-radios-item>
    <govuk-radios-item value="wales">Wales</govuk-radios-item>
    <govuk-radios-item value="northern-ireland">Northern Ireland</govuk-radios-item>
</govuk-radios>
```


### API

#### `<govuk-radios>`

| Attribute | Type | Description |
| --- | --- | --- |
| `fieldset` | `bool` | Whether a `fieldset` should be generated to wrap the component. A `fieldset` is generated automatically when a `govuk-radios-fieldset` element or a `govuk-radios-fieldset-legend` element is used, or when any `fieldset-*`, `legend-*` or `legend-is-page-heading` attribute is specified; this attribute is only required when a `fieldset` is wanted but none of those are used.  The legend's content is deduced from the `For` expression's metadata. |
| `fieldset-*` |  | Additional attributes for the generated `fieldset` element. |
| `for` | `Microsoft.AspNetCore.Mvc.ViewFeatures.ModelExpression` | An expression to be evaluated against the current model. |
| `id-prefix` | `string` | The prefix to use when generating IDs for the hint, error message and items. Required unless `For` or `Name` is specified. |
| `ignore-modelstate-errors` | `bool?` | Whether the `Errors` for the `For` expression should be used to deduce an error message. |
| `legend-*` |  | Additional attributes for the generated `fieldset`'s `legend` element. These are combined with any attributes specified on a `govuk-radios-fieldset-legend` element; where both specify the same attribute the one on the element wins, except for `class`, where the two values are combined. |
| `legend-is-page-heading` | `bool?` | Whether the generated `fieldset`'s `legend` also acts as the heading for the page. An `is-page-heading` attribute on a `govuk-radios-fieldset-legend` element takes precedence over this. |
| `name` | `string` | The `name` attribute for the generated `input` elements. Required unless `For` or `IdPrefix` is specified. |
| `radios-*` |  | Additional attributes for the container element that wraps the items. |


#### `<govuk-radios-fieldset>`

A container element used when the radios should be contained within a fieldset element. When used, every hint, error message, item and divider must be placed inside this element rather than the root radios element.

Must be inside a `<govuk-radios>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `described-by` | `string` | One or more element IDs to add to the `aria-describedby` attribute. |


#### `<govuk-radios-fieldset-legend>`

The content is the HTML to use within the legend. When this element is specified directly inside the root radios element a fieldset is generated automatically.

Must be inside a `<govuk-radios>` or `<govuk-radios-fieldset>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `is-page-heading` | `bool?` | Whether the legend also acts as the heading for the page. The default is `false`. |


#### `<govuk-radios-hint>`

The content is the HTML to use within the component's hint.

Must be inside a `<govuk-radios>` or `<govuk-radios-fieldset>` element.


#### `<govuk-radios-error-message>`

The content is the HTML to use within the component's error message.

Must be inside a `<govuk-radios>` or `<govuk-radios-fieldset>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `visually-hidden-text` | `string` | A visually hidden prefix used before the error message. The default is `"Error"`. |


#### `<govuk-radios-before-inputs>`

The content is the HTML to use before the radios.

Must be inside a `<govuk-radios>` or `<govuk-radios-fieldset>` element.


#### `<govuk-radios-item>`

The content is the HTML to use within the label for the generated input element.

Must be inside a `<govuk-radios>` or `<govuk-radios-fieldset>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `checked` | `bool?` | Whether the item should be checked. If `null` and `For` is not `null` on the parent `RadiosTagHelper` then the value will be computed by comparing the specified model expression with `Value`. The default is `false`. |
| `disabled` | `bool?` | Whether the `disabled` attribute should be added to the generated `input` element. The default is `false`. |
| `id` | `string` | The `id` attribute for the generated `input` element. If not specified then a value is generated from the `name` attribute. |
| `input-*` |  | Additional attributes to add to the generated `input` element. |
| `label-*` |  | Additional attributes to add to the generated `label` element. |
| `value` | `string` | The `value` attribute for the item. |


#### `<govuk-radios-item-hint>`

The content is the HTML to use within the item's hint.

Must be inside a `<govuk-radios-item>` element.


#### `<govuk-radios-item-conditional>`

The content is the HTML to use within the conditional reveal for the item.

Must be inside a `<govuk-radios-item>` element.


#### `<govuk-radios-divider>`

The content is the HTML to use within the item divider.

Must be inside a `<govuk-radios>` or `<govuk-radios-fieldset>` element.


#### `<govuk-radios-after-inputs>`

The content is the HTML to use after the radios.

Must be inside a `<govuk-radios>` or `<govuk-radios-fieldset>` element.

