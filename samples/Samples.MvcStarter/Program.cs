using GovUk.Frontend.AspNetCore;
using Joonasw.AspNetCore.SecurityHeaders;
using Joonasw.AspNetCore.SecurityHeaders.Csp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddCsp();

builder.Services.AddGovUkFrontend(options =>
{
    options.GetCspNonceForRequest = context =>
    {
        var cspService = context.RequestServices.GetRequiredService<ICspNonceService>();
        return cspService.GetNonce();
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseGovUkFrontend();

app.MapStaticAssets();

app.UseCsp(csp =>
{
    csp.ByDefaultAllow
        .FromSelf();

    csp.AllowScripts
        .FromSelf()
        .AddNonce();
});

app.MapControllers().WithStaticAssets();

app.MapDefaultControllerRoute();

app.Run();
