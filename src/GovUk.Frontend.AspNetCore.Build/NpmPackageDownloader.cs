using System.Diagnostics;
using System.Security.Cryptography;
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
        var version = typeof(NpmPackageDownloader).Assembly.GetName().Version!.ToString();

        _httpClient = new();
        _httpClient.DefaultRequestHeaders.UserAgent.Add(new("GovUk.Frontend.AspNetCore.Build", version));
    }

    public async Task DownloadPackage(
        string package,
        string version,
        string packageBaseDirectory,
        string destinationDirectory,
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
            async (r, ct) =>
            {
                var hash = ((NpmPackageFileInfo)contents.GetFile(r.Path)!).Hash;
                await DownloadFileAsync(package, version, r.Path, hash, packageBaseDirectory, destinationDirectory, ct);
            });
    }

    private async Task DownloadFileAsync(
        string package,
        string version,
        string fileName,
        string hash,
        string packageBaseDirectory,
        string destinationRootDirectory,
        CancellationToken cancellationToken)
    {
        var destinationPath = Path.Combine(destinationRootDirectory, fileName);
        var destinationDirectory = Path.GetDirectoryName(destinationPath)!;

        if (File.Exists(destinationPath))
        {
            await using var efs = File.OpenRead(destinationPath);
            using var sha = SHA256.Create();
            var currentFileHash = Convert.ToBase64String(await sha.ComputeHashAsync(efs, cancellationToken));

            if (currentFileHash == hash)
            {
                return;
            }
        }

        var response = await _httpClient.GetAsync(
            $"https://cdn.jsdelivr.net/npm/{Uri.EscapeDataString(package)}@{Uri.EscapeDataString(version)}{packageBaseDirectory}{fileName}",
            cancellationToken);

        response.EnsureSuccessStatusCode();

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
                var hash = element.GetProperty("hash").GetString()!;
                return new NpmPackageFileInfo(name, fullName, hash, parent);
            }
            else
            {
                Debug.Assert(type == "directory");

                return CreateDirectory(element, name, fullName, parent);
            }
        }
    }

    public void Dispose() => _httpClient.Dispose();

    private class NpmPackageFileInfo(string name, string fullName, string hash, DirectoryInfoBase? parentDirectory) : FileInfoBase
    {
        public override string Name => name;

        public override string FullName => fullName;

        public string Hash => hash;

        public override DirectoryInfoBase? ParentDirectory => parentDirectory;
    }

    private class NpmPackageDirectoryInfo(string name, string fullName, DirectoryInfoBase? parentDirectory, IEnumerable<FileSystemInfoBase> children) : DirectoryInfoBase
    {
        public override string Name => name;

        public override string FullName => fullName;

        public override DirectoryInfoBase? ParentDirectory => parentDirectory;

        public override IEnumerable<FileSystemInfoBase> EnumerateFileSystemInfos() => children;

        public override DirectoryInfoBase? GetDirectory(string path) => Find(path) as DirectoryInfoBase;

        public override FileInfoBase? GetFile(string path) => Find(path) as FileInfoBase;

        private FileSystemInfoBase? Find(string path)
        {
            var parts = path.TrimStart(PathSeparator).TrimEnd(PathSeparator).Split(PathSeparator);
            return Find(this, parts);
        }

        private static FileSystemInfoBase? Find(DirectoryInfoBase directory, string[] pathParts)
        {
            var head = pathParts.First();
            var tail = pathParts[1..];

            foreach (var child in directory.EnumerateFileSystemInfos())
            {
                if (child.Name == head)
                {
                    return tail.Length == 0 ? child : Find((DirectoryInfoBase)child, tail);
                }
            }

            return null;
        }
    }
}
