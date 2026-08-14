<!-- Generated from src/GovUk.Frontend.AspNetCore.Docs/Templates/components/details.liquid -->
# Details

[GOV.UK Design System details component](https://design-system.service.gov.uk/components/details/)


## Tag helpers

### Example
<img alt="Details example" src="../images/details-example.png" />

```razor
<govuk-details>
    <summary>
        Help with nationality
    </summary>
    <text>
        We need to know your nationality so we can work out which elections you’re entitled to vote in.
        If you cannot provide your nationality, you’ll have to send copies of identity documents through the post.
    </text>
</govuk-details>
```


### Example expanded
<img alt="Details expanded example" src="../images/details-expanded-example.png" />

```razor
<govuk-details open="true">
    <summary>
        Help with nationality
    </summary>
    <text>
        We need to know your nationality so we can work out which elections you’re entitled to vote in.
        If you cannot provide your nationality, you’ll have to send copies of identity documents through the post.
    </text>
</govuk-details>
```


### API

#### `<govuk-details>`

| Attribute | Type | Description |
| --- | --- | --- |
| `open` | `bool?` | Whether the details element should be expanded. The default is `false`. |


#### `<summary>` / `<govuk-details-summary>`

The content is the HTML to use within the details summary.

Must be inside a `<govuk-details>` element.


#### `<text>` / `<govuk-details-text>`

The content is the HTML to use within the disclosed part of the details element.

Must be inside a `<govuk-details>` element.

