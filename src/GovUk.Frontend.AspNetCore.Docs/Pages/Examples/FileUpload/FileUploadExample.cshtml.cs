using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GovUk.Frontend.AspNetCore.Docs.Pages.Examples.FileUpload;

public class FileUploadExampleModel : PageModel
{
    public IFormFile? Document { get; set; }
}
