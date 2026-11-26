using System.Net;
using System.Text;

namespace GovUk.Frontend.AspNetCore.Testing.Tests;

public class HttpResponseMessageExtensionsTests
{
    [Fact]
    public async Task GetHtmlDocumentAsync_ParsesContentWithRequestAddress()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("<html><body><a id=\"link\" href=\"next\">Next</a></body></html>", Encoding.UTF8, "text/html"),
            RequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://localhost/questions/first")
        };

        var doc = await response.GetHtmlDocumentAsync(TestContext.Current.CancellationToken);

        Assert.Equal("https://localhost/questions/first", doc.Url);
        var link = (AngleSharp.Html.Dom.IHtmlAnchorElement)doc.GetElementById("link")!;
        Assert.Equal("https://localhost/questions/next", link.Href);
    }

    [Fact]
    public async Task GetHtmlDocumentAsync_NotHtml_Throws()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json")
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => response.GetHtmlDocumentAsync(TestContext.Current.CancellationToken));

        Assert.Equal("Expected an HTML response but the content type was 'application/json' (status code 404 NotFound).", ex.Message);
    }
}
