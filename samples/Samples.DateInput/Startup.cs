using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;

namespace Samples.DateInput;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGovUkFrontend(options =>
        {
            options.Rebrand = true;
            options.RegisterDateInputModelConverter(typeof(LocalDate), new LocalDateDateInputModelConverter());
            options.RegisterDateInputModelConverter(typeof(YearMonth), new YearMonthDateInputModelConverter());
        });

        services.AddRazorPages()
            .AddViewOptions(options => options.HtmlHelperOptions.ClientValidationEnabled = false);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseGovUkFrontend();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
        });
    }
}
