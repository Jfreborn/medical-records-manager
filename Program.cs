using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MedicalRecordsManager.Data;
using MedicalRecordsManager.Models;

var builder = WebApplication.CreateBuilder(args);

// ── 1. Database ──────────────────────────────────────────────
var connectionString =
    Environment.GetEnvironmentVariable("DATABASE_URL") ??
    builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// ── 2. Identity ──────────────────────────────────────────────
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// ── 3. Cookie Settings ───────────────────────────────────────
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

// ── 4. MVC ───────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Urls.Add($"http://*:{port}");

// ── 5. Seed Roles and Default Users ─────────────────────────
using (var scope = app.Services.CreateScope())
{
    try
    {
        await SeedData.InitializeAsync(scope.ServiceProvider);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error seeding database");
    }
}

// ── 6. Middleware Pipeline ───────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();   // Must be BEFORE UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();