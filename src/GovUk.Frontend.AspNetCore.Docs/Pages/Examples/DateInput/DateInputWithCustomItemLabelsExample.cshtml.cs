using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GovUk.Frontend.AspNetCore.Docs.Pages.Examples.DateInput;

public class DateInputWithCustomItemLabelsExampleModel : PageModel
{
    [DateInput(ErrorMessagePrefix = "Dyddiad cyhoeddi eich pasbort")]
    public DateOnly? PassportIssued { get; set; }
}
