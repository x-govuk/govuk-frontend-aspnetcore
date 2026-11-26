using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace GovUk.Frontend.AspNetCore.Testing;

public static partial class ComponentQueryExtensions
{
    extension(IParentNode node)
    {
        /// <summary>
        /// Gets every form, in document order.
        /// </summary>
        /// <remarks>
        /// <see cref="Form.CreateSubmission()"/> submits each with its first submit button. (Named so as not to clash
        /// with the document's <c>Forms</c>, which gives the <c>&lt;form&gt;</c> elements.)
        /// </remarks>
        public IReadOnlyList<Form> AllForms =>
            All(node, "form", e => new Form((IHtmlFormElement)e, button: null));

        /// <summary>
        /// Gets the form with a submit button whose text is <paramref name="buttonText"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="Form.CreateSubmission()"/> submits the form with that button.
        /// </remarks>
        /// <exception cref="ComponentLookupException">There is no such button, or there's more than one.</exception>
        public Form GetForm(string buttonText) => FindForm(node, buttonText, required: true)!;

        /// <summary>
        /// Gets the form with a submit button whose text is <paramref name="buttonText"/>, or <see langword="null"/> if
        /// there isn't one.
        /// </summary>
        /// <remarks>
        /// <see cref="Form.CreateSubmission()"/> submits the form with that button.
        /// </remarks>
        /// <exception cref="ComponentLookupException">There's more than one such button.</exception>
        public Form? FindForm(string buttonText) => FindForm(node, buttonText, required: false);
    }

    private static Form? FindForm(IParentNode node, string buttonText, bool required)
    {
        ArgumentNullException.ThrowIfNull(node);
        ArgumentNullException.ThrowIfNull(buttonText);

        var buttons = node.QuerySelectorAll("form").OfType<IHtmlFormElement>()
            .SelectMany(Form.GetButtons)
            .ToArray();

        var match = Lookup.Single(
            (INode)node,
            buttons,
            b => b.Text == buttonText,
            b => b.Text,
            Form.Subject,
            $"a form with the button {LookupFailure.Quote(buttonText)}",
            required);

        return match is null ? null : new Form(match.Form, match);
    }
}
