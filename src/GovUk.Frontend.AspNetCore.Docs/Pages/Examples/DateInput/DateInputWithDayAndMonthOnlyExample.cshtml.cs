using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GovUk.Frontend.AspNetCore.Docs.Pages.Examples.DateInput;

public class DateInputWithDayAndMonthOnlyExampleModel : PageModel
{
    public (int Day, int Month)? Birthday { get; set; }
}
