using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GovUk.Frontend.AspNetCore.ModelBinding;

/// <summary>
/// Provides extension methods for working with <see cref="ModelMetadata"/>.
/// </summary>
public static class ModelMetadataExtensions
{
    /// <summary>
    /// Gets the <see cref="DateInputModelMetadata"/> for the specified <see cref="ModelMetadata"/>, if there is any.
    /// </summary>
    /// <param name="modelMetadata">The <see cref="ModelMetadata"/>.</param>
    /// <param name="dateInputModelMetadata">
    /// When this method returns, the <see cref="DateInputModelMetadata"/> for <paramref name="modelMetadata"/>,
    /// if there is any; otherwise <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="modelMetadata"/> has a <see cref="DateInputModelMetadata"/>;
    /// otherwise <see langword="false"/>.
    /// </returns>
    public static bool TryGetDateInputModelMetadata(this ModelMetadata modelMetadata, [NotNullWhen(true)] out DateInputModelMetadata? dateInputModelMetadata)
    {
        ArgumentNullException.ThrowIfNull(modelMetadata);

        if (modelMetadata.AdditionalValues.TryGetValue(typeof(DateInputModelMetadata), out var metadataObj))
        {
            dateInputModelMetadata = (DateInputModelMetadata)metadataObj;
            return true;
        }

        dateInputModelMetadata = null;
        return false;
    }
}
