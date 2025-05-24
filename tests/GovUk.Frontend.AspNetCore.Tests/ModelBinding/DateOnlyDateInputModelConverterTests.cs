using GovUk.Frontend.AspNetCore.ModelBinding;

namespace GovUk.Frontend.AspNetCore.Tests.ModelBinding;

public class DateOnlyDateInputModelConverterTests
{
    [Theory]
    [MemberData(nameof(ConvertToModelData))]
    public void ConvertToModel_ReturnsExpectedResult(
        Type modelType,
        DateInputItemValues date,
        object expectedResult)
    {
        // Arrange
        var converter = new DateOnlyDateInputModelConverter();

        // Act
        var result = converter.ConvertToModel(new(modelType, DateInputItemTypes.DayMonthAndYear, date));

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
        var converter = new DateOnlyDateInputModelConverter();

        // Act
        var result = converter.ConvertFromModel(new(modelType, DateInputItemTypes.DayMonthAndYear, model));

        // Assert
        Assert.Equal(expectedResult, result);
    }

    public static TheoryData<Type, DateInputItemValues, object> ConvertToModelData { get; } = new()
    {
        { typeof(DateOnly), new DateInputItemValues(1, 4, 2020), new DateOnly(2020, 4, 1) },
        { typeof(DateOnly?), new DateInputItemValues(1, 4, 2020), (DateOnly?)new DateOnly(2020, 4, 1) }
    };

    public static TheoryData<Type, object, DateInputItemValues?> ConvertFromModelData { get; } = new()
    {
        { typeof(DateOnly), new DateOnly(2020, 4, 1), new DateInputItemValues(1, 4, 2020) },
        { typeof(DateOnly?), (DateOnly?)new DateOnly(2020, 4, 1), new DateInputItemValues(1, 4, 2020) },
    };
}
