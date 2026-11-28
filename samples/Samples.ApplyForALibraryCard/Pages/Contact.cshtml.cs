using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Samples.ApplyForALibraryCard.Pages;

public class ContactModel(ApplicationStore store) : QuestionPageModel
{
    [BindProperty]
    [Required(ErrorMessage = "Select how you would like to be contacted")]
    public ContactMethod? ContactMethod { get; set; }

    [BindProperty]
    public string? Email { get; set; }

    [BindProperty]
    public string? PhoneNumber { get; set; }

    public override string PreviousPage => "/date-of-birth";

    public void OnGet()
    {
        var application = store.Get();
        ContactMethod = application.ContactMethod;
        Email = application.Email;
        PhoneNumber = application.PhoneNumber;
    }

    public IActionResult OnPost()
    {
        // The email address and phone number are only needed for the option they're revealed by
        if (ContactMethod == ApplyForALibraryCard.ContactMethod.Email && string.IsNullOrWhiteSpace(Email))
        {
            ModelState.AddModelError(nameof(Email), "Enter an email address");
        }

        if (ContactMethod == ApplyForALibraryCard.ContactMethod.Phone && string.IsNullOrWhiteSpace(PhoneNumber))
        {
            ModelState.AddModelError(nameof(PhoneNumber), "Enter a phone number");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        store.Update(a =>
        {
            a.ContactMethod = ContactMethod;
            a.Email = ContactMethod == ApplyForALibraryCard.ContactMethod.Email ? Email : null;
            a.PhoneNumber = ContactMethod == ApplyForALibraryCard.ContactMethod.Phone ? PhoneNumber : null;
        });

        return Continue("/check-answers");
    }
}
