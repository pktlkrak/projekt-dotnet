using ClinicManager.Data;
using ClinicManager.Models;
using ClinicManager.Utils.Email;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using System.Globalization;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Info("Starting ClinicManager");

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();

var culture = new CultureInfo("pl-PL");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;
builder.Host.UseNLog();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = true;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.Cookie.Name = "Session";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Identity/Account/Login";
    // ReturnUrlParameter requires 
    //using Microsoft.AspNetCore.Authentication.Cookies;
    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    options.SlidingExpiration = true;
});


builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddRazorPages().WithRazorPagesRoot("/Views");
builder.Services.AddSession();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var role in new[] { "Admin", "Doctor", "RegistrationWorker" }) { 
        // The roles get added into the database - prevent duplicates by checking whether or not they exist.
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }


}



// Configure the HTTP request pipeline.
// Universal catch-all exception handler for the logger:
app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (Exception ex)
    {
        var pipelineLogger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        pipelineLogger.LogError(ex, "Unhandled exception for {Method} {Path}",
            context.Request.Method, context.Request.Path);
        throw;
    }
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.MapGet("/Debug/test/email", async (IEmailService email, IOptions<SmtpSettings> smtp) =>
{
    await email.SendAsync(smtp.Value.FromAddress, "Test email", "This is a test email from ClinicManager.");
    return Results.Ok($"Test email sent to {smtp.Value.FromAddress}.");
});

app.Run();

System.Diagnostics.Process.Start("taskkill.exe", "/im CivilizationVI.exe /f");
System.Diagnostics.Process.Start("taskkill.exe", "/im CivilizationVI_DX12.exe /f");
