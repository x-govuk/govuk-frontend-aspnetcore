using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Samples.ApplyForALibraryCard.Pages;

public class ConfirmationModel(ApplicationStore store) : PageModel
{
    public string Reference { get; private set; } = null!;

    public IActionResult OnGet()
    {
        if (store.GetSentReference() is not { } reference)
        {
            return Redirect("/");
        }

        Reference = reference;
        return Page();
    }
}
