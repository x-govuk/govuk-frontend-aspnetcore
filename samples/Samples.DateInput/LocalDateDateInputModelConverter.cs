using GovUk.Frontend.AspNetCore.ModelBinding;
using NodaTime;

namespace Samples.DateInput;

public class LocalDateDateInputModelConverter : DateInputModelConverter
{
    protected override object ConvertToModelCore(DateInputConvertToModelContext context)
    {
        return new LocalDate(context.ItemValues.Year!.Value, context.ItemValues.Month!.Value, context.ItemValues.Day!.Value);
    }

    protected override DateInputItemValues? ConvertFromModelCore(DateInputConvertFromModelContext context)
    {
        var localDate = (LocalDate)context.Model;
        return new DateInputItemValues(localDate.Day, localDate.Month, localDate.Year);
    }
}
