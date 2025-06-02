using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.HtmlGeneration;
using GovUk.Frontend.AspNetCore.ModelBinding;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace GovUk.Frontend.AspNetCore;

/// <summary>
/// Extension methods for setting up GovUk.Frontend.AspNetCore.
/// </summary>
public static class GovUkFrontendExtensions
{
    /// <summary>
    /// Adds GovUk.Frontend.AspNetCore services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddGovUkFrontend(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return AddGovUkFrontend(services, _ => { });
    }

    /// <summary>
    /// Adds GovUk.Frontend.AspNetCore services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="setupAction">An <see cref="Action{GovUkFrontendOptions}"/> to configure the provided <see cref="GovUkFrontendOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddGovUkFrontend(
        this IServiceCollection services,
        Action<GovUkFrontendOptions> setupAction)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(setupAction);

        services.AddMvcCore();

        services.TryAddSingleton<IGovUkHtmlGenerator, ComponentGenerator>();
        services.TryAddSingleton<IComponentGenerator, DefaultComponentGenerator>();
        services.TryAddSingleton<IModelHelper, DefaultModelHelper>();
        services.AddSingleton<IStartupFilter, GovUkFrontendStartupFilter>();
        services.AddSingleton<IConfigureOptions<MvcOptions>, ConfigureMvcOptions>();
        services.AddTransient<PageTemplateHelper>();
        services.AddSingleton<ITagHelperInitializer<ButtonTagHelper>, ButtonTagHelperInitializer>();
        services.AddSingleton<ITagHelperInitializer<FileUploadTagHelper>, FileUploadTagHelperInitializer>();

        services.Configure(setupAction);

        return services;
    }

    private class ConfigureMvcOptions : IConfigureOptions<MvcOptions>
    {
        private readonly IOptions<GovUkFrontendOptions> _optionsAccessor;

        public ConfigureMvcOptions(IOptions<GovUkFrontendOptions> optionsAccessor)
        {
            ArgumentNullException.ThrowIfNull(optionsAccessor);
            _optionsAccessor = optionsAccessor;
        }

        public void Configure(MvcOptions options)
        {
            options.ModelBinderProviders.Insert(2, new DateInputModelBinderProvider(_optionsAccessor));
            options.ModelMetadataDetailsProviders.Add(new GovUkFrontendMetadataDetailsProvider());
        }
    }
}
