using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GovUk.Frontend.AspNetCore.ModelBinding;

internal static class ModelMetadataExtensions
{
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
