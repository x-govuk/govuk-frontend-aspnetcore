namespace GovUk.Frontend.AspNetCore.IntegrationTests.TagHelperModelBindingTests;

public class TagHelperModelBindingTestsFixture : ServerFixture
{
    protected override void Configure(IApplicationBuilder app)
    {
        base.Configure(app);

        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddGovUkFrontend();

        services
            .AddMvc()
            .AddRazorOptions(options =>
            {
                options.ViewLocationFormats.Add("/TagHelperModelBindingTests/{0}.cshtml");
            });
    }
}
