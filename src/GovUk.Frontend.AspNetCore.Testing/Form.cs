using System.Text;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace GovUk.Frontend.AspNetCore.Testing;

/// <summary>
/// A <c>&lt;form&gt;</c> on a page.
/// </summary>
/// <remarks>
/// A form isn't changed by testing it: <see cref="CreateSubmission()"/> copies its current values into a
/// <see cref="FormSubmission"/>, which is changed and sent instead.
/// </remarks>
public sealed class Form
{
    internal static readonly Lookup.Subject Subject = new("form", "forms", "buttons");
    private static readonly Lookup.Subject _buttonSubject = new("submit button", "submit buttons", "text");

    internal Form(IHtmlFormElement form, FormButton? button)
    {
        Element = form;
        Buttons = GetButtons(form);
        Button = button;
    }

    /// <summary>
    /// Gets the <c>&lt;form&gt;</c> element.
    /// </summary>
    /// <remarks>
    /// Fields in the form can be found by calling the lookup members on this element, e.g.
    /// <c>form.Element.GetTextInput("Email address")</c>.
    /// </remarks>
    public IHtmlFormElement Element { get; }

    /// <summary>
    /// Gets the buttons that submit the form, in order, including those outside it that refer to it with a
    /// <c>form</c> attribute.
    /// </summary>
    public IReadOnlyList<FormButton> Buttons { get; }

    /// <summary>
    /// Gets the button the form was found by, which <see cref="CreateSubmission()"/> submits it with.
    /// </summary>
    public FormButton? Button { get; }

    /// <summary>
    /// Creates a submission of the form's current values, as if <see cref="Button"/> was clicked.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The submission includes the button's name and value, and uses its <c>formaction</c>, <c>formmethod</c> and
    /// <c>formenctype</c> if it has them. If <see cref="Button"/> is <see langword="null"/>, the form's first submit
    /// button is used, as when Enter is pressed in a field.
    /// </para>
    /// <para>
    /// Values follow the rules a browser uses: unchecked radios and checkboxes and disabled fields are left out, and
    /// the antiforgery token and other hidden inputs are included. File uploads are left out; add files with
    /// <see cref="FormSubmission.AddFile(string, byte[], string, string?)"/>.
    /// </para>
    /// </remarks>
    public FormSubmission CreateSubmission() => CreateSubmission(Button ?? (Buttons.Count > 0 ? Buttons[0] : null));

    /// <summary>
    /// Creates a submission of the form's current values, as if the button with the text <paramref name="buttonText"/>
    /// was clicked.
    /// </summary>
    /// <exception cref="ComponentLookupException">The form has no such button, or more than one.</exception>
    public FormSubmission CreateSubmission(string buttonText)
    {
        ArgumentNullException.ThrowIfNull(buttonText);

        var button = Lookup.Single(
            Element,
            Buttons,
            b => b.Text == buttonText,
            b => b.Text,
            _buttonSubject,
            $"a submit button with the text {LookupFailure.Quote(buttonText)}",
            required: true)!;

        return CreateSubmission(button);
    }

    /// <inheritdoc/>
    public override string ToString() => $"Form: {string.Join(", ", Buttons.Select(b => LookupFailure.Quote(b.Text)))}";

    internal static IReadOnlyList<FormButton> GetButtons(IHtmlFormElement form) =>
        form.Elements.OfType<IHtmlElement>().Where(FormButton.IsSubmitButton).Select(b => new FormButton(b, form)).ToArray();

    private FormSubmission CreateSubmission(FormButton? button)
    {
        var request = (button is null ? Element.GetSubmission() : Element.GetSubmission(button.Element)) ?? throw new InvalidOperationException(
            "The form can't be submitted: its method is 'dialog'.");

        var target = new Uri(request.Target.Href);

        if (request.Method == AngleSharp.Io.HttpMethod.Get)
        {
            var submission = new FormSubmission(new Uri(target.GetLeftPart(UriPartial.Path)), HttpMethod.Get);
            AddUrlEncoded(submission, target.Query);
            return submission;
        }

        var post = new FormSubmission(target, HttpMethod.Post);
        var mediaType = MediaTypeHeaderValue.Parse(request.MimeType);
        var body = request.Body;

        if (mediaType.MediaType.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
        {
            using var reader = new StreamReader(body, Encoding.UTF8);
            AddUrlEncoded(post, reader.ReadToEnd());
        }
        else if (mediaType.MediaType.Equals("multipart/form-data", StringComparison.OrdinalIgnoreCase))
        {
            post.Multipart = true;
            AddMultipart(post, HeaderUtilities.RemoveQuotes(mediaType.Boundary).ToString(), body);
        }
        else
        {
            throw new NotSupportedException($"Forms encoded as '{mediaType.MediaType}' aren't supported.");
        }

        return post;
    }

    private static void AddUrlEncoded(FormSubmission submission, string encoded)
    {
        foreach (var pair in encoded.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var separator = pair.IndexOf('=', StringComparison.Ordinal);
            var name = separator == -1 ? pair : pair[..separator];
            var value = separator == -1 ? "" : pair[(separator + 1)..];

            submission.Add(Decode(name), Decode(value));
        }

        static string Decode(string value) => Uri.UnescapeDataString(value.Replace('+', ' '));
    }

    private static void AddMultipart(FormSubmission submission, string boundary, Stream body)
    {
        var reader = new MultipartReader(boundary, body);

        // The body is a MemoryStream, so reading it completes synchronously.
#pragma warning disable VSTHRD002
        while (reader.ReadNextSectionAsync().GetAwaiter().GetResult() is { } section)
#pragma warning restore VSTHRD002
        {
            if (!ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var disposition))
            {
                continue;
            }

            var name = HeaderUtilities.RemoveQuotes(disposition.Name).ToString();

            // A file upload, sent with filename="" when no file is chosen; a test adds files to the submission itself.
            if (disposition.FileName.HasValue || disposition.FileNameStar.HasValue)
            {
                continue;
            }

            using var sectionReader = new StreamReader(section.Body, Encoding.UTF8);
            submission.Add(name, sectionReader.ReadToEnd());
        }
    }
}

/// <summary>
/// A button that submits a <see cref="Form"/>: a <c>&lt;button&gt;</c> or an <c>&lt;input type="submit"&gt;</c>.
/// </summary>
public sealed class FormButton
{
    internal FormButton(IHtmlElement element, IHtmlFormElement form)
    {
        Element = element;
        Form = form;
        Text = element is IHtmlInputElement input ? input.Value : TextExtraction.GetText(element);
    }

    /// <summary>
    /// Gets the button's element.
    /// </summary>
    public IHtmlElement Element { get; }

    internal IHtmlFormElement Form { get; }

    /// <summary>
    /// Gets the button's text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the button's <c>name</c>, or <see langword="null"/> if it doesn't have one.
    /// </summary>
    public string? Name => Element.GetAttribute("name");

    /// <summary>
    /// Gets the button's <c>value</c>, or <see langword="null"/> if it doesn't have one.
    /// </summary>
    public string? Value => Element.GetAttribute("value");

    /// <inheritdoc/>
    public override string ToString() => Text;

    internal static bool IsSubmitButton(IElement element) => element switch
    {
        IHtmlButtonElement button => button.Type.Equals("submit", StringComparison.OrdinalIgnoreCase),
        IHtmlInputElement input => input.Type.Equals("submit", StringComparison.OrdinalIgnoreCase),
        _ => false
    };
}
