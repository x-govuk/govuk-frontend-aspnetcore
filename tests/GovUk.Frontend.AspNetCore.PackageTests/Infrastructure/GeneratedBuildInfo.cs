using System.Text.RegularExpressions;

namespace GovUk.Frontend.AspNetCore.PackageTests.Infrastructure;

/// <summary>
/// The <c>GovUkFrontendBuildInfoAttribute</c> the targets emitted, read back out of the generated
/// assembly info source.
/// </summary>
/// <remarks>
/// Reading the generated C# rather than the compiled attribute is deliberate: <c>WriteCodeFragment</c>
/// emits literal parameters verbatim, so a directory containing a backslash or a quote produces source
/// that is subtly wrong, and the raw text is what makes that visible.
/// </remarks>
public sealed partial class GeneratedBuildInfo
{
    private GeneratedBuildInfo(string arguments, IReadOnlyList<string?> values)
    {
        RawArguments = arguments;
        EnableGovUkFrontendSupport = values[0] == "true";
        AssetsDirectory = values[1];
        JavaScriptDirectory = values[2];
        StylesheetDirectory = values[3];
    }

    /// <summary>The argument list exactly as it appears in the generated source.</summary>
    public string RawArguments { get; }

    public bool EnableGovUkFrontendSupport { get; }

    public string? AssetsDirectory { get; }

    public string? JavaScriptDirectory { get; }

    public string? StylesheetDirectory { get; }

    public static GeneratedBuildInfo Read(FixtureProject project, string targetFramework)
    {
        var objDirectory = Path.Combine(project.Directory, "obj", FixtureProject.Configuration, targetFramework);

        var assemblyInfoFiles = Directory.Exists(objDirectory)
            ? Directory.GetFiles(objDirectory, "*.AssemblyInfo.cs")
            : [];

        if (assemblyInfoFiles.Length != 1)
        {
            throw new InvalidOperationException(
                $"Expected exactly one generated assembly info file in '{objDirectory}' but found {assemblyInfoFiles.Length}.");
        }

        var source = File.ReadAllText(assemblyInfoFiles[0]);
        var match = AttributeRegex().Match(source);

        if (!match.Success)
        {
            throw new InvalidOperationException(
                $"No GovUkFrontendBuildInfoAttribute found in '{assemblyInfoFiles[0]}'.{Environment.NewLine}{source}");
        }

        var arguments = match.Groups[1].Value;
        var values = ParseArguments(arguments);

        if (values.Count != 4)
        {
            throw new InvalidOperationException(
                $"Expected 4 arguments to GovUkFrontendBuildInfoAttribute but found {values.Count}: {arguments}");
        }

        return new GeneratedBuildInfo(arguments, values);
    }

    /// <summary>
    /// Splits the argument list and unquotes the string literals; a bare <c>null</c> becomes
    /// <see langword="null"/>.
    /// </summary>
    private static List<string?> ParseArguments(string arguments)
    {
        var values = new List<string?>();

        foreach (var argument in arguments.Split(',').Select(a => a.Trim()))
        {
            if (argument == "null")
            {
                values.Add(null);
            }
            else if (argument.StartsWith('"') && argument.EndsWith('"'))
            {
                // The generated source escapes backslashes; undo that so the value can be compared with
                // the directory the project asked for.
                values.Add(argument[1..^1].Replace("\\\\", "\\", StringComparison.Ordinal));
            }
            else
            {
                values.Add(argument);
            }
        }

        return values;
    }

    [GeneratedRegex(@"GovUkFrontendBuildInfoAttribute\(([^)]*)\)")]
    private static partial Regex AttributeRegex();
}
