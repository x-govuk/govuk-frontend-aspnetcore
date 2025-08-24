using System.Diagnostics;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GovUk.Frontend.AspNetCore.Tests;

internal static class TestUtils
{
    private static readonly ServiceProvider _serviceProvider;

    static TestUtils()
    {
        var services = new ServiceCollection();
        services
            .AddMvc()
            .AddApplicationPart(typeof(GovUkFrontendExtensions).Assembly);
        var listener = new DiagnosticListener("Microsoft.AspNetCore");
        services.AddSingleton<DiagnosticListener>(listener);
        services.AddSingleton<DiagnosticSource>(listener);
        _serviceProvider = services.BuildServiceProvider();
    }

    public static IRazorViewEngine RazorViewEngine =>
        _serviceProvider.GetRequiredService<IRazorViewEngine>();

    public static IHttpContextAccessor HttpContextAccessor =>
        new DummyHttpContextAccessor(_serviceProvider);

    public static DefaultComponentGenerator CreateComponentGenerator() =>
        new(
            RazorViewEngine,
            HttpContextAccessor);

    public static Mock<DefaultComponentGenerator> CreateComponentGeneratorMock() =>
        new(RazorViewEngine, HttpContextAccessor) { CallBase = true };

    public static ViewContext CreateViewContext() =>
        new() { HttpContext = new DefaultHttpContext() };

    private class DummyHttpContextAccessor : IHttpContextAccessor
    {
        private readonly HttpContext? _httpContext;

        public DummyHttpContextAccessor(IServiceProvider services)
        {
            _httpContext = new DefaultHttpContext();
            _httpContext.RequestServices = services;
            _httpContext.SetEndpoint(
                new Endpoint(
                    _ => throw new NotImplementedException(),
                    metadata: new EndpointMetadataCollection(new ActionDescriptor()),
                    displayName: "Tests"));
        }

        public HttpContext? HttpContext
        {
            get => _httpContext;
            set => throw new NotSupportedException();
        }
    }
}
