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
        services.AddSingleton(listener);
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

    public static string GetAllTagNameElementsMessage(IEnumerable<string> allTagNames, string conjunction) =>
        allTagNames.Select(t => $"<{t}>").Aggregate((a, b) => $"{a} {conjunction} {b}");

    private class DummyHttpContextAccessor(IServiceProvider services) : IHttpContextAccessor
    {
        private readonly AsyncLocal<HttpContext> _current = new();

        public HttpContext? HttpContext
        {
            get
            {
                if (_current.Value is null)
                {
                    var scope = services.CreateScope();  // TODO Figure out how to dispose this

                    var httpContext = new DefaultHttpContext
                    {
                        RequestServices = scope.ServiceProvider
                    };

                    httpContext.SetEndpoint(
                        new Endpoint(
                            _ => throw new NotImplementedException(),
                            metadata: new EndpointMetadataCollection(new ActionDescriptor()),
                            displayName: "Tests"));

                    _current.Value = httpContext;
                }

                return _current.Value;
            }
            set => throw new NotSupportedException();
        }
    }
}
