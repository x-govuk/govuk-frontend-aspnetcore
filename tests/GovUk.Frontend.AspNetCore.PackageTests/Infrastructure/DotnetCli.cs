using System.Diagnostics;
using System.Text;

namespace GovUk.Frontend.AspNetCore.PackageTests.Infrastructure;

/// <summary>
/// Runs the <c>dotnet</c> CLI as a child process.
/// </summary>
public static class DotnetCli
{
    /// <summary>
    /// The MSBuild state that the surrounding build and test run leak through the environment. A nested
    /// build that inherits these picks up the wrong SDK or the wrong project extensions path.
    /// </summary>
    private static readonly string[] InheritedVariablesToClear =
    [
        "MSBuildExtensionsPath",
        "MSBuildLoadMicrosoftTargetsReadOnly",
        "MSBuildProjectFullPath",
        "MSBuildSDKsPath",
        "MSBuildStartupDirectory",
        "MSBUILD_EXE_PATH",
        "NUGET_PACKAGES",
        "VSINSTALLDIR"
    ];

    public static string ExecutablePath =>
        Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") is { Length: > 0 } path && File.Exists(path)
            ? path
            : "dotnet";

    public static async Task<DotnetCliResult> RunAsync(
        IEnumerable<string> arguments,
        string workingDirectory,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        var startInfo = CreateStartInfo(arguments, workingDirectory);

        using var process = new Process() { StartInfo = startInfo };

        var output = new StringBuilder();

        process.OutputDataReceived += AppendLine;
        process.ErrorDataReceived += AppendLine;

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        using var timeoutSource = new CancellationTokenSource(timeout ?? TimeSpan.FromMinutes(10));
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(timeoutSource.Token, cancellationToken);

        try
        {
            await process.WaitForExitAsync(linkedSource.Token);
        }
        catch (OperationCanceledException) when (timeoutSource.IsCancellationRequested)
        {
            TryKill(process);

            throw new TimeoutException(
                $"'{startInfo.FileName} {string.Join(' ', arguments)}' did not complete in time.{Environment.NewLine}{output}");
        }

        return new DotnetCliResult(process.ExitCode, output.ToString(), string.Join(' ', arguments));

        void AppendLine(object sender, DataReceivedEventArgs e)
        {
            if (e.Data is not null)
            {
                lock (output)
                {
                    output.AppendLine(e.Data);
                }
            }
        }
    }

    public static ProcessStartInfo CreateStartInfo(IEnumerable<string> arguments, string workingDirectory)
    {
        var startInfo = new ProcessStartInfo()
        {
            FileName = ExecutablePath,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        foreach (var variable in InheritedVariablesToClear)
        {
            startInfo.Environment.Remove(variable);
        }

        // Nothing in the sandbox needs the first-run experience or telemetry, and both add noise to the
        // output the tests parse.
        startInfo.Environment["DOTNET_NOLOGO"] = "1";
        startInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
        startInfo.Environment["DOTNET_SKIP_FIRST_TIME_EXPERIENCE"] = "1";

        return startInfo;
    }

    public static void TryKill(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
            }
        }
        catch (InvalidOperationException)
        {
        }
        catch (NotSupportedException)
        {
        }
    }
}

public sealed record DotnetCliResult(int ExitCode, string Output, string Arguments)
{
    public bool Succeeded => ExitCode == 0;

    public DotnetCliResult AssertSucceeded()
    {
        if (!Succeeded)
        {
            Assert.Fail($"'dotnet {Arguments}' failed with exit code {ExitCode}.{Environment.NewLine}{Output}");
        }

        return this;
    }

    public DotnetCliResult AssertFailed()
    {
        if (Succeeded)
        {
            Assert.Fail($"'dotnet {Arguments}' was expected to fail but succeeded.{Environment.NewLine}{Output}");
        }

        return this;
    }

    public void AssertHasWarning(string containing)
    {
        if (!Output.Contains(containing, StringComparison.Ordinal))
        {
            Assert.Fail($"Expected a warning containing '{containing}'.{Environment.NewLine}{Output}");
        }
    }

    public void AssertHasNoWarning(string containing)
    {
        if (Output.Contains(containing, StringComparison.Ordinal))
        {
            Assert.Fail($"Expected no warning containing '{containing}'.{Environment.NewLine}{Output}");
        }
    }
}
