using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GovUk.Frontend.AspNetCore.Docs.Pages.Examples.Checkboxes;

public class CheckboxesFromEnumExampleModel : PageModel
{
    public enum Country
    {
        France,
        Portugal,
        Spain,
        [Display(Name = "No, I will not be travelling to any of these countries")]
        None
    }

    public Country[]? CountriesTravellingTo { get; set; }
}
