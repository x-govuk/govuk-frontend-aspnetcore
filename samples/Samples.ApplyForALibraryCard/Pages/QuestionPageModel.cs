using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Samples.ApplyForALibraryCard.Pages;

/// <summary>
/// A page that asks a question. Opened from a Change link on the check answers page (with <c>?change=true</c>), it
/// goes back there when the answer is saved.
/// </summary>
public abstract class QuestionPageModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public bool Change { get; set; }

    /// <summary>
    /// The page before this one in the journey.
    /// </summary>
    public abstract string PreviousPage { get; }

    public string BackLink => Change ? "/check-answers" : PreviousPage;

    protected IActionResult Continue(string nextPage) => Redirect(Change ? "/check-answers" : nextPage);
}
