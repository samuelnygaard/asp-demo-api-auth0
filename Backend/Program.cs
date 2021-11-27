
using Backend;
using Data;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;

var builder = WebApplication.CreateBuilder(args);

Startup.ConfigureServices(builder.Services, builder);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.Initialize(services);
}

if (app.Environment.IsDevelopment())
{
    Startup.ConfigureSwagger(app, builder);
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
} else
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles("/admin");

app.UseRouting();

app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "api/v{apiVersion}/{controller}/{action=Index}/{id?}");
    endpoints.MapBlazorHub();
    endpoints.MapFallbackToPage("/admin/{*page}", "/_Host");
});

app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientApp";

    if (app.Environment.IsDevelopment())
    {
        spa.UseReactDevelopmentServer(npmScript: "start");
    }
});

app.Run();
