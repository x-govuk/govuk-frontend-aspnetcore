using GovUk.Frontend.AspNetCore.ModelBinding;

namespace GovUk.Frontend.AspNetCore.Tests.ModelBinding;

public class DateTimeDateInputModelConverterTests
{
    [Theory]
    [MemberData(nameof(ConvertToModelData))]
    public void ConvertToModel_ReturnsExpectedResult(
        Type modelType,
        DateInputItemValues dateInput,
        object expectedResult)
    {
        // Arrange
        var converter = new DateTimeDateInputModelConverter();

        // Act
        var result = converter.ConvertToModel(new(modelType, DateInputItemTypes.DayMonthAndYear, dateInput));

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [MemberData(nameof(ConvertFromModelData))]
    public void ConvertFromModel_ReturnsExpectedResult(
       Type modelType,
       object model,
       DateInputItemValues? expectedResult)
    {
        // Arrange
        var converter = new DateTimeDateInputModelConverter();

        // Act
        var result = converter.ConvertFromModel(new(modelType, DateInputItemTypes.DayMonthAndYear, model));

        // Assert
        Assert.Equal(expectedResult, result);
    }

    public static TheoryData<Type, DateInputItemValues, object> ConvertToModelData { get; } = new()
    {
        { typeof(DateTime), new DateInputItemValues(1, 4, 2020), new DateTime(2020, 4, 1) },
        { typeof(DateTime?), new DateInputItemValues(1, 4, 2020), (DateTime?)new DateTime(2020, 4, 1) }
    };

    public static TheoryData<Type, object, DateInputItemValues?> ConvertFromModelData { get; } = new()
    {
        { typeof(DateTime), new DateTime(2020, 4, 1), new DateInputItemValues(1, 4, 2020) },
        { typeof(DateTime?), (DateTime?)new DateTime(2020, 4, 1), new DateInputItemValues(1, 4, 2020) },
    };
}
