using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace GovUk.Frontend.AspNetCore;

internal static class Guard
{
    public static T ArgumentNotNull<T>(string argName, [NotNull] T? argValue)
        where T : class
    {
        return argValue is null ? throw new ArgumentNullException(argName) : argValue;
    }

    public static T ArgumentNotNull<T>(
        string argName,
        string message,
        [NotNull] T? testValue)
        where T : struct
    {
        return testValue is null ? throw new ArgumentException(message, argName) : testValue.Value;
    }

    public static string ArgumentNotNullOrEmpty(string argName, [NotNull] string? argValue)
    {
        return argValue is null
            ? throw new ArgumentNullException(argName)
            : string.IsNullOrEmpty(argValue) ? throw new ArgumentException("Argument was empty.", argName) : argValue;
    }

    public static T ArgumentNotNullOrEmpty<T>(string argName, [NotNull] T? argValue)
        where T : class, IEnumerable
    {
        ArgumentNotNull(argName, argValue);

        return !argValue.GetEnumerator().MoveNext() ? throw new ArgumentException("Argument was empty.", argName) : argValue;
    }

    public static void ArgumentValid(
        string argName,
        string message,
        bool test)
    {
        if (!test)
        {
            throw new ArgumentException(message, argName);
        }
    }

    public static T ArgumentValidNotNull<T>(
        string argName,
        string message,
        [NotNull] T? testValue,
        bool test)
        where T : class
    {
        return testValue is null || !test ? throw new ArgumentException(message, argName) : testValue;
    }
}
