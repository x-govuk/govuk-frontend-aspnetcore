using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GovUk.Frontend.AspNetCore.Docs.Pages.Examples.DateInput;

public class DateInputWithMonthAndYearOnlyExampleModel : PageModel
{
    public (int Month, int Year)? DateMovedIn { get; set; }
}
