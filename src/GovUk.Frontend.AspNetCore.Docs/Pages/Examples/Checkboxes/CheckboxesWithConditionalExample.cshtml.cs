using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GovUk.Frontend.AspNetCore.Docs.Pages.Examples.Checkboxes;

public class CheckboxesWithConditionalExampleModel : PageModel
{
    public string[]? ContactPreferences { get; set; }

    public string? EmailAddress { get; set; }

    public string? PhoneNumber { get; set; }

    public string? MobilePhoneNumber { get; set; }
}
