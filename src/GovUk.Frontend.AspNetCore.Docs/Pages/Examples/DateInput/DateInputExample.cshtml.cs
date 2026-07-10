using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GovUk.Frontend.AspNetCore.Docs.Pages.Examples.DateInput;

public class DateInputExampleModel : PageModel
{
    public DateOnly? PassportIssued { get; set; }
}
