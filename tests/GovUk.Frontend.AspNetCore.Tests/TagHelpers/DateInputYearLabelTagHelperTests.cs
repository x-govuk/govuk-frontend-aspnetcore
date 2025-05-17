using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class DateInputYearLabelTagHelperTests() :
    DateInputItemLabelTagHelperBaseTests<DateInputYearLabelTagHelper>(DateInputYearLabelTagHelper.TagName, parentTagName: DateInputYearTagHelper.TagName)
{
}
