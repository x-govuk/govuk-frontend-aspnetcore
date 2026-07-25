using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GovUk.Frontend.AspNetCore.Docs.Pages.Examples.Radios;

public class RadiosWithGeneratedFieldsetExampleModel : PageModel
{
    [Display(Name = "Where do you live?")]
    public string? WhereDoYouLive { get; set; }
}
