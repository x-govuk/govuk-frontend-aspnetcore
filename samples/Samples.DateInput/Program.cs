using GovUk.Frontend.AspNetCore;
using NodaTime;
using Samples.DateInput;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddGovUkFrontend(options =>
{
    options.RegisterDateInputModelConverter(typeof(LocalDate), new LocalDateDateInputModelConverter());
    options.RegisterDateInputModelConverter(typeof(YearMonth), new YearMonthDateInputModelConverter());
});

var app = builder.Build();

app.UseRouting();

app.UseAuthorization();

app.UseGovUkFrontend();

app.UseStaticFiles();

app.MapRazorPages();

app.Run();
