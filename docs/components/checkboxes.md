<!-- Generated from src/GovUk.Frontend.AspNetCore.Docs/Templates/components/checkboxes.liquid -->
# Checkboxes

[GOV.UK Design System checkboxes component](https://design-system.service.gov.uk/components/checkboxes/)


## Tag helpers

### Example
<img alt="Checkboxes example" src="../images/checkboxes-example.png" />

```razor
<govuk-checkboxes for="Nationalities">
    <govuk-checkboxes-fieldset>
        <govuk-checkboxes-fieldset-legend is-page-heading="true" class="govuk-fieldset__legend--l">
            What is your nationality?
        </govuk-checkboxes-fieldset-legend>

        <govuk-checkboxes-hint>
            If you have dual nationality, select all options that are relevant to you.
        </govuk-checkboxes-hint>

        <govuk-checkboxes-item value="british">
            British
            <govuk-checkboxes-item-hint>including English, Scottish, Welsh and Northern Irish</govuk-checkboxes-item-hint>
        </govuk-checkboxes-item>
        <govuk-checkboxes-item value="irish">Irish</govuk-checkboxes-item>
        <govuk-checkboxes-item value="other">Citizen of another country</govuk-checkboxes-item>
    </govuk-checkboxes-fieldset>
</govuk-checkboxes>
```


### Example without fieldset
<img alt="Checkboxes without fieldset example" src="../images/checkboxes-without-fieldset-example.png" />

```razor
<govuk-checkboxes for="AcceptedTermsAndConditions">
    <govuk-checkboxes-item value="true">
        I agree to the terms and conditions
    </govuk-checkboxes-item>
</govuk-checkboxes>
```


### Example with conditional reveal
<img alt="Checkboxes with conditional reveal example" src="../images/checkboxes-with-conditional-example.png" />

```razor
<govuk-checkboxes for="ContactPreferences">
    <govuk-checkboxes-fieldset>
        <govuk-checkboxes-fieldset-legend is-page-heading="true" class="govuk-fieldset__legend--l">
            How would you like to be contacted?
        </govuk-checkboxes-fieldset-legend>

        <govuk-checkboxes-hint>
            Select all options that are relevant to you.
        </govuk-checkboxes-hint>

        <govuk-checkboxes-item value="email">
            Email
            <govuk-checkboxes-item-conditional>
                <govuk-input for="EmailAddress" type="email" autocomplete="email" spellcheck="false" input-class="govuk-!-width-one-third">
                    <govuk-input-label>Email address</govuk-input-label>
                </govuk-input>
            </govuk-checkboxes-item-conditional>
        </govuk-checkboxes-item>

        <govuk-checkboxes-item value="phone">
            Phone
            <govuk-checkboxes-item-conditional>
                <govuk-input for="PhoneNumber" type="tel" autocomplete="tel" input-class="govuk-!-width-one-third">
                    <govuk-input-label>Phone number</govuk-input-label>
                </govuk-input>
            </govuk-checkboxes-item-conditional>
        </govuk-checkboxes-item>

        <govuk-checkboxes-item value="text message">
            Text message
            <govuk-checkboxes-item-conditional>
                <govuk-input for="MobilePhoneNumber" type="tel" autocomplete="tel" input-class="govuk-!-width-one-third">
                    <govuk-input-label>Mobile phone number</govuk-input-label>
                </govuk-input>
            </govuk-checkboxes-item-conditional>
        </govuk-checkboxes-item>
    </govuk-checkboxes-fieldset>
</govuk-checkboxes>
```


### Example with 'none' option
<img alt="Checkboxes with &#x27;none&#x27; option example" src="../images/checkboxes-with-none-example.png" />

```razor
<govuk-checkboxes for="CountriesTravellingTo">
    <govuk-checkboxes-fieldset>
        <govuk-checkboxes-fieldset-legend is-page-heading="true" class="govuk-fieldset__legend--l">
            Will you be travelling to any of these countries?
        </govuk-checkboxes-fieldset-legend>

        <govuk-checkboxes-hint>
            Select all countries that apply
        </govuk-checkboxes-hint>

        <govuk-checkboxes-item value="france">France</govuk-checkboxes-item>
        <govuk-checkboxes-item value="portugal">Portugal</govuk-checkboxes-item>
        <govuk-checkboxes-item value="spain">Spain</govuk-checkboxes-item>
        <govuk-checkboxes-divider>or</govuk-checkboxes-divider>
        <govuk-checkboxes-item value="none" behavior="CheckboxesItemBehavior.Exclusive">No, I will not be travelling to any of these countries</govuk-checkboxes-item>
    </govuk-checkboxes-fieldset>
</govuk-checkboxes>
```


### Example with error message
<img alt="Checkboxes with error message example" src="../images/checkboxes-with-error-example.png" />

```razor
<govuk-checkboxes name="nationality">
    <govuk-checkboxes-fieldset>
        <govuk-checkboxes-fieldset-legend is-page-heading="true" class="govuk-fieldset__legend--l">
            What is your nationality?
        </govuk-checkboxes-fieldset-legend>

        <govuk-checkboxes-hint>
            If you have dual nationality, select all options that are relevant to you.
        </govuk-checkboxes-hint>

        <govuk-checkboxes-error-message>
            Select if you are British, Irish or a citizen of a different country
        </govuk-checkboxes-error-message>

        <govuk-checkboxes-item value="british">
            British
            <govuk-checkboxes-item-hint>including English, Scottish, Welsh and Northern Irish</govuk-checkboxes-item-hint>
        </govuk-checkboxes-item>
        <govuk-checkboxes-item value="irish">Irish</govuk-checkboxes-item>
        <govuk-checkboxes-item value="other">Citizen of another country</govuk-checkboxes-item>
    </govuk-checkboxes-fieldset>
</govuk-checkboxes>
```


### API

#### `<govuk-checkboxes>`

| Attribute | Type | Description |
| --- | --- | --- |
| `checkboxes-*` |  | Additional attributes for the container element that wraps the items. |
| `described-by` | `string` | One or more element IDs to add to the `aria-describedby` attribute of the generated elements. |
| `for` | `Microsoft.AspNetCore.Mvc.ViewFeatures.ModelExpression` | An expression to be evaluated against the current model. |
| `id-prefix` | `string` | The prefix to use when generating IDs for the hint, error message and items. Required unless `For` or `Name` is specified. |
| `ignore-modelstate-errors` | `bool?` | Whether the `Errors` for the `For` expression should be used to deduce an error message. |
| `name` | `string` | The `name` attribute for the generated `input` elements. Required unless `For` or `IdPrefix` is specified. |


#### `<govuk-checkboxes-fieldset>`

A container element used when the checkboxes should be contained within a fieldset element. When used, every hint, error message, item and divider must be placed inside this element rather than the root checkboxes element.

Must be inside a `<govuk-checkboxes>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `described-by` | `string` | One or more element IDs to add to the `aria-describedby` attribute. |


#### `<govuk-checkboxes-fieldset-legend>`

The content is the HTML to use within the legend.

Must be inside a `<govuk-checkboxes-fieldset>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `is-page-heading` | `bool?` | Whether the legend also acts as the heading for the page. The default is `false`. |


#### `<govuk-checkboxes-hint>`

The content is the HTML to use within the component's hint.

Must be inside a `<govuk-checkboxes>` or `<govuk-checkboxes-fieldset>` element.


#### `<govuk-checkboxes-error-message>`

The content is the HTML to use within the component's error message.

Must be inside a `<govuk-checkboxes>` or `<govuk-checkboxes-fieldset>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `visually-hidden-text` | `string` | A visually hidden prefix used before the error message. The default is `"Error"`. |


#### `<govuk-checkboxes-before-inputs>`

The content is the HTML to use before the checkboxes.

Must be inside a `<govuk-checkboxes>` or `<govuk-checkboxes-fieldset>` element.


#### `<govuk-checkboxes-item>`

The content is the HTML to use within the label for the generated input element.

Must be inside a `<govuk-checkboxes>` or `<govuk-checkboxes-fieldset>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `checked` | `bool?` | Whether the item should be checked. If `null` and `For` is not `null` on the parent `CheckboxesTagHelper` then the value will be computed by comparing the specified model expression with `Value`. The default is `false`. |
| `disabled` | `bool?` | Whether the `disabled` attribute should be added to the generated `input` element. The default is `false`. |
| `id` | `string` | The `id` attribute for the generated `input` element. If not specified then a value is generated from the `name` attribute. |
| `input-*` |  | Additional attributes to add to the generated `input` element. |
| `label-*` |  | Additional attributes to add to the generated `label` element. |
| `name` | `string` | The `name` attribute for the generated `input` element. Required unless `For` or `Name` is specified on the parent `CheckboxesTagHelper`. |
| `value` | `string` | The `value` attribute for the item. |


#### `<govuk-checkboxes-item-hint>`

The content is the HTML to use within the item's hint.

Must be inside a `<govuk-checkboxes-item>` element.


#### `<govuk-checkboxes-item-conditional>`

The content is the HTML to use within the conditional reveal for the item.

Must be inside a `<govuk-checkboxes-item>` element.


#### `<govuk-checkboxes-divider>`

The content is the HTML to use within the item divider.

Must be inside a `<govuk-checkboxes>` or `<govuk-checkboxes-fieldset>` element.


#### `<govuk-checkboxes-after-inputs>`

The content is the HTML to use after the checkboxes.

Must be inside a `<govuk-checkboxes>` or `<govuk-checkboxes-fieldset>` element.

