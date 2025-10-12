using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.ModelBinding;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class DateInputTagHelperTests() : TagHelperTestBase("govuk-date-input")
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var viewContext = CreateViewContext();
        var id = "dateinput-id";
        var namePrefix = "dateinput";
        var value = new DateOnly(2025, 5, 25);
        var disabled = true;
        var dateInputAttributes = CreateDummyDataAttributes();
        var hintContent = "Hint";
        var hintAttributes = CreateDummyDataAttributes();

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var dateInputContext = context.GetContextItem<DateInputContext>();

                dateInputContext.SetHint(
                    attributes: new(hintAttributes),
                    hintContent,
                    DateInputHintTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            HtmlEncoder.Default)
        {
            Id = id,
            NamePrefix = namePrefix,
            Value = value,
            Disabled = disabled,
            DateInputAttributes = dateInputAttributes,
            ViewContext = viewContext
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(id, actualOptions.Id);
        Assert.Null(actualOptions.NamePrefix);
        Assert.Null(actualOptions.Fieldset);
        Assert.Equal(hintContent, actualOptions.Hint?.Html);
        AssertContainsAttributes(hintAttributes, actualOptions.Hint?.Attributes);
        Assert.Null(actualOptions.ErrorMessage);
        Assert.NotNull(actualOptions.Attributes);
        AssertContainsAttributes(dateInputAttributes, actualOptions.Attributes);
        Assert.NotNull(actualOptions.Items);
        Assert.Collection(
            actualOptions.Items,
            item =>
            {
                Assert.Equal("Day", item.Label);
                Assert.Equal(value.Day.ToString(), item.Value);
                Assert.Equal(namePrefix + ".Day", item.Name);
                Assert.NotNull(item.Attributes);
                Assert.Contains(
                    item.Attributes,
                    attr => attr.Key == "disabled");
            },
            item =>
            {
                Assert.Equal("Month", item.Label);
                Assert.Equal(value.Month.ToString(), item.Value);
                Assert.Equal(namePrefix + ".Month", item.Name);
                Assert.NotNull(item.Attributes);
                Assert.Contains(
                    item.Attributes,
                    attr => attr.Key == "disabled");
            },
            item =>
            {
                Assert.Equal("Year", item.Label);
                Assert.Equal(value.Year.ToString(), item.Value);
                Assert.Equal(namePrefix + ".Year", item.Name);
                Assert.NotNull(item.Attributes);
                Assert.Contains(
                    item.Attributes,
                    attr => attr.Key == "disabled");
            });
    }

    [Fact]
    public async Task ProcessAsync_WithFieldset_GeneratesOptionsWithFieldset()
    {
        // Arrange
        var viewContext = CreateViewContext();
        var id = "dateinput-id";
        var namePrefix = "dateinput";
        var fieldsetAttributes = CreateDummyDataAttributes();
        var fieldsetDescribedBy = "described-by";
        var legendContent = "Legend";
        var legendIsPageHeading = true;
        var legendAttributes = CreateDummyDataAttributes();

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var dateInputContext = context.GetContextItem<DateInputContext>();

                var fieldsetContext = new DateInputFieldsetContext(fieldsetDescribedBy, new(fieldsetAttributes), @for: null);

                dateInputContext.OpenFieldset();
                fieldsetContext.SetLegend(legendIsPageHeading, new(legendAttributes), legendContent);
                dateInputContext.CloseFieldset(fieldsetContext);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            HtmlEncoder.Default)
        {
            Id = id,
            NamePrefix = namePrefix,
            ViewContext = viewContext
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(id, actualOptions.Id);
        Assert.Null(actualOptions.NamePrefix);
        Assert.NotNull(actualOptions.Fieldset);
        AssertContainsAttributes(fieldsetAttributes, actualOptions.Fieldset.Attributes);
        Assert.NotNull(actualOptions.Fieldset.DescribedBy);
        Assert.Contains(fieldsetDescribedBy, actualOptions.Fieldset.DescribedBy.ToString().Split(' '));
        Assert.NotNull(actualOptions.Fieldset.Legend);
        Assert.Equal(legendIsPageHeading, actualOptions.Fieldset.Legend.IsPageHeading);
        AssertContainsAttributes(legendAttributes, actualOptions.Fieldset.Legend.Attributes);
        Assert.Equal(legendContent, actualOptions.Fieldset.Legend.Html);
    }

    [Fact]
    public async Task ProcessAsync_WithForAndFieldsetButNoLegendTagContent_GetsLegendContentFromModelMetadata()
    {
        // Arrange
        var viewContext = CreateViewContext();
        var modelMetadataDisplayName = "ModelMetadata display name";
        var @for = CreateModelExpression(viewContext, date: null, displayName: modelMetadataDisplayName);

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var dateInputContext = context.GetContextItem<DateInputContext>();

                var fieldsetContext = new DateInputFieldsetContext(describedBy: null, attributes: [], @for);

                dateInputContext.OpenFieldset();
                fieldsetContext.SetLegend(isPageHeading: false, attributes: [], html: null);
                dateInputContext.CloseFieldset(fieldsetContext);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            HtmlEncoder.Default)
        {
            For = @for,
            ViewContext = viewContext
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(modelMetadataDisplayName, actualOptions.Fieldset?.Legend?.Html);
    }

    [Fact]
    public async Task ProcessAsync_WithForAndFieldsetButAndLegendTagContent_GetsLegendContentFromTagContent()
    {
        // Arrange
        var viewContext = CreateViewContext();
        var legendContent = "Legend";
        var modelMetadataDisplayName = "ModelMetadata display name";
        var @for = CreateModelExpression(viewContext, date: null, displayName: modelMetadataDisplayName);

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var dateInputContext = context.GetContextItem<DateInputContext>();

                var fieldsetContext = new DateInputFieldsetContext(describedBy: null, attributes: [], @for);

                dateInputContext.OpenFieldset();
                fieldsetContext.SetLegend(isPageHeading: false, attributes: [], html: legendContent);
                dateInputContext.CloseFieldset(fieldsetContext);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            HtmlEncoder.Default)
        {
            For = @for,
            ViewContext = viewContext
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(legendContent, actualOptions.Fieldset?.Legend?.Html);
    }

    [Fact]
    public async Task ProcessAsync_WithForAndNoHintTagContent_GetsHintContentFromModelMetadata()
    {
        // Arrange
        var viewContext = CreateViewContext();
        var modelMetadataDescription = "ModelMetadata description";
        var @for = CreateModelExpression(viewContext, date: null, description: modelMetadataDescription);

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput();

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            HtmlEncoder.Default)
        {
            For = @for,
            ViewContext = viewContext
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(modelMetadataDescription, actualOptions.Hint?.Html);
    }

    [Fact]
    public async Task ProcessAsync_WithForAndHintTagContent_GetsHintContentFromHintTagContent()
    {
        // Arrange
        var viewContext = CreateViewContext();
        var hintContent = "Hint";
        var modelMetadataDescription = "ModelMetadata description";
        var @for = CreateModelExpression(viewContext, date: null, description: modelMetadataDescription);

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var dateInputContext = context.GetContextItem<DateInputContext>();
                dateInputContext.SetHint(attributes: [], html: hintContent, tagName: DateInputHintTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            HtmlEncoder.Default)
        {
            For = @for,
            ViewContext = viewContext
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(hintContent, actualOptions.Hint?.Html);
    }

    [Fact]
    public async Task ProcessAsync_WithErrorMessageTag_GeneratesOptionsWithErrorMessage()
    {
        // Arrange
        var viewContext = CreateViewContext();
        var id = "dateinput-id";
        var namePrefix = "dateinput";
        var errorContent = "The error message";
        var errorVht = "visually hidden text";
        var errorAttributes = CreateDummyDataAttributes();

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var dateInputContext = context.GetContextItem<DateInputContext>();

                dateInputContext.SetErrorMessage(
                    errorFields: null,
                    visuallyHiddenText: errorVht,
                    attributes: new(errorAttributes),
                    html: errorContent,
                    DateInputHintTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            HtmlEncoder.Default)
        {
            Id = id,
            NamePrefix = namePrefix,
            ViewContext = viewContext
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions.ErrorMessage);
        Assert.Equal(errorContent, actualOptions.ErrorMessage.Html);
        Assert.Equal(errorVht, actualOptions.ErrorMessage.VisuallyHiddenText);
        AssertContainsAttributes(errorAttributes, actualOptions.ErrorMessage.Attributes);
        AssertAllItemsHaveErrorClass(actualOptions);
    }

    [Fact]
    public async Task ProcessAsync_WithModelStateErrorAndNoErrorMessageTagContent_GeneratesOptionsWithErrorMessageContentFromModelState()
    {
        // Arrange
        var viewContext = CreateViewContext();
        var modelStateErrorMessage = "Error";
        var @for = CreateModelExpression(viewContext, date: null, errorMessage: modelStateErrorMessage);

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput();

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            HtmlEncoder.Default)
        {
            For = @for,
            ViewContext = viewContext
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions.ErrorMessage);
        Assert.Equal(modelStateErrorMessage, actualOptions.ErrorMessage.Html);
        AssertAllItemsHaveErrorClass(actualOptions);
    }

    [Fact]
    public async Task ProcessAsync_WithModelStateErrorAndErrorMessageTagContent_GeneratesOptionsWithErrorMessageContentFromTag()
    {
        // Arrange
        var viewContext = CreateViewContext();
        var errorContent = "The error message";
        var modelStateErrorMessage = "Error";
        var @for = CreateModelExpression(viewContext, date: null, errorMessage: modelStateErrorMessage);

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var dateInputContext = context.GetContextItem<DateInputContext>();

                dateInputContext.SetErrorMessage(
                    errorFields: null,
                    visuallyHiddenText: null,
                    attributes: [],
                    html: errorContent,
                    DateInputHintTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            HtmlEncoder.Default)
        {
            For = @for,
            ViewContext = CreateViewContext()
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions.ErrorMessage);
        Assert.Equal(errorContent, actualOptions.ErrorMessage.Html);
        AssertAllItemsHaveErrorClass(actualOptions);
    }

    [Fact]
    public async Task ProcessAsync_WithFor_GeneratesOptionsWithItemValuesFromModel()
    {
        // Arrange
        var viewContext = CreateViewContext();
        var value = new DateOnly(2025, 5, 25);
        var @for = CreateModelExpression(viewContext, value);

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput();

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            HtmlEncoder.Default)
        {
            For = @for,
            ViewContext = viewContext
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions.Items);
        Assert.Collection(
            actualOptions.Items,
            item =>
            {
                Assert.Equal($"{nameof(Model.Date)}.Day", item.Id);
                Assert.Equal($"{nameof(Model.Date)}.Day", item.Name);
                Assert.Equal(value.Day.ToString(), item.Value);
            },
            item =>
            {
                Assert.Equal($"{nameof(Model.Date)}.Month", item.Id);
                Assert.Equal($"{nameof(Model.Date)}.Month", item.Name);
                Assert.Equal(value.Month.ToString(), item.Value);
            },
            item =>
            {
                Assert.Equal($"{nameof(Model.Date)}.Year", item.Id);
                Assert.Equal($"{nameof(Model.Date)}.Year", item.Name);
                Assert.Equal(value.Year.ToString(), item.Value);
            });
    }

    [Fact]
    public async Task ProcessAsync_WithForAndSpecifiedValue_GeneratesOptionsWithItemValuesFromSpecifiedValue()
    {
        // Arrange
        var viewContext = CreateViewContext();
        var id = "dateinput-id";
        var namePrefix = "dateinput";
        var value = new DateOnly(2025, 5, 25);
        var @for = CreateModelExpression(viewContext, date: new(2000, 4, 1));

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput();

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            HtmlEncoder.Default)
        {
            For = @for,
            Id = id,
            NamePrefix = namePrefix,
            Value = value,
            ViewContext = viewContext
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions.Items);
        Assert.Collection(
            actualOptions.Items,
            item => { Assert.Equal(value.Day.ToString(), item.Value); },
            item => { Assert.Equal(value.Month.ToString(), item.Value); },
            item => { Assert.Equal(value.Year.ToString(), item.Value); });
    }

    [Fact]
    public async Task ProcessAsync_WithSpecifiedItems_GeneratesOptionsWithItemsFromItemTags()
    {
        // Arrange
        var viewContext = CreateViewContext();
        var id = "dateinput-id";
        var namePrefix = "dateinput";
        var dayValue = "1";
        var dayId = "foo";
        var dayName = "FOO";
        var dayLabel = "dey";
        var monthValue = "2";
        var monthId = "bar";
        var monthName = "BAR";
        var monthLabel = "menth";
        var yearValue = "3";
        var yearId = "baz";
        var yearName = "BAZ";
        var yearLabel = "yeer";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var dateInputContext = context.GetContextItem<DateInputContext>();

                dateInputContext.SetItem(DateInputItemTypes.Day, new DateInputContextItem()
                {
                    TagName = DateInputDayTagHelper.TagName,
                    Id = dayId,
                    Name = dayName,
                    LabelHtml = dayLabel,
                    Value = dayValue,
                    ValueSpecified = true
                });

                dateInputContext.SetItem(DateInputItemTypes.Month, new DateInputContextItem()
                {
                    TagName = DateInputMonthTagHelper.TagName,
                    Id = monthId,
                    Name = monthName,
                    LabelHtml = monthLabel,
                    Value = monthValue,
                    ValueSpecified = true
                });

                dateInputContext.SetItem(DateInputItemTypes.Year, new DateInputContextItem()
                {
                    TagName = DateInputYearTagHelper.TagName,
                    Id = yearId,
                    Name = yearName,
                    LabelHtml = yearLabel,
                    Value = yearValue,
                    ValueSpecified = true
                });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            HtmlEncoder.Default)
        {
            Id = id,
            NamePrefix = namePrefix,
            ViewContext = CreateViewContext()
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions.Items);
        Assert.Collection(
            actualOptions.Items,
            item =>
            {
                Assert.Equal(dayValue, item.Value);
                Assert.Equal(dayId, item.Id);
                Assert.Equal(dayName, item.Name);
                Assert.Equal(dayLabel, item.Label);
            },
            item =>
            {
                Assert.Equal(monthValue, item.Value);
                Assert.Equal(monthId, item.Id);
                Assert.Equal(monthName, item.Name);
                Assert.Equal(monthLabel, item.Label);
            },
            item =>
            {
                Assert.Equal(yearValue, item.Value);
                Assert.Equal(yearId, item.Id);
                Assert.Equal(yearName, item.Name);
                Assert.Equal(yearLabel, item.Label);
            });
    }

    [Fact]
    public async Task ProcessAsync_WithForAndSpecifiedItemValues_GeneratesOptionsWithItemValuesFromSpecifiedItemValues()
    {
        // Arrange
        var viewContext = CreateViewContext();
        var id = "dateinput-id";
        var namePrefix = "dateinput";
        var dayValue = "1";
        var dayId = "foo";
        var dayName = "FOO";
        var dayLabel = "dey";
        var monthValue = "2";
        var monthId = "bar";
        var monthName = "BAR";
        var monthLabel = "menth";
        var yearValue = "3";
        var yearId = "baz";
        var yearName = "BAZ";
        var yearLabel = "yeer";
        var @for = CreateModelExpression(viewContext, date: new(2025, 1, 1));

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var dateInputContext = context.GetContextItem<DateInputContext>();

                dateInputContext.SetItem(DateInputItemTypes.Day, new DateInputContextItem()
                {
                    TagName = DateInputDayTagHelper.TagName,
                    Id = dayId,
                    Name = dayName,
                    LabelHtml = dayLabel,
                    Value = dayValue,
                    ValueSpecified = true
                });

                dateInputContext.SetItem(DateInputItemTypes.Month, new DateInputContextItem()
                {
                    TagName = DateInputMonthTagHelper.TagName,
                    Id = monthId,
                    Name = monthName,
                    LabelHtml = monthLabel,
                    Value = monthValue,
                    ValueSpecified = true
                });

                dateInputContext.SetItem(DateInputItemTypes.Year, new DateInputContextItem()
                {
                    TagName = DateInputYearTagHelper.TagName,
                    Id = yearId,
                    Name = yearName,
                    LabelHtml = yearLabel,
                    Value = yearValue,
                    ValueSpecified = true
                });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            HtmlEncoder.Default)
        {
            For = @for,
            Id = id,
            NamePrefix = namePrefix,
            ViewContext = CreateViewContext()
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions.Items);
        Assert.Collection(
            actualOptions.Items,
            item =>
            {
                Assert.Equal(dayValue, item.Value);
                Assert.Equal(dayId, item.Id);
                Assert.Equal(dayName, item.Name);
                Assert.Equal(dayLabel, item.Label);
            },
            item =>
            {
                Assert.Equal(monthValue, item.Value);
                Assert.Equal(monthId, item.Id);
                Assert.Equal(monthName, item.Name);
                Assert.Equal(monthLabel, item.Label);
            },
            item =>
            {
                Assert.Equal(yearValue, item.Value);
                Assert.Equal(yearId, item.Id);
                Assert.Equal(yearName, item.Name);
                Assert.Equal(yearLabel, item.Label);
            });
    }

    [Fact]
    public async Task ProcessAsync_WithForAndNamePrefix_GeneratesItemNamesUsingSpecifiedNamePrefix()
    {
        // Arrange
        var viewContext = CreateViewContext();
        var id = "dateinput-id";
        var namePrefix = "dateinput";
        var modelMetadataDisplayName = "ModelMetadata display name";
        var @for = CreateModelExpression(viewContext, date: null, displayName: modelMetadataDisplayName);

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput();

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            HtmlEncoder.Default)
        {
            For = @for,
            Id = id,
            NamePrefix = namePrefix,
            ViewContext = viewContext
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions.Items);
        Assert.Collection(
            actualOptions.Items,
            item => Assert.StartsWith(namePrefix, item.Name?.ToString()),
            item => Assert.StartsWith(namePrefix, item.Name?.ToString()),
            item => Assert.StartsWith(namePrefix, item.Name?.ToString()));
    }

    [Fact]
    public async Task ProcessAsync_WithCustomDateTypeInValue_GeneratesOptionsWithItemValuesFromModel()
    {
        // Arrange
        var viewContext = CreateViewContext();
        var id = "dateinput-id";
        var namePrefix = "dateinput";
        var value = new CustomDateType(2025, 5, 25);

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput();

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var customDateTypeConverter = new CustomDateTypeConverter();

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(options => options.RegisterDateInputModelConverter(typeof(CustomDateType), customDateTypeConverter)),
            HtmlEncoder.Default)
        {
            Id = id,
            NamePrefix = namePrefix,
            Value = value,
            ViewContext = viewContext
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions.Items);
        Assert.Collection(
            actualOptions.Items,
            item => { Assert.Equal(value.D.ToString(), item.Value); },
            item => { Assert.Equal(value.M.ToString(), item.Value); },
            item => { Assert.Equal(value.Y.ToString(), item.Value); });
    }

    [Fact]
    public async Task ProcessAsync_WithCustomDateTypeInModel_GeneratesOptionsWithItemValuesFromModel()
    {
        // Arrange
        var viewContext = CreateViewContext();
        var value = new CustomDateType(2025, 5, 25);
        var @for = CreateModelExpressionForCustomDateType(viewContext, date: value);

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput();

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var customDateTypeConverter = new CustomDateTypeConverter();

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(options => options.RegisterDateInputModelConverter(typeof(CustomDateType), customDateTypeConverter)),
            HtmlEncoder.Default)
        {
            For = @for,
            ViewContext = viewContext
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions.Items);
        Assert.Collection(
            actualOptions.Items,
            item => { Assert.Equal(value.D.ToString(), item.Value); },
            item => { Assert.Equal(value.M.ToString(), item.Value); },
            item => { Assert.Equal(value.Y.ToString(), item.Value); });
    }

    [Theory]
    [InlineData(null, true, true, true)]
    [InlineData(DateInputItemTypes.All, true, true, true)]
    [InlineData(DateInputItemTypes.Day, true, false, false)]
    [InlineData(DateInputItemTypes.Month, false, true, false)]
    [InlineData(DateInputItemTypes.Year, false, false, true)]
    [InlineData(DateInputItemTypes.Day | DateInputItemTypes.Month, true, true, false)]
    [InlineData(DateInputItemTypes.Day | DateInputItemTypes.Year, true, false, true)]
    [InlineData(DateInputItemTypes.Month | DateInputItemTypes.Year, false, true, true)]
    public async Task ProcessAsync_HaveErrorClassesWhenErrorSpecified(
        DateInputItemTypes? specifiedErrorFields,
        bool expectDayError,
        bool expectMonthError,
        bool expectYearError)
    {
        // Arrange
        var viewContext = CreateViewContext();
        var id = "dateinput-id";
        var namePrefix = "dateinput";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var dateInputContext = context.GetContextItem<DateInputContext>();

                dateInputContext.SetErrorMessage(
                    specifiedErrorFields,
                    visuallyHiddenText: null,
                    attributes: [],
                    html: "Error",
                    tagName: DateInputErrorMessageTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            HtmlEncoder.Default)
        {
            Id = id,
            NamePrefix = namePrefix,
            ViewContext = viewContext
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions.Items);
        Assert.Collection(
            actualOptions.Items,
            day => AssertItemHasExpectedError(day, expectDayError),
            month => AssertItemHasExpectedError(month, expectMonthError),
            year => AssertItemHasExpectedError(year, expectYearError));
    }

    [Theory]
    [MemberData(nameof(DateInputParseErrorsWithExpectedErrorItemsData))]
    public async Task ProcessAsync_ErrorItemsNotSpecifiedAndErrorsFromModelBinderFromModelExpression_InfersErrorItems(
        DateInputParseErrors parseErrors,
        bool expectDayError,
        bool expectMonthError,
        bool expectYearError)
    {
        // Arrange
        var viewContext = CreateViewContext();
        var id = "dateinput-id";
        var namePrefix = "dateinput";
        var @for = CreateModelExpression(viewContext, date: null, errorMessage: "An error");

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var dateInputContext = context.GetContextItem<DateInputContext>();

                dateInputContext.SetErrorMessage(
                    errorFields: null,
                    visuallyHiddenText: null,
                    attributes: [],
                    html: "Error",
                    tagName: DateInputErrorMessageTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        AddDateInputParseException(viewContext, nameof(Model.Date), parseErrors);

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            HtmlEncoder.Default)
        {
            For = @for,
            Id = id,
            NamePrefix = namePrefix,
            ViewContext = viewContext
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions.Items);
        Assert.Collection(
            actualOptions.Items,
            day => AssertItemHasExpectedError(day, expectDayError),
            month => AssertItemHasExpectedError(month, expectMonthError),
            year => AssertItemHasExpectedError(year, expectYearError));
    }

    [Theory]
    [MemberData(nameof(DateInputParseErrorsWithExpectedErrorItemsData))]
    public async Task ProcessAsync_ErrorItemsNotSpecifiedAndErrorsFromModelBinderFromName_InfersErrorItems(
        DateInputParseErrors parseErrors,
        bool expectDayError,
        bool expectMonthError,
        bool expectYearError)
    {
        // Arrange
        var viewContext = CreateViewContext();
        var id = "dateinput-id";
        var namePrefix = "dateinput";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var dateInputContext = context.GetContextItem<DateInputContext>();

                dateInputContext.SetErrorMessage(
                    errorFields: null,
                    visuallyHiddenText: null,
                    attributes: [],
                    html: "Error",
                    tagName: DateInputErrorMessageTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        AddDateInputParseException(viewContext, namePrefix, parseErrors);

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            HtmlEncoder.Default)
        {
            Id = id,
            NamePrefix = namePrefix,
            ViewContext = viewContext
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions.Items);
        Assert.Collection(
            actualOptions.Items,
            day => AssertItemHasExpectedError(day, expectDayError),
            month => AssertItemHasExpectedError(month, expectMonthError),
            year => AssertItemHasExpectedError(year, expectYearError));
    }

    [Fact]
    public async Task ProcessAsync_ErrorItemsSpecifiedAndErrorsFromModelBinder_UsesSpecifiedErrorItems()
    {
        // Arrange
        var viewContext = CreateViewContext();
        var id = "dateinput-id";
        var namePrefix = "dateinput";
        var @for = CreateModelExpression(viewContext, date: null, errorMessage: "An error");

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var dateInputContext = context.GetContextItem<DateInputContext>();

                // Explictly set a different set of DateInputErrorComponents than we have in BindingResultInfoProvider
                dateInputContext.SetErrorMessage(
                    errorFields: DateInputItemTypes.Month | DateInputItemTypes.Year,
                    visuallyHiddenText: null,
                    attributes: [],
                    html: "Error",
                    tagName: DateInputErrorMessageTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        AddDateInputParseException(viewContext, nameof(Model.Date), DateInputParseErrors.InvalidDay);

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            HtmlEncoder.Default)
        {
            For = @for,
            Id = id,
            NamePrefix = namePrefix,
            ViewContext = viewContext
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions.Items);
        Assert.Collection(
            actualOptions.Items,
            day => AssertItemHasExpectedError(day, false),
            month => AssertItemHasExpectedError(month, true),
            year => AssertItemHasExpectedError(year, true));
    }

    [Theory]
    [InlineData(DateInputItemTypes.Day, "dateinput-id.Day")]
    [InlineData(DateInputItemTypes.Day | DateInputItemTypes.Month, "dateinput-id.Day")]
    [InlineData(DateInputItemTypes.Day | DateInputItemTypes.Month | DateInputItemTypes.Year, "dateinput-id.Day")]
    [InlineData(DateInputItemTypes.Month, "dateinput-id.Month")]
    [InlineData(DateInputItemTypes.Month | DateInputItemTypes.Year, "dateinput-id.Month")]
    [InlineData(null, "dateinput-id.Day")]
    public async Task ProcessAsync_WithError_AddsErrorWithCorrectFieldIdToFormErrorContext(
        DateInputItemTypes? errorFields,
        string expectedErrorFieldId)
    {
        // Arrange
        var viewContext = CreateViewContext();
        var id = "dateinput-id";
        var namePrefix = "dateinput";
        var @for = CreateModelExpression(viewContext, date: null, errorMessage: "An error");

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
             getChildContentAsync: (useCachedResult, encoder) =>
             {
                 var dateInputContext = context.GetContextItem<DateInputContext>();

                 dateInputContext.SetErrorMessage(
                     errorFields,
                     visuallyHiddenText: null,
                     attributes: [],
                     html: "Error",
                     tagName: DateInputErrorMessageTagHelper.TagName);

                 var tagHelperContent = new DefaultTagHelperContent();
                 return Task.FromResult<TagHelperContent>(tagHelperContent);
             });

        var (componentGenerator, _) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        AddDateInputParseException(viewContext, nameof(Model.Date), DateInputParseErrors.InvalidDay);

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            HtmlEncoder.Default)
        {
            For = @for,
            Id = id,
            NamePrefix = namePrefix,
            ViewContext = viewContext
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            tagHelper.ViewContext.HttpContext.GetContainerErrorContext().Errors,
            error =>
            {
                Assert.Equal("Error", error.Html);
                Assert.Equal("#" + expectedErrorFieldId, error.Href);
            });
    }

    [Fact]
    public async Task ProcessAsync_WithSpecifiedItemTypes_CreatesCorrectItems()
    {
        // Arrange
        var viewContext = CreateViewContext();
        var @for = CreateModelExpressionForTuple(viewContext, dateParts: null);
        var itemTypes = DateInputItemTypes.DayAndMonth;

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput();

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            HtmlEncoder.Default)
        {
            For = @for,
            ItemTypes = itemTypes,
            ViewContext = viewContext
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions.Items);
        Assert.Collection(
            actualOptions.Items,
            item => Assert.Equal("Day", item.Label),
            item => Assert.Equal("Month", item.Label));
    }

    [Fact]
    public async Task ProcessAsync_WithModelExpressionAndItemTypesInMetadata_CreatesCorrectItems()
    {
        // Arrange
        var viewContext = CreateViewContext();
        var itemTypes = DateInputItemTypes.DayAndMonth;
        var @for = CreateModelExpressionForTuple(viewContext, dateParts: null, itemTypes: itemTypes);

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput();

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            HtmlEncoder.Default)
        {
            For = @for,
            ViewContext = viewContext
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions.Items);
        Assert.Collection(
            actualOptions.Items,
            item => Assert.Equal("Day", item.Label),
            item => Assert.Equal("Month", item.Label));
    }

    [Fact(Skip = "Skipped until we figure out how to do this")]
    public Task ProcessAsync_WithNameAndItemTypesInMetadata_CreatesCorrectItems()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task ProcessAsync_WithModelExpressionAndDefaultItemTypesOnConverter_CreatesCorrectItems()
    {
        // Arrange
        var viewContext = CreateViewContext();
        var @for = CreateModelExpressionForModelWithCustomDayAndMonthType(viewContext, dateParts: null);

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput();

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(options => options.RegisterDateInputModelConverter(typeof(CustomDayAndMonthType), new CustomDayAndMonthTypeConverter())),
            HtmlEncoder.Default)
        {
            For = @for,
            ViewContext = viewContext
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions.Items);
        Assert.Collection(
            actualOptions.Items,
            item => Assert.Equal("Day", item.Label),
            item => Assert.Equal("Month", item.Label));
    }

    [Fact(Skip = "Skipped until we figure out how to do this")]
    public Task ProcessAsync_WithNameAndDefaultItemTypesOnConverter_CreatesCorrectItems()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task ProcessAsync_DayTagWhenDateInputItemTypesDoesNotContainDay_ThrowsInvalidOperationException()
    {
        // Arrange
        var viewContext = CreateViewContext();
        var id = "dateinput-id";
        var namePrefix = "dateinput";
        var itemTypes = DateInputItemTypes.MonthAndYear;
        var itemTagName = DateInputDayTagHelper.TagName;

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var dateInputContext = context.GetContextItem<DateInputContext>();

                dateInputContext.SetItem(
                    DateInputItemTypes.Day,
                    new DateInputContextItem() { TagName = itemTagName });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, _) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            HtmlEncoder.Default)
        {
            Id = id,
            ItemTypes = itemTypes,
            NamePrefix = namePrefix,
            ViewContext = viewContext
        };

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"Cannot declare a <{itemTagName}> when the parent's {nameof(DateInputItemTypes)} does not contain {DateInputItemTypes.Day}.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_YearTagWhenDateInputItemTypesDoesNotContainYear_ThrowsInvalidOperationException()
    {
        // Arrange
        var viewContext = CreateViewContext();
        var id = "dateinput-id";
        var namePrefix = "dateinput";
        var itemTypes = DateInputItemTypes.DayAndMonth;
        var itemTagName = DateInputYearTagHelper.TagName;

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var dateInputContext = context.GetContextItem<DateInputContext>();

                dateInputContext.SetItem(
                    DateInputItemTypes.Year,
                    new DateInputContextItem() { TagName = itemTagName });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, _) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            HtmlEncoder.Default)
        {
            Id = id,
            ItemTypes = itemTypes,
            NamePrefix = namePrefix,
            ViewContext = viewContext
        };

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"Cannot declare a <{itemTagName}> when the parent's {nameof(DateInputItemTypes)} does not contain {DateInputItemTypes.Year}.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_GeneratesErrorMessageHtmlFromErrorMessagePrefixAttribute()
    {
        // Arrange
        var viewContext = CreateViewContext();
        var errorMessagePrefix = "XYZ";
        var dateInputParseException = new DateInputParseException("{0} must contain a valid month", nameof(Model.Date), DateInputParseErrors.InvalidMonth);
        var @for = CreateModelExpression(viewContext, date: null, dateInputParseException: dateInputParseException);

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput();

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            HtmlEncoder.Default)
        {
            For = @for,
            ViewContext = viewContext,
            ErrorMessagePrefix = errorMessagePrefix
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions.ErrorMessage);
        Assert.StartsWith(errorMessagePrefix, actualOptions.ErrorMessage.Html?.ToHtmlString(HtmlEncoder.Default));
    }

    public static TheoryData<DateInputParseErrors, bool, bool, bool> DateInputParseErrorsWithExpectedErrorItemsData { get; } = new()
    {
        { DateInputParseErrors.MissingDay, true, false, false },
        { DateInputParseErrors.MissingMonth, false, true, false },
        { DateInputParseErrors.MissingYear, false, false, true },
        { DateInputParseErrors.MissingDay | DateInputParseErrors.MissingMonth, true, true, false },
        { DateInputParseErrors.MissingDay | DateInputParseErrors.MissingYear, true, false, true },
        { DateInputParseErrors.MissingMonth | DateInputParseErrors.MissingYear, false, true, true },
        { DateInputParseErrors.MissingDay | DateInputParseErrors.MissingMonth | DateInputParseErrors.InvalidYear, true, true, true },
        { DateInputParseErrors.InvalidDay, true, false, false },
        { DateInputParseErrors.InvalidMonth, false, true, false },
        { DateInputParseErrors.InvalidYear, false, false, true },
        { DateInputParseErrors.InvalidDay | DateInputParseErrors.InvalidMonth, true, true, false },
        { DateInputParseErrors.InvalidDay | DateInputParseErrors.InvalidYear, true, false, true },
        { DateInputParseErrors.InvalidMonth | DateInputParseErrors.InvalidYear, false, true, true },
        { DateInputParseErrors.InvalidDay | DateInputParseErrors.MissingMonth | DateInputParseErrors.InvalidYear, true, true, true }
    };

    private static void AssertItemHasExpectedError(DateInputOptionsItem item, bool expectError)
    {
        var classes = item.Classes?.ToString().Split(' ') ?? [];

        if (expectError)
        {
            Assert.Contains("govuk-input--error", classes);
        }
        else
        {
            Assert.DoesNotContain("govuk-input--error", classes);
        }
    }

    private static void AssertAllItemsHaveErrorClass(DateInputOptions actualOptions)
    {
        Assert.NotNull(actualOptions.Items);
        Assert.Collection(
            actualOptions.Items,
            item => AssertItemHasExpectedError(item, true),
            item => AssertItemHasExpectedError(item, true),
            item => AssertItemHasExpectedError(item, true));
    }

    private ModelExpression CreateModelExpression(
        ViewContext viewContext,
        DateOnly? date,
        string? displayName = null,
        string? description = null,
        string? errorMessage = null,
        DateInputParseException? dateInputParseException = null)
    {
        return CreateModelExpression(
            viewContext,
            typeof(DateOnly?),
            new Model() { Date = date },
            nameof(Model.Date),
            displayName,
            description,
            errorMessage,
            dateInputParseException);
    }

    private ModelExpression CreateModelExpressionForCustomDateType(
        ViewContext viewContext,
        CustomDateType? date,
        string? displayName = null,
        string? description = null,
        string? errorMessage = null,
        DateInputParseException? dateInputParseException = null,
        DateInputItemTypes? itemTypes = null)
    {
        return CreateModelExpression(
            viewContext,
            typeof(CustomDateType),
            new ModelWithCustomDateType() { Date = date },
            nameof(ModelWithCustomDateType.Date),
            displayName,
            description,
            errorMessage,
            dateInputParseException,
            itemTypes);
    }

    private ModelExpression CreateModelExpressionForTuple(
        ViewContext viewContext,
        (int Day, int Month)? dateParts,
        string? displayName = null,
        string? description = null,
        string? errorMessage = null,
        DateInputItemTypes? itemTypes = null)
    {
        return CreateModelExpression(
            viewContext,
            typeof(ValueTuple<int, int>?),
            new ModelWithTupleDate() { Date = dateParts },
            nameof(ModelWithTupleDate.Date),
            displayName,
            description,
            errorMessage,
            dateInputParseException: null,
            itemTypes);
    }

    private ModelExpression CreateModelExpressionForModelWithCustomDayAndMonthType(
        ViewContext viewContext,
        CustomDayAndMonthType? dateParts,
        string? displayName = null,
        string? description = null,
        string? errorMessage = null,
        DateInputParseException? dateInputParseException = null,
        DateInputItemTypes? itemTypes = null)
    {
        return CreateModelExpression(
            viewContext,
            typeof(CustomDayAndMonthType),
            new ModelWithCustomDayAndMonthType() { Date = dateParts },
            nameof(ModelWithCustomDayAndMonthType.Date),
            displayName,
            description,
            errorMessage,
            dateInputParseException,
            itemTypes);
    }

    private ModelExpression CreateModelExpression(
        ViewContext viewContext,
        Type propertyType,
        object model,
        string modelPropertyName,
        string? displayName = null,
        string? description = null,
        string? errorMessage = null,
        DateInputParseException? dateInputParseException = null,
        DateInputItemTypes? itemTypes = null)
    {
        var modelType = model.GetType();
        var identity = ModelMetadataIdentity.ForProperty(modelType.GetProperty(modelPropertyName)!, propertyType, modelType);

        var modelMetadataProvider = new TestModelMetadataProvider();
        modelMetadataProvider.DetailsProvider.SetDisplayNameForProperty(identity, displayName);
        modelMetadataProvider.DetailsProvider.SetDescriptionForProperty(identity, description);
        modelMetadataProvider.DetailsProvider.SetDateInputItemTypesForProperty(identity, itemTypes);

        var modelExplorer = modelMetadataProvider.GetModelExplorerForType(modelType, model)
            .GetExplorerForProperty(modelPropertyName);

        if (errorMessage is not null)
        {
            viewContext.ModelState.AddModelError(modelPropertyName, errorMessage);
        }
        else if (dateInputParseException is not null)
        {
            AddDateInputParseException(viewContext, modelPropertyName, dateInputParseException);
        }

        return new ModelExpression(modelPropertyName, modelExplorer);
    }

    private class Model
    {
        public DateOnly? Date { get; set; }
    }

    private class ModelWithCustomDateType
    {
        public CustomDateType? Date { get; set; }
    }

    private class ModelWithTupleDate
    {
        public (int, int)? Date { get; set; }
    }

    private class ModelWithCustomDayAndMonthType
    {
        public CustomDayAndMonthType? Date { get; set; }
    }

    private record CustomDateType(int Year, int Month, int Day)
    {
        public int D => Day;
        public int M => Month;
        public int Y => Year;
    }

    private class CustomDateTypeConverter : DateInputModelConverter
    {
        protected override object ConvertToModelCore(DateInputConvertToModelContext context) =>
            new CustomDateType(context.ItemValues.Year!.Value, context.ItemValues.Month!.Value, context.ItemValues.Day!.Value);

        protected override DateInputItemValues? ConvertFromModelCore(DateInputConvertFromModelContext context)
        {
            var cdt = (CustomDateType)context.Model;
            return new(cdt.D, cdt.M, cdt.Y);
        }
    }

    private record CustomDayAndMonthType(int Day, int Month)
    {
        public int D => Day;
        public int M => Month;
    }

    private class CustomDayAndMonthTypeConverter : DateInputModelConverter
    {
        public override DateInputItemTypes? DefaultItemTypes => DateInputItemTypes.DayAndMonth;

        protected override object ConvertToModelCore(DateInputConvertToModelContext context) =>
            new CustomDayAndMonthType(context.ItemValues.Day!.Value, context.ItemValues.Month!.Value);

        protected override DateInputItemValues? ConvertFromModelCore(DateInputConvertFromModelContext context)
        {
            var cdt = (CustomDayAndMonthType)context.Model;
            return new(cdt.D, cdt.M, null);
        }
    }
}
