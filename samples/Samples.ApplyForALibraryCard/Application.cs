using System.Globalization;
using System.Text.Json;

namespace Samples.ApplyForALibraryCard;

public enum ContactMethod
{
    Email,
    Phone,
    Post
}

/// <summary>
/// The answers given so far in an application for a library card.
/// </summary>
public class Application
{
    public string? FullName { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public ContactMethod? ContactMethod { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public bool IsComplete => FullName is not null && DateOfBirth is not null && ContactMethod is not null;

    public static string FormatDate(DateOnly date) => date.ToString("d MMMM yyyy", CultureInfo.InvariantCulture);
}

/// <summary>
/// Keeps the current <see cref="Application"/> and the reference of the last one sent in session state.
/// </summary>
public class ApplicationStore(IHttpContextAccessor httpContextAccessor)
{
    private const string ApplicationKey = "Application";
    private const string ReferenceKey = "Reference";

    private ISession Session => httpContextAccessor.HttpContext!.Session;

    public Application Get() =>
        Session.GetString(ApplicationKey) is { } json ? JsonSerializer.Deserialize<Application>(json)! : new Application();

    public void Update(Action<Application> update)
    {
        var application = Get();
        update(application);
        Session.SetString(ApplicationKey, JsonSerializer.Serialize(application));
    }

    public string Send()
    {
        var reference = Convert.ToHexString(Guid.NewGuid().ToByteArray(), 0, 4);
        Session.Remove(ApplicationKey);
        Session.SetString(ReferenceKey, reference);
        return reference;
    }

    public string? GetSentReference() => Session.GetString(ReferenceKey);
}
