using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CheckboxesTagHelperEnumItemsTests : TagHelperTestBase<CheckboxesTagHelper>
{
    [Fact]
    public async Task ProcessAsync_ForEnumCollectionWithNoItems_GeneratesItemFromEachMember()
    {
        // Arrange
        var model = new EnumItemsTestModel { Colours = [EnumItemsColour.Red, EnumItemsColour.None] };

        // Act
        var options = await ProcessAsync(model, nameof(EnumItemsTestModel.Colours));

        // Assert
        Assert.Collection(
            options.Items!,
            item =>
            {
                Assert.Equal("Blue", item!.Text);
                Assert.Equal("2", item.Value);
                Assert.False(item.Checked);
            },
            item =>
            {
                Assert.Equal("Bright red", item!.Text);
                Assert.Equal("1", item.Value);
                Assert.Equal("Like a postbox", item.Hint?.Text);
                Assert.True(item.Checked);
            },
            item =>
            {
                Assert.Equal("None of these", item!.Text);
                Assert.Equal("0", item.Value);
                Assert.True(item.Checked);
            });
    }

    [Fact]
    public async Task ProcessAsync_WithRawValuesInModelState_ChecksMatchingItems()
    {
        // Arrange
        var model = new EnumItemsTestModel { Colours = [EnumItemsColour.None] };
        var viewContext = TestUtils.CreateViewContext();
        string[] rawValue = ["Blue", "1"];
        viewContext.ModelState.SetModelValue(nameof(EnumItemsTestModel.Colours), rawValue, string.Join(",", rawValue));

        // Act
        var options = await ProcessAsync(model, nameof(EnumItemsTestModel.Colours), viewContext: viewContext);

        // Assert
        Assert.Equal(["2", "1"], options.Items!.Where(i => i!.Checked == true).Select(i => i!.Value!.ToHtmlString()));
    }

    [Fact]
    public async Task ProcessAsync_WithGenerateItemsAndDeclaredItem_MergesItems()
    {
        // Arrange
        var model = new EnumItemsTestModel();

        // Act
        var options = await ProcessAsync(
            model,
            nameof(EnumItemsTestModel.Colours),
            configure: t => t.GenerateItems = true,
            declareItems: ctx =>
            {
                ctx.AddItem(new CheckboxesOptionsItem { Divider = "or" }, CheckboxesItemDividerTagHelper.TagName);
                ctx.AddItem(
                    new CheckboxesOptionsItem { Html = new TemplateString(""), Value = "None", Behaviour = "exclusive" },
                    CheckboxesItemTagHelper.TagName);
            });

        // Assert
        Assert.Collection(
            options.Items!,
            item => Assert.Equal("2", item!.Value),
            item => Assert.Equal("1", item!.Value),
            item => Assert.Equal("or", item!.Divider),
            item =>
            {
                Assert.Equal("None", item!.Value);
                Assert.Equal("None of these", item.Text);
                Assert.Equal("exclusive", item.Behaviour);
            });
    }

    [Fact]
    public async Task ProcessAsync_WithDeclaredItems_DoesNotGenerateItems()
    {
        // Arrange
        var model = new EnumItemsTestModel();

        // Act
        var options = await ProcessAsync(
            model,
            nameof(EnumItemsTestModel.Colours),
            declareItems: ctx => ctx.AddItem(
                new CheckboxesOptionsItem { Html = new TemplateString("Custom"), Value = "Red" },
                CheckboxesItemTagHelper.TagName));

        // Assert
        Assert.Single(options.Items!);
    }

    [Fact]
    public async Task ProcessAsync_ForFlagsEnum_GeneratesItemForEachSingleBitMember()
    {
        // Arrange
        var model = new EnumItemsTestModel { Flags = EnumItemsFlags.Both };

        // Act
        var options = await ProcessAsync(model, nameof(EnumItemsTestModel.Flags));

        // Assert
        // None (zero) and Both (a combination) don't get items
        Assert.Collection(
            options.Items!,
            item =>
            {
                Assert.Equal("Second one", item!.Text);
                Assert.Equal("2", item.Value);
                Assert.True(item.Checked);
            },
            item =>
            {
                Assert.Equal("First", item!.Text);
                Assert.Equal("1", item.Value);
                Assert.True(item.Checked);
            },
            item =>
            {
                Assert.Equal("Third", item!.Text);
                Assert.Equal("4", item.Value);
                Assert.False(item.Checked);
            });
    }

    [Fact]
    public async Task ProcessAsync_ForNullableFlagsEnumWithNullValue_GeneratesUncheckedItems()
    {
        // Arrange
        var model = new EnumItemsTestModel { NullableFlags = null };

        // Act
        var options = await ProcessAsync(model, nameof(EnumItemsTestModel.NullableFlags));

        // Assert
        Assert.Equal(["2", "1", "4"], options.Items!.Select(i => i!.Value!.ToHtmlString()));
        Assert.All(options.Items!, i => Assert.False(i!.Checked));
    }

    [Fact]
    public async Task ProcessAsync_ForFlagsEnumWithExcludeValues_DoesNotGenerateExcludedMembers()
    {
        // Arrange
        var model = new EnumItemsTestModel();

        // Act
        var options = await ProcessAsync(
            model,
            nameof(EnumItemsTestModel.Flags),
            configure: t => t.ExcludeValues = [EnumItemsFlags.Third]);

        // Assert
        Assert.Equal(["2", "1"], options.Items!.Select(i => i!.Value!.ToHtmlString()));
    }

    [Fact]
    public async Task ProcessAsync_ForFlagsEnumWithRawValuesInModelState_ChecksItemsFromAllValues()
    {
        // Arrange
        var model = new EnumItemsTestModel();
        var viewContext = TestUtils.CreateViewContext();
        string[] rawValue = ["First", "4"];
        viewContext.ModelState.SetModelValue(nameof(EnumItemsTestModel.Flags), rawValue, string.Join(",", rawValue));

        // Act
        var options = await ProcessAsync(model, nameof(EnumItemsTestModel.Flags), viewContext: viewContext);

        // Assert
        Assert.Equal(["1", "4"], options.Items!.Where(i => i!.Checked == true).Select(i => i!.Value!.ToHtmlString()));
    }

    [Fact]
    public async Task ProcessAsync_ForFlagsEnumWithDeclaredItemMatchingByNumber_ReplacesGeneratedItem()
    {
        // Arrange
        var model = new EnumItemsTestModel();

        // Act
        var options = await ProcessAsync(
            model,
            nameof(EnumItemsTestModel.Flags),
            configure: t => t.GenerateItems = true,
            declareItems: ctx => ctx.AddItem(
                new CheckboxesOptionsItem { Html = new TemplateString("Custom"), Value = "4" },
                CheckboxesItemTagHelper.TagName));

        // Assert
        Assert.Collection(
            options.Items!,
            item => Assert.Equal("2", item!.Value),
            item => Assert.Equal("1", item!.Value),
            item => Assert.Equal("Custom", item!.Html?.ToHtmlString()));
    }

    [Fact]
    public async Task ProcessAsync_WithGenerateItemsForSingleEnum_Throws()
    {
        // Arrange
        var model = new EnumItemsTestModel();

        // Act
        var ex = await Record.ExceptionAsync(() =>
            ProcessAsync(model, nameof(EnumItemsTestModel.Colour), configure: t => t.GenerateItems = true));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            "The 'generate-items' attribute requires the 'for' attribute to be bound to a collection of an enum type or to a [Flags] enum.",
            ex.Message);
    }

    private async Task<CheckboxesOptions> ProcessAsync(
        EnumItemsTestModel model,
        string propertyName,
        Action<CheckboxesTagHelper>? configure = null,
        Action<CheckboxesContext>? declareItems = null,
        ViewContext? viewContext = null)
    {
        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (_, _) =>
            {
                declareItems?.Invoke(context.GetContextItem<CheckboxesContext>());
                return Task.FromResult<TagHelperContent>(new DefaultTagHelperContent());
            });

        var (componentGenerator, getActualOptions) =
            CreateComponentGenerator<CheckboxesOptions>(nameof(IComponentGenerator.GenerateCheckboxesAsync));

        var tagHelper = new CheckboxesTagHelper(componentGenerator, new DefaultModelHelper())
        {
            For = EnumItemsTestModel.CreateModelExpression(model, propertyName, viewContext?.HttpContext?.RequestServices),
            ViewContext = viewContext ?? TestUtils.CreateViewContext()
        };

        configure?.Invoke(tagHelper);

        tagHelper.Init(context);
        await tagHelper.ProcessAsync(context, output);

        return getActualOptions();
    }
}
