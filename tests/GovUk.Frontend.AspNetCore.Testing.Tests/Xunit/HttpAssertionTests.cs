using System.Net;

namespace GovUk.Frontend.AspNetCore.Testing.Tests.Xunit;

public class HttpAssertionTests(TestApp app) : IClassFixture<TestApp>
{
    [Fact]
    public async Task HtmlResponseAsync_HtmlWithExpectedStatusCode_ReturnsDocument()
    {
        using var client = app.CreateClient();
        using var response = await client.GetAsync(new Uri("/Fields", UriKind.Relative), TestContext.Current.CancellationToken);

        var doc = await Assert.HtmlResponseAsync(response, cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal("Fields", doc.Title);
        Assert.Equal("http://localhost/Fields", doc.Url);
    }

    [Fact]
    public async Task HtmlResponseAsync_OtherStatusCodeExpected_ChecksThatOne()
    {
        using var client = app.CreateClient();
        using var response = await client.GetAsync(new Uri("/Fields", UriKind.Relative), TestContext.Current.CancellationToken);

        await AssertFailure.ThrowsAsync(
            """
            Assert.HtmlResponseAsync() Failure: Status code differs
            Expected: 400 Bad Request
            Actual:   200 OK
            """,
            () => Assert.HtmlResponseAsync(response, HttpStatusCode.BadRequest));
    }

    [Fact]
    public async Task HtmlResponseAsync_Redirect_FailsWithLocation()
    {
        using var client = app.CreateClient();
        using var response = await client.GetAsync(new Uri("/Redirect/302/0/target", UriKind.Relative), TestContext.Current.CancellationToken);

        var ex = await AssertFailure.ThrowsAsync(
            """
            Assert.HtmlResponseAsync() Failure: Status code differs
            Expected: 200 OK
            Actual:   302 Found
            Location: http://localhost/Echo/target
            """,
            () => Assert.HtmlResponseAsync(response));

        AssertFailure.StackTraceStartsIn(nameof(HtmlResponseAsync_Redirect_FailsWithLocation), ex);
    }

    [Fact]
    public async Task HtmlResponseAsync_NotHtml_Fails()
    {
        using var client = app.CreateClient();
        using var response = await client.GetAsync(new Uri("/Echo/target", UriKind.Relative), TestContext.Current.CancellationToken);

        await AssertFailure.ThrowsAsync(
            """
            Assert.HtmlResponseAsync() Failure: Not an HTML response
            Expected: text/html
            Actual:   text/plain
            """,
            () => Assert.HtmlResponseAsync(response));
    }

    [Theory]
    [InlineData("/Echo/target")]
    [InlineData("http://localhost/Echo/target")]
    public async Task RedirectsTo_MatchingLocation_ReturnsResolvedLocation(string expectedLocation)
    {
        using var client = app.CreateClient();
        using var response = await client.GetAsync(new Uri("/Redirect/302/0/target", UriKind.Relative), TestContext.Current.CancellationToken);

        var location = Assert.RedirectsTo(response, expectedLocation);

        Assert.Equal(new Uri("http://localhost/Echo/target"), location);
    }

    [Theory]
    [InlineData(301)]
    [InlineData(303)]
    [InlineData(307)]
    [InlineData(308)]
    public async Task RedirectsTo_OtherRedirectStatusCodes_Pass(int statusCode)
    {
        using var client = app.CreateClient();
        using var response = await client.GetAsync(new Uri($"/Redirect/{statusCode}/0/target", UriKind.Relative), TestContext.Current.CancellationToken);

        Assert.RedirectsTo(response, "/Echo/target");
    }

    [Fact]
    public async Task RedirectsTo_DifferentLocation_Fails()
    {
        using var client = app.CreateClient();
        using var response = await client.GetAsync(new Uri("/Redirect/302/0/target", UriKind.Relative), TestContext.Current.CancellationToken);

        var ex = AssertFailure.Throws(
            """
            Assert.RedirectsTo() Failure: Location differs
            Expected: http://localhost/Echo/other
            Actual:   http://localhost/Echo/target
            """,
            () => Assert.RedirectsTo(response, "/Echo/other"));

        AssertFailure.StackTraceStartsIn(nameof(RedirectsTo_DifferentLocation_Fails), ex);
    }

    [Fact]
    public async Task RedirectsTo_NotARedirect_Fails()
    {
        using var client = app.CreateClient();
        using var response = await client.GetAsync(new Uri("/Fields", UriKind.Relative), TestContext.Current.CancellationToken);

        AssertFailure.Throws(
            """
            Assert.RedirectsTo() Failure: Not a redirect
            Expected: a redirect to http://localhost/Echo/target
            Actual:   200 OK
            """,
            () => Assert.RedirectsTo(response, "/Echo/target"));
    }

    [Fact]
    public async Task RedirectsTo_NoLocationHeader_Fails()
    {
        using var client = app.CreateClient();
        using var response = await client.GetAsync(new Uri("/NoLocation", UriKind.Relative), TestContext.Current.CancellationToken);

        AssertFailure.Throws(
            """
            Assert.RedirectsTo() Failure: No Location header
            Expected: a redirect to http://localhost/Echo/target
            Actual:   302 Found with no Location header
            """,
            () => Assert.RedirectsTo(response, "/Echo/target"));
    }

    [Fact]
    public void RedirectsTo_WithoutRequest_ComparesRelativeLocationsAsWritten()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.Redirect);
        response.Headers.Location = new Uri("/next", UriKind.Relative);

        Assert.Equal(new Uri("/next", UriKind.Relative), Assert.RedirectsTo(response, "/next"));

        AssertFailure.Throws(
            """
            Assert.RedirectsTo() Failure: Location differs
            Expected: /other
            Actual:   /next
            """,
            () => Assert.RedirectsTo(response, "/other"));
    }
}
