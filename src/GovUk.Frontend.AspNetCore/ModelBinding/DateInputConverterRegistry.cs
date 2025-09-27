namespace GovUk.Frontend.AspNetCore.ModelBinding;

internal class DateInputConverterRegistry
{
    private readonly Dictionary<Type, Entry> _entries = [];

    public DateInputModelConverter? FindConverter(Type modelType)
    {
        ArgumentNullException.ThrowIfNull(modelType);

        return _entries.GetValueOrDefault(modelType)?.Converter;
    }

    public void RegisterConverter(Type modelType, DateInputModelConverter converter)
    {
        ArgumentNullException.ThrowIfNull(modelType);
        ArgumentNullException.ThrowIfNull(converter);

        if (converter.DefaultItemTypes is DateInputItemTypes itemTypes &&
            !DateInputModelBinder.SupportedItemTypes.Contains(itemTypes))
        {
            throw new ArgumentException($"{nameof(DateInputItemTypes)} combination is not supported.", nameof(converter));
        }

        if (!_entries.TryAdd(modelType, new Entry(converter)))
        {
            throw new ArgumentException($"A {nameof(DateInputModelConverter)} is already registered for the {modelType.FullName} type.");
        }
    }

    private record Entry(DateInputModelConverter Converter);
}
