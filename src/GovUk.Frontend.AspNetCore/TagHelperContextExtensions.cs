using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore;

internal static class TagHelperContextExtensions
{
    private sealed record FormGroupContextKey;

    public static TItem GetRequiredContextItem<TItem>(this TagHelperContext context)
        where TItem : class
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Items.TryGetValue(typeof(TItem), out var obj) && obj is TItem item)
        {
            return item;
        }

        throw new InvalidOperationException($"No context item found for type: '{typeof(TItem).FullName}'.");
    }

    public static TItem GetContextItem<TItem>(this TagHelperContext context)
        where TItem : class
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!TryGetContextItem<TItem>(context, out var item))
        {
            throw new InvalidOperationException($"No context item found for type: '{typeof(TItem).FullName}'.");
        }

        return item;
    }

    public static void SetContextItem<TItem>(this TagHelperContext context, TItem item)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(item);

        context.Items[typeof(TItem)] = item;
    }

    public static IDisposable SetScopedContextItem<TItem>(this TagHelperContext context, TItem item)
        where TItem : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(item);

        return SetScopedContextItem(context, typeof(TItem), item);
    }

    public static IDisposable SetScopedContextItem(
        this TagHelperContext context,
        object key,
        object value)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);

        context.Items.TryGetValue(key, out var previousValue);
        context.Items[key] = value;

        return new RestoreItemsOnDispose(context, key, previousValue);
    }

    public static bool TryGetContextItem<TItem>(this TagHelperContext context, [NotNullWhen(true)] out TItem? item)
    {
        ArgumentNullException.ThrowIfNull(context);

        var key = typeof(TItem);

        if (context.Items.TryGetValue(key, out var itemObj) && itemObj is TItem typedItem)
        {
            item = typedItem;
            return true;
        }
        else
        {
            item = default;
            return false;
        }
    }

    internal class RestoreItemsOnDispose : IDisposable
    {
        private readonly TagHelperContext _context;
        private readonly object _key;
        private readonly object? _previousValue;

        public RestoreItemsOnDispose(
            TagHelperContext context,
            object key,
            object? previousValue)
        {
            _context = Guard.ArgumentNotNull(nameof(context), context);
            _key = Guard.ArgumentNotNull(nameof(key), key);
            _previousValue = previousValue;
        }

        public void Dispose()
        {
            if (_previousValue is not null)
            {
                _context.Items[_key] = _previousValue;
            }
            else
            {
                _context.Items.Remove(_key);
            }
        }
    }
}
