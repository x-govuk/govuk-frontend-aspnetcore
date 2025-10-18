using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class DateInputDayTagHelperTests : DateInputItemTagHelperBaseTests<DateInputDayTagHelper>
{
    [Fact]
    public Task ProcessAsync_AddDayItemToContext() => ProcessAsync_AddItemToContext("day-id", "day-name", value: "2");
}
