<!-- Generated from src/GovUk.Frontend.AspNetCore.Docs/Templates/components/feedback.liquid -->
# Feedback

[GOV.UK Design System feedback component](https://design-system.service.gov.uk/components/feedback/)


## Tag helpers

### Example
<img alt="Feedback example" src="../images/feedback-example.png" />

```razor
<govuk-feedback>
    <feedback-title>Help us improve this service</feedback-title>
    <feedback-body>
        <p class="govuk-body">
            Tell us about your experience using this service.
            <a href="#" class="govuk-link">Give us your feedback</a>
        </p>
    </feedback-body>
</govuk-feedback>
```


### Example with long tag names
<img alt="Feedback with long tag names example" src="../images/feedback-with-long-tag-names-example.png" />

```razor
<govuk-feedback>
    <govuk-feedback-title>Help us improve this service</govuk-feedback-title>
    <govuk-feedback-body>
        <p class="govuk-body">
            Tell us about your experience using this service.
            <a href="#" class="govuk-link">Give us your feedback</a>
        </p>
    </govuk-feedback-body>
</govuk-feedback>
```


### API

#### `<govuk-feedback>`

| Attribute | Type | Description |
| --- | --- | --- |
| `heading-level` | `int?` | The heading level of the title. Must be between `1` and `6` (inclusive). The default is `2`. |


#### `<feedback-title>` / `<govuk-feedback-title>`

The content is the HTML to use within the feedback title.

Must be inside a `<govuk-feedback>` element.


#### `<feedback-body>` / `<govuk-feedback-body>`

The content is the HTML to use within the feedback body.

Must be inside a `<govuk-feedback>` element.

