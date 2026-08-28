using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class LanguageNavigationItemTagHelperTests : TagHelperTestBase<LanguageNavigationItemTagHelper>
{
    [Fact]
    public async Task ProcessAsync_AddsItemToContext()
    {
        // Arrange
        var content = "Cymraeg";
        var href = "/cy";
        var lang = "cy";
        var hrefLang = "cy-GB";
        var dir = "ltr";
        var languageDescriptionText = "Newid yr iaith i'r Cymraeg";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();

        var languageNavigationContext = new LanguageNavigationContext();

        var context = CreateTagHelperContext(
            className: className,
            attributes: attributes,
            contexts: languageNavigationContext);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: new Dictionary<string, string?>(attributes) { { "href", href } },
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new LanguageNavigationItemTagHelper()
        {
            Lang = lang,
            HrefLang = hrefLang,
            Dir = dir,
            LanguageDescriptionText = languageDescriptionText
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var item = Assert.Single(languageNavigationContext.Items);
        Assert.Equal(content, item.Html?.ToHtmlString());
        Assert.Equal(href, item.Href);
        Assert.Equal(lang, item.Lang);
        Assert.Equal(hrefLang, item.HrefLang);
        Assert.Equal(dir, item.Dir);
        Assert.Equal(languageDescriptionText, item.LanguageDescriptionText);
        Assert.Null(item.Current);
        Assert.Equal(className, item.Classes);
        AssertContainsAttributes(attributes, item.Attributes);
        Assert.DoesNotContain(item.Attributes!, a => a.Key == "href");
    }

    [Fact]
    public async Task ProcessAsync_WithCurrent_AddsItemToContextWithCurrent()
    {
        // Arrange
        var languageNavigationContext = new LanguageNavigationContext();

        var context = CreateTagHelperContext(contexts: languageNavigationContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("English");
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new LanguageNavigationItemTagHelper()
        {
            Lang = "en",
            Current = true
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var item = Assert.Single(languageNavigationContext.Items);
        Assert.True(item.Current);
        Assert.Null(item.Href);
    }
}
