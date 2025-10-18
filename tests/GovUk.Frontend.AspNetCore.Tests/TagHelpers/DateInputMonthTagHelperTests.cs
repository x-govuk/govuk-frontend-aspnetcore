using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class DateInputMonthTagHelperTests : DateInputItemTagHelperBaseTests<DateInputMonthTagHelper>
{
    [Fact]
    public Task ProcessAsync_AddMonthItemToContext() => ProcessAsync_AddItemToContext("month-id", "month-name", value: "12");
}
