using AngleSharp.Dom;

namespace GovUk.Frontend.AspNetCore.Testing.Tests.Xunit;

internal static class AssertFailure
{
    /// <summary>
    /// Checks that <paramref name="assertion"/> fails with exactly <paramref name="expectedMessage"/>, followed by the
    /// HTML of <paramref name="htmlOf"/> if it's given.
    /// </summary>
    public static GovUkAssertException Throws(string expectedMessage, Action assertion, INode? htmlOf = null)
    {
        var ex = Assert.Throws<GovUkAssertException>(assertion);

        var expected = htmlOf is null ? expectedMessage : $"{expectedMessage}\n\nHTML:\n{LookupFailure.FormatHtml(htmlOf)}";
        Assert.Equal(expected, ex.Message);

        return ex;
    }

    /// <inheritdoc cref="Throws"/>
    public static async Task<GovUkAssertException> ThrowsAsync(string expectedMessage, Func<Task> assertion)
    {
        var ex = await Assert.ThrowsAsync<GovUkAssertException>(assertion);

        Assert.Equal(expectedMessage, ex.Message);

        return ex;
    }

    /// <summary>
    /// Checks that the stack trace starts in the test, not in the assertion's implementation.
    /// </summary>
    public static void StackTraceStartsIn(string testMethodName, Exception ex)
    {
        var firstFrame = ex.StackTrace!.Split('\n')[0];
        Assert.Contains(testMethodName, firstFrame, StringComparison.Ordinal);
    }
}
