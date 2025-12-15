// ensure cookie auth is configured so unauthenticated requests redirect correctly
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";
        options.AccessDeniedPath = "/Home/AccessDenied";
        options.Cookie.HttpOnly = true;
    });

app.UseAuthentication();
app.UseAuthorization();