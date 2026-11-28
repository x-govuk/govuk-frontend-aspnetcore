using GovUk.Frontend.AspNetCore;
using Samples.ApplyForALibraryCard;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddGovUkFrontend();

// The answers are kept in session state until the application is sent
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ApplicationStore>();
builder.Services.AddSingleton(TimeProvider.System);

var app = builder.Build();

app.UseGovUkFrontendErrorPages();

app.UseRouting();

app.UseSession();

app.UseGovUkFrontend();

app.MapStaticAssets();

app.MapRazorPages().WithStaticAssets();

app.Run();
