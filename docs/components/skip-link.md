<!-- Generated from src/GovUk.Frontend.AspNetCore.Docs/Templates/components/skip-link.liquid -->
# Skip link

[GOV.UK Design System skip link component](https://design-system.service.gov.uk/components/skip-link/)


## Tag helpers

### Example with default href
<img alt="Skip link with default href example" src="../images/skip-link-with-default-href-example.png" />

```razor
<govuk-skip-link>Skip to main content</govuk-skip-link>
```


### Example with custom href
<img alt="Skip link with custom href example" src="../images/skip-link-with-custom-href-example.png" />

```razor
<govuk-skip-link href="#main">Skip to main content</govuk-skip-link>
```


### API

#### `<govuk-skip-link>`

The content is the HTML to use within the skip link.

| Attribute | Type | Description |
| --- | --- | --- |
| `href` | `string` | The `href` attribute for the link. The default is `"#content"`. |

