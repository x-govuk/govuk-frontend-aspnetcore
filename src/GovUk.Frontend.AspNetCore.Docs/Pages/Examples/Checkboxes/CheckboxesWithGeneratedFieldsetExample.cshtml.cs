using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GovUk.Frontend.AspNetCore.Docs.Pages.Examples.Checkboxes;

public class CheckboxesWithGeneratedFieldsetExampleModel : PageModel
{
    [Display(Name = "What is your nationality?")]
    public string[]? Nationalities { get; set; }
}
