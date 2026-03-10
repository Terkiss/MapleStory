using MapleStoryMarketGraph.Components;
using MapleStoryMarketGraph.Services;
using MapleStoryMarketGraph.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Controllers
builder.Services.AddControllers();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication & Authorization
var jwtConfig = builder.Configuration.GetSection("Authentication:Jwt");
var key = Encoding.UTF8.GetBytes(jwtConfig["Key"] ?? "VERY_SECRET_DEFAULT_KEY_THAT_MUST_BE_CHANGED");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig["Issuer"] ?? "MesoMarket",
        ValidAudience = jwtConfig["Issuer"] ?? "MesoMarket",
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
builder.Services.AddScoped<JwtAuthenticationStateProvider>(sp => (JwtAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());

// HttpClient for general use
builder.Services.AddHttpClient();

// HttpClient for NexonApiService
builder.Services.AddHttpClient<NexonApiService>();

// Application Services
builder.Services.AddSingleton<MarketDataService>();
builder.Services.AddScoped<AccountBookService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<GoogleAuthService>();
builder.Services.AddScoped<NexonApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
