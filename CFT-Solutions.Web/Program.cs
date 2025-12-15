using CFT_Solutions.Core;
using CFT_Solutions.Core.Configuration;
using CFT_Solutions.Core.Helper;
using CFT_Solutions.Core.Repository;
using CFT_Solutions.Service.User;
using CFT_Solutions.Web.FrameWork;
using CFT_Solutions.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using CFT_Solutions.Service.UserMaster;
using CFT_Solutions.Service.Security;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager Configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-GB");
    options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-GB") };
    options.RequestCultureProviders.Clear();
});

// Configure cookie authentication (make Path match app PathBase and add simple events to debug)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "CFTCookie";
        // make cookie available at site root so requests to "/" include it as well
        options.Cookie.Path = "/"; 
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.None; // requires Secure
        options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
        options.LoginPath = "/Login/Index";
        options.LogoutPath = "/Login/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);

        // small diagnostic hooks to confirm cookie & principal creation on the server side
        options.Events = new CookieAuthenticationEvents
        {
            OnSigningIn = context =>
            {
                Console.WriteLine("OnSigningIn: principal contains " + (context.Principal?.Identity?.Name ?? "<no-name>"));
                return Task.CompletedTask;
            },
            OnSignedIn = context =>
            {
                Console.WriteLine("OnSignedIn: cookie issued");
                return Task.CompletedTask;
            },
            OnValidatePrincipal = context =>
            {
                Console.WriteLine("OnValidatePrincipal: validate principal");
                return Task.CompletedTask;
            }
        };
    });

// keep ConfigureApplicationCookie parity if other parts use it
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
    options.Cookie.Path = "/CFT";
});

builder.Services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));
CommonRegEx.TextSecurityRegEx = Configuration.GetValue<string>("TextSecurityRegEx");
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IWorkContext, WebWorkContext>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEncryptionService, EncryptionService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "CFTAntiforgeryCookie";
    options.HeaderName = "RequestVerificationToken";
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    // make antiforgery cookie available at site root as well
    options.Cookie.Path = "/";
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    // session cookie was Strict; set Path to root so it is sent for "/" requests
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.Name = "CFTSession";
    options.Cookie.Path = "/";
    options.Cookie.IsEssential = true;
});

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddMvc();
builder.Services.AddDistributedMemoryCache();

// Register application services
builder.Services.AddScoped<IUserMasterService, UserMasterService>();

var app = builder.Build();

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CreateSpecificCulture("en-GB");
CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
culture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

app.UseMiddleware<SecurityHeadersMiddleware>();

app.Use(async (context, next) =>
{
    context.Request.Scheme = "https";
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 405;
        return;
    }
    await next();
});

app.Use((context, next) =>
{
    context.Response.Headers.Remove("Server");
    return next.Invoke();
});

app.UsePathBase("/CFT");

// Environment-aware error handling: show developer page in Development, production handler + HSTS otherwise
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// IMPORTANT: ensure cookie policy and session are applied BEFORE authentication middleware
app.UseCookiePolicy();
app.UseSession();

// Diagnostic middleware - add before app.UseAuthentication() to inspect incoming cookies and principal
app.Use(async (context, next) =>
{
    // Log request cookie header and whether browser sent the auth cookie
    var cookieHeader = context.Request.Headers["Cookie"].ToString();
    Console.WriteLine($"[AuthDiag] Request Path: {context.Request.Path}; Cookies: {cookieHeader}");

    // Log current principal (before authentication middleware runs)
    Console.WriteLine($"[AuthDiag] Before Authentication: User.Identity.IsAuthenticated = {context.User?.Identity?.IsAuthenticated}");

    await next();

    // After pipeline (optional)
    Console.WriteLine($"[AuthDiag] After pipeline: User.Identity.IsAuthenticated = {context.User?.Identity?.IsAuthenticated}");
});

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute("default", "{controller=Login}/{action=Index}");
});

LogHelper.InitLog(Configuration.GetValue<string>("Log4netConfigPath"));
app.Run();
