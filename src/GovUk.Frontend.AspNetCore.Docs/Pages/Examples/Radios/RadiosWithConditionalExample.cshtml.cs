using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GovUk.Frontend.AspNetCore.Docs.Pages.Examples.Radios;

public class RadiosWithConditionalExampleModel : PageModel
{
    public string? HowContacted { get; set; }

    public string? PhoneNumber { get; set; }

    public string? MobilePhoneNumber { get; set; }
}
