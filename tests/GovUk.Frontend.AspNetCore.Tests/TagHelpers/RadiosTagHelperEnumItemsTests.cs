using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class RadiosTagHelperEnumItemsTests : TagHelperTestBase<RadiosTagHelper>
{
    [Fact]
    public async Task ProcessAsync_ForEnumWithNoItems_GeneratesItemFromEachMember()
    {
        // Arrange
        var model = new EnumItemsTestModel { Colour = EnumItemsColour.Red };

        // Act
        var options = await ProcessAsync(model, nameof(EnumItemsTestModel.Colour));

        // Assert
        Assert.Collection(
            options.Items!,
            item =>
            {
                Assert.Equal("Blue", item!.Text);
                Assert.Equal("2", item.Value);
                Assert.Null(item.Hint);
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
                Assert.False(item.Checked);
            });
    }

    [Fact]
    public async Task ProcessAsync_ForNullableEnumWithNullValue_GeneratesUncheckedItems()
    {
        // Arrange
        var model = new EnumItemsTestModel { NullableColour = null };

        // Act
        var options = await ProcessAsync(model, nameof(EnumItemsTestModel.NullableColour));

        // Assert
        Assert.Equal(["2", "1", "0"], options.Items!.Select(i => i!.Value!.ToHtmlString()));
        Assert.All(options.Items!, i => Assert.False(i!.Checked));
    }

    [Fact]
    public async Task ProcessAsync_WithExcludeValues_DoesNotGenerateExcludedMembers()
    {
        // Arrange
        var model = new EnumItemsTestModel();

        // Act
        var options = await ProcessAsync(
            model,
            nameof(EnumItemsTestModel.Colour),
            configure: t => t.ExcludeValues = [EnumItemsColour.None]);

        // Assert
        Assert.Equal(["2", "1"], options.Items!.Select(i => i!.Value!.ToHtmlString()));
    }

    [Fact]
    public async Task ProcessAsync_WithExcludeValuesOfDifferentEnumType_Throws()
    {
        // Arrange
        var model = new EnumItemsTestModel();

        // Act
        var ex = await Record.ExceptionAsync(() => ProcessAsync(
            model,
            nameof(EnumItemsTestModel.Colour),
            configure: t => t.ExcludeValues = [EnumItemsFlags.First]));

        // Assert
        Assert.IsType<ArgumentException>(ex);
    }

    [Theory]
    [InlineData("Red")]
    [InlineData("red")]
    [InlineData("1")]
    public async Task ProcessAsync_WithAttemptedValueInModelState_ChecksMatchingItem(string attemptedValue)
    {
        // Arrange
        var model = new EnumItemsTestModel { Colour = EnumItemsColour.Blue };
        var viewContext = TestUtils.CreateViewContext();
        viewContext.ModelState.SetModelValue(nameof(EnumItemsTestModel.Colour), attemptedValue, attemptedValue);

        // Act
        var options = await ProcessAsync(model, nameof(EnumItemsTestModel.Colour), viewContext: viewContext);

        // Assert
        Assert.Equal(["1"], options.Items!.Where(i => i!.Checked == true).Select(i => i!.Value!.ToHtmlString()));
    }

    [Fact]
    public async Task ProcessAsync_WithDeclaredItems_DoesNotGenerateItems()
    {
        // Arrange
        var model = new EnumItemsTestModel();

        // Act
        var options = await ProcessAsync(
            model,
            nameof(EnumItemsTestModel.Colour),
            declareItems: ctx => ctx.AddItem(
                new RadiosOptionsItem { Html = new TemplateString("Custom"), Value = "Red" },
                RadiosItemTagHelper.TagName));

        // Assert
        var item = Assert.Single(options.Items!);
        Assert.Equal("Custom", item!.Html?.ToHtmlString());
        Assert.Null(item.Text);
    }

    [Fact]
    public async Task ProcessAsync_WithGenerateItemsAndDeclaredItems_MergesItems()
    {
        // Arrange
        var model = new EnumItemsTestModel();
        var conditional = new RadiosOptionsItemConditional { Html = new HtmlString("Conditional") };

        // Act
        var options = await ProcessAsync(
            model,
            nameof(EnumItemsTestModel.Colour),
            configure: t => t.GenerateItems = true,
            declareItems: ctx =>
            {
                // Replaces the generated item, keeping the generated label and hint
                ctx.AddItem(
                    new RadiosOptionsItem { Html = new TemplateString(""), Value = "Red", Conditional = conditional },
                    RadiosItemTagHelper.TagName);

                // Goes before the next declared item that's a member
                ctx.AddItem(new RadiosOptionsItem { Divider = "or" }, RadiosItemDividerTagHelper.TagName);
                ctx.AddItem(
                    new RadiosOptionsItem { Html = new TemplateString("None"), Value = "None" },
                    RadiosItemTagHelper.TagName);

                // Not a member and nothing follows it; goes at the end
                ctx.AddItem(
                    new RadiosOptionsItem { Html = new TemplateString("Other"), Value = "Other" },
                    RadiosItemTagHelper.TagName);
            });

        // Assert
        Assert.Collection(
            options.Items!,
            item => Assert.Equal("2", item!.Value),
            item =>
            {
                Assert.Equal("Red", item!.Value);
                Assert.Equal("Bright red", item.Text);
                Assert.Equal("Like a postbox", item.Hint?.Text);
                Assert.Same(conditional, item.Conditional);
            },
            item => Assert.Equal("or", item!.Divider),
            item =>
            {
                Assert.Equal("None", item!.Value);
                Assert.Equal("None", item.Html?.ToHtmlString());
                Assert.Null(item.Text);
            },
            item => Assert.Equal("Other", item!.Value));
    }

    [Fact]
    public async Task ProcessAsync_WithGenerateItemsAndDeclaredItemMatchingByNumber_ReplacesGeneratedItem()
    {
        // Arrange
        var model = new EnumItemsTestModel();

        // Act
        var options = await ProcessAsync(
            model,
            nameof(EnumItemsTestModel.Colour),
            configure: t => t.GenerateItems = true,
            declareItems: ctx => ctx.AddItem(
                new RadiosOptionsItem { Html = new TemplateString(""), Value = "1" },
                RadiosItemTagHelper.TagName));

        // Assert
        Assert.Collection(
            options.Items!,
            item => Assert.Equal("2", item!.Value),
            item =>
            {
                Assert.Equal("1", item!.Value);
                Assert.Equal("Bright red", item.Text);
                Assert.Equal("Like a postbox", item.Hint?.Text);
            },
            item => Assert.Equal("0", item!.Value));
    }

    [Fact]
    public async Task ProcessAsync_WithDataAnnotationsLocalization_LocalizesLabelsAndHints()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IStringLocalizerFactory, FakeStringLocalizerFactory>();
        services.AddMvc().AddDataAnnotationsLocalization();

        var viewContext = TestUtils.CreateViewContext();
        viewContext.HttpContext.RequestServices = services.BuildServiceProvider();

        var model = new EnumItemsTestModel();

        // Act
        var options = await ProcessAsync(model, nameof(EnumItemsTestModel.Colour), viewContext: viewContext);

        // Assert
        // The fake localizer prefixes strings with the name of the type it's for; MVC uses the enum type.
        var red = options.Items!.Single(i => i!.Value == "1");
        Assert.Equal("EnumItemsColour:Bright red", red!.Text);
        Assert.Equal("EnumItemsColour:Like a postbox", red.Hint?.Text);
    }

    [Fact]
    public async Task ProcessAsync_WithoutDataAnnotationsLocalization_DoesNotLocalizeHints()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMvc();

        var viewContext = TestUtils.CreateViewContext();
        viewContext.HttpContext.RequestServices = services.BuildServiceProvider();

        var model = new EnumItemsTestModel();

        // Act
        var options = await ProcessAsync(model, nameof(EnumItemsTestModel.Colour), viewContext: viewContext);

        // Assert
        var red = options.Items!.Single(i => i!.Value == "1");
        Assert.Equal("Bright red", red!.Text);
        Assert.Equal("Like a postbox", red.Hint?.Text);
    }

    [Fact]
    public async Task ProcessAsync_WithGenerateItemsFalse_DoesNotGenerateItems()
    {
        // Arrange
        var model = new EnumItemsTestModel();

        // Act
        var options = await ProcessAsync(model, nameof(EnumItemsTestModel.Colour), configure: t => t.GenerateItems = false);

        // Assert
        Assert.Empty(options.Items!);
    }

    [Fact]
    public async Task ProcessAsync_ForNonEnum_DoesNotGenerateItems()
    {
        // Arrange
        var model = new EnumItemsTestModel();

        // Act
        var options = await ProcessAsync(model, nameof(EnumItemsTestModel.Text));

        // Assert
        Assert.Empty(options.Items!);
    }

    [Fact]
    public async Task ProcessAsync_WithGenerateItemsForNonEnum_Throws()
    {
        // Arrange
        var model = new EnumItemsTestModel();

        // Act
        var ex = await Record.ExceptionAsync(() =>
            ProcessAsync(model, nameof(EnumItemsTestModel.Text), configure: t => t.GenerateItems = true));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            "The 'generate-items' attribute requires the 'for' attribute to be bound to an enum or nullable enum.",
            ex.Message);
    }

    private async Task<RadiosOptions> ProcessAsync(
        EnumItemsTestModel model,
        string propertyName,
        Action<RadiosTagHelper>? configure = null,
        Action<RadiosContext>? declareItems = null,
        ViewContext? viewContext = null)
    {
        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (_, _) =>
            {
                declareItems?.Invoke(context.GetContextItem<RadiosContext>());
                return Task.FromResult<TagHelperContent>(new DefaultTagHelperContent());
            });

        var (componentGenerator, getActualOptions) =
            CreateComponentGenerator<RadiosOptions>(nameof(IComponentGenerator.GenerateRadiosAsync));

        var tagHelper = new RadiosTagHelper(componentGenerator, new DefaultModelHelper())
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
