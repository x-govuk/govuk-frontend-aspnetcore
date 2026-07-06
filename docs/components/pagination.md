<!-- Generated from src/GovUk.Frontend.AspNetCore.Docs/Templates/components/pagination.liquid -->
# Pagination

[GOV.UK Design System pagination component](https://design-system.service.gov.uk/components/pagination/)


## Tag helpers

### Example
<img alt="Pagination example" src="../images/pagination-example.png" />

```razor
<govuk-pagination>
    <govuk-pagination-previous href="#" />
    <govuk-pagination-item href="#">1</govuk-pagination-item>
    <govuk-pagination-item href="#" current="true">2</govuk-pagination-item>
    <govuk-pagination-item href="#">3</govuk-pagination-item>
    <govuk-pagination-next href="#" />
</govuk-pagination>
```


### Example stacked
<img alt="Pagination stacked example" src="../images/pagination-stacked-example.png" />

```razor
<govuk-pagination>
    <govuk-pagination-previous href="#" label-text="Applying for a provisional lorry or bus licence" />
    <govuk-pagination-next href="#" label-text="Driver CPC part 1 test: theory" />
</govuk-pagination>
```


### Example with ellipsis
<img alt="Pagination with ellipsis example" src="../images/pagination-with-ellipsis-example.png" />

```razor
<govuk-pagination>
    <govuk-pagination-previous href="#" />
    <govuk-pagination-item href="#">1</govuk-pagination-item>
    <govuk-pagination-ellipsis />
    <govuk-pagination-item href="#">6</govuk-pagination-item>
    <govuk-pagination-item href="#" current="true">7</govuk-pagination-item>
    <govuk-pagination-item href="#">8</govuk-pagination-item>
    <govuk-pagination-ellipsis />
    <govuk-pagination-item href="#">42</govuk-pagination-item>
    <govuk-pagination-next href="#" />
</govuk-pagination>
```


### API

#### `<govuk-pagination>`

| Attribute | Type | Description |
| --- | --- | --- |
| `landmark-label` | `string` | The label for the navigation landmark that wraps the pagination. The default is `results`. Cannot be `null` or empty. |


#### `<govuk-pagination-previous>`

The content is the text for the link to the previous page. The default is 'Previous page'.

Must be inside a `<govuk-pagination>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `label-text` | `string` | The optional label that goes underneath the link to the previous page, providing further context for the user about where the link goes. |
| `link-*` |  | Additional attributes to add to the generated `a` element. |
| (link attributes) |  | See [documentation on links](../links.md) for more information. |


#### `<govuk-pagination-item>`

The content is the pagination item text, usually a page number.

Must be inside a `<govuk-pagination>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `current` | `bool?` | Whether this item is the current page the user is on. By default, this is determined by comparing the current URL to this item's generated `href` attribute. |
| `visually-hidden-text` | `string` | The visually hidden text for the pagination item. This should include the page number. The default is `Page <number>`. |
| (link attributes) |  | See [documentation on links](../links.md) for more information. |


#### `<govuk-pagination-ellipsis>`

> [!NOTE]
> This tag helper should not have any child content.

Must be inside a `<govuk-pagination>` element.


#### `<govuk-pagination-next>`

The content is the text for the link to the next page. The default is 'Next page'.

Must be inside a `<govuk-pagination>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `label-text` | `string` | The optional label that goes underneath the link to the next page, providing further context for the user about where the link goes. |
| `link-*` |  | Additional attributes to add to the generated `a` element. |
| (link attributes) |  | See [documentation on links](../links.md) for more information. |

