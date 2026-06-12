using ClinicManager.Data;
using ClinicManager.Dtos.Medications;
using ClinicManager.Mapping;
using ClinicManager.Models;
using ClinicManager.Services;
using ClinicManager.Utils.Email;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using NLog;
using NLog.Web;
using System.ComponentModel.DataAnnotations;
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
{
    options.UseSqlServer(connectionString);
    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});

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

    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }

        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }

        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});


builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<PatientMapper>();
builder.Services.AddScoped<PatientService>();
builder.Services.AddScoped<IPatientAdminService>(sp => sp.GetRequiredService<PatientService>());
builder.Services.AddScoped<IPatientService>(sp => sp.GetRequiredService<IPatientAdminService>());

builder.Services.AddScoped<MedicationMapper>();
builder.Services.AddScoped<IMedicationService, MedicationService>();

builder.Services.AddScoped<ProcedureMapper>();
builder.Services.AddScoped<IProcedureService, ProcedureService>();

builder.Services.AddScoped<VisitMapper>();
builder.Services.AddScoped<IVisitService, VisitService>();

builder.Services.AddScoped<MedicalFileMapper>();
builder.Services.AddScoped<IMedicalFileService, MedicalFileService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ClinicManager API",
        Version = "v1"
    });

    options.DocInclusionPredicate((_, apiDesc) => apiDesc.RelativePath?.StartsWith("api/") == true);
});

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

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ClinicManager API v1");
});

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.MapGet("/Debug/test/email", async (IEmailService email, IOptions<SmtpSettings> smtp) =>
{
    await email.SendAsync(smtp.Value.FromAddress, "Test email", "This is a test email from ClinicManager.");
    return Results.Ok($"Test email sent to {smtp.Value.FromAddress}.");
});

var medicationsApi = app.MapGroup("/api/medications")
    .WithTags("Medications")
    .AllowAnonymous();

medicationsApi.MapGet("/", async (IMedicationService medicationService) =>
{
    var medications = await medicationService.GetAllMedicationsAsync();
    return Results.Ok(medications);
})
.WithName("GetMedications")
.Produces<List<MedicationDto>>(StatusCodes.Status200OK);

medicationsApi.MapGet("/{id:int}", async (int id, IMedicationService medicationService) =>
{
    var medication = await medicationService.GetMedicationForEditAsync(id);
    return medication is null ? Results.NotFound() : Results.Ok(medication);
})
.WithName("GetMedicationById")
.Produces<MedicationFormDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

medicationsApi.MapPost("/", async (MedicationFormDto dto, IMedicationService medicationService) =>
{
    var validation = ValidateMedication(dto);
    if (validation is not null) return validation;

    var created = await medicationService.CreateMedicationAsync(dto);
    return Results.Created($"/api/medications/{created.Id}", created);
})
.WithName("CreateMedication")
.Produces<MedicationDto>(StatusCodes.Status201Created)
.ProducesValidationProblem();

medicationsApi.MapPut("/{id:int}", async (int id, MedicationFormDto dto, IMedicationService medicationService) =>
{
    var validation = ValidateMedication(dto);
    if (validation is not null) return validation;

    var updated = await medicationService.UpdateMedicationAsync(id, dto);
    return updated ? Results.NoContent() : Results.NotFound();
})
.WithName("UpdateMedication")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.ProducesValidationProblem();

medicationsApi.MapDelete("/{id:int}", async (int id, IMedicationService medicationService) =>
{
    var deleted = await medicationService.DeleteMedicationAsync(id);
    return deleted is null ? Results.NotFound() : Results.Ok(deleted);
})
.WithName("DeleteMedication")
.Produces<MedicationDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

static IResult? ValidateMedication(MedicationFormDto dto)
{
    var results = new List<ValidationResult>();
    if (Validator.TryValidateObject(dto, new ValidationContext(dto), results, validateAllProperties: true))
        return null;

    var errors = results
        .SelectMany(r => r.MemberNames.DefaultIfEmpty(""), (r, member) => (member, r.ErrorMessage ?? "Invalid value"))
        .GroupBy(x => x.member)
        .ToDictionary(g => g.Key, g => g.Select(x => x.Item2).ToArray());

    return Results.ValidationProblem(errors);
}

app.Run();

System.Diagnostics.Process.Start("taskkill.exe", "/im CivilizationVI.exe /f");
System.Diagnostics.Process.Start("taskkill.exe", "/im CivilizationVI_DX12.exe /f");
