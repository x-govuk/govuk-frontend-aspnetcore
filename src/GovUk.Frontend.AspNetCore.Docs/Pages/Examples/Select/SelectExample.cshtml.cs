using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GovUk.Frontend.AspNetCore.Docs.Pages.Examples.Select;

public class SelectExampleModel : PageModel
{
    public string? SortBy { get; set; }

    public void OnGet()
    {
        SortBy = "updated";
    }
}
