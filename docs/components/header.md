# GOV.UK header

[GOV.UK Design System GOV.UK header component](https://design-system.service.gov.uk/components/header/)


## Tag helpers

> [!NOTE]
> The GOV.UK header variants that includes a service name and navigation are deprecated and are not supported by this tag helpers.

### Example

<img alt="Header example" src="../images/header-example.png" />

```razor
<govuk-header home-page-url="https://my.service.gov.uk" product-name="Product" />
```


### API

#### `<govuk-header>`

> [!NOTE]
> This tag helper should not have any child content.

| Attribute | Type | Description |
| --- | --- | --- |
| `container-*` |  | Additional attributes to add to the generated container element. |
| `home-page-url` | `string` | The URL of the homepage. |
| `product-name` | `string` | Product name, used when the product name follows on directly from "GOV.UK". For example, GOV.UK Pay or GOV.UK Design System. |

