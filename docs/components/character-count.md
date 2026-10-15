<!-- Generated from src/GovUk.Frontend.AspNetCore.Docs/Templates/components/character-count.liquid -->
# Character count

[GOV.UK Design System character count component](https://design-system.service.gov.uk/components/character-count/)

Check out the [character count validators](../validation/character-count.md) for adding server-side validation that counts the text the same way as the component.

By default the limit is a number of characters, counted as JavaScript's `String.length` does. Set `count-type` to `CharacterCountType.Characters` to count the characters a user would perceive instead, so an emoji or an accented letter counts as one, or to `CharacterCountType.Words` to make `max-length` a number of words:

```razor
<govuk-character-count for="MoreDetail" max-length="150" count-type="CharacterCountType.Words">
    <label>Can you provide more detail?</label>
</govuk-character-count>
```

`CharacterCountType.Characters` and `CharacterCountType.Words` need a browser that supports `Intl.Segmenter`. In other browsers the count isn't shown, only the description of the limit.

The `max-words` attribute is deprecated; use `max-length` with `count-type="CharacterCountType.Words"` instead.


## Tag helpers

### Example
<img alt="Character count example" src="../images/character-count-example.png" />

```razor
<govuk-character-count for="MoreDetail" max-length="200">
    <label class="govuk-label--l" is-page-heading="true">
        Can you provide more detail?
    </label>
    <hint>
        Do not include personal or financial information like your National Insurance number or credit card details.
    </hint>
</govuk-character-count>
```


### Example with long tag names
<img alt="Character count with long tag names example" src="../images/character-count-with-long-tag-names-example.png" />

```razor
<govuk-character-count name="MoreDetail" max-length="200">
    <govuk-character-count-label is-page-heading="true" class="govuk-label--l">Can you provide more detail?</govuk-character-count-label>
    <govuk-character-count-hint>
        Do not include personal or financial information like your National Insurance number or credit card details.
    </govuk-character-count-hint>
    <govuk-character-count-error-message>Enter more detail</govuk-character-count-error-message>
    <govuk-character-count-before-input>
        <p class="govuk-body">Content before the textarea</p>
    </govuk-character-count-before-input>
    <govuk-character-count-after-input>
        <p class="govuk-body">Content after the textarea</p>
    </govuk-character-count-after-input>
    <govuk-character-count-value>The existing content of the textarea</govuk-character-count-value>
</govuk-character-count>
```


### API

#### `<govuk-character-count>`

| Attribute | Type | Description |
| --- | --- | --- |
| `autocomplete` | `string` | The `autocomplete` attribute for the generated `textarea` element. |
| `count-message-*` |  | Additional attributes to add to the generated count message hint element. |
| `count-type` | `GovUk.Frontend.AspNetCore.CharacterCountType?` | How the text entered is counted. The default is `Length`, or `Words` when `MaxWords` is specified. `Characters` and `Words` need a browser that supports `Intl.Segmenter`; in other browsers, the textarea description is shown and the count is not. |
| `disabled` | `bool?` | Whether the `disabled` attribute should be added to the generated `textarea` element. |
| `for` | `Microsoft.AspNetCore.Mvc.ViewFeatures.ModelExpression` | An expression to be evaluated against the current model. |
| `form-group-*` |  | Additional attributes to add to the generated form-group wrapper element. |
| `id` | `string` | The `id` attribute for the generated `textarea` element. If not specified then a value is generated from the `name` attribute. |
| `ignore-modelstate-errors` | `bool?` | Whether the `Errors` for the `For` expression should be used to deduce an error message. When there are multiple errors in the `ModelErrorCollection` the first is used. |
| `label-class` | `string` | Additional classes for the generated `label` element. |
| `max-length` | `int?` | The maximum number of characters the generated `textarea` may contain, or the maximum number of words when `CountType` is `Words`. Required unless `MaxWords` is specified. |
| `max-words` | `int?` | The maximum number of words the generated `textarea` may contain. Required unless `MaxLength` is specified. This is deprecated; specify `MaxLength` with a `CountType` of `Words` instead. The two count words differently: `max-words` counts runs of characters separated by whitespace, whereas `Words` uses `Intl.Segmenter`, so punctuation on its own isn't counted as a word, and a hyphenated word counts as two. |
| `name` | `string` | The `name` attribute for the generated `textarea` element. Required unless `For` is specified. |
| `readonly` | `bool?` | Whether the `readonly` attribute should be added to the generated `textarea` element. |
| `rows` | `int?` | The `rows` attribute for the generated `textarea` element. The default is `5`. |
| `spellcheck` | `bool?` | The `spellcheck` attribute for the generated `textarea` element. |
| `textarea-*` |  | Additional attributes to add to the generated `textarea` element. |
| `threshold` | `decimal?` | The percentage value of the limit at which point the count message is displayed. If this is specified the count message will be hidden by default. |


#### `<label>` / `<govuk-character-count-label>`

The content is the HTML to use within the component's label.

Must be inside a `<govuk-character-count>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `is-page-heading` | `bool?` | Whether the label also acts as the heading for the page. |


#### `<hint>` / `<govuk-character-count-hint>`

The content is the HTML to use within the component's hint.

Must be inside a `<govuk-character-count>` element.


#### `<error-message>` / `<govuk-character-count-error-message>`

The content is the HTML to use within the component's error message.

Must be inside a `<govuk-character-count>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `visually-hidden-text` | `string` | A visually hidden prefix used before the error message. The default is `"Error"`. |


#### `<before-input>` / `<govuk-character-count-before-input>`

The content is the HTML to use before the generated textarea element.

Must be inside a `<govuk-character-count>` element.


#### `<value>` / `<govuk-character-count-value>`

The content is the HTML to use within the generated textarea.

Must be inside a `<govuk-character-count>` element.


#### `<after-input>` / `<govuk-character-count-after-input>`

The content is the HTML to use after the generated textarea element.

Must be inside a `<govuk-character-count>` element.

