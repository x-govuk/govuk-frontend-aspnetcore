namespace GovUk.Frontend.AspNetCore.ModelBinding;

internal class TupleDateInputModelConverter : DateInputModelConverter
{
    public static Type ModelType => typeof(ValueTuple<int, int>);

    public override DateInputItemTypes? DefaultItemTypes => null;

    protected override object ConvertToModelCore(DateInputConvertToModelContext context)
    {
        if (context.ItemTypes is DateInputItemTypes.DayAndMonth)
        {
            return (context.ItemValues.Day!.Value, context.ItemValues.Month!.Value);
        }

        if (context.ItemTypes is DateInputItemTypes.MonthAndYear)
        {
            return (context.ItemValues.Month!.Value, context.ItemValues.Year!.Value);
        }

        throw new NotSupportedException($"Only a date with either Day and Month OR Month and Year can be converted to a {nameof(ValueTuple<int, int>)}.");
    }

    protected override DateInputItemValues? ConvertFromModelCore(DateInputConvertFromModelContext context)
    {
        var tuple = (ValueTuple<int, int>)context.Model;

        if (context.ItemTypes is DateInputItemTypes.DayAndMonth)
        {
            return new DateInputItemValues(tuple.Item1, tuple.Item2, null);
        }

        if (context.ItemTypes is DateInputItemTypes.MonthAndYear)
        {
            return new DateInputItemValues(null, tuple.Item1, tuple.Item2);
        }

        throw new NotSupportedException($"Only a date with either Day and Month OR Month and Year can be converted from a {nameof(ValueTuple<int, int>)}.");
    }
}
