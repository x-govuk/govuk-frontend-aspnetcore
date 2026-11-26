using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GovUk.Frontend.AspNetCore.Testing.Tests.Pages;

/// <summary>
/// A form with one of each field, bound with <c>for="…"</c>; <c>?errors=true</c> adds a model state error to each.
/// </summary>
public class FieldsModel : PageModel
{
    [Display(Name = "Full name")]
    public string? Name { get; set; }

    [Display(Name = "Can you provide more detail?")]
    public string? Detail { get; set; }

    [Display(Name = "Describe your job")]
    public string? JobDescription { get; set; }

    [Display(Name = "Country")]
    public string? Country { get; set; }

    [Display(Name = "Password")]
    public string? Password { get; set; }

    [Display(Name = "Upload a file")]
    public IFormFile? Upload { get; set; }

    [Display(Name = "Date of birth")]
    public DateOnly? DateOfBirth { get; set; }

    [Display(Name = "How would you like to be contacted?")]
    public string? Contact { get; set; }

    [Display(Name = "Email address")]
    public string? Email { get; set; }

    [Display(Name = "What is your nationality?")]
    public List<string>? Nationality { get; set; }

    public void OnGet([FromQuery] bool errors)
    {
        Name = "Sarah Philips";
        Detail = "Some detail";
        JobDescription = "Teacher";
        Country = "wales";
        Password = "hunter2";
        DateOfBirth = new DateOnly(2000, 3, 27);
        Contact = "email";
        Email = "sarah@example.com";
        Nationality = ["british", "irish"];

        if (errors)
        {
            ModelState.AddModelError(nameof(Name), "Enter your full name");
            ModelState.AddModelError(nameof(Detail), "Enter more detail");
            ModelState.AddModelError(nameof(JobDescription), "Job description must be 200 characters or fewer");
            ModelState.AddModelError(nameof(Country), "Select a country");
            ModelState.AddModelError(nameof(Password), "Enter a password");
            ModelState.AddModelError(nameof(Upload), "Select a file");
            ModelState.AddModelError(nameof(DateOfBirth), "Date of birth must be in the past");
            ModelState.AddModelError(nameof(Contact), "Select how you would like to be contacted");
            ModelState.AddModelError(nameof(Email), "Enter an email address");
            ModelState.AddModelError(nameof(Nationality), "Select your nationality");
        }
    }
}
