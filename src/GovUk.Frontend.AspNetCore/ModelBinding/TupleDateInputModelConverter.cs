namespace GovUk.Frontend.AspNetCore.ModelBinding;

internal class TupleDateInputModelConverter : DateInputModelConverter
{
    public static Type ModelType => typeof(ValueTuple<int, int>);

    public override DateInputItemTypes? DefaultItemTypes => null;

    protected override object ConvertToModelCore(DateInputConvertToModelContext context)
    {
        return context.ItemTypes is DateInputItemTypes.DayAndMonth
            ? (context.ItemValues.Day!.Value, context.ItemValues.Month!.Value)
            : context.ItemTypes is DateInputItemTypes.MonthAndYear
            ? (object)(context.ItemValues.Month!.Value, context.ItemValues.Year!.Value)
            : throw new NotSupportedException($"Only a date with either Day and Month OR Month and Year can be converted to a {nameof(ValueTuple<int, int>)}.");
    }

    protected override DateInputItemValues? ConvertFromModelCore(DateInputConvertFromModelContext context)
    {
        var tuple = (ValueTuple<int, int>)context.Model;

        return context.ItemTypes is DateInputItemTypes.DayAndMonth
            ? new DateInputItemValues(tuple.Item1, tuple.Item2, null)
            : context.ItemTypes is DateInputItemTypes.MonthAndYear
            ? (DateInputItemValues?)new DateInputItemValues(null, tuple.Item1, tuple.Item2)
            : throw new NotSupportedException($"Only a date with either Day and Month OR Month and Year can be converted from a {nameof(ValueTuple<int, int>)}.");
    }
}
