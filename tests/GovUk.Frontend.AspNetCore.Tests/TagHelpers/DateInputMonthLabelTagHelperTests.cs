using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class DateInputMonthLabelTagHelperTests() :
    DateInputItemLabelTagHelperBaseTests<DateInputMonthLabelTagHelper>(DateInputMonthLabelTagHelper.TagName, parentTagName: DateInputMonthTagHelper.TagName)
{
}
