using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace GovUk.Frontend.AspNetCore.ModelBinding;

/// <summary>
/// An <see cref="IModelBinderProvider"/> for binding Date input components.
/// </summary>
public class DateInputModelBinderProvider : IModelBinderProvider
{
    private readonly IOptions<GovUkFrontendOptions> _optionsAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="DateInputModelBinderProvider"/>.
    /// </summary>
    /// <param name="optionsAccessor">The options.</param>
    public DateInputModelBinderProvider(IOptions<GovUkFrontendOptions> optionsAccessor)
    {
        ArgumentNullException.ThrowIfNull(optionsAccessor);
        _optionsAccessor = optionsAccessor;
    }

    /// <inheritdoc/>
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var modelType = context.Metadata.UnderlyingOrModelType;
        var converter = _optionsAccessor.Value.FindDateInputModelConverterForType(modelType);

        return converter is not null ? new DateInputModelBinder(converter) : (IModelBinder?)null;
    }
}
