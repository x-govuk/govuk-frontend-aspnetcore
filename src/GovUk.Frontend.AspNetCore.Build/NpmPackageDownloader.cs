using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace GovUk.Frontend.AspNetCore.Build;

public sealed class NpmPackageDownloader : IDisposable
{
    private const char PathSeparator = '/';

    private readonly HttpClient _httpClient;

    public NpmPackageDownloader()
    {
        _httpClient = new();
        _httpClient.DefaultRequestHeaders.UserAgent.Add(new("GovUk.Frontend.AspNetCore.Build", "1.0.0"));
    }

    public async Task DownloadPackage(
        string package,
        string version,
        string destinationDirectory,
        string packageBaseDirectory,
        IEnumerable<string> includePatterns)
    {
        ArgumentNullException.ThrowIfNull(package);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(destinationDirectory);

        Directory.CreateDirectory(destinationDirectory);

        var contents = await GetPackageContentsAsync(package, version);

        packageBaseDirectory = packageBaseDirectory.TrimStart('/').TrimEnd('/');
        if (!packageBaseDirectory.StartsWith(PathSeparator))
        {
            packageBaseDirectory = PathSeparator + packageBaseDirectory;
        }
        if (!packageBaseDirectory.EndsWith(PathSeparator))
        {
            packageBaseDirectory = packageBaseDirectory + PathSeparator;
        }

        if (packageBaseDirectory != $"{PathSeparator}")
        {
            contents = contents.GetDirectory(packageBaseDirectory);

            if (contents is null)
            {
                return;
            }
        }

        var matcher = new Matcher();
        matcher.AddIncludePatterns(includePatterns);

        var results = matcher.Execute(contents);

        await Parallel.ForEachAsync(
            results.Files,
            async (r, ct) => await DownloadFileAsync(package, version, r.Path, packageBaseDirectory, destinationDirectory, ct));
    }

    private async Task DownloadFileAsync(
        string package,
        string version,
        string fileName,
        string packageBaseDirectory,
        string destinationRootDirectory,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(
            $"https://cdn.jsdelivr.net/npm/{Uri.EscapeDataString(package)}@{Uri.EscapeDataString(version)}{packageBaseDirectory}{fileName}",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var destinationPath = Path.Combine(destinationRootDirectory, fileName);
        var destinationDirectory = Path.GetDirectoryName(destinationPath)!;

        Directory.CreateDirectory(destinationDirectory);

        await using var fs = File.Create(destinationPath);
        await response.Content.CopyToAsync(fs, cancellationToken);
    }

    private async Task<DirectoryInfoBase> GetPackageContentsAsync(string package, string version)
    {
        var response = await _httpClient.GetAsync(
            $"https://data.jsdelivr.com/v1/packages/npm/{Uri.EscapeDataString(package)}@{Uri.EscapeDataString(version)}");

        response.EnsureSuccessStatusCode();

        var responseJson = JsonSerializer.Deserialize<JsonElement>(
            await response.Content.ReadAsStringAsync());

        var root = CreateDirectory(responseJson, name: "", fullName: "/", parent: null);

        return root;

        static NpmPackageDirectoryInfo CreateDirectory(
            JsonElement element,
            string name,
            string fullName,
            NpmPackageDirectoryInfo? parent)
        {
            var children = new List<FileSystemInfoBase>();
            var directory = new NpmPackageDirectoryInfo(name, fullName, parent, children);

            foreach (var file in element.GetProperty("files").EnumerateArray())
            {
                children.Add(CreateFileSystemInfoBase(file, directory, fullName));
            }

            return new NpmPackageDirectoryInfo(name, fullName, directory, children);
        }

        static FileSystemInfoBase CreateFileSystemInfoBase(
            JsonElement element,
            NpmPackageDirectoryInfo parent,
            string path)
        {
            var name = element.GetProperty("name").GetString()!;
            var fullName = path.TrimEnd(PathSeparator) + PathSeparator + name;
            var type = element.GetProperty("type").GetString()!;

            if (type == "file")
            {
                return new NpmPackageFileInfo(name, fullName, parent);
            }
            else
            {
                Debug.Assert(type == "directory");

                return CreateDirectory(element, name, fullName, parent);
            }
        }
    }

    public void Dispose() => _httpClient.Dispose();

    private class NpmPackageFileInfo(string name, string fullName, DirectoryInfoBase? parentDirectory) : FileInfoBase
    {
        public override string Name => name;

        public override string FullName => fullName;

        public override DirectoryInfoBase? ParentDirectory => parentDirectory;
    }

    private class NpmPackageDirectoryInfo(string name, string fullName, DirectoryInfoBase? parentDirectory, IEnumerable<FileSystemInfoBase> children) : DirectoryInfoBase
    {
        public override string Name => name;

        public override string FullName => fullName;

        public override DirectoryInfoBase? ParentDirectory => parentDirectory;

        public override IEnumerable<FileSystemInfoBase> EnumerateFileSystemInfos() => children;

        public override DirectoryInfoBase? GetDirectory(string path)
        {
            var parts = path.TrimStart(PathSeparator).TrimEnd(PathSeparator).Split(PathSeparator);

            var firstPart = parts[0];
            NpmPackageDirectoryInfo? result = null;

            foreach (var child in children)
            {
                if (child.Name == firstPart)
                {
                    if (child is NpmPackageDirectoryInfo directoryInfo)
                    {
                        result = directoryInfo;
                        break;
                    }

                    return null;
                }
            }

            if (result is null)
            {
                return null;
            }

            if (parts.Length > 1)
            {
                return result.GetDirectory(string.Join(PathSeparator, parts.Skip(1)));
            }

            return result;
        }

        public override FileInfoBase? GetFile(string path)
        {
            throw new NotSupportedException();
        }
    }
}
