using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.Tests.ComponentGeneration;

/// <summary>
/// Pins the contract that an <c>Html</c> option is emitted verbatim only when it actually holds HTML.
/// Assertions are made against the parsed DOM rather than the HTML string: a double-encode then shows
/// up as text that doesn't match the payload, and a raw injection as an element that shouldn't exist.
/// </summary>
public class EncodingTests
{
    private const string Payload = "Iechyd & Gofal <b>\"'";

    private readonly DefaultComponentGenerator _componentGenerator = TestUtils.CreateComponentGenerator();

    /// <summary>
    /// Every component whose content reaches the output through an <c>Html</c>/<c>Text</c> pair.
    /// </summary>
    public static TheoryData<string> ComponentsWithContent =>
    [
        "back-link", "button", "details", "error-message", "error-summary", "hint", "inset-text",
        "label", "notification-banner", "panel", "tag", "warning-text"
    ];

    [Theory]
    [MemberData(nameof(ComponentsWithContent))]
    public async Task TextOption_IsEncodedExactlyOnce(string component)
    {
        // Act
        var html = await RenderWithContentAsync(component, text: Payload, html: null);

        // Assert
        var element = HtmlHelper.ParseHtmlElement(html);
        Assert.Contains(Payload, element.TextContent, StringComparison.Ordinal);
    }

    [Theory]
    [MemberData(nameof(ComponentsWithContent))]
    public async Task HtmlOption_HoldingHtml_IsEmittedVerbatim(string component)
    {
        // Act
        var html = await RenderWithContentAsync(
            component,
            text: null,
            html: TemplateString.FromEncoded("<em data-probe=\"1\">Content</em>"));

        // Assert
        var element = HtmlHelper.ParseHtmlElement(html);
        Assert.NotNull(element.QuerySelector("[data-probe]"));
    }

    [Theory]
    [MemberData(nameof(ComponentsWithContent))]
    public async Task HtmlOption_HoldingText_IsEncoded(string component)
    {
        // A bare string is text, whatever slot it's assigned to. Emitting it verbatim is how a
        // validation message containing markup used to reach the page as markup.

        // Act
        var html = await RenderWithContentAsync(component, text: null, html: "<em data-probe=\"1\">Content</em>");

        // Assert
        var element = HtmlHelper.ParseHtmlElement(html);
        Assert.Null(element.QuerySelector("[data-probe]"));
        Assert.Contains("<em data-probe=\"1\">Content</em>", element.TextContent, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ErrorSummaryItem_HoldingText_IsEncoded()
    {
        // The path a model binding error message takes: ModelError.ErrorMessage is a string, and
        // MVC's default type-conversion message quotes the value that was submitted.
        var errorMessage = "The value '<img src=x onerror=alert(1)>' is not valid for Age.";

        // Act
        var component = await _componentGenerator.GenerateErrorSummaryAsync(new ErrorSummaryOptions
        {
            TitleText = "There is a problem",
            ErrorList = [new ErrorSummaryOptionsErrorItem { Html = new TemplateString(errorMessage), Href = "#age" }]
        });

        // Assert
        var element = HtmlHelper.ParseHtmlElement(component.GetHtml());
        Assert.Null(element.QuerySelector("img"));
        Assert.Contains(errorMessage, element.TextContent, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ErrorMessage_HoldingText_IsEncoded()
    {
        var errorMessage = "The value '<img src=x onerror=alert(1)>' is not valid for Age.";

        // Act
        var component = await _componentGenerator.GenerateErrorMessageAsync(new ErrorMessageOptions
        {
            Html = new TemplateString(errorMessage)
        });

        // Assert
        var element = HtmlHelper.ParseHtmlElement(component.GetHtml());
        Assert.Null(element.QuerySelector("img"));
        Assert.Contains(errorMessage, element.TextContent, StringComparison.Ordinal);
    }

    // Text is passed as a string because some options records type Text as string? and others as
    // TemplateString?; both mean plain text, and string converts implicitly to either.
    private async Task<string> RenderWithContentAsync(string component, string? text, TemplateString? html)
    {
        GovUkComponent generated = component switch
        {
            "back-link" => await _componentGenerator.GenerateBackLinkAsync(
                new BackLinkOptions { Text = text, Html = html, Href = "#" }),
            "button" => await _componentGenerator.GenerateButtonAsync(
                new ButtonOptions { Text = text, Html = html }),
            "details" => await _componentGenerator.GenerateDetailsAsync(
                new DetailsOptions { SummaryText = "Summary", Text = text, Html = html }),
            "error-message" => await _componentGenerator.GenerateErrorMessageAsync(
                new ErrorMessageOptions { Text = text, Html = html }),
            "error-summary" => await _componentGenerator.GenerateErrorSummaryAsync(
                new ErrorSummaryOptions { TitleText = text, TitleHtml = html }),
            "hint" => await _componentGenerator.GenerateHintAsync(
                new HintOptions { Text = text, Html = html }),
            "inset-text" => await _componentGenerator.GenerateInsetTextAsync(
                new InsetTextOptions { Text = text, Html = html }),
            "label" => await _componentGenerator.GenerateLabelAsync(
                new LabelOptions { Text = text, Html = html }),
            "notification-banner" => await _componentGenerator.GenerateNotificationBannerAsync(
                new NotificationBannerOptions { Text = text, Html = html }),
            "panel" => await _componentGenerator.GeneratePanelAsync(
                new PanelOptions { TitleText = "Title", Text = text, Html = html }),
            "tag" => await _componentGenerator.GenerateTagAsync(
                new TagOptions { Text = text, Html = html }),
            "warning-text" => await _componentGenerator.GenerateWarningTextAsync(
                new WarningTextOptions { Text = text, Html = html }),
            _ => throw new NotSupportedException($"Unknown component '{component}'.")
        };

        return generated.GetHtml();
    }
}
