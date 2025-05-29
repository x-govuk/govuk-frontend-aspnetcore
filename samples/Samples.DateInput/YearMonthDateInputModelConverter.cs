using GovUk.Frontend.AspNetCore;
using GovUk.Frontend.AspNetCore.ModelBinding;
using NodaTime;

namespace Samples.DateInput;

public class YearMonthDateInputModelConverter : DateInputModelConverter
{
    public override DateInputItemTypes? DefaultItemTypes => DateInputItemTypes.MonthAndYear;

    protected override object ConvertToModelCore(DateInputConvertToModelContext context)
    {
        return new YearMonth(context.ItemValues.Year!.Value, context.ItemValues.Month!.Value);
    }

    protected override DateInputItemValues? ConvertFromModelCore(DateInputConvertFromModelContext context)
    {
        var yearMonth = (YearMonth)context.Model;
        return new DateInputItemValues(null, yearMonth.Month, yearMonth.Year);
    }
}
