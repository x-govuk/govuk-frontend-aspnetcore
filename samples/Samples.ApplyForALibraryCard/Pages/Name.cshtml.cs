using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Samples.ApplyForALibraryCard.Pages;

public class NameModel(ApplicationStore store) : QuestionPageModel
{
    [BindProperty]
    [Required(ErrorMessage = "Enter your full name")]
    public string? FullName { get; set; }

    public override string PreviousPage => "/";

    public void OnGet() => FullName = store.Get().FullName;

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        store.Update(a => a.FullName = FullName);
        return Continue("/date-of-birth");
    }
}
