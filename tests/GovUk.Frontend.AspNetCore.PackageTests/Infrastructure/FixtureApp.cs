using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace GovUk.Frontend.AspNetCore.PackageTests.Infrastructure;

/// <summary>
/// A built fixture app, running as a child process so requests go through the real host, the real
/// static file handling and the real middleware pipeline.
/// </summary>
public sealed partial class FixtureApp : IAsyncDisposable
{
    private static readonly TimeSpan StartupTimeout = TimeSpan.FromSeconds(90);

    private readonly Process _process;
    private readonly StringBuilder _output;
    private bool _disposed;

    private FixtureApp(Process process, StringBuilder output, Uri baseAddress)
    {
        _process = process;
        _output = output;
        BaseAddress = baseAddress;

        Client = new HttpClient(new HttpClientHandler() { AllowAutoRedirect = false })
        {
            BaseAddress = baseAddress,
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    public Uri BaseAddress { get; }

    public HttpClient Client { get; }

    /// <summary>Everything the app has written to stdout and stderr so far.</summary>
    public string Output
    {
        get
        {
            lock (_output)
            {
                return _output.ToString();
            }
        }
    }

    public static async Task<FixtureApp> StartAsync(FixtureProject project, string targetFramework)
    {
        var assemblyPath = project.AssemblyPathFor(targetFramework);

        if (!File.Exists(assemblyPath))
        {
            throw new InvalidOperationException($"'{assemblyPath}' does not exist; build the project first.");
        }

        // The project directory is the content root when you 'dotnet run', which is what puts wwwroot in
        // the right place.
        var startInfo = DotnetCli.CreateStartInfo([assemblyPath], project.Directory);
        startInfo.Environment["ASPNETCORE_URLS"] = "http://127.0.0.1:0";
        startInfo.Environment["ASPNETCORE_ENVIRONMENT"] = "Development";

        var output = new StringBuilder();
        var listening = new TaskCompletionSource<Uri>(TaskCreationOptions.RunContinuationsAsynchronously);

        var process = new Process() { StartInfo = startInfo, EnableRaisingEvents = true };

        process.OutputDataReceived += OnOutput;
        process.ErrorDataReceived += OnOutput;
        process.Exited += (_, _) => listening.TrySetException(
            new InvalidOperationException($"The app exited with code {process.ExitCode} before it started listening.{Environment.NewLine}{output}"));

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        try
        {
            var baseAddress = await listening.Task.WaitAsync(StartupTimeout);
            return new FixtureApp(process, output, baseAddress);
        }
        catch (Exception ex)
        {
            DotnetCli.TryKill(process);
            process.Dispose();

            throw ex is TimeoutException
                ? new TimeoutException($"The app did not start listening within {StartupTimeout}.{Environment.NewLine}{output}")
                : ex;
        }

        void OnOutput(object sender, DataReceivedEventArgs e)
        {
            if (e.Data is null)
            {
                return;
            }

            lock (output)
            {
                output.AppendLine(e.Data);
            }

            // Kestrel binds an ephemeral port, so the only way to know it is to read it back.
            if (ListeningRegex().Match(e.Data) is { Success: true } match)
            {
                listening.TrySetResult(new Uri(match.Groups[1].Value));
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        Client.Dispose();

        DotnetCli.TryKill(_process);

        try
        {
            await _process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(15));
        }
        catch (TimeoutException)
        {
        }

        _process.Dispose();
    }

    [GeneratedRegex(@"Now listening on:\s*(http://\S+)")]
    private static partial Regex ListeningRegex();
}
