using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GovUk.Frontend.AspNetCore.Tests;

internal static class TestUtils
{
    public static DefaultComponentGenerator CreateComponentGenerator() => new();

    public static Mock<DefaultComponentGenerator> CreateComponentGeneratorMock() =>
        new() { CallBase = true };

    public static ViewContext CreateViewContext() =>
        new() { HttpContext = new DefaultHttpContext() };

    public static string GetAllTagNameElementsMessage(IEnumerable<string> allTagNames, string conjunction) =>
        allTagNames.Select(t => $"<{t}>").Aggregate((a, b) => $"{a} {conjunction} {b}");
}
