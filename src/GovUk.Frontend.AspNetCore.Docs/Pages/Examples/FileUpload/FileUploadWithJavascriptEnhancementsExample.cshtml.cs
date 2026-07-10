using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GovUk.Frontend.AspNetCore.Docs.Pages.Examples.FileUpload;

public class FileUploadWithJavascriptEnhancementsExampleModel : PageModel
{
    public IFormFile? Document { get; set; }
}
