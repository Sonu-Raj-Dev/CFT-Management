using CFT_Solutions.Core;
using CFT_Solutions.Core.Configuration;
using CFT_Solutions.Core.Helper;
using CFT_Solutions.Core.Repository;
using CFT_Solutions.Service.User;
using CFT_Solutions.Service.UserMaster;
using CFT_Solutions.Service.Security;
using CFT_Solutions.Service.CustomerMaster;
using CFT_Solutions.Service.ComplaintMster;
using CFT_Solutions.Web.FrameWork;
using CFT_Solutions.Web.Helper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager Configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;

#region Localization
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en-GB");
    options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-GB") };
    options.RequestCultureProviders.Clear();
});
#endregion

#region 🔥 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCFT", policy =>
    {
        policy
            .WithOrigins(
                "https://cftmanagement.somee.com",
                "http://cftmanagement.somee.com"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
#endregion

#region Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "CFTCookie";
        options.Cookie.Path = "/";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

        options.LoginPath = "/Login/Index";
        options.LogoutPath = "/Login/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";

        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });
#endregion

#region Cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Path = "/";
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
#endregion

#region Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Path = "/";
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.Name = "CFTSession";
});
#endregion

#region Antiforgery
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "CFTAntiforgeryCookie";
    options.Cookie.Path = "/";
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.HeaderName = "RequestVerificationToken";
});
#endregion

#region DI
builder.Services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IWorkContext, WebWorkContext>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserMasterService, UserMasterService>();
builder.Services.AddScoped<ICustomerMasterService, CustomerMasterService>();
builder.Services.AddScoped<IEncryptionService, EncryptionService>();
builder.Services.AddScoped<IComplaintService, ComplaintService>();
#endregion

builder.Services.AddDistributedMemoryCache();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

#region Culture
CultureInfo culture = CultureInfo.CreateSpecificCulture("en-GB");
culture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;
#endregion

#region Middleware Pipeline (CORRECT ORDER)

// Exception handling should be first
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Security headers
app.UseMiddleware<SecurityHeadersMiddleware>();

// HTTPS redirection
app.UseHttpsRedirection();

// Static files


// App hosted under /CFT - Should come before routing
app.UsePathBase("/CFT");

app.UseStaticFiles();
// ✅ ROUTING MUST COME BEFORE AUTH
app.UseRouting();

// ✅ CORS (after UseRouting, before UseAuthentication)
app.UseCors("AllowCFT");

// ✅ AUTHENTICATION
app.UseAuthentication();

// ✅ AUTHORIZATION (CRITICAL - must be after UseRouting and before UseEndpoints)
app.UseAuthorization();

// Cookies & Session
app.UseCookiePolicy();
app.UseSession();

// Endpoints (implicit with MapControllerRoute)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}"
);

// For Razor Pages (if you use them)
app.MapRazorPages();

#endregion

LogHelper.InitLog(Configuration.GetValue<string>("Log4netConfigPath"));
app.Run();