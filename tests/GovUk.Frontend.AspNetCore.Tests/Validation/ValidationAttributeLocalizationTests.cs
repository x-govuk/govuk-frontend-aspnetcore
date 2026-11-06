using System.ComponentModel.DataAnnotations;
using System.Globalization;
using GovUk.Frontend.AspNetCore.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace GovUk.Frontend.AspNetCore.Tests.Validation;

public class ValidationAttributeLocalizationTests
{
    private static readonly Dictionary<string, string> _resources = new()
    {
        ["TooLong"] = "Rhaid i {0} fod yn {1} nod neu lai",
        ["TooManyWords"] = "Rhaid i {0} fod yn {1} gair neu lai"
    };

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CharacterCount_WithDataAnnotationsLocalization_LocalizesErrorMessage(bool addGovUkFrontendFirst)
    {
        // Arrange
        var services = new ServiceCollection();
        AddServices(services, addGovUkFrontendFirst);
        services.AddSingleton<IStringLocalizerFactory>(new DictionaryStringLocalizerFactory(_resources));

        // Act
        var errorMessage = GetErrorMessage(services, new LocalizedModel { Characters = "abc" }, nameof(LocalizedModel.Characters));

        // Assert
        Assert.Equal("Rhaid i Job description fod yn 2 nod neu lai", errorMessage);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void MaxWords_WithDataAnnotationsLocalization_LocalizesErrorMessage(bool addGovUkFrontendFirst)
    {
        // Arrange
        var services = new ServiceCollection();
        AddServices(services, addGovUkFrontendFirst);
        services.AddSingleton<IStringLocalizerFactory>(new DictionaryStringLocalizerFactory(_resources));

        // Act
        var errorMessage = GetErrorMessage(services, new LocalizedModel { Words = "one two" }, nameof(LocalizedModel.Words));

        // Assert
        Assert.Equal("Rhaid i Job summary fod yn 1 gair neu lai", errorMessage);
    }

    [Fact]
    public void WithoutLocalization_FormatsErrorMessageWithDisplayNameAndLimit()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddControllersWithViews();
        services.AddGovUkFrontend();

        // Act
        var characterCountMessage = GetErrorMessage(services, new TemplatedModel { Characters = "abc" }, nameof(TemplatedModel.Characters));
        var maxWordsMessage = GetErrorMessage(services, new TemplatedModel { Words = "one two" }, nameof(TemplatedModel.Words));

        // Assert
        Assert.Equal("Characters must be 2 or fewer", characterCountMessage);
        Assert.Equal("Words must be 1 or fewer", maxWordsMessage);
    }

    [Fact]
    public void ExistingAdapterProvider_IsUsedForOtherAttributes()
    {
        // Arrange
        var attribute = new RequiredAttribute();
        var adapter = Mock.Of<IAttributeAdapter>();
        var existingProvider = new Mock<IValidationAttributeAdapterProvider>();
        existingProvider.Setup(p => p.GetAttributeAdapter(attribute, null)).Returns(adapter);

        var services = new ServiceCollection();
        services.AddSingleton(existingProvider.Object);
        services.AddGovUkFrontend();

        // Act
        var provider = services.BuildServiceProvider().GetRequiredService<IValidationAttributeAdapterProvider>();

        // Assert
        Assert.Same(adapter, provider.GetAttributeAdapter(attribute, stringLocalizer: null));
        Assert.NotNull(provider.GetAttributeAdapter(new MaxWordsAttribute(1), stringLocalizer: null));
    }

    private static void AddServices(IServiceCollection services, bool addGovUkFrontendFirst)
    {
        services.AddLogging();

        if (addGovUkFrontendFirst)
        {
            services.AddGovUkFrontend();
        }

        services.AddControllersWithViews().AddDataAnnotationsLocalization();

        if (!addGovUkFrontendFirst)
        {
            services.AddGovUkFrontend();
        }
    }

    private static string GetErrorMessage(IServiceCollection services, object model, string propertyName)
    {
        var serviceProvider = services.BuildServiceProvider();
        var httpContext = new DefaultHttpContext { RequestServices = serviceProvider };
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

        serviceProvider.GetRequiredService<IObjectModelValidator>()
            .Validate(actionContext, validationState: null, prefix: string.Empty, model);

        return Assert.Single(actionContext.ModelState[propertyName]!.Errors).ErrorMessage;
    }

    private class LocalizedModel
    {
        [Display(Name = "Job description")]
        [CharacterCount(2, ErrorMessage = "TooLong")]
        public string? Characters { get; set; }

        [Display(Name = "Job summary")]
        [MaxWords(1, ErrorMessage = "TooManyWords")]
        public string? Words { get; set; }
    }

    private class TemplatedModel
    {
        [CharacterCount(2, ErrorMessage = "{0} must be {1} or fewer")]
        public string? Characters { get; set; }

        [MaxWords(1, ErrorMessage = "{0} must be {1} or fewer")]
        public string? Words { get; set; }
    }

    private sealed class DictionaryStringLocalizerFactory(IReadOnlyDictionary<string, string> resources) : IStringLocalizerFactory
    {
        public IStringLocalizer Create(Type resourceSource) => new DictionaryStringLocalizer(resources);

        public IStringLocalizer Create(string baseName, string location) => new DictionaryStringLocalizer(resources);
    }

    private sealed class DictionaryStringLocalizer(IReadOnlyDictionary<string, string> resources) : IStringLocalizer
    {
        public LocalizedString this[string name] => new(name, resources.GetValueOrDefault(name, name));

        public LocalizedString this[string name, params object[] arguments] =>
            new(name, string.Format(CultureInfo.InvariantCulture, this[name].Value, arguments));

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => [];
    }
}
