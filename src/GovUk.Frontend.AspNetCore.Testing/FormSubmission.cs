using System.Collections;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;

namespace GovUk.Frontend.AspNetCore.Testing;

/// <summary>
/// A form submission: the URL and method to submit to, and the values and files to send.
/// </summary>
/// <remarks>
/// <para>
/// Create one directly to post to an endpoint without requesting a page first, or with
/// <see cref="Form.CreateSubmission()"/> to start from a form's current values. Either way, values are added and
/// replaced by name, and the submission is sent with <c>HttpClient.SubmitAsync</c>.
/// </para>
/// <para>
/// Values are formatted as ASP.NET Core model binding reads them back: a <see cref="DateOnly"/> becomes the
/// <c>.Day</c>, <c>.Month</c> and <c>.Year</c> fields of a date input, <see cref="bool"/>s are <c>true</c> or
/// <c>false</c>, enums use their names, numbers, <see cref="Guid"/>s and other <see cref="IFormattable"/> values are
/// formatted with the invariant culture and collections add a value for each item. <see langword="null"/> values are
/// left out. Any other value is rejected rather than posted as its <see cref="object.ToString"/>, which is rarely what
/// a form would send; add it as a string instead.
/// </para>
/// <para>
/// A <c>(int, int)</c> for a month and year or a day and month date input is rejected, since it doesn't say which
/// parts it holds. Add those fields by name instead, e.g. <c>Add("Issued.Month", "3").Add("Issued.Year", "2020")</c>.
/// </para>
/// </remarks>
public sealed class FormSubmission : IEnumerable<KeyValuePair<string, string>>
{
    /// <summary>
    /// The name of the field that holds the antiforgery token in ASP.NET Core's default configuration.
    /// </summary>
    public static string DefaultAntiforgeryFieldName => "__RequestVerificationToken";

    private readonly List<KeyValuePair<string, string>> _values = [];
    private readonly List<FormSubmissionFile> _files = [];

    /// <summary>
    /// Creates a submission to <paramref name="url"/>.
    /// </summary>
    /// <param name="url">The URL to submit to; a relative URL is resolved against the client's base address.</param>
    /// <param name="method">The method, <see cref="HttpMethod.Post"/> (the default) or <see cref="HttpMethod.Get"/>.</param>
    public FormSubmission(string url, HttpMethod? method = null)
        : this(new Uri(url ?? throw new ArgumentNullException(nameof(url)), UriKind.RelativeOrAbsolute), method)
    {
    }

    /// <summary>
    /// Creates a submission to <paramref name="url"/>.
    /// </summary>
    /// <param name="url">The URL to submit to; a relative URL is resolved against the client's base address.</param>
    /// <param name="method">The method, <see cref="HttpMethod.Post"/> (the default) or <see cref="HttpMethod.Get"/>.</param>
    public FormSubmission(Uri url, HttpMethod? method = null)
    {
        ArgumentNullException.ThrowIfNull(url);

        method ??= HttpMethod.Post;

        if (method != HttpMethod.Post && method != HttpMethod.Get)
        {
            throw new ArgumentException("A form can only be submitted with GET or POST.", nameof(method));
        }

        Url = url;
        Method = method;
    }

    /// <summary>
    /// Gets the URL to submit to.
    /// </summary>
    /// <remarks>
    /// For a <c>GET</c>, the values replace any query string, as they do in a browser.
    /// </remarks>
    public Uri Url { get; }

    /// <summary>
    /// Gets the method to submit with: <c>GET</c> or <c>POST</c>.
    /// </summary>
    public HttpMethod Method { get; }

    /// <summary>
    /// Gets or sets whether a <c>POST</c> is encoded as <c>multipart/form-data</c> rather than
    /// <c>application/x-www-form-urlencoded</c>.
    /// </summary>
    /// <remarks>
    /// This is always <see langword="true"/> when there are files, and is <see langword="true"/> for a submission
    /// created from a form with <c>enctype="multipart/form-data"</c>.
    /// </remarks>
    public bool IsMultipart
    {
        get => field || _files.Count > 0;
        set;
    }

    /// <summary>
    /// Gets the values, in order, as name/value pairs.
    /// </summary>
    public IReadOnlyList<KeyValuePair<string, string>> Values => _values.AsReadOnly();

    /// <summary>
    /// Gets the files, in order.
    /// </summary>
    public IReadOnlyList<FormSubmissionFile> Files => _files.AsReadOnly();

    /// <summary>
    /// Adds <paramref name="value"/> for <paramref name="name"/>, after any values already there.
    /// </summary>
    /// <returns>This submission.</returns>
    public FormSubmission Add(string name, string? value)
    {
        ArgumentNullException.ThrowIfNull(name);

        if (value is not null)
        {
            _values.Add(new(name, value));
        }

        return this;
    }

    /// <summary>
    /// Adds a date as the <c>.Day</c>, <c>.Month</c> and <c>.Year</c> fields of the date input named
    /// <paramref name="name"/>.
    /// </summary>
    /// <returns>This submission.</returns>
    public FormSubmission Add(string name, DateOnly? value) => Add(name, (object?)value);

    /// <summary>
    /// Adds each of <paramref name="values"/> for <paramref name="name"/>, as for checkboxes.
    /// </summary>
    /// <returns>This submission.</returns>
    public FormSubmission Add(string name, IEnumerable<string?>? values) => Add(name, (object?)values);

    /// <summary>
    /// Adds <paramref name="value"/> for <paramref name="name"/>, formatted as model binding reads it back.
    /// </summary>
    /// <returns>This submission.</returns>
    public FormSubmission Add(string name, object? value)
    {
        ArgumentNullException.ThrowIfNull(name);

        _values.AddRange(Format(name, value));

        return this;
    }

    /// <summary>
    /// Replaces the values for <paramref name="name"/> with <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// The new value takes the place of the first value it replaces, or goes at the end if there wasn't one.
    /// </remarks>
    /// <returns>This submission.</returns>
    public FormSubmission Set(string name, string? value) => Set(name, (object?)value);

    /// <summary>
    /// Replaces the <c>.Day</c>, <c>.Month</c> and <c>.Year</c> fields of the date input named
    /// <paramref name="name"/> with <paramref name="value"/>.
    /// </summary>
    /// <returns>This submission.</returns>
    public FormSubmission Set(string name, DateOnly? value) => Set(name, (object?)value);

    /// <summary>
    /// Replaces the values for <paramref name="name"/> with <paramref name="values"/>, as for checkboxes.
    /// </summary>
    /// <returns>This submission.</returns>
    public FormSubmission Set(string name, IEnumerable<string?>? values) => Set(name, (object?)values);

    /// <summary>
    /// Replaces the values for <paramref name="name"/> with <paramref name="value"/>, formatted as model binding reads
    /// it back.
    /// </summary>
    /// <remarks>
    /// The new values take the place of the first value they replace, or go at the end if there wasn't one.
    /// Setting a <see cref="DateOnly"/> also replaces the <c>.Day</c>, <c>.Month</c> and <c>.Year</c> fields.
    /// Setting <see langword="null"/> removes the values.
    /// </remarks>
    /// <returns>This submission.</returns>
    public FormSubmission Set(string name, object? value)
    {
        ArgumentNullException.ThrowIfNull(name);

        var replacements = Format(name, value).ToArray();
        var names = new HashSet<string>(StringComparer.Ordinal) { name };
        names.UnionWith(replacements.Select(r => r.Key));
        names.UnionWith(DatePartNames(name));

        var index = _values.FindIndex(v => names.Contains(v.Key));
        _values.RemoveAll(v => names.Contains(v.Key));
        _values.InsertRange(index == -1 ? _values.Count : Math.Min(index, _values.Count), replacements);

        return this;
    }

    /// <summary>
    /// Removes the values and files for <paramref name="name"/>.
    /// </summary>
    /// <returns><see langword="true"/> if anything was removed.</returns>
    public bool Remove(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        return _values.RemoveAll(v => v.Key == name) + _files.RemoveAll(f => f.Name == name) > 0;
    }

    /// <summary>
    /// Gets the values for <paramref name="name"/>, in order.
    /// </summary>
    public IReadOnlyList<string> GetValuesFor(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        return _values.Where(v => v.Key == name).Select(v => v.Value).ToArray();
    }

    /// <summary>
    /// Adds a file for the file upload named <paramref name="name"/>. The submission is then sent as
    /// <c>multipart/form-data</c>.
    /// </summary>
    /// <param name="name">The file upload's name.</param>
    /// <param name="content">The file's content.</param>
    /// <param name="fileName">The file's name, e.g. <c>passport.jpg</c>.</param>
    /// <param name="contentType">The file's content type; <c>application/octet-stream</c> if not given.</param>
    /// <returns>This submission.</returns>
    public FormSubmission AddFile(string name, byte[] content, string fileName, string? contentType = null)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(fileName);

        _files.Add(new FormSubmissionFile(name, content, fileName, contentType ?? "application/octet-stream"));

        return this;
    }

    /// <summary>
    /// Adds a file for the file upload named <paramref name="name"/>, reading its content from
    /// <paramref name="content"/>. The submission is then sent as <c>multipart/form-data</c>.
    /// </summary>
    /// <param name="name">The file upload's name.</param>
    /// <param name="content">A stream with the file's content, which is read to its end now.</param>
    /// <param name="fileName">The file's name, e.g. <c>passport.jpg</c>.</param>
    /// <param name="contentType">The file's content type; <c>application/octet-stream</c> if not given.</param>
    /// <returns>This submission.</returns>
    public FormSubmission AddFile(string name, Stream content, string fileName, string? contentType = null)
    {
        ArgumentNullException.ThrowIfNull(content);

        using var buffer = new MemoryStream();
        content.CopyTo(buffer);

        return AddFile(name, buffer.ToArray(), fileName, contentType);
    }

    /// <summary>
    /// Adds a file for the file upload named <paramref name="name"/>, reading its content from
    /// <paramref name="content"/>. The submission is then sent as <c>multipart/form-data</c>.
    /// </summary>
    /// <param name="name">The file upload's name.</param>
    /// <param name="content">The file's content, which is read now.</param>
    /// <param name="fileName">The file's name, e.g. <c>passport.jpg</c>.</param>
    /// <param name="contentType">
    /// The file's content type; the <c>Content-Type</c> of <paramref name="content"/> if not given, or
    /// <c>application/octet-stream</c> if it has none.
    /// </param>
    /// <returns>This submission.</returns>
    public FormSubmission AddFile(string name, HttpContent content, string fileName, string? contentType = null)
    {
        ArgumentNullException.ThrowIfNull(content);

        using var stream = content.ReadAsStream();

        return AddFile(name, stream, fileName, contentType ?? content.Headers.ContentType?.MediaType);
    }

    /// <summary>
    /// Requests <see cref="Url"/> and sets the antiforgery token from the page it returns.
    /// </summary>
    /// <remarks>
    /// The token is paired with a cookie set by the same response, so <paramref name="client"/> must keep cookies, as
    /// clients from <c>WebApplicationFactory.CreateClient()</c> do. A submission created from a form already has the
    /// form's token.
    /// </remarks>
    /// <param name="client">The client to request the page with, and to submit with afterwards.</param>
    /// <param name="fieldName">
    /// The name of the antiforgery field; <see cref="DefaultAntiforgeryFieldName"/> if not given.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>This submission.</returns>
    /// <exception cref="InvalidOperationException">
    /// The request failed, the response isn't HTML, or the page doesn't have exactly one distinct token.
    /// </exception>
    public async Task<FormSubmission> AddAntiforgeryTokenAsync(
        HttpClient client,
        string? fieldName = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        fieldName ??= DefaultAntiforgeryFieldName;

        var token = await client.GetAntiforgeryTokenAsync(Url, fieldName, cancellationToken).ConfigureAwait(false);

        return Set(fieldName, token);
    }

    /// <summary>
    /// Creates the request's content: <see langword="null"/> for a <c>GET</c>, otherwise URL-encoded or multipart.
    /// </summary>
    public HttpContent? ToContent()
    {
        if (Method == HttpMethod.Get)
        {
            return null;
        }

        if (!IsMultipart)
        {
            return new FormUrlEncodedContent(_values);
        }

        var content = new MultipartFormDataContent();

        // The parts are disposed with the content they're added to.
#pragma warning disable CA2000
        foreach (var (name, value) in _values)
        {
            content.Add(new StringContent(value, Encoding.UTF8), name);
        }

        foreach (var file in _files)
        {
            var fileContent = new ByteArrayContent(file.Content.ToArray());
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
            content.Add(fileContent, file.Name, file.FileName);
        }
#pragma warning restore CA2000

        return content;
    }

    /// <summary>
    /// Creates the request: a <c>GET</c> with the values in its query string, or a <c>POST</c> with them in its body.
    /// </summary>
    public HttpRequestMessage ToRequest()
    {
        if (Method == HttpMethod.Get)
        {
            if (_files.Count > 0)
            {
                throw new InvalidOperationException("Files can't be submitted with GET.");
            }

            return new HttpRequestMessage(HttpMethod.Get, WithQuery(Url, EncodeQuery(_values)));
        }

        return new HttpRequestMessage(HttpMethod.Post, Url) { Content = ToContent() };
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() =>
        $"{Method} {Url} {string.Join("&", _values.Select(v => $"{v.Key}={v.Value}").Concat(_files.Select(f => $"{f.Name}=<{f.FileName}>")))}";

    private static IEnumerable<string> DatePartNames(string name) => [$"{name}.Day", $"{name}.Month", $"{name}.Year"];

    private static IEnumerable<KeyValuePair<string, string>> Format(string name, object? value)
    {
        switch (value)
        {
            case null:
                yield break;

            case string s:
                yield return new(name, s);
                break;

            case DateOnly date:
                yield return new($"{name}.Day", date.Day.ToString(CultureInfo.InvariantCulture));
                yield return new($"{name}.Month", date.Month.ToString(CultureInfo.InvariantCulture));
                yield return new($"{name}.Year", date.Year.ToString(CultureInfo.InvariantCulture));
                break;

            case ValueTuple<int, int>:
                throw new ArgumentException(
                    $"A (int, int) value for '{name}' doesn't say whether it's a day and month or a month and year. " +
                    $"Add the '{name}.Day', '{name}.Month' or '{name}.Year' fields separately.",
                    nameof(value));

            case bool b:
                yield return new(name, b ? "true" : "false");
                break;

            case Enum e:
                yield return new(name, e.ToString());
                break;

            case DateTime dt:
                yield return new(name, dt.ToString("O", CultureInfo.InvariantCulture));
                break;

            case DateTimeOffset dto:
                yield return new(name, dto.ToString("O", CultureInfo.InvariantCulture));
                break;

            case IFormattable formattable:
                yield return new(name, formattable.ToString(null, CultureInfo.InvariantCulture));
                break;

            case IEnumerable items:
                foreach (var item in items)
                {
                    if (item is IEnumerable and not string)
                    {
                        throw new ArgumentException($"The values for '{name}' can't contain nested collections.", nameof(value));
                    }

                    foreach (var formatted in Format(name, item))
                    {
                        yield return formatted;
                    }
                }

                break;

            default:
                throw new ArgumentException(
                    $"Don't know how to submit a {value.GetType()} for '{name}'. Add it as a string instead.",
                    nameof(value));
        }
    }

    internal static string EncodeQuery(IEnumerable<KeyValuePair<string, string>> values) =>
        string.Join("&", values.Select(v => $"{Uri.EscapeDataString(v.Key)}={Uri.EscapeDataString(v.Value)}"));

    private static Uri WithQuery(Uri url, string query)
    {
        var s = url.OriginalString;
        var fragmentIndex = s.IndexOf('#', StringComparison.Ordinal);
        var withoutFragment = fragmentIndex == -1 ? s : s[..fragmentIndex];
        var queryIndex = withoutFragment.IndexOf('?', StringComparison.Ordinal);
        var path = queryIndex == -1 ? withoutFragment : withoutFragment[..queryIndex];

        return new Uri(query.Length == 0 ? path : $"{path}?{query}", UriKind.RelativeOrAbsolute);
    }
}

/// <summary>
/// A file in a <see cref="FormSubmission"/>.
/// </summary>
/// <param name="Name">The file upload's name.</param>
/// <param name="Content">The file's content.</param>
/// <param name="FileName">The file's name.</param>
/// <param name="ContentType">The file's content type.</param>
public sealed record FormSubmissionFile(string Name, ReadOnlyMemory<byte> Content, string FileName, string ContentType);
