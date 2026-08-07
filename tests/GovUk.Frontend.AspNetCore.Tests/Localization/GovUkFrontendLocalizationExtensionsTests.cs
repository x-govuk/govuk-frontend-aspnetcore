using System.Globalization;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.Localization;
using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.Localization;

public class GovUkFrontendLocalizationExtensionsTests
{
    [Fact]
    public void AddGovUkFrontend_WithNoLocalization_RegistersTheNullLocalizer()
    {
        var services = new ServiceCollection();
        services.AddGovUkFrontend();

        var localizer = services.BuildServiceProvider().GetRequiredService<IGovUkFrontendLocalizer>();

        Assert.Same(NullGovUkFrontendLocalizer.Instance, localizer);
    }

    [Fact]
    public void AddGovUkFrontendLocalization_CalledAfterAddGovUkFrontend_Wins()
    {
        var services = new ServiceCollection();
        services.AddGovUkFrontend();
        services.AddGovUkFrontendLocalization(_ => DelegateLocalizer.ForAllNames("x"));

        var localizer = services.BuildServiceProvider().GetRequiredService<IGovUkFrontendLocalizer>();

        Assert.IsType<DelegateLocalizer>(localizer);
    }

    [Fact]
    public void AddGovUkFrontendLocalization_CalledBeforeAddGovUkFrontend_Wins()
    {
        var services = new ServiceCollection();
        services.AddGovUkFrontendLocalization(_ => DelegateLocalizer.ForAllNames("x"));
        services.AddGovUkFrontend();

        var localizer = services.BuildServiceProvider().GetRequiredService<IGovUkFrontendLocalizer>();

        Assert.IsType<DelegateLocalizer>(localizer);
    }

    [Fact]
    public void AddSingleton_OverridesTheDefault()
    {
        var services = new ServiceCollection();
        services.AddGovUkFrontend();
        services.AddSingleton<IGovUkFrontendLocalizer>(DelegateLocalizer.ForAllNames("x"));

        var localizer = services.BuildServiceProvider().GetRequiredService<IGovUkFrontendLocalizer>();

        Assert.IsType<DelegateLocalizer>(localizer);
    }

    [Theory]
    [InlineData(typeof(TitleTagHelper))]
    [InlineData(typeof(ErrorSummaryTagHelper))]
    [InlineData(typeof(GeneratedErrorSummaryTagHelper))]
    public void TagHelpersWithALocalizerConstructor_CanBeActivated(Type tagHelperType)
    {
        // These tag helpers take the localizer, so AddGovUkFrontend has to register one for MVC to be
        // able to create them at render time.
        var services = new ServiceCollection();
        services.AddGovUkFrontend();
        var serviceProvider = services.BuildServiceProvider();

        var tagHelper = ActivatorUtilities.CreateInstance(serviceProvider, tagHelperType);

        Assert.IsType(tagHelperType, tagHelper);
    }

    [Fact]
    public async Task AddGovUkFrontendLocalization_UsesResourcesForTheCurrentUICulture()
    {
        var services = new ServiceCollection();
        services.AddGovUkFrontend();
        services.AddGovUkFrontendLocalization<TestResources>();
        var componentGenerator = services.BuildServiceProvider().GetRequiredService<IComponentGenerator>();

        using (new CultureScope("cy"))
        {
            var component = await componentGenerator.GenerateErrorMessageAsync(new ErrorMessageOptions { Text = "Neges" });

            Assert.Contains("Gwall", component.GetContent().ToHtmlString(), StringComparison.Ordinal);
        }
    }

    [Fact]
    public async Task AddGovUkFrontendLocalization_WithNoResourceForTheName_FallsBackToEnglish()
    {
        // TestResources.cy.resx deliberately has no entry for the warning text, so the built-in
        // English must still be used rather than the resource name leaking into the output.
        var services = new ServiceCollection();
        services.AddGovUkFrontend();
        services.AddGovUkFrontendLocalization<TestResources>();
        var componentGenerator = services.BuildServiceProvider().GetRequiredService<IComponentGenerator>();

        using (new CultureScope("cy"))
        {
            var component = await componentGenerator.GenerateWarningTextAsync(new WarningTextOptions { Text = "Byddwch yn ofalus" });
            var html = component.GetContent().ToHtmlString();

            Assert.Contains("Warning", html, StringComparison.Ordinal);
            Assert.DoesNotContain(GovUkFrontendResourceNames.WarningTextIconFallbackText, html, StringComparison.Ordinal);
        }
    }

    [Fact]
    public async Task AddGovUkFrontendLocalization_WithAnEnglishCulture_UsesTheBuiltInContent()
    {
        var services = new ServiceCollection();
        services.AddGovUkFrontend();
        services.AddGovUkFrontendLocalization<TestResources>();
        var componentGenerator = services.BuildServiceProvider().GetRequiredService<IComponentGenerator>();

        using (new CultureScope("en-GB"))
        {
            var component = await componentGenerator.GenerateErrorMessageAsync(new ErrorMessageOptions { Text = "Message" });

            Assert.Contains("Error", component.GetContent().ToHtmlString(), StringComparison.Ordinal);
        }
    }

    /// <summary>
    /// Sets <see cref="CultureInfo.CurrentUICulture"/> for the duration of a test. It's stored in an
    /// AsyncLocal and flows with the ExecutionContext, so this doesn't affect tests running in parallel.
    /// </summary>
    private sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo _originalUICulture = CultureInfo.CurrentUICulture;

        public CultureScope(string name) => CultureInfo.CurrentUICulture = new CultureInfo(name);

        public void Dispose() => CultureInfo.CurrentUICulture = _originalUICulture;
    }
}
