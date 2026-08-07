using GovUk.Frontend.AspNetCore.Localization;
using GovUk.Frontend.AspNetCore.ModelBinding;

namespace GovUk.Frontend.AspNetCore.Tests.Localization;

public class DateInputModelBinderLocalizationTests
{
    [Theory]
    [InlineData(DateInputParseErrors.MissingDay, GovUkFrontendResourceNames.DateInputErrorMessageMissingDay)]
    [InlineData(DateInputParseErrors.MissingMonth, GovUkFrontendResourceNames.DateInputErrorMessageMissingMonth)]
    [InlineData(DateInputParseErrors.MissingYear, GovUkFrontendResourceNames.DateInputErrorMessageMissingYear)]
    [InlineData(
        DateInputParseErrors.MissingDay | DateInputParseErrors.MissingMonth,
        GovUkFrontendResourceNames.DateInputErrorMessageMissingDayAndMonth)]
    [InlineData(
        DateInputParseErrors.MissingDay | DateInputParseErrors.MissingYear,
        GovUkFrontendResourceNames.DateInputErrorMessageMissingDayAndYear)]
    [InlineData(
        DateInputParseErrors.MissingMonth | DateInputParseErrors.MissingYear,
        GovUkFrontendResourceNames.DateInputErrorMessageMissingMonthAndYear)]
    [InlineData(DateInputParseErrors.InvalidDay, GovUkFrontendResourceNames.DateInputErrorMessageInvalidDate)]
    [InlineData(DateInputParseErrors.InvalidMonth, GovUkFrontendResourceNames.DateInputErrorMessageInvalidDate)]
    [InlineData(DateInputParseErrors.InvalidYear, GovUkFrontendResourceNames.DateInputErrorMessageInvalidDate)]
    [InlineData(
        DateInputParseErrors.MissingDay | DateInputParseErrors.InvalidMonth,
        GovUkFrontendResourceNames.DateInputErrorMessageMissingDay)]
    public void GetModelStateErrorMessageTemplate_UsesTheLocalizedMessage(
        DateInputParseErrors parseErrors,
        string expectedResourceName)
    {
        var localizer = DelegateLocalizer.ForName(expectedResourceName, "{0} yn anghywir");

        var result = DateInputModelBinder.GetModelStateErrorMessageTemplate(parseErrors, localizer);

        Assert.Equal("{0} yn anghywir", result);
    }

    [Theory]
    [InlineData(DateInputParseErrors.MissingDay, "{0} must include a day")]
    [InlineData(DateInputParseErrors.MissingMonth, "{0} must include a month")]
    [InlineData(DateInputParseErrors.MissingYear, "{0} must include a year")]
    [InlineData(DateInputParseErrors.MissingDay | DateInputParseErrors.MissingMonth, "{0} must include a day and month")]
    [InlineData(DateInputParseErrors.MissingDay | DateInputParseErrors.MissingYear, "{0} must include a day and year")]
    [InlineData(DateInputParseErrors.MissingMonth | DateInputParseErrors.MissingYear, "{0} must include a month and year")]
    [InlineData(DateInputParseErrors.InvalidDay, "{0} must be a real date")]
    public void GetModelStateErrorMessageTemplate_WithNoLocalizedMessage_UsesTheBuiltInEnglish(
        DateInputParseErrors parseErrors,
        string expectedTemplate)
    {
        var result = DateInputModelBinder.GetModelStateErrorMessageTemplate(parseErrors, NullGovUkFrontendLocalizer.Instance);

        Assert.Equal(expectedTemplate, result);
    }
}
