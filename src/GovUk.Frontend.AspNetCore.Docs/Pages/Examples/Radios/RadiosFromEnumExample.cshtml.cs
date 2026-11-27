using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GovUk.Frontend.AspNetCore.Docs.Pages.Examples.Radios;

public class RadiosFromEnumExampleModel : PageModel
{
    public enum Country
    {
        Unknown,
        England,
        Scotland,
        Wales,
        [Display(Name = "Northern Ireland")]
        NorthernIreland,
        [Display(Name = "Abroad", Description = "For British citizens living outside the UK")]
        Abroad
    }

    public Country? WhereDoYouLive { get; set; }
}
