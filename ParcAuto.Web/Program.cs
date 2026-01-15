using Microsoft.EntityFrameworkCore;
using ParcAuto.Web.Data;
using ParcAuto.Web.Models;
using ParcAuto.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddRazorPages();
builder.Services.AddControllers(); // Add API controllers
builder.Services.AddHostedService<ReservationReminderService>();

builder.Services.AddDbContext<ParcAutoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    
    // Password settings - same rules as mobile app validation
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
})
.AddEntityFrameworkStores<ParcAutoContext>();

// Configure Authentication: Support BOTH Cookies (for website) AND JWT (for mobile app)
var jwtKey = builder.Configuration["Jwt:Key"] ?? "YourSecureSecretKeyForJwtTokenGeneration12345";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ParcAutoAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ParcAutoClients";

// Add JWT Bearer authentication alongside Identity cookies
builder.Services.Configure<IdentityOptions>(options =>
{
    // Identity already adds Cookie authentication via AddDefaultIdentity
    // We just need to add JWT for mobile API
});

builder.Services.AddAuthentication()
.AddJwtBearer(options =>  // For mobile app (API)
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// Add CORS for mobile app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll"); // Enable CORS

app.UseAuthentication();   // 🔴 OBLIGATORIU
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers(); // Map API controllers

// Seed database with test data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

app.Run();
