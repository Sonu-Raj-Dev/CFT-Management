using CFT_Solutions.Core;
using CFT_Solutions.Core.Helper;
using CFT_Solutions.Service.User;
using CFT_Solutions.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization; // added


namespace CFT_Solutions.Web.Controllers
{
    [AllowAnonymous] // allow anonymous access to Login actions
    public class LoginController : BaseController
    {
        private IWorkContext _workContextRepository;
        private IHttpContextAccessor _httpContextAccessor;
        private IConfiguration _configuration;
        private IUserService _userService;
        public LoginController(
            IWorkContext workContextRepository,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IUserService userService
        ) : base(workContextRepository, httpContextAccessor, userService)
        {
            _httpContextAccessor = httpContextAccessor;
            _workContextRepository = workContextRepository;
            _configuration = configuration;
            _userService = userService;
        }

        // GET: show login page or redirect if already signed in
        [AllowAnonymous]
        public ActionResult Index()
        {
            try
            {
                if (User?.Identity?.IsAuthenticated == true)
                {
                    // already authenticated
                    return RedirectToAction("Index", "Dashboard");
                }
                    
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // POST: perform custom authentication (form fields: Email, Password)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Index(string EmailId, string Password)
        {
            try
            {
                if (!string.IsNullOrEmpty(EmailId) && !string.IsNullOrEmpty(Password))
                {
                    var user = _userService.ValidateUser(EmailId);
                    if (user == null)
                    {
                        // invalid user
                        ModelState.AddModelError(string.Empty, "Invalid credentials");
                        return View();
                    }

                    // If you have to check password, do it here. Example placeholder:
                    // if (!VerifyPassword(Password, user.PasswordHash)) { ... }

                    // Create claims - include any claims you need (Name, roles, user id, etc.)
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.EmailID ?? EmailId),
                    new Claim("UserId", user.Id.ToString())
                };

                    // add roles/permissions as claims if available
                    if (user.DefaultRoleName != null)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, user.DefaultRoleName));
                    }

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    // optional: configure auth properties
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true, // keep cookie across sessions
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                    };

                    // Sign in
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

                    // create the same session value your app expects
                    string _browserInfo = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString() + "~"
                                                     + _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

                    string _sessionValue = Convert.ToString(user.EmailID) + "^"
                                                            + DateTime.Now.Ticks + "^"
                                                            + _browserInfo + "^"
                                                            + System.Guid.NewGuid();

                    byte[] _encodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(_sessionValue);
                    string _encryptedString = System.Convert.ToBase64String(_encodeAsBytes);

                    _httpContextAccessor.HttpContext.Session.SetString("encryptedSession", _encryptedString);

                    // use the base controller work-context field (not an undefined symbol 'WorkContext')
                    if (_workContext != null)
                    {
                        _workContext.CurrentUser = _userService.GetUserByLoginId(user.LoginId ?? user.EmailID);
                    }
                    else if (_workContextRepository != null)
                    {
                        _workContextRepository.CurrentUser = _userService.GetUserByLoginId(user.LoginId ?? user.EmailID);
                    }

                    return RedirectToAction("Index", "Dashboard");
                }
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // POST: logout (form in layout posts to this)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // 1) Clear server-side session data
                try
                {
                    _httpContextAccessor?.HttpContext?.Session?.Clear();
                }
                catch { /* ignore if session unavailable */ }

                // 2) Clear current user in work context
                try
                {
                    if (_workContextRepository != null)
                        _workContextRepository.CurrentUser = null;

                    if (_workContext != null)
                        _workContext.CurrentUser = null;
                }
                catch { /* ignore */ }

                // 3) Sign out cookie authentication (issues expired auth cookie)
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                // 4) Explicitly delete application cookies (use same Path/SameSite/Secure used in Program.cs)
                try
                {
                    // Auth cookie
                    Response.Cookies.Delete("CFTCookie", new CookieOptions { Path = "/CFT", HttpOnly = true, Secure = true, SameSite = SameSiteMode.None });
                    Response.Cookies.Append("CFTCookie", "", new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(-1), Path = "/CFT", HttpOnly = true, Secure = true, SameSite = SameSiteMode.None });

                    // Antiforgery cookie (name from Program.cs)
                    Response.Cookies.Delete("CFTAntiforgeryCookie", new CookieOptions { Path = "/CFT", Secure = true, HttpOnly = false, SameSite = SameSiteMode.None });
                    Response.Cookies.Append("CFTAntiforgeryCookie", "", new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(-1), Path = "/CFT", Secure = true, HttpOnly = false, SameSite = SameSiteMode.None });

                    // Session cookie (was configured SameSite=Strict)
                    Response.Cookies.Delete("CFTSession", new CookieOptions { Path = "/CFT", HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict });
                    Response.Cookies.Append("CFTSession", "", new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(-1), Path = "/CFT", HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict });
                }
                catch { /* ignore cookie deletion errors */ }

                // 5) Clear principal for the current request so User.IsAuthenticated is false immediately
                HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
                if (_httpContextAccessor?.HttpContext != null)
                    _httpContextAccessor.HttpContext.User = HttpContext.User;

                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Optional GET handler so bookmark /Logout works (non-POST)
        [HttpGet]
        public async Task<IActionResult> LogoutGet()
        {
            // reuse POST logic
            return await Logout();
        }
    }
}
