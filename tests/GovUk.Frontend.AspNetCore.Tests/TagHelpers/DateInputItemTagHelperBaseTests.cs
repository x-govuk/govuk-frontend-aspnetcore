using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public abstract class DateInputItemTagHelperBaseTests<T> : TagHelperTestBase<T>
    where T : DateInputItemTagHelperBase, new()
{
    protected async Task ProcessAsync_AddItemToContext(string id, string name, string value)
    {
        // Arrange
        var labelContent = "Label";
        var pattern = "*";
        var autoComplete = "off";
        var inputMode = "im";
        var attributes = CreateDummyDataAttributes();

        var dateInputContext = new DateInputContext(haveExplicitValue: false, @for: null);

        var context = CreateTagHelperContext(
            attributes: attributes,
            contexts: dateInputContext);

        var output = CreateTagHelperOutput(
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var itemContext = context.GetContextItem<DateInputItemContext>();
                itemContext.SetLabel(html: labelContent, attributes: [], tagName: TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new T()
        {
            AutoComplete = autoComplete,
            Id = id,
            InputMode = inputMode,
            Name = name,
            Pattern = pattern,
            Value = value
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            dateInputContext.Items.Values,
            item =>
            {
                Assert.Equal(autoComplete, item.AutoComplete);
                Assert.Equal(id, item.Id);
                Assert.Equal(inputMode, item.InputMode);
                Assert.Equal(labelContent, item.LabelHtml);
                Assert.Equal(name, item.Name);
                Assert.Equal(pattern, item.Pattern);
                Assert.Equal(value, item.Value);
                Assert.True(item.ValueSpecified);
                AssertContainsAttributes(attributes, item.Attributes);
            });
    }

    [Fact]
    public async Task ProcessAsync_ValueSpecifiedAndParentHasValue_ThrowsInvalidOperationException()
    {
        // Arrange
        var value = "1";

        var dateInputContext = new DateInputContext(haveExplicitValue: true, @for: null);

        var context = CreateTagHelperContext(contexts: dateInputContext);

        var output = CreateTagHelperOutput();

        var tagHelper = new T()
        {
            Value = value
        };

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"Value cannot be specified for both <{TagName}> and the parent <{ParentTagName}>.", ex.Message);
    }
}
