using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GovUk.Frontend.AspNetCore.Docs.Pages.Examples.DateInput;

public class DateInputWithCustomItemValuesExampleModel : PageModel
{
    public DateOnly? PassportIssued { get; set; }
}
