#pragma warning disable CS0618 // Type or member is obsolete

namespace GovUk.Frontend.AspNetCore.Tests;

public class PageTemplateHelperTests
{
    [Fact]
    public void GenerateJsEnabledScript_WithNonce_AppendsNonceToScript()
    {
        // Arrange
        var pageTemplateHelper = new PageTemplateHelper();
        var nonce = "nonce123";

        // Act
        var result = pageTemplateHelper.GenerateJsEnabledScript(nonce);

        // Assert
        var element = result.RenderToElement();
        Assert.Equal(nonce, element.GetAttribute("nonce"));
    }

    [Fact]
    public void GenerateJsEnabledScript_WithoutNonce_DoesNotHaveNonceAttribute()
    {
        // Arrange
        var pageTemplateHelper = new PageTemplateHelper();

        // Act
        var result = pageTemplateHelper.GenerateJsEnabledScript(cspNonce: null);

        // Assert
        var element = result.RenderToElement();
        Assert.DoesNotContain(element.Attributes, attr => attr.Name == "nonce");
    }

    [Fact]
    public void GenerateScriptImports_WithNonce_AppendsNonceToScript()
    {
        // Arrange
        var pageTemplateHelper = new PageTemplateHelper();
        var nonce = "nonce123";

        // Act
        var result = pageTemplateHelper.GenerateScriptImports(nonce);

        // Assert
        var inlineScriptElement = result.RenderToElements().Last();
        Assert.Equal(nonce, inlineScriptElement.GetAttribute("nonce"));
    }

    [Fact]
    public void GenerateScriptImports_WithoutNonce_DoesNotHaveNonceAttribute()
    {
        // Arrange
        var pageTemplateHelper = new PageTemplateHelper();

        // Act
        var result = pageTemplateHelper.GenerateScriptImports(cspNonce: null);

        // Assert
        var inlineScriptElement = result.RenderToElements().Last();
        Assert.DoesNotContain(inlineScriptElement.Attributes, attr => attr.Name == "nonce");
    }

    [Fact]
    public void GenerateScriptImports_WithPathBase_UsesPathBaseAndVersionInScriptSrc()
    {
        // Arrange
        var pageTemplateHelper = new PageTemplateHelper();
        var pathBase = new PathString("/foo");

        // Act
        var result = pageTemplateHelper.GenerateScriptImports(pathBase, cspNonce: null);

        // Assert
        var importScriptElement = result.RenderToElements().First();
        Assert.Equal(
            $"/foo/{PageTemplateHelper.JavascriptFileName}?v={GovUkFrontendInfo.Version}",
            importScriptElement.GetAttribute("src"));
    }

    [Fact]
    public void GenerateScriptImports_WithPathBaseAndNonce_AppendsNonceToScript()
    {
        // Arrange
        var pageTemplateHelper = new PageTemplateHelper();
        var pathBase = new PathString("/foo");
        var nonce = "nonce123";

        // Act
        var result = pageTemplateHelper.GenerateScriptImports(pathBase, nonce);

        // Assert
        var inlineScriptElement = result.RenderToElements().Last();
        Assert.Equal(nonce, inlineScriptElement.GetAttribute("nonce"));
    }

    [Fact]
    public void GenerateStyleImports_RendersStylesheetLinkWithVersion()
    {
        // Arrange
        var pageTemplateHelper = new PageTemplateHelper();

        // Act
        var result = pageTemplateHelper.GenerateStyleImports();

        // Assert
        var element = result.RenderToElement();
        Assert.Equal("link", element.TagName, ignoreCase: true);
        Assert.Equal("stylesheet", element.GetAttribute("rel"));
        Assert.Equal(
            $"/{PageTemplateHelper.StylesheetFileName}?v={GovUkFrontendInfo.Version}",
            element.GetAttribute("href"));
    }

    [Fact]
    public void GenerateStyleImports_WithPathBase_UsesPathBaseAndVersionInHref()
    {
        // Arrange
        var pageTemplateHelper = new PageTemplateHelper();
        var pathBase = new PathString("/foo");

        // Act
        var result = pageTemplateHelper.GenerateStyleImports(pathBase);

        // Assert
        var element = result.RenderToElement();
        Assert.Equal(
            $"/foo/{PageTemplateHelper.StylesheetFileName}?v={GovUkFrontendInfo.Version}",
            element.GetAttribute("href"));
    }

    [Fact]
    public void GetCspScriptHashes()
    {
        // Arrange
        var pageTemplateHelper = new PageTemplateHelper();

        // Act
        var result = pageTemplateHelper.GetCspScriptHashes();

        // Assert
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GetCspScriptHashes_WithPathBase_ReturnsHashes()
    {
        // Arrange
        var pageTemplateHelper = new PageTemplateHelper();
        var pathBase = new PathString("/foo");

        // Act
        var result = pageTemplateHelper.GetCspScriptHashes(pathBase);

        // Assert
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GetJsEnabledScriptCspHash()
    {
        // Arrange
        var pageTemplateHelper = new PageTemplateHelper();

        // Act
        var result = pageTemplateHelper.GetJsEnabledScriptCspHash();

        // Assert
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GetInitScriptCspHash()
    {
        // Arrange
        var pageTemplateHelper = new PageTemplateHelper();

        // Act
        var result = pageTemplateHelper.GetInitScriptCspHash();

        // Assert
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GetInitScriptCspHash_WithPathBase_ReturnsHash()
    {
        // Arrange
        var pageTemplateHelper = new PageTemplateHelper();
        var pathBase = new PathString("/foo");

        // Act
        var result = pageTemplateHelper.GetInitScriptCspHash(pathBase);

        // Assert
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GetJavascriptFileName_WithPathBase_ReturnsPathBaseAndVersionedFileName()
    {
        // Arrange
        var pageTemplateHelper = new PageTemplateHelper();
        var pathBase = new PathString("/foo");

        // Act
        var result = pageTemplateHelper.GetJavascriptFileName(pathBase);

        // Assert
        Assert.Equal(
            $"/foo/{PageTemplateHelper.JavascriptFileName}?v={GovUkFrontendInfo.Version}",
            result);
    }

    [Fact]
    public void GetJavascriptFileName_WithHttpContext_ReturnsPathBaseAndVersionedFileName()
    {
        // Arrange
        var pageTemplateHelper = new PageTemplateHelper();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.PathBase = "/foo";

        // Act
        var result = pageTemplateHelper.GetJavascriptFileName(httpContext);

        // Assert
        Assert.Equal(
            $"/foo/{PageTemplateHelper.JavascriptFileName}?v={GovUkFrontendInfo.Version}",
            result);
    }

    [Fact]
    public void GetStylesheetFileName_WithPathBase_ReturnsPathBaseAndVersionedFileName()
    {
        // Arrange
        var pageTemplateHelper = new PageTemplateHelper();
        var pathBase = new PathString("/foo");

        // Act
        var result = pageTemplateHelper.GetStylesheetFileName(pathBase);

        // Assert
        Assert.Equal(
            $"/foo/{PageTemplateHelper.StylesheetFileName}?v={GovUkFrontendInfo.Version}",
            result);
    }

    [Fact]
    public void GetStylesheetFileName_WithHttpContext_ReturnsPathBaseAndVersionedFileName()
    {
        // Arrange
        var pageTemplateHelper = new PageTemplateHelper();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.PathBase = "/foo";

        // Act
        var result = pageTemplateHelper.GetStylesheetFileName(httpContext);

        // Assert
        Assert.Equal(
            $"/foo/{PageTemplateHelper.StylesheetFileName}?v={GovUkFrontendInfo.Version}",
            result);
    }

    [Fact]
    public void GetGovUkFrontendVersion_ReturnsVersion()
    {
        // Arrange

        // Act
        var result = PageTemplateHelper.GovUkFrontendVersion;

        // Assert
        Assert.NotNull(result);
    }
}
