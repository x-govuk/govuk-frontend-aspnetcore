using System.ComponentModel.DataAnnotations;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NodaTime;

namespace Samples.DateInput.Pages;

public class PartialDateModel : PageModel
{
    [BindProperty]
    [Display(Name = "What is your birth month?")]
    [DateInput(/*DateInputItemTypes.MonthAndYear, */ErrorMessagePrefix = "Your birth month")]
    [Required(ErrorMessage = "Enter your birth month")]
    public YearMonth? BirthMonth { get; set; }

    public void OnGet()
    {
    }

    public void OnPost()
    {
    }
}
