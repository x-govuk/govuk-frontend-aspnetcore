using System.IO.Compression;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using GovUk.Frontend.AspNetCore.PackageTests.Infrastructure;

namespace GovUk.Frontend.AspNetCore.PackageTests;

/// <summary>
/// The symbol package that ships alongside the package, and the Source Link data in it that lets a
/// debugger step into the library's source.
/// </summary>
public partial class SymbolsTests(PackageTestContext context)
{
    private const string RepositoryUrl = "https://github.com/x-govuk/govuk-frontend-aspnetcore";

    private static readonly Guid SourceLinkKind = new("CC110556-A091-4D38-9FEC-25AB9A351A6A");
    private static readonly Guid EmbeddedSourceKind = new("0E8A571B-6926-466E-B4AD-8AB04611F5FE");

    private string PackagePath => Path.Combine(context.PackageFeed, $"GovUk.Frontend.AspNetCore.{context.PackageVersion}.nupkg");

    private string SymbolPackagePath => Path.ChangeExtension(PackagePath, ".snupkg");

    public static TheoryData<string> TargetFrameworks => [.. FixtureProject.AllTargetFrameworks];

    [Fact]
    public void SymbolPackage_IsPackedAlongsideThePackage()
    {
        Assert.True(File.Exists(SymbolPackagePath), $"Expected a symbol package at '{SymbolPackagePath}'.");
    }

    [Fact]
    public void Package_DoesNotContainPdbs()
    {
        // They belong in the symbol package; NuGet.org rejects a symbol package that duplicates them.
        using var package = ZipFile.OpenRead(PackagePath);

        Assert.DoesNotContain(package.Entries, e => e.FullName.EndsWith(".pdb", StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [MemberData(nameof(TargetFrameworks))]
    public void SymbolPackage_ContainsAPdbMatchingThePackagedAssembly(string targetFramework)
    {
        using var package = ZipFile.OpenRead(PackagePath);
        using var symbolPackage = ZipFile.OpenRead(SymbolPackagePath);

        var codeView = ReadCodeViewEntry(package, targetFramework);
        using var pdb = OpenPdb(symbolPackage, targetFramework);
        var pdbId = new BlobContentId(pdb.GetMetadataReader().DebugMetadataHeader!.Id);

        // A debugger won't load a PDB whose ID differs from the one the assembly records, which is what
        // happens if the two come from different builds.
        Assert.Equal(codeView.Guid, pdbId.Guid);
    }

    [Theory]
    [MemberData(nameof(TargetFrameworks))]
    public void Pdb_SourceLinkPointsAtTheRepositoryAtThePackagedCommit(string targetFramework)
    {
        using var package = ZipFile.OpenRead(PackagePath);
        using var symbolPackage = ZipFile.OpenRead(SymbolPackagePath);

        using var pdb = OpenPdb(symbolPackage, targetFramework);
        var reader = pdb.GetMetadataReader();

        var urls = ReadSourceLinkMap(reader).Values.ToArray();
        var commit = ReadRepositoryCommit(package);

        Assert.NotEmpty(urls);
        Assert.All(urls, url => Assert.StartsWith(
            $"https://raw.githubusercontent.com/x-govuk/govuk-frontend-aspnetcore/{commit}/",
            url,
            StringComparison.Ordinal));
    }

    [Theory]
    [MemberData(nameof(TargetFrameworks))]
    public void Pdb_EveryDocumentIsEitherEmbeddedOrMappedBySourceLink(string targetFramework)
    {
        using var symbolPackage = ZipFile.OpenRead(SymbolPackagePath);

        using var pdb = OpenPdb(symbolPackage, targetFramework);
        var reader = pdb.GetMetadataReader();

        var map = ReadSourceLinkMap(reader);

        var embedded = reader.CustomDebugInformation
            .Select(reader.GetCustomDebugInformation)
            .Where(cdi => reader.GetGuid(cdi.Kind) == EmbeddedSourceKind && cdi.Parent.Kind == HandleKind.Document)
            .Select(cdi => (DocumentHandle)cdi.Parent)
            .ToHashSet();

        var documents = reader.Documents.ToArray();

        Assert.NotEmpty(documents);

        // A document that's neither is one the debugger can't find. Generated sources in obj/ are untracked,
        // so they have to be embedded, and everything tracked has to fall under a Source Link mapping.
        var unresolvable = documents
            .Where(h => !embedded.Contains(h))
            .Select(h => reader.GetString(reader.GetDocument(h).Name))
            .Where(path => !map.Keys.Any(key => IsMappedBy(key, path)))
            .ToArray();

        Assert.Empty(unresolvable);
    }

    private static bool IsMappedBy(string key, string path) =>
        key.EndsWith('*')
            ? path.StartsWith(key[..^1], StringComparison.Ordinal)
            : path == key;

    private static CodeViewDebugDirectoryData ReadCodeViewEntry(ZipArchive package, string targetFramework)
    {
        using var stream = CopyToMemory(GetEntry(package, $"lib/{targetFramework}/GovUk.Frontend.AspNetCore.dll"));
        using var peReader = new PEReader(stream);

        var entry = Assert.Single(peReader.ReadDebugDirectory(), e => e.Type == DebugDirectoryEntryType.CodeView);

        return peReader.ReadCodeViewDebugDirectoryData(entry);
    }

    private static MetadataReaderProvider OpenPdb(ZipArchive symbolPackage, string targetFramework) =>
        MetadataReaderProvider.FromPortablePdbStream(
            CopyToMemory(GetEntry(symbolPackage, $"lib/{targetFramework}/GovUk.Frontend.AspNetCore.pdb")));

    private static Dictionary<string, string> ReadSourceLinkMap(MetadataReader reader)
    {
        var sourceLink = Assert.Single(
            reader.GetCustomDebugInformation(EntityHandle.ModuleDefinition),
            h => reader.GetGuid(reader.GetCustomDebugInformation(h).Kind) == SourceLinkKind);

        var json = reader.GetBlobBytes(reader.GetCustomDebugInformation(sourceLink).Value);

        using var document = JsonDocument.Parse(json);

        return document.RootElement.GetProperty("documents")
            .EnumerateObject()
            .ToDictionary(p => p.Name, p => p.Value.GetString()!);
    }

    private static string ReadRepositoryCommit(ZipArchive package)
    {
        var nuspecEntry = Assert.Single(package.Entries, e => e.FullName.EndsWith(".nuspec", StringComparison.Ordinal));

        using var stream = nuspecEntry.Open();
        var nuspec = XDocument.Load(stream);

        var repository = Assert.Single(nuspec.Descendants(), e => e.Name.LocalName == "repository");

        Assert.Equal(RepositoryUrl, repository.Attribute("url")?.Value);

        var commit = repository.Attribute("commit")?.Value;

        Assert.NotNull(commit);
        Assert.Matches(CommitShaRegex(), commit);

        return commit;
    }

    private static ZipArchiveEntry GetEntry(ZipArchive archive, string path) =>
        archive.GetEntry(path) ??
        throw new InvalidOperationException(
            $"No '{path}' in the package. It contains:{Environment.NewLine}{string.Join(Environment.NewLine, archive.Entries.Select(e => e.FullName))}");

    // Both readers need a seekable stream, which a zip entry's isn't.
    private static MemoryStream CopyToMemory(ZipArchiveEntry entry)
    {
        var memory = new MemoryStream();

        using (var stream = entry.Open())
        {
            stream.CopyTo(memory);
        }

        memory.Position = 0;

        return memory;
    }

    [GeneratedRegex("^[0-9a-f]{40}$")]
    private static partial Regex CommitShaRegex();
}
