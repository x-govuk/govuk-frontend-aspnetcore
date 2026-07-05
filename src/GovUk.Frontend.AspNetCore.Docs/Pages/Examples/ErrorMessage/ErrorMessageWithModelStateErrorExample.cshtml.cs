using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GovUk.Frontend.AspNetCore.Docs.Pages.Examples.ErrorMessage;

public class ErrorMessageWithModelStateErrorExampleModel : PageModel
{
    public string? FullName { get; set; }

    public void OnGet()
    {
        ModelState.AddModelError(nameof(FullName), "Enter your full name");
    }
}
