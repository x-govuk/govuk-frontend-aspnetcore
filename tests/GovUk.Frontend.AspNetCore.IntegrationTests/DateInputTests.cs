using System.ComponentModel.DataAnnotations;
using GovUk.Frontend.AspNetCore.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Playwright;

namespace GovUk.Frontend.AspNetCore.IntegrationTests;

public class DateInputTests(DateInputTestsFixture fixture) : IClassFixture<DateInputTestsFixture>
{
    public IBrowser Browser { get; } = fixture.Browser!;

    [Fact]
    public async Task ForDateProperty()
    {
        var page = await Browser.NewPageAsync(new BrowserNewPageOptions() { BaseURL = ServerFixture.BaseUrl });

        // Initial values should come from For property
        await page.GotoAsync("/DateInputTests/ForDate");
        await AssertFieldsForCompleteDate(page, "1", "4", "2020");

        // Change the values and POST them, including some invalid values
        var day = "x";
        var month = "y";
        var year = "2022";

        await page.FillAsync("[name='Date.Day']", day);
        await page.FillAsync("[name='Date.Month']", month);
        await page.FillAsync("[name='Date.Year']", year);

        await page.RunAndWaitForNavigationAsync(() => page.Keyboard.PressAsync("Enter"));

        // Verify POSTed values have been round-tripped, including the invalid values
        await AssertFieldsForCompleteDate(page, day, month, year, expectDayToHaveError: true, expectMonthToHaveError: true, expectYearToHaveError: false, expectedErrorMessage: "Date of birth must be a real date");
    }

    [Fact]
    public async Task ForCustomDateProperty()
    {
        var page = await Browser.NewPageAsync(new BrowserNewPageOptions() { BaseURL = ServerFixture.BaseUrl });

        // Initial values should come from For property
        await page.GotoAsync("/DateInputTests/ForCustomDate");
        await AssertFieldsForCompleteDate(page, "1", "4", "2020");

        // Change the values and POST them, including some invalid values
        var day = "x";
        var month = "y";
        var year = "2022";

        await page.FillAsync("[name='CustomDate.Day']", day);
        await page.FillAsync("[name='CustomDate.Month']", month);
        await page.FillAsync("[name='CustomDate.Year']", year);

        await page.RunAndWaitForNavigationAsync(() => page.Keyboard.PressAsync("Enter"));

        // Verify POSTed values have been round-tripped, including the invalid values
        await AssertFieldsForCompleteDate(page, day, month, year, expectDayToHaveError: true, expectMonthToHaveError: true, expectYearToHaveError: false, expectedErrorMessage: "Date of birth must be a real date");
    }

    [Fact]
    public async Task ValueDateProperty()
    {
        var page = await Browser.NewPageAsync(new BrowserNewPageOptions() { BaseURL = ServerFixture.BaseUrl });

        // Initial values should come from For property
        await page.GotoAsync("/DateInputTests/ValueDate");
        await AssertFieldsForCompleteDate(page, "1", "4", "2020");

        // Change the values and POST them
        var day = "3";
        var month = "4";
        var year = "2022";

        await page.FillAsync("[name='Date.Day']", day);
        await page.FillAsync("[name='Date.Month']", month);
        await page.FillAsync("[name='Date.Year']", year);

        await page.RunAndWaitForNavigationAsync(() => page.Keyboard.PressAsync("Enter"));

        // Verify POSTed values have been round-tripped
        await AssertFieldsForCompleteDate(page, day, month, year);
    }

    [Fact]
    public async Task ValueCustomDateProperty()
    {
        var page = await Browser.NewPageAsync(new BrowserNewPageOptions() { BaseURL = ServerFixture.BaseUrl });

        // Initial values should come from For property
        await page.GotoAsync("/DateInputTests/ValueCustomDate");
        await AssertFieldsForCompleteDate(page, "1", "4", "2020");

        // Change the values and POST them
        var day = "3";
        var month = "4";
        var year = "2022";

        await page.FillAsync("[name='CustomDate.Day']", day);
        await page.FillAsync("[name='CustomDate.Month']", month);
        await page.FillAsync("[name='CustomDate.Year']", year);

        await page.RunAndWaitForNavigationAsync(() => page.Keyboard.PressAsync("Enter"));

        // Verify POSTed values have been round-tripped
        await AssertFieldsForCompleteDate(page, day, month, year);
    }

    [Fact]
    public async Task IndividualItemValues()
    {
        var page = await Browser.NewPageAsync(new BrowserNewPageOptions() { BaseURL = ServerFixture.BaseUrl });

        // Initial values should come from item Value properties
        await page.GotoAsync("/DateInputTests/ItemValues");
        await AssertFieldsForCompleteDate(page, "1", "4", "2020");

        // Change the values and POST them
        var day = "3";
        var month = "4";
        var year = "2022";

        await page.FillAsync("[name='Date.Day']", day);
        await page.FillAsync("[name='Date.Month']", month);
        await page.FillAsync("[name='Date.Year']", year);

        await page.RunAndWaitForNavigationAsync(() => page.Keyboard.PressAsync("Enter"));

        // Verify POSTed values have been round-tripped
        await AssertFieldsForCompleteDate(page, day, month, year);
    }

    [Fact]
    public async Task ForDatePartsProperty()
    {
        var page = await Browser.NewPageAsync(new BrowserNewPageOptions() { BaseURL = ServerFixture.BaseUrl });

        // Initial values should come from For property
        await page.GotoAsync("/DateInputTests/ForDateParts");
        await AssertFieldsForMonthAndYearOnly(page, "4", "2020");

        // Change the values and POST them, including some invalid values
        var month = "y";
        var year = "2022";

        await page.FillAsync("[name='MonthAndYear.Month']", month);
        await page.FillAsync("[name='MonthAndYear.Year']", year);

        await page.RunAndWaitForNavigationAsync(() => page.Keyboard.PressAsync("Enter"));

        // Verify POSTed values have been round-tripped, including the invalid values
        await AssertFieldsForMonthAndYearOnly(page, month, year, expectMonthToHaveError: true, expectYearToHaveError: false, expectedErrorMessage: "Month of birth must be a real date");
    }

    [Fact]
    public async Task ForCustomDatePartsProperty()
    {
        var page = await Browser.NewPageAsync(new BrowserNewPageOptions() { BaseURL = ServerFixture.BaseUrl });

        // Initial values should come from For property
        await page.GotoAsync("/DateInputTests/ForCustomDateParts");
        await AssertFieldsForMonthAndYearOnly(page, "4", "2020");

        // Change the values and POST them, including some invalid values
        var month = "y";
        var year = "2022";

        await page.FillAsync("[name='CustomMonthAndYear.Month']", month);
        await page.FillAsync("[name='CustomMonthAndYear.Year']", year);

        await page.RunAndWaitForNavigationAsync(() => page.Keyboard.PressAsync("Enter"));

        // Verify POSTed values have been round-tripped, including the invalid values
        await AssertFieldsForMonthAndYearOnly(page, month, year, expectMonthToHaveError: true, expectYearToHaveError: false, expectedErrorMessage: "Month of birth must be a real date");
    }

    [Fact]
    public async Task ValueDatePartsProperty()
    {
        var page = await Browser.NewPageAsync(new BrowserNewPageOptions() { BaseURL = ServerFixture.BaseUrl });

        // Initial values should come from For property
        await page.GotoAsync("/DateInputTests/ValueDateParts");
        await AssertFieldsForMonthAndYearOnly(page, "4", "2020");

        // Change the values and POST them
        var month = "4";
        var year = "2022";

        await page.FillAsync("[name='MonthAndYear.Month']", month);
        await page.FillAsync("[name='MonthAndYear.Year']", year);

        await page.RunAndWaitForNavigationAsync(() => page.Keyboard.PressAsync("Enter"));

        // Verify POSTed values have been round-tripped
        await AssertFieldsForMonthAndYearOnly(page, month, year);
    }

    [Fact]
    public async Task ValueCustomDatePartsProperty()
    {
        var page = await Browser.NewPageAsync(new BrowserNewPageOptions() { BaseURL = ServerFixture.BaseUrl });

        // Initial values should come from For property
        await page.GotoAsync("/DateInputTests/ValueCustomDateParts");
        await AssertFieldsForMonthAndYearOnly(page, "4", "2020");

        // Change the values and POST them
        var month = "4";
        var year = "2022";

        await page.FillAsync("[name='CustomMonthAndYear.Month']", month);
        await page.FillAsync("[name='CustomMonthAndYear.Year']", year);

        await page.RunAndWaitForNavigationAsync(() => page.Keyboard.PressAsync("Enter"));

        // Verify POSTed values have been round-tripped
        await AssertFieldsForMonthAndYearOnly(page, month, year);
    }

    [Fact]
    public async Task IndividualItemValuesForParts()
    {
        var page = await Browser.NewPageAsync(new BrowserNewPageOptions() { BaseURL = ServerFixture.BaseUrl });

        // Initial values should come from item Value properties
        await page.GotoAsync("/DateInputTests/ItemValuesForParts");
        await AssertFieldsForMonthAndYearOnly(page, "4", "2020");

        // Change the values and POST them
        var month = "4";
        var year = "2022";

        await page.FillAsync("[name='MonthAndYear.Month']", month);
        await page.FillAsync("[name='MonthAndYear.Year']", year);

        await page.RunAndWaitForNavigationAsync(() => page.Keyboard.PressAsync("Enter"));

        // Verify POSTed values have been round-tripped
        await AssertFieldsForMonthAndYearOnly(page, month, year);
    }

    private async Task AssertFieldsForCompleteDate(
        IPage page,
        string expectedDay,
        string expectedMonth,
        string expectedYear,
        string? expectedErrorMessage = null,
        bool? expectDayToHaveError = null,
        bool? expectMonthToHaveError = null,
        bool? expectYearToHaveError = null)
    {
        var inputs = await page.QuerySelectorAllAsync("input[type='text']");

        await Assert.CollectionAsync(
            inputs,
            input => AssertInput(input, expectedDay, expectDayToHaveError),
            input => AssertInput(input, expectedMonth, expectMonthToHaveError),
            input => AssertInput(input, expectedYear, expectYearToHaveError));

        if (expectedErrorMessage is not null)
        {
            await AssertErrorMessage(page, expectedErrorMessage);
        }
    }

    private async Task AssertFieldsForMonthAndYearOnly(
        IPage page,
        string expectedMonth,
        string expectedYear,
        string? expectedErrorMessage = null,
        bool? expectMonthToHaveError = null,
        bool? expectYearToHaveError = null)
    {
        var inputs = await page.QuerySelectorAllAsync("input[type='text']");

        await Assert.CollectionAsync(
            inputs,
            input => AssertInput(input, expectedMonth, expectMonthToHaveError),
            input => AssertInput(input, expectedYear, expectYearToHaveError));

        if (expectedErrorMessage is not null)
        {
            await AssertErrorMessage(page, expectedErrorMessage);
        }
    }

    private async Task AssertErrorMessage(IPage page, string expectedErrorMessage)
    {
        var error = (await page.TextContentAsync(".govuk-error-message"))?.Trim().TrimStart("Error: ".ToCharArray());
        Assert.Equal(expectedErrorMessage, error);

        var errorSummaryError = (await page.TextContentAsync(".govuk-error-summary__list>li>a"))?.Trim();
        Assert.Equal(expectedErrorMessage, errorSummaryError);
    }

    private static async Task AssertInput(IElementHandle input, string expectedValue, bool? expectError)
    {
        var value = await input.GetAttributeAsync("value");
        Assert.Equal(expectedValue, value);

        var classes = await input.GetClassListAsync();

        if (expectError is true)
        {
            Assert.Contains("govuk-input--error", classes);
        }
        else if (expectError is false)
        {
            Assert.DoesNotContain("govuk-input--error", classes);
        }
    }
}

public class DateInputTestsFixture : ServerFixture
{
    protected override void Configure(IApplicationBuilder app)
    {
        base.Configure(app);

        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddGovUkFrontend(options =>
        {
            options.RegisterDateInputModelConverter(typeof(CustomDateType), new CustomDateTypeConverter());
            options.RegisterDateInputModelConverter(typeof(MonthAndYear), new MonthAndYearConverter());
        });

        services
            .AddMvc()
            .AddRazorOptions(options =>
            {
                options.ViewLocationFormats.Add("/DateInputTestsViews/{0}.cshtml");
            });
    }
}

[Route("DateInputTests")]
public class DateInputsTestController : Controller
{
    [HttpGet("ForDate")]
    public IActionResult GetForDate() => View("ForDate", new DateInputsTestsModel() { Date = new(2020, 4, 1) });

    [HttpPost("ForDate")]
    public IActionResult PostForDate(DateInputsTestsModel model) => View("ForDate", model);

    [HttpGet("ValueDate")]
    public IActionResult GetValueDate() => View("ValueDate", new DateInputsTestsModel() { Date = new(2020, 4, 1) });

    [HttpPost("ValueDate")]
    public IActionResult PostValueDate(DateInputsTestsModel model) => View("ValueDate", model);

    [HttpGet("ForCustomDate")]
    public IActionResult GetForCustomDate() => View(
        "ForCustomDate",
        new DateInputsTestsModel() { CustomDate = new CustomDateType(2020, 4, 1) });

    [HttpPost("ForCustomDate")]
    public IActionResult PostForCustomDate(DateInputsTestsModel model) => View("ForCustomDate", model);

    [HttpGet("ValueCustomDate")]
    public IActionResult GetValueCustomDate() => View(
        "ValueCustomDate",
        new DateInputsTestsModel() { CustomDate = new CustomDateType(2020, 4, 1) });

    [HttpPost("ValueCustomDate")]
    public IActionResult PostValueCustomDate(DateInputsTestsModel model) => View("ValueCustomDate", model);

    [HttpGet("ItemValues")]
    public IActionResult GetItemValues() => View("ItemValues", new DateInputsTestsModel() { Date = new(2020, 4, 1) });

    [HttpPost("ItemValues")]
    public IActionResult PostItemValues(DateInputsTestsModel model) => View("ItemValues", model);

    [HttpGet("ForDateParts")]
    public IActionResult GetForDateParts() => View("ForDateParts", new DateInputsTestsModel() { MonthAndYear = new(4, 2020) });

    [HttpPost("ForDateParts")]
    public IActionResult PostForDateParts(DateInputsTestsModel model) => View("ForDateParts", model);

    [HttpGet("ValueDateParts")]
    public IActionResult GetValueDateParts() => View("ValueDateParts", new DateInputsTestsModel() { MonthAndYear = new(4, 2020) });

    [HttpPost("ValueDateParts")]
    public IActionResult PostValueDateParts(DateInputsTestsModel model) => View("ValueDateParts", model);

    [HttpGet("ForCustomDateParts")]
    public IActionResult GetForCustomDateParts() => View(
        "ForCustomDateParts",
        new DateInputsTestsModel() { CustomMonthAndYear = new MonthAndYear(4, 2020) });

    [HttpPost("ForCustomDateParts")]
    public IActionResult PostForCustomDateParts(DateInputsTestsModel model) => View("ForCustomDateParts", model);

    [HttpGet("ValueCustomDateParts")]
    public IActionResult GetValueCustomDateParts() => View(
        "ValueCustomDateParts",
        new DateInputsTestsModel() { CustomMonthAndYear = new MonthAndYear(4, 2020) });

    [HttpPost("ValueCustomDateParts")]
    public IActionResult PostValueCustomDateParts(DateInputsTestsModel model) => View("ValueCustomDateParts", model);

    [HttpGet("ItemValuesForParts")]
    public IActionResult GetItemValuesForParts() => View("ItemValuesForParts", new DateInputsTestsModel() { MonthAndYear = new(4, 2020) });

    [HttpPost("ItemValuesForParts")]
    public IActionResult PostItemValuesForParts(DateInputsTestsModel model) => View("ItemValuesForParts", model);
}

public class DateInputsTestsModel
{
    [Display(Name = "Date of birth")]
    [Required(ErrorMessage = "Enter your date of birth")]
    public DateOnly? Date { get; set; }

    [Display(Name = "Date of birth")]
    [Required(ErrorMessage = "Enter your date of birth")]
    public CustomDateType? CustomDate { get; set; }

    [Display(Name = "Month of birth")]
    [Required(ErrorMessage = "Enter your month of birth")]
    [DateInput(DateInputItemTypes.MonthAndYear)]
    public (int, int)? MonthAndYear { get; set; }

    [Display(Name = "Month of birth")]
    [Required(ErrorMessage = "Enter your month of birth")]
    public MonthAndYear? CustomMonthAndYear { get; set; }
}

public class CustomDateType(int year, int month, int day)
{
    public int D { get; } = day;
    public int M { get; } = month;
    public int Y { get; } = year;
}

public class CustomDateTypeConverter : DateInputModelConverter
{
    protected override object ConvertToModelCore(DateInputConvertToModelContext context) =>
        new CustomDateType(context.ItemValues.Year!.Value, context.ItemValues.Month!.Value, context.ItemValues.Day!.Value);

    protected override DateInputItemValues? ConvertFromModelCore(DateInputConvertFromModelContext context)
    {
        var cdt = (CustomDateType)context.Model;
        return new DateInputItemValues(cdt.D, cdt.M, cdt.Y);
    }
}

public record struct MonthAndYear(int Month, int Year);

public class MonthAndYearConverter : DateInputModelConverter
{
    public override DateInputItemTypes? DefaultItemTypes => DateInputItemTypes.MonthAndYear;

    protected override object ConvertToModelCore(DateInputConvertToModelContext context)
    {
        return new MonthAndYear(context.ItemValues.Month!.Value, context.ItemValues.Year!.Value);
    }

    protected override DateInputItemValues? ConvertFromModelCore(DateInputConvertFromModelContext context)
    {
        var monthAndYear = (MonthAndYear)context.Model;
        return new DateInputItemValues(Day: null, monthAndYear.Month, monthAndYear.Year);
    }
}
