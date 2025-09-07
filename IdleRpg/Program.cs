using IdleRpg.Components;
using IdleRpg.Components.Account;
using IdleRpg.Data;
using IdleRpg.Game;
using IdleRpg.Game.Core;
using IdleRpg.Services.Discord;
using IdleRpg.Util;
using L1PathFinder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


Console.WriteLine(@"  ___    _ _      ____  ____   ____ 
 |_ _|__| | | ___|  _ \|  _ \ / ___|
  | |/ _` | |/ _ \ |_) | |_) | |  _ 
  | | (_| | |  __/  _ <|  __/| |_| |
 |___\__,_|_|\___|_| \_\_|    \____|
                           \By Borf/");

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole(c => c.FormatterName = "proper")
               .AddConsoleFormatter<ProperFormatter, ProperFormatterOptions>(c => { });

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDiscord();
builder.Services.AddSingleton<GameService>();
builder.Services.AddHostedService<GameHostedService>();
builder.Services.AddSingleton<BgTaskManager>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

    using (var scope = app.Services.CreateScope())
    {
        using (var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
        {
            if(!dbContext.Database.EnsureCreated())
            {
                Console.WriteLine("Making database");
            }
        }
    }


}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}



app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();

