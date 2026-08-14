<!-- Generated from src/GovUk.Frontend.AspNetCore.Docs/Templates/components/footer.liquid -->
# GOV.UK footer

[GOV.UK Design System GOV.UK footer component](https://design-system.service.gov.uk/components/footer/)


### Example without links
<img alt="Footer without links example" src="../images/footer-without-links-example.png" />

```razor
<govuk-footer>
    <content-licence />
    <copyright />
</govuk-footer>
```


### Example with links
<img alt="Footer with links example" src="../images/footer-with-links-example.png" />

```razor
<govuk-footer>
    <meta>
        <meta-items>
            <meta-item href="#">Item 1</meta-item>
            <meta-item href="#">Item 2</meta-item>
            <meta-item href="#">Item 3</meta-item>
        </meta-items>
    </meta>
    <content-licence />
    <copyright />
</govuk-footer>
```


### Example with secondary navigation
<img alt="Footer with secondary navigation example" src="../images/footer-with-secondary-navigation-example.png" />

```razor
<govuk-footer>
    <nav width="two-thirds" columns="2">
        <nav-title>Two column list</nav-title>
        <nav-items>
            <nav-item href="#">Navigation item 1</nav-item>
            <nav-item href="#">Navigation item 2</nav-item>
            <nav-item href="#">Navigation item 3</nav-item>
            <nav-item href="#">Navigation item 4</nav-item>
            <nav-item href="#">Navigation item 5</nav-item>
            <nav-item href="#">Navigation item 6</nav-item>
        </nav-items>
    </nav>
    <nav width="one-third">
        <nav-title>Single column list</nav-title>
        <nav-items>
            <nav-item href="#">Navigation item 1</nav-item>
            <nav-item href="#">Navigation item 2</nav-item>
            <nav-item href="#">Navigation item 3</nav-item>
        </nav-items>
    </nav>
    <content-licence />
    <copyright />
</govuk-footer>
```


### Example with links and secondary navigation
<img alt="Footer with links and secondary navigation example" src="../images/footer-with-links-and-secondary-navigation-example.png" />

```razor
<govuk-footer>
    <nav width="two-thirds" columns="2">
        <nav-title>Services and information</nav-title>
        <nav-items>
            <nav-item href="#">Benefits</nav-item>
            <nav-item href="#">Births, deaths, marriages and care</nav-item>
            <nav-item href="#">Business and self-employed</nav-item>
            <nav-item href="#">Childcare and parenting</nav-item>
            <nav-item href="#">Citizenship and living in the UK</nav-item>
            <nav-item href="#">Crime, justice and the law</nav-item>
            <nav-item href="#">Disabled people</nav-item>
            <nav-item href="#">Driving and transport</nav-item>
            <nav-item href="#">Education and learning</nav-item>
            <nav-item href="#">Employing people</nav-item>
            <nav-item href="#">Environment and countryside</nav-item>
            <nav-item href="#">Housing and local services</nav-item>
            <nav-item href="#">Money and tax</nav-item>
            <nav-item href="#">Passports, travel and living abroad</nav-item>
            <nav-item href="#">Visas and immigration</nav-item>
            <nav-item href="#">Working, jobs and pensions</nav-item>
        </nav-items>
    </nav>
    <nav width="one-third">
        <nav-title>Departments and policy</nav-title>
        <nav-items>
            <nav-item href="#">How government works</nav-item>
            <nav-item href="#">Departments</nav-item>
            <nav-item href="#">Worldwide</nav-item>
            <nav-item href="#">Policies</nav-item>
            <nav-item href="#">Publications</nav-item>
            <nav-item href="#">Announcements</nav-item>
        </nav-items>
    </nav>
    <meta>
        <meta-items>
            <meta-item href="#">Help</meta-item>
            <meta-item href="#">Cookies</meta-item>
            <meta-item href="#">Contact</meta-item>
            <meta-item href="#">Terms and conditions</meta-item>
            <meta-item href="#" lang="cy" hreflang="cy">Rhestr o Wasanaethau Cymraeg</meta-item>
        </meta-items>
        <content>Built by the <a href="#" class="govuk-footer__link">Government Digital Service</a></content>
    </meta>
    <content-licence />
    <copyright />
</govuk-footer>
```


### Example with no content licence or copyright
<img alt="Footer with no content licence or copyright example" src="../images/footer-with-no-content-licence-or-copyright-example.png" />

```razor
<govuk-footer />
```


### Example with long tag names
<img alt="Footer with long tag names example" src="../images/footer-with-long-tag-names-example.png" />

```razor
<govuk-footer>
    <govuk-footer-nav width="two-thirds" columns="2">
        <govuk-footer-nav-title>Services and information</govuk-footer-nav-title>
        <govuk-footer-nav-items>
            <govuk-footer-nav-item href="#">Benefits</govuk-footer-nav-item>
            <govuk-footer-nav-item href="#">Births, deaths, marriages and care</govuk-footer-nav-item>
            <govuk-footer-nav-item href="#">Business and self-employed</govuk-footer-nav-item>
            <govuk-footer-nav-item href="#">Childcare and parenting</govuk-footer-nav-item>
            <govuk-footer-nav-item href="#">Citizenship and living in the UK</govuk-footer-nav-item>
            <govuk-footer-nav-item href="#">Crime, justice and the law</govuk-footer-nav-item>
        </govuk-footer-nav-items>
    </govuk-footer-nav>
    <govuk-footer-nav width="one-third">
        <govuk-footer-nav-title>Departments and policy</govuk-footer-nav-title>
        <govuk-footer-nav-items>
            <govuk-footer-nav-item asp-controller="Home" asp-action="Index">How government works</govuk-footer-nav-item>
            <govuk-footer-nav-item href="#">Departments</govuk-footer-nav-item>
            <govuk-footer-nav-item href="#">Worldwide</govuk-footer-nav-item>
        </govuk-footer-nav-items>
    </govuk-footer-nav>
    <govuk-footer-meta>
        <govuk-footer-meta-items>
            <govuk-footer-meta-item href="#">Help</govuk-footer-meta-item>
            <govuk-footer-meta-item href="#">Cookies</govuk-footer-meta-item>
            <govuk-footer-meta-item href="#" lang="cy" hreflang="cy">Rhestr o Wasanaethau Cymraeg</govuk-footer-meta-item>
        </govuk-footer-meta-items>
        <govuk-footer-meta-content>Built by the <a href="#" class="govuk-footer__link">Government Digital Service</a></govuk-footer-meta-content>
    </govuk-footer-meta>
    <govuk-footer-content-licence />
    <govuk-footer-copyright />
</govuk-footer>
```


### API

#### `<govuk-footer>`

| Attribute | Type | Description |
| --- | --- | --- |
| `container-class` | `string` | Classes to add to the inner container. |


#### `<nav>` / `<govuk-footer-nav>`

Must be inside a `<govuk-footer>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `columns` | `int?` | The number of columns to display items in. |
| `width` | `string` | The width of this navigation section. For example, `one-third`, `two-thirds` or `one-half`. If not specified, `full` will be used. |


#### `<nav-title>` / `<govuk-footer-nav-title>`

Must be inside a `<nav>` or `<govuk-footer-nav>` element.


#### `<nav-items>` / `<govuk-footer-nav-items>`

Must be inside a `<nav>` or `<govuk-footer-nav>` element.


#### `<nav-item>` / `<govuk-footer-nav-item>`

Must be inside a `<nav-items>` or `<govuk-footer-nav-items>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `link-*` |  | Additional attributes to add to the generated `<a>` element. |
| (link attributes) |  | See [documentation on links](../links.md) for more information. |


#### `<meta>` / `<govuk-footer-meta>`

Must be inside a `<govuk-footer>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `visually-hidden-title` | `string` | The title of the meta item section. If not specified, `"Support links"` will be used. |


#### `<meta-items>` / `<govuk-footer-meta-items>`

Must be inside a `<meta>` or `<govuk-footer-meta>` element.


#### `<meta-item>` / `<govuk-footer-meta-item>`

Must be inside a `<meta-items>` or `<govuk-footer-meta-items>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `link-*` |  | Additional attributes to add to the generated `<a>` element. |
| (link attributes) |  | See [documentation on links](../links.md) for more information. |


#### `<content>` / `<govuk-footer-meta-content>`

Must be inside a `<meta>` or `<govuk-footer-meta>` element.


#### `<content-licence>` / `<govuk-footer-content-licence>`

Must be inside a `<govuk-footer>` element.


#### `<copyright>` / `<govuk-footer-copyright>`

Must be inside a `<govuk-footer>` element.

