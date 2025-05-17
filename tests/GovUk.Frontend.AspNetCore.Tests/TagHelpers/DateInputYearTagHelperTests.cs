using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class DateInputYearTagHelperTests() : DateInputItemTagHelperBaseTests<DateInputYearTagHelper>(DateInputYearTagHelper.TagName)
{
    [Fact]
    public Task ProcessAsync_AddYearItemToContext() => ProcessAsync_AddItemToContext("year-id", "year-name", value: "2025");
}
