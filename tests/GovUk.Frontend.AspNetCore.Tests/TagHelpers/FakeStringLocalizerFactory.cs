using Microsoft.Extensions.Localization;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

/// <summary>
/// An <see cref="IStringLocalizerFactory"/> whose localizers prefix every string with the name of the resource type.
/// </summary>
public class FakeStringLocalizerFactory : IStringLocalizerFactory
{
    public List<Type> RequestedTypes { get; } = [];

    public IStringLocalizer Create(Type resourceSource)
    {
        RequestedTypes.Add(resourceSource);
        return new Localizer(resourceSource.Name);
    }

    public IStringLocalizer Create(string baseName, string location) => new Localizer(baseName);

    private sealed class Localizer(string prefix) : IStringLocalizer
    {
        public LocalizedString this[string name] => new(name, $"{prefix}:{name}");

        public LocalizedString this[string name, params object[] arguments] => this[name];

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => [];
    }
}
