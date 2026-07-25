using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GovUk.Frontend.AspNetCore.Docs.Pages.Examples.DateInput;

public class DateInputWithGeneratedFieldsetExampleModel : PageModel
{
    [Display(Name = "When was your passport issued?")]
    public DateOnly? PassportIssued { get; set; }
}
