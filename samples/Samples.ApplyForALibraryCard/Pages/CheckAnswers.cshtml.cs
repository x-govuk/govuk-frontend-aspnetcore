using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Samples.ApplyForALibraryCard.Pages;

public class CheckAnswersModel(ApplicationStore store) : PageModel
{
    public Application Application { get; private set; } = null!;

    public IActionResult OnGet()
    {
        Application = store.Get();

        // Answers can only be checked once every question has been answered
        return Application.IsComplete ? Page() : Redirect("/");
    }

    public IActionResult OnPost()
    {
        if (!store.Get().IsComplete)
        {
            return Redirect("/");
        }

        store.Send();
        return Redirect("/confirmation");
    }
}
