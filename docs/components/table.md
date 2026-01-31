# Table

[GDS Table component](https://design-system.service.gov.uk/components/table/)

## Example - with head

```razor
<govuk-table>
    <govuk-table-head>
        <govuk-table-head-cell>Month you apply</govuk-table-head-cell>
        <govuk-table-head-cell format="numeric">Rate for bicycles</govuk-table-head-cell>
        <govuk-table-head-cell format="numeric">Rate for vehicles</govuk-table-head-cell>
    </govuk-table-head>
    <govuk-table-row>
        <govuk-table-cell>January</govuk-table-cell>
        <govuk-table-cell format="numeric">£85</govuk-table-cell>
        <govuk-table-cell format="numeric">£95</govuk-table-cell>
    </govuk-table-row>
    <govuk-table-row>
        <govuk-table-cell>February</govuk-table-cell>
        <govuk-table-cell format="numeric">£75</govuk-table-cell>
        <govuk-table-cell format="numeric">£55</govuk-table-cell>
    </govuk-table-row>
    <govuk-table-row>
        <govuk-table-cell>March</govuk-table-cell>
        <govuk-table-cell format="numeric">£165</govuk-table-cell>
        <govuk-table-cell format="numeric">£125</govuk-table-cell>
    </govuk-table-row>
</govuk-table>
```

## Example - with head and caption

```razor
<govuk-table>
    <govuk-table-caption class="govuk-table__caption--m">Dates and amounts</govuk-table-caption>
    <govuk-table-head>
        <govuk-table-head-cell>Date</govuk-table-head-cell>
        <govuk-table-head-cell format="numeric">Amount</govuk-table-head-cell>
    </govuk-table-head>
    <govuk-table-body>
        <govuk-table-row>
            <govuk-table-cell>First 6 weeks</govuk-table-cell>
            <govuk-table-cell format="numeric">£109.80 per week</govuk-table-cell>
        </govuk-table-row>
        <govuk-table-row>
            <govuk-table-cell>Next 33 weeks</govuk-table-cell>
            <govuk-table-cell format="numeric">£109.80 per week</govuk-table-cell>
        </govuk-table-row>
        <govuk-table-row>
            <govuk-table-cell>Total estimated pay</govuk-table-cell>
            <govuk-table-cell format="numeric">£4,282.20</govuk-table-cell>
        </govuk-table-row>
    </govuk-table-body>
</govuk-table>
```

## Example - with first cell as header

When using `first-cell-is-header="true"`, the first cell in each body row will be rendered as a `<th>` element with `scope="row"` instead of a `<td>` element.

```razor
<govuk-table first-cell-is-header="true">
    <govuk-table-head>
        <govuk-table-head-cell>Month you apply</govuk-table-head-cell>
        <govuk-table-head-cell format="numeric">Rate for bicycles</govuk-table-head-cell>
        <govuk-table-head-cell format="numeric">Rate for vehicles</govuk-table-head-cell>
    </govuk-table-head>
    <govuk-table-row>
        <govuk-table-cell>January</govuk-table-cell>
        <govuk-table-cell format="numeric">£85</govuk-table-cell>
        <govuk-table-cell format="numeric">£95</govuk-table-cell>
    </govuk-table-row>
    <govuk-table-row>
        <govuk-table-cell>February</govuk-table-cell>
        <govuk-table-cell format="numeric">£75</govuk-table-cell>
        <govuk-table-cell format="numeric">£55</govuk-table-cell>
    </govuk-table-row>
</govuk-table>
```

## Table options

The `<govuk-table>` element supports the following attributes:

| Attribute | Type | Description |
|---|---|---|
| `first-cell-is-header` | `bool` | If `true`, the first cell in each body row will be a table header (`<th>` with `scope="row"`) |

## Table caption options

The `<govuk-table-caption>` element can be used to add a caption to the table. It supports the `class` attribute to add custom CSS classes.

## Table head cell options

The `<govuk-table-head-cell>` element supports the following attributes:

| Attribute | Type | Description |
|---|---|---|
| `format` | `string` | Format of the cell. Set to `"numeric"` to right-align the cell content |
| `colspan` | `int` | Number of columns the cell should span |
| `rowspan` | `int` | Number of rows the cell should span |

## Table cell options

The `<govuk-table-cell>` element supports the following attributes:

| Attribute | Type | Description |
|---|---|---|
| `format` | `string` | Format of the cell. Set to `"numeric"` to right-align the cell content |
| `colspan` | `int` | Number of columns the cell should span |
| `rowspan` | `int` | Number of rows the cell should span |

## Structure

A table can contain the following child elements:

- `<govuk-table-caption>` (optional) - The table caption
- `<govuk-table-head>` (optional) - The table head section containing header cells
  - Contains `<govuk-table-head-cell>` elements
- `<govuk-table-body>` (optional) - The table body section containing rows
  - Contains `<govuk-table-row>` elements
- `<govuk-table-row>` - A table row (can be direct child of `<govuk-table>` or within `<govuk-table-body>`)
  - Contains `<govuk-table-cell>` elements
