<!-- Generated from src/GovUk.Frontend.AspNetCore.Docs/Templates/components/radios.liquid -->
# Radios

[GOV.UK Design System radios component](https://design-system.service.gov.uk/components/radios/)


## Tag helpers

### Example
<img alt="Radios example" src="../images/radios-example.png" />

```razor
<govuk-radios for="WhereDoYouLive">
    <legend is-page-heading="true" class="govuk-fieldset__legend--l">
        Where do you live?
    </legend>

    <hint>
        Select one option.
    </hint>

    <radios-item value="england">England</radios-item>
    <radios-item value="scotland">Scotland</radios-item>
    <radios-item value="wales">Wales</radios-item>
    <radios-item value="northern-ireland">Northern Ireland</radios-item>
    <divider>or</divider>
    <radios-item value="abroad">I am a British citizen living abroad</radios-item>
</govuk-radios>
```


### Example with a fieldset generated from the model metadata
<img alt="Radios with a generated fieldset example" src="../images/radios-with-generated-fieldset-example.png" />

```razor
<govuk-radios for="WhereDoYouLive" fieldset legend-class="govuk-fieldset__legend--l" legend-is-page-heading="true">
    <radios-item value="england">England</radios-item>
    <radios-item value="scotland">Scotland</radios-item>
    <radios-item value="wales">Wales</radios-item>
    <radios-item value="northern-ireland">Northern Ireland</radios-item>
</govuk-radios>
```


### Example with conditional reveal
<img alt="Radios with conditional reveal example" src="../images/radios-with-conditional-example.png" />

```razor
<govuk-radios for="HowContacted">
    <legend is-page-heading="true" class="govuk-fieldset__legend--l">
        How would you prefer to be contacted?
    </legend>

    <hint>
        Select one option.
    </hint>

    <radios-item value="email">
        Email
        <conditional>
            <govuk-input id="contact-by-email" name="contact-by-email" type="email" autocomplete="email" spellcheck="false" class="govuk-!-width-one-half">
                <label>Email address</label>
                <error-message>Email address cannot be blank</error-message>
            </govuk-input>
        </conditional>
    </radios-item>

    <radios-item value="phone">
        Phone
        <conditional>
            <govuk-input for="PhoneNumber" type="tel" autocomplete="tel" class="govuk-!-width-one-third">
                <label>Phone number</label>
            </govuk-input>
        </conditional>
    </radios-item>

    <radios-item value="text">
        Text message
        <conditional>
            <govuk-input for="MobilePhoneNumber" type="tel" autocomplete="tel" class="govuk-!-width-one-third">
                <label>Mobile phone number</label>
            </govuk-input>
        </conditional>
    </radios-item>
</govuk-radios>
```


### Example with error message
<img alt="Radios with error message example" src="../images/radios-with-error-example.png" />

```razor
<govuk-radios name="where-do-you-live">
    <legend is-page-heading="true" class="govuk-fieldset__legend--l">
        Where do you live?
    </legend>

    <hint>
        Select one option.
    </hint>

    <error-message>
        Select the country where you live
    </error-message>

    <radios-item value="england">England</radios-item>
    <radios-item value="scotland">Scotland</radios-item>
    <radios-item value="wales">Wales</radios-item>
    <radios-item value="northern-ireland">Northern Ireland</radios-item>
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

A container element used when the radios should be contained within a fieldset element. When used, every hint, error message, item and divider must be placed inside this element rather than the root radios element, and each must use its govuk- prefixed name; the short names are only available directly inside the root radios element.

Must be inside a `<govuk-radios>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `described-by` | `string` | One or more element IDs to add to the `aria-describedby` attribute. |


#### `<legend>` / `<govuk-radios-fieldset-legend>`

The content is the HTML to use within the legend. When this element is specified directly inside the root radios element a fieldset is generated automatically.

Must be inside a `<govuk-radios>` or `<govuk-radios-fieldset>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `is-page-heading` | `bool?` | Whether the legend also acts as the heading for the page. The default is `false`. |


#### `<hint>` / `<govuk-radios-hint>`

The content is the HTML to use within the component's hint.

Must be inside a `<govuk-radios>` or `<govuk-radios-fieldset>` element.


#### `<error-message>` / `<govuk-radios-error-message>`

The content is the HTML to use within the component's error message.

Must be inside a `<govuk-radios>` or `<govuk-radios-fieldset>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `visually-hidden-text` | `string` | A visually hidden prefix used before the error message. The default is `"Error"`. |


#### `<before-inputs>` / `<govuk-radios-before-inputs>`

The content is the HTML to use before the radios.

Must be inside a `<govuk-radios>` or `<govuk-radios-fieldset>` element.


#### `<radios-item>` / `<govuk-radios-item>`

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


#### `<hint>` / `<govuk-radios-item-hint>`

The content is the HTML to use within the item's hint.

Must be inside a `<radios-item>` or `<govuk-radios-item>` element.


#### `<conditional>` / `<govuk-radios-item-conditional>`

The content is the HTML to use within the conditional reveal for the item.

Must be inside a `<radios-item>` or `<govuk-radios-item>` element.


#### `<divider>` / `<govuk-radios-divider>`

The content is the HTML to use within the item divider.

Must be inside a `<govuk-radios>` or `<govuk-radios-fieldset>` element.


#### `<after-inputs>` / `<govuk-radios-after-inputs>`

The content is the HTML to use after the radios.

Must be inside a `<govuk-radios>` or `<govuk-radios-fieldset>` element.

