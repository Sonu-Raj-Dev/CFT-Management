using CFT_Solutions.Core;
using CFT_Solutions.Core.Entity.User;
using CFT_Solutions.Service.User;
using System.Security.Claims;

namespace CFT_Solutions.Web.FrameWork
{
    public partial class WebWorkContext : IWorkContext
    {
        #region Fields      

        private UserEntity _cachedUser;
        protected internal readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        #endregion

        #region Constructor

        public WebWorkContext(IHttpContextAccessor httpContextAccessor,
                                                        IUserService userService)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._userService = userService;
        }

        #endregion

        #region Property


        public UserEntity CurrentUser
        {
            get
            {
                UserEntity user = null;
                // If session already contains cached value, return it
                if (_httpContextAccessor.HttpContext.Session != null && _httpContextAccessor.HttpContext.Session.GetComplexData<UserEntity>("_CurrentUser") != null)
                {
                    return _httpContextAccessor.HttpContext.Session.GetComplexData<UserEntity>("_CurrentUser");
                }

                // If no cached user, try to read from authenticated principal claims
                if (_httpContextAccessor.HttpContext.Session != null)
                {
                    var claims = _httpContextAccessor.HttpContext.User?.Claims?.ToList() ?? new List<Claim>();

                    // Prefer preferred_username if present, else use common name claim types
                    string loginId = claims.FirstOrDefault(c => string.Equals(c.Type, "preferred_username", StringComparison.OrdinalIgnoreCase))?.Value
                                     ?? claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
                                     ?? claims.FirstOrDefault(c => c.Type == "name")?.Value;

                    if (!string.IsNullOrEmpty(loginId))
                    {
                        user = _userService.GetUserByLoginId(loginId);
                        if (user != null)
                        {
                            _httpContextAccessor.HttpContext.Session.SetComplexData("_CurrentUser", user);
                            _cachedUser = user;
                        }
                    }

                    return user;
                }
                else
                {
                    return _cachedUser;
                }
            }
            set
            {
                if (value != null && _httpContextAccessor.HttpContext.Session != null)
                {
                    _httpContextAccessor.HttpContext.Session.SetComplexData("_CurrentUser", value);
                    _cachedUser = value;
                }
            }
        }

        public virtual int AuthenticationType { get; set; }

        #endregion
    }
}
