using System.Text;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace GovUk.Frontend.AspNetCore.ConformanceTests;

public class StringFileProvider : IFileProvider
{
    private readonly Dictionary<string, (string Value, DateTimeOffset Created)> _values;

    public StringFileProvider()
    {
        _values = [];
    }

    public void Add(string path, string value)
    {
        if (path is null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        if (!path.StartsWith("/"))
        {
            throw new ArgumentException($"{nameof(path)} must start with '/'.", nameof(path));
        }

        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        _values.Add(path, (value, DateTimeOffset.Now));
    }

    public IDirectoryContents GetDirectoryContents(string subpath) => new NotFoundDirectoryContents();

    public IFileInfo GetFileInfo(string subpath)
    {
        return _values.TryGetValue(subpath, out var entry)
            ? new StringFileInfo(subpath, subpath, entry.Value, entry.Created)
            : new NotFoundFileInfo(subpath);
    }

    public IChangeToken Watch(string filter) => NullChangeToken.Singleton;

    private class StringFileInfo(string name, string physicalPath, string value, DateTimeOffset created) : IFileInfo
    {
        private readonly string _value = value;

        public bool Exists => true;

        public long Length => _value.Length;

        public string PhysicalPath { get; } = physicalPath;

        public string Name { get; } = name;

        public DateTimeOffset LastModified { get; } = created;

        public bool IsDirectory => false;

        public Stream CreateReadStream()
        {
            var bytes = Encoding.UTF8.GetBytes(_value);

            var ms = new MemoryStream();
            ms.Write(bytes);
            ms.Seek(0L, SeekOrigin.Begin);

            return ms;
        }
    }
}
