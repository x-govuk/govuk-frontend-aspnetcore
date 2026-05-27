using GovUk.Frontend.AspNetCore.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace GovUk.Frontend.AspNetCore.Tests;

public class TestCompositeMetadataDetailsProvider : ICompositeMetadataDetailsProvider
{
    private readonly Dictionary<ModelMetadataIdentity, Entry> _entries = [];

    public void SetDisplayNameForProperty(ModelMetadataIdentity identity, string? displayName)
    {
        _entries.TryAdd(identity, new Entry());
        _entries[identity].DisplayName = displayName;
    }

    public void SetDescriptionForProperty(ModelMetadataIdentity identity, string? description)
    {
        _entries.TryAdd(identity, new Entry());
        _entries[identity].Description = description;
    }

    public void SetDateInputErrorMessagePrefixForProperty(ModelMetadataIdentity identity, string? errorMessagePrefix)
    {
        _entries.TryAdd(identity, new Entry());
        _entries[identity].DateInputErrorMessagePrefix = errorMessagePrefix;
    }

    public void SetDateInputItemTypesForProperty(ModelMetadataIdentity identity, DateInputItemTypes? itemTypes)
    {
        _entries.TryAdd(identity, new Entry());
        _entries[identity].DateInputItemTypes = itemTypes;
    }

    public void CreateBindingMetadata(BindingMetadataProviderContext context)
    {
    }

    public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
    {
        if (!_entries.TryGetValue(context.Key, out var entry))
        {
            return;
        }

        context.DisplayMetadata.DisplayName = () => entry.DisplayName;
        context.DisplayMetadata.Description = () => entry.Description;

        context.DisplayMetadata.AdditionalValues[typeof(DateInputModelMetadata)] = new DateInputModelMetadata()
        {
            ErrorMessagePrefix = entry.DateInputErrorMessagePrefix,
            ItemTypes = entry.DateInputItemTypes
        };
    }

    public void CreateValidationMetadata(ValidationMetadataProviderContext context)
    {
    }

    private record Entry
    {
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public string? DateInputErrorMessagePrefix { get; set; }
        public DateInputItemTypes? DateInputItemTypes { get; set; }
    }
}
