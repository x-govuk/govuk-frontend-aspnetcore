using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class DateInputDayLabelTagHelperTests() :
    DateInputItemLabelTagHelperBaseTests<DateInputDayLabelTagHelper>(DateInputDayLabelTagHelper.TagName, parentTagName: DateInputDayTagHelper.TagName)
{
}
