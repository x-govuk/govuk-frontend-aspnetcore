<!-- Generated from src/GovUk.Frontend.AspNetCore.Docs/Templates/components/notification-banner.liquid -->
# Notification banner

[GOV.UK Design System notification banner component](https://design-system.service.gov.uk/components/notification-banner/)


## Tag helpers

### Example
<img alt="Notification banner example" src="../images/notification-banner-example.png" />

```razor
<govuk-notification-banner>
    <p class="govuk-notification-banner__heading">
        You have 7 days left to send your application.
        <a class="govuk-notification-banner__link" href="#">View application</a>.
    </p>
</govuk-notification-banner>
```


### Example success
<img alt="Notification banner success example" src="../images/notification-banner-success-example.png" />

```razor
<govuk-notification-banner type="NotificationBannerType.Success">
    <p class="govuk-notification-banner__heading">
        Training outcome recorded and trainee withdrawn
    </p>
    <p class="govuk-body">Contact <a class="govuk-notification-banner__link" href="#">example@department.gov.uk</a> if you think there's a problem.</p>
</govuk-notification-banner>
```


### Example with overridden title
<img alt="Notification banner with overridden title example" src="../images/notification-banner-with-overridden-title-example.png" />

```razor
<govuk-notification-banner>
    <title heading-level="2" id="banner-title">
        Important information
    </title>
    <p class="govuk-notification-banner__heading">
        You have 7 days left to send your application.
        <a class="govuk-notification-banner__link" href="#">View application</a>.
    </p>
</govuk-notification-banner>
```


### API

#### `<govuk-notification-banner>`

The content is the HTML to use within the notification banner.

| Attribute | Type | Description |
| --- | --- | --- |
| `disable-auto-focus` | `bool?` | Whether to disable the behavior that focuses the notification banner when the page loads. Only applies when `Type` is `Success`. |
| `role` | `string` | The `role` attribute for the notification banner. If `Type` is `Success` then the default is `"alert"` otherwise `"region"`. |
| `type` | `GovUk.Frontend.AspNetCore.NotificationBannerType?` | The type of notification. |


#### `<title>` / `<govuk-notification-banner-title>`

The content is the HTML to use within the notification banner's title. Use a self-closing tag to keep the default content.

Must be inside a `<govuk-notification-banner>` element.

| Attribute | Type | Description |
| --- | --- | --- |
| `heading-level` | `int?` | The heading level for the notification banner title. Must be between `1` and `6` (inclusive). The default is `2`. |
| `id` | `string` | The `id` attribute for the notification banner title. The default is `"govuk-notification-banner-title"`. |

