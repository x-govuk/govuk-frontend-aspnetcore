<!-- Generated from src/GovUk.Frontend.AspNetCore.Docs/Templates/components/service-navigation.liquid -->
# Service navigation

[GOV.UK Design System service navigation component](https://design-system.service.gov.uk/components/service-navigation/)


## Tag helpers

### Example
<img alt="Service navigation example" src="../images/service-navigation-example.png" />

```razor
<govuk-service-navigation service-name="Service name" service-url="#">
    <nav>
        <nav-item href="#">Navigation item 1</nav-item>
        <nav-item href="#" active="true">Navigation item 2</nav-item>
        <nav-item href="#">Navigation item 3</nav-item>
    </nav>
</govuk-service-navigation>
```


### Example with long tag names
<img alt="Service navigation with long tag names example" src="../images/service-navigation-with-long-tag-names-example.png" />

```razor
<govuk-service-navigation service-name="Service name" service-url="#">
    <govuk-service-navigation-start>
        <span class="govuk-body">Before the navigation</span>
    </govuk-service-navigation-start>
    <govuk-service-navigation-nav aria-label="Menu" menu-button-text="Menu">
        <govuk-service-navigation-nav-start>
            <li class="govuk-service-navigation__item">First navigation item</li>
        </govuk-service-navigation-nav-start>
        <govuk-service-navigation-nav-item href="#">Navigation item 1</govuk-service-navigation-nav-item>
        <govuk-service-navigation-nav-item href="#" active="true">Navigation item 2</govuk-service-navigation-nav-item>
        <govuk-service-navigation-nav-item asp-controller="Home" asp-action="Index">Navigation item 3</govuk-service-navigation-nav-item>
        <govuk-service-navigation-nav-end>
            <li class="govuk-service-navigation__item">Last navigation item</li>
        </govuk-service-navigation-nav-end>
    </govuk-service-navigation-nav>
    <govuk-service-navigation-end>
        <span class="govuk-body">After the navigation</span>
    </govuk-service-navigation-end>
</govuk-service-navigation>
```


### API

#### `<govuk-service-navigation>`

| Attribute | Type | Description |
| --- | --- | --- |
| `service-name` | `string` | The name of your service. |
| `service-url` | `string` | The homepage of your service. |


#### `<start>` / `<govuk-service-navigation-start>`

The content is the HTML at the start of the service header container.

Must be inside a `<govuk-service-navigation>` element.


#### `<nav>` / `<govuk-service-navigation-nav>`

Must be inside a `<govuk-service-navigation>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `aria-label` | `string` | The text for the `aria-label` which labels the service navigation container when a service name is included. If not specified, "Service information" will be used. |
| `collapse-navigation-on-mobile` | `bool?` | Whether the navigation should be collapsed inside a menu on mobile. If not specified, the navigation will be collapsed on mobile if there is more than one navigation item. |
| `id` | `string` | The ID used to associate the mobile navigation toggle with the navigation menu. If not specified, `navigation` will be used. |
| `label` | `string` | The screen reader label for the mobile navigation menu. If not specified, the value of the `menu-button-text` attribute will be used. |
| `menu-button-label` | `string` | The screen reader label for the mobile navigation menu toggle. If not specified, the value of the `menu-button-text` attribute will be used. |
| `menu-button-text` | `string` | The text of the mobile navigation menu toggle. |


#### `<start>` / `<govuk-service-navigation-nav-start>`

The content is the HTML before the first list item in the navigation list.

Must be inside a `<nav>` or `<govuk-service-navigation-nav>` element.


#### `<nav-item>` / `<govuk-service-navigation-nav-item>`

The content is the HTML to use within the generated service navigation item.

Must be inside a `<nav>` or `<govuk-service-navigation-nav>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `active` | `bool?` | Whether the user is within this group of pages in the navigation hierarchy. |
| `current` | `bool?` | Whether the user is currently on this page. This takes precedence over the `active` attribute. By default, this is determined by comparing the current URL to this item's generated `href` attribute. |
| `link-*` |  | Additional attributes to add to the generated `<a>` element. |
| (link attributes) |  | See [documentation on links](../links.md) for more information. |


#### `<end>` / `<govuk-service-navigation-nav-end>`

The content is the HTML after the last list item in the navigation list.

Must be inside a `<nav>` or `<govuk-service-navigation-nav>` element.


#### `<end>` / `<govuk-service-navigation-end>`

The content is the HTML at the end of the service header container.

Must be inside a `<govuk-service-navigation>` element.

