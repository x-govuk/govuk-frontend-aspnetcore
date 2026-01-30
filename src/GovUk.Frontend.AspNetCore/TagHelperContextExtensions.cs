using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore;

internal static class TagHelperContextExtensions
{
    public static TItem GetRequiredContextItem<TItem>(this TagHelperContext context)
        where TItem : class
    {
        ArgumentNullException.ThrowIfNull(context);

        return context.Items.TryGetValue(typeof(TItem), out var obj) && obj is TItem item
            ? item
            : throw new InvalidOperationException($"No context item found for type: '{typeof(TItem).FullName}'.");
    }

    public static TItem GetContextItem<TItem>(this TagHelperContext context)
        where TItem : class
    {
        ArgumentNullException.ThrowIfNull(context);

        return !TryGetContextItem<TItem>(context, out var item)
            ? throw new InvalidOperationException($"No context item found for type: '{typeof(TItem).FullName}'.")
            : item;
    }

    public static void SetContextItem<TItem>(this TagHelperContext context, TItem item)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(item);

        context.Items[typeof(TItem)] = item;
    }

    public static void SetContextItem(this TagHelperContext context, object key, object value)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);

        context.Items[key] = value;
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
}
