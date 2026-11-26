using Xunit.Sdk;

namespace GovUk.Frontend.AspNetCore.Testing.Xunit;

/// <summary>
/// The exception thrown when one of the GOV.UK Frontend assertions on <c>Assert</c> fails.
/// </summary>
public sealed class GovUkAssertException : XunitException
{
    /// <summary>
    /// Creates a new <see cref="GovUkAssertException"/>.
    /// </summary>
    /// <param name="message">The failure message.</param>
    public GovUkAssertException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Creates a new <see cref="GovUkAssertException"/>.
    /// </summary>
    public GovUkAssertException()
        : base(null)
    {
    }

    /// <summary>
    /// Creates a new <see cref="GovUkAssertException"/>.
    /// </summary>
    /// <param name="message">The failure message.</param>
    /// <param name="innerException">The exception that caused the failure.</param>
    public GovUkAssertException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
