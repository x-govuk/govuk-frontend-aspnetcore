using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.ModelBinding;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
                    DateInputTagHelper.HintTagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            new DateInputParseErrorsProvider(),
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
            new DateInputParseErrorsProvider(),
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

                var fieldsetContext = new DateInputFieldsetContext(describedBy: null, attributes: new(), @for);

                dateInputContext.OpenFieldset();
                fieldsetContext.SetLegend(isPageHeading: false, attributes: new(), html: null);
                dateInputContext.CloseFieldset(fieldsetContext);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            new DateInputParseErrorsProvider(),
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

                var fieldsetContext = new DateInputFieldsetContext(describedBy: null, attributes: new(), @for);

                dateInputContext.OpenFieldset();
                fieldsetContext.SetLegend(isPageHeading: false, attributes: new(), html: legendContent);
                dateInputContext.CloseFieldset(fieldsetContext);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            new DateInputParseErrorsProvider(),
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
            new DateInputParseErrorsProvider(),
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
                dateInputContext.SetHint(attributes: new(), html: hintContent, tagName: DateInputTagHelper.HintTagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            new DateInputParseErrorsProvider(),
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
                    DateInputTagHelper.HintTagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            new DateInputParseErrorsProvider(),
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
            new DateInputParseErrorsProvider(),
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
                    attributes: new(),
                    html: errorContent,
                    DateInputTagHelper.HintTagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            new DateInputParseErrorsProvider(),
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
            new DateInputParseErrorsProvider(),
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
                Assert.Equal($"{nameof(Model.Date)}-Day", item.Id);
                Assert.Equal($"{nameof(Model.Date)}.Day", item.Name);
                Assert.Equal(value.Day.ToString(), item.Value);
            },
            item =>
            {
                Assert.Equal($"{nameof(Model.Date)}-Month", item.Id);
                Assert.Equal($"{nameof(Model.Date)}.Month", item.Name);
                Assert.Equal(value.Month.ToString(), item.Value);
            },
            item =>
            {
                Assert.Equal($"{nameof(Model.Date)}-Year", item.Id);
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
            new DateInputParseErrorsProvider(),
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

                dateInputContext.SetItem(DateInputItemType.Day, new DateInputContextItem()
                {
                    TagName = DateInputDayTagHelper.TagName,
                    Id = dayId,
                    Name = dayName,
                    LabelHtml = dayLabel,
                    Value = dayValue,
                    ValueSpecified = true
                });

                dateInputContext.SetItem(DateInputItemType.Month, new DateInputContextItem()
                {
                    TagName = DateInputMonthTagHelper.TagName,
                    Id = monthId,
                    Name = monthName,
                    LabelHtml = monthLabel,
                    Value = monthValue,
                    ValueSpecified = true
                });

                dateInputContext.SetItem(DateInputItemType.Year, new DateInputContextItem()
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
            new DateInputParseErrorsProvider(),
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

                dateInputContext.SetItem(DateInputItemType.Day, new DateInputContextItem()
                {
                    TagName = DateInputDayTagHelper.TagName,
                    Id = dayId,
                    Name = dayName,
                    LabelHtml = dayLabel,
                    Value = dayValue,
                    ValueSpecified = true
                });

                dateInputContext.SetItem(DateInputItemType.Month, new DateInputContextItem()
                {
                    TagName = DateInputMonthTagHelper.TagName,
                    Id = monthId,
                    Name = monthName,
                    LabelHtml = monthLabel,
                    Value = monthValue,
                    ValueSpecified = true
                });

                dateInputContext.SetItem(DateInputItemType.Year, new DateInputContextItem()
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
            new DateInputParseErrorsProvider(),
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
            new DateInputParseErrorsProvider(),
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
            CreateOptions(options => options.DateInputModelConverters.Add(customDateTypeConverter)),
            new DateInputParseErrorsProvider(),
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
            CreateOptions(options => options.DateInputModelConverters.Add(customDateTypeConverter)),
            new DateInputParseErrorsProvider(),
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
    [InlineData(DateInputItems.All, true, true, true)]
    [InlineData(DateInputItems.Day, true, false, false)]
    [InlineData(DateInputItems.Month, false, true, false)]
    [InlineData(DateInputItems.Year, false, false, true)]
    [InlineData(DateInputItems.Day | DateInputItems.Month, true, true, false)]
    [InlineData(DateInputItems.Day | DateInputItems.Year, true, false, true)]
    [InlineData(DateInputItems.Month | DateInputItems.Year, false, true, true)]
    public async Task ProcessAsync_HaveErrorClassesWhenErrorSpecified(
        DateInputItems? specifiedErrorFields,
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
                    attributes: new(),
                    html: "Error",
                    tagName: DateInputTagHelper.ErrorMessageTagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            new DateInputParseErrorsProvider(),
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
    [InlineData(DateInputParseErrors.MissingDay, true, false, false)]
    [InlineData(DateInputParseErrors.MissingMonth, false, true, false)]
    [InlineData(DateInputParseErrors.MissingYear, false, false, true)]
    [InlineData(DateInputParseErrors.MissingDay | DateInputParseErrors.MissingMonth, true, true, false)]
    [InlineData(DateInputParseErrors.MissingDay | DateInputParseErrors.MissingYear, true, false, true)]
    [InlineData(DateInputParseErrors.MissingMonth | DateInputParseErrors.MissingYear, false, true, true)]
    [InlineData(DateInputParseErrors.MissingDay | DateInputParseErrors.MissingMonth | DateInputParseErrors.InvalidYear, true, true, true)]
    [InlineData(DateInputParseErrors.InvalidDay, true, false, false)]
    [InlineData(DateInputParseErrors.InvalidMonth, false, true, false)]
    [InlineData(DateInputParseErrors.InvalidYear, false, false, true)]
    [InlineData(DateInputParseErrors.InvalidDay | DateInputParseErrors.InvalidMonth, true, true, false)]
    [InlineData(DateInputParseErrors.InvalidDay | DateInputParseErrors.InvalidYear, true, false, true)]
    [InlineData(DateInputParseErrors.InvalidMonth | DateInputParseErrors.InvalidYear, false, true, true)]
    [InlineData(DateInputParseErrors.InvalidDay | DateInputParseErrors.MissingMonth | DateInputParseErrors.InvalidYear, true, true, true)]
    public async Task ProcessAsync_ErrorItemsNotSpecifiedAndErrorsFromModelBinder_InfersErrorItems(
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
                    attributes: new(),
                    html: "Error",
                    tagName: DateInputTagHelper.ErrorMessageTagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var dateInputParseErrorsProvider = new DateInputParseErrorsProvider();
        SetModelErrors(nameof(Model.Date), parseErrors, dateInputParseErrorsProvider, viewContext);

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            dateInputParseErrorsProvider,
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

                // Explictly set a different set of DateInputErrorComponents than we have in DateInputParseErrorsProvider
                dateInputContext.SetErrorMessage(
                    errorFields: DateInputItems.Month | DateInputItems.Year,
                    visuallyHiddenText: null,
                    attributes: new(),
                    html: "Error",
                    tagName: DateInputTagHelper.ErrorMessageTagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var dateInputParseErrorsProvider = new DateInputParseErrorsProvider();
        SetModelErrors(nameof(Model.Date), DateInputParseErrors.InvalidDay, dateInputParseErrorsProvider, viewContext);

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            dateInputParseErrorsProvider,
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
    [InlineData(DateInputItems.Day, "dateinput-id-Day")]
    [InlineData(DateInputItems.Day | DateInputItems.Month, "dateinput-id-Day")]
    [InlineData(DateInputItems.Day | DateInputItems.Month | DateInputItems.Year, "dateinput-id-Day")]
    [InlineData(DateInputItems.Month, "dateinput-id-Month")]
    [InlineData(DateInputItems.Month | DateInputItems.Year, "dateinput-id-Month")]
    [InlineData(null, "dateinput-id-Day")]
    public async Task ProcessAsync_WithError_AddsErrorWithCorrectFieldIdToFormErrorContext(
        DateInputItems? errorFields,
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
                     attributes: new(),
                     html: "Error",
                     tagName: DateInputTagHelper.ErrorMessageTagName);

                 var tagHelperContent = new DefaultTagHelperContent();
                 return Task.FromResult<TagHelperContent>(tagHelperContent);
             });

        var (componentGenerator, _) = CreateComponentGenerator<DateInputOptions>(nameof(IComponentGenerator.GenerateDateInputAsync));

        var dateInputParseErrorsProvider = new DateInputParseErrorsProvider();
        SetModelErrors(nameof(Model.Date), DateInputParseErrors.InvalidDay, dateInputParseErrorsProvider, viewContext);

        var tagHelper = new DateInputTagHelper(
            componentGenerator,
            CreateOptions(),
            dateInputParseErrorsProvider,
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

    private static void SetModelErrors(
        string modelName,
        DateInputParseErrors dateInputParseErrors,
        DateInputParseErrorsProvider dateInputParseErrorsProvider,
        ViewContext viewContext,
        string? modelStateError = null)
    {
        viewContext.ModelState.SetModelValue(modelName, new ValueProviderResult(""));
        var propertyModelState = viewContext.ModelState[modelName]!;
        dateInputParseErrorsProvider.SetErrorsForModel(propertyModelState, dateInputParseErrors);
        viewContext.ModelState.AddModelError(modelName, modelStateError ?? $"{modelName} must be a real date.");
    }

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
        string? errorMessage = null)
    {
        var modelName = nameof(Model.Date);

        var modelMetadataProvider = new DefaultModelMetadataProvider(
            new TestCompositeMetadataDetailsProvider(modelName, displayName, description));

        var modelExplorer = modelMetadataProvider.GetModelExplorerForType(typeof(Model), new Model() { Date = date })
            .GetExplorerForProperty(modelName);

        if (errorMessage is not null)
        {
            viewContext.ModelState.AddModelError(modelName, errorMessage);
        }

        return new ModelExpression(modelName, modelExplorer);
    }

    private ModelExpression CreateModelExpressionForCustomDateType(
        ViewContext viewContext,
        CustomDateType? date,
        string? displayName = null,
        string? description = null,
        string? errorMessage = null)
    {
        var modelName = nameof(ModelWithCustomDateType.Date);

        var modelMetadataProvider = new DefaultModelMetadataProvider(
            new TestCompositeMetadataDetailsProvider(modelName, displayName, description));

        var modelExplorer = modelMetadataProvider.GetModelExplorerForType(typeof(ModelWithCustomDateType), new ModelWithCustomDateType() { Date = date })
            .GetExplorerForProperty(modelName);

        if (errorMessage is not null)
        {
            viewContext.ModelState.AddModelError(modelName, errorMessage);
        }

        return new ModelExpression(modelName, modelExplorer);
    }

    private class Model
    {
        public DateOnly? Date { get; set; }
    }

    private class ModelWithCustomDateType
    {
        public CustomDateType? Date { get; set; }
    }

    private class CustomDateType(int year, int month, int day)
    {
        public int D { get; } = day;
        public int M { get; } = month;
        public int Y { get; } = year;
    }

    private class CustomDateTypeConverter : DateInputModelConverter
    {
        public override bool CanConvertModelType(Type modelType) => modelType == typeof(CustomDateType);

        public override object CreateModelFromDate(Type modelType, DateOnly date) => new CustomDateType(date.Year, date.Month, date.Day);

        public override DateOnly? GetDateFromModel(Type modelType, object model)
        {
            var cdt = (CustomDateType)model;
            return new DateOnly(cdt.Y, cdt.M, cdt.D);
        }
    }
}
