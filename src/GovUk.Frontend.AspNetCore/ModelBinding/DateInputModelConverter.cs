namespace GovUk.Frontend.AspNetCore.ModelBinding;

/// <summary>
/// Converts date parts to and from a model type.
/// </summary>
public abstract class DateInputModelConverter
{
    /// <summary>
    /// The <see cref="DateInputItemTypes"/> supported by this <see cref="DateInputModelConverter"/>.
    /// </summary>
    /// <remarks>
    /// <para>If this converter can support multiple <see cref="DateInputItemTypes"/> combinations
    /// then this property should be set to <see langword="null"/>.</para>
    /// <para>The default is <c>DateInputItemTypes.DayMonthAndYear</c>.</para>
    /// </remarks>
    public virtual DateInputItemTypes? DefaultItemTypes { get; } = DateInputItemTypes.DayMonthAndYear;

    /// <summary>
    /// Converts a <see cref="DateInputItemValues"/> to a model value.
    /// </summary>
    public object ConvertToModel(DateInputConvertToModelContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (DefaultItemTypes is DateInputItemTypes itemTypes && context.ItemTypes != itemTypes)
        {
            throw new NotSupportedException($"Specified {nameof(DateInputItemTypes)} combination is not supported.");
        }

        return ConvertToModelCore(context);
    }

    /// <summary>
    /// Converts a model value to <see cref="DateInputItemValues"/>.
    /// </summary>
    public DateInputItemValues? ConvertFromModel(DateInputConvertFromModelContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (DefaultItemTypes is DateInputItemTypes itemTypes && context.ItemTypes != itemTypes)
        {
            throw new NotSupportedException($"Specified {nameof(DateInputItemTypes)} combination is not supported.");
        }

        return ConvertFromModelCore(context);
    }

    /// <summary>
    /// Converts a <see cref="DateInputItemValues"/> to a model value.
    /// </summary>
    protected abstract object ConvertToModelCore(DateInputConvertToModelContext context);

    /// <summary>
    /// Converts a model value to <see cref="DateInputItemValues"/>.
    /// </summary>
    protected abstract DateInputItemValues? ConvertFromModelCore(DateInputConvertFromModelContext context);
}

/// <summary>
/// Contains information used for converting a date input's item values to a model.
/// </summary>
public class DateInputConvertToModelContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DateInputConvertToModelContext"/>.
    /// </summary>
    public DateInputConvertToModelContext(Type modelType, DateInputItemTypes itemTypes, DateInputItemValues itemValues)
    {
        ArgumentNullException.ThrowIfNull(modelType);
        ArgumentNullException.ThrowIfNull(itemTypes);
        ArgumentNullException.ThrowIfNull(itemValues);

        ModelType = modelType;
        ItemTypes = itemTypes;
        ItemValues = itemValues;
    }

    /// <summary>
    /// The <see cref="Type"/> to convert to.
    /// </summary>
    public Type ModelType { get; }

    /// <summary>
    /// The <see cref="DateInputItemTypes"/> in the date input.
    /// </summary>
    public DateInputItemTypes ItemTypes { get; }

    /// <summary>
    /// The values of the date input to convert.
    /// </summary>
    public DateInputItemValues ItemValues { get; }
}

/// <summary>
/// Contains information used for converting a model to a date input's item values.
/// </summary>
public class DateInputConvertFromModelContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DateInputConvertFromModelContext"/>.
    /// </summary>
    public DateInputConvertFromModelContext(Type modelType, DateInputItemTypes itemTypes, object model)
    {
        ArgumentNullException.ThrowIfNull(modelType);
        ArgumentNullException.ThrowIfNull(itemTypes);
        ArgumentNullException.ThrowIfNull(model);

        ModelType = modelType;
        ItemTypes = itemTypes;
        Model = model;
    }

    /// <summary>
    /// The <see cref="Type"/> to convert from.
    /// </summary>
    public Type ModelType { get; }

    /// <summary>
    /// The <see cref="DateInputItemTypes"/> in the date input.
    /// </summary>
    public DateInputItemTypes ItemTypes { get; }

    /// <summary>
    /// The value to convert.
    /// </summary>
    public object Model { get; }
}
