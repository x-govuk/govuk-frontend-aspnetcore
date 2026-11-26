using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GovUk.Frontend.AspNetCore.Testing.Tests.Pages;

public enum ContactMethod
{
    Email,
    Phone
}

/// <summary>
/// A form whose POST handlers return what was bound as JSON, so tests can check what a submission sent.
/// <c>?multipart=true</c> renders the form with <c>enctype="multipart/form-data"</c>.
/// </summary>
public class ApplyModel : PageModel
{
    [FromQuery]
    public bool Multipart { get; set; }

    [BindProperty]
    public string? Reference { get; set; }

    [BindProperty]
    [Display(Name = "Full name")]
    public string? Name { get; set; }

    [BindProperty]
    [Display(Name = "Date of birth")]
    public DateOnly? DateOfBirth { get; set; }

    [BindProperty]
    [Display(Name = "Nationality")]
    public List<string>? Nationality { get; set; }

    [BindProperty]
    [Display(Name = "How should we contact you?")]
    public ContactMethod? Contact { get; set; }

    [BindProperty]
    [Display(Name = "Country")]
    public string? Country { get; set; }

    [BindProperty]
    [Display(Name = "Upload a file")]
    public IFormFile? Upload { get; set; }

    [BindProperty]
    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    [BindProperty]
    public bool Agree { get; set; }

    [BindProperty]
    public decimal? Amount { get; set; }

    [BindProperty]
    public string? Action { get; set; }

    public void OnGet()
    {
        Name = "Sarah Philips";
        DateOfBirth = new DateOnly(2000, 3, 27);
        Nationality = ["irish"];
        Contact = ContactMethod.Phone;
        Country = "wales";
        Notes = "Disabled";
    }

    public Task<IActionResult> OnPostAsync() => EchoAsync("default");

    public Task<IActionResult> OnPostDraftAsync() => EchoAsync("draft");

    private async Task<IActionResult> EchoAsync(string handler)
    {
        string? upload = null;

        if (Upload is not null)
        {
            using var reader = new StreamReader(Upload.OpenReadStream());
            upload = $"{Upload.FileName} ({Upload.ContentType}): {await reader.ReadToEndAsync()}";
        }

        return new JsonResult(new
        {
            handler,
            contentType = Request.ContentType?.Split(';')[0],
            valid = ModelState.IsValid,
            Reference,
            Name,
            DateOfBirth,
            Nationality,
            Contact = Contact?.ToString(),
            Country,
            Upload = upload,
            Notes,
            Agree,
            Amount,
            Action
        });
    }
}
