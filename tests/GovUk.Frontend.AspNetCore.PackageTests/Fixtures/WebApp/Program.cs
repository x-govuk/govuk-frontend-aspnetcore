using GovUk.Frontend.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGovUkFrontend();
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseGovUkFrontend();

#if NET9_0_OR_GREATER
// MapStaticAssets only serves files that made it into the static web assets manifest, so this is
// also how the tests check that the restored files were added to @(Content) early enough.
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();
#else
app.UseStaticFiles();
app.MapRazorPages();
#endif

app.Run();
