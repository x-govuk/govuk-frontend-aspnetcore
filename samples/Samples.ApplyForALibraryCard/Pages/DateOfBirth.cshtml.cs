using System.ComponentModel.DataAnnotations;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Samples.ApplyForALibraryCard.Pages;

public class DateOfBirthModel(ApplicationStore store, TimeProvider timeProvider) : QuestionPageModel
{
    [BindProperty]
    [DateInput(ErrorMessagePrefix = "Date of birth")]
    [Required(ErrorMessage = "Enter your date of birth")]
    public DateOnly? DateOfBirth { get; set; }

    public override string PreviousPage => "/name";

    public void OnGet() => DateOfBirth = store.Get().DateOfBirth;

    public IActionResult OnPost()
    {
        if (ModelState.IsValid && DateOfBirth >= DateOnly.FromDateTime(timeProvider.GetLocalNow().Date))
        {
            ModelState.AddModelError(nameof(DateOfBirth), "Date of birth must be in the past");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        store.Update(a => a.DateOfBirth = DateOfBirth);
        return Continue("/contact");
    }
}
