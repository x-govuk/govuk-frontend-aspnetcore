using Microsoft.AspNetCore.Mvc;

namespace GovUk.Frontend.AspNetCore.Docs.StubControllers;

// Provides endpoints that the asp-* attributes in the example pages can generate links to.
[Route("[controller]/[action]")]
public class HomeController : Controller
{
    public IActionResult Index() => Ok();

    public IActionResult Confirm() => Ok();
}
