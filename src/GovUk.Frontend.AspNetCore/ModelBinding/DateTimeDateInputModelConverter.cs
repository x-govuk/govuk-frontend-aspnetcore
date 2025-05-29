namespace GovUk.Frontend.AspNetCore.ModelBinding;

internal class DateTimeDateInputModelConverter : DateInputModelConverter
{
    public static Type ModelType => typeof(DateTime);

    protected override object ConvertToModelCore(DateInputConvertToModelContext context)
    {
        var values = context.ItemValues;
        return new DateTime(values.Year!.Value, values.Month!.Value, values.Day!.Value);
    }

    protected override DateInputItemValues? ConvertFromModelCore(DateInputConvertFromModelContext context)
    {
        var dateTime = (DateTime)context.Model;
        return new DateInputItemValues(dateTime.Day, dateTime.Month, dateTime.Year);
    }
}
