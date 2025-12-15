using AngleSharp.Dom;
using CFT_Solutions.Core;
using CFT_Solutions.Core.Entity.UserMaster;
using CFT_Solutions.Core.Enum;
using CFT_Solutions.Core.Helper;
using CFT_Solutions.Service.Security;
using CFT_Solutions.Service.User;
using CFT_Solutions.Service.UserMaster;
using CFT_Solutions.Web.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CFT_Solutions.Web.Controllers
{
    public class UserMasterController : BaseController
    {
        #region Fields
        private readonly IUserMasterService _userMasterService;
        private readonly IEncryptionService _encryptionService;
        private IWorkContext _workContextRepository;
        #endregion

        #region Constructor
        public UserMasterController(IUserMasterService userMasterService, IEncryptionService encryptionService, IUserService userService, IWorkContext workContextRepository, IHttpContextAccessor httpContextAccessor) : base(workContextRepository, httpContextAccessor, userService)
        {
            _userMasterService = userMasterService;
            _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
            _workContextRepository = workContextRepository;
        }
        #endregion

        #region Actions

        /// <summary>
        /// GET: UserMaster/Index - Display user master list
        /// </summary>
        public IActionResult Index()
        {
            if (base._workContext.CurrentUser.DefaultPermissions.Contains(PermissionEnum.UserMaster.ToString()))
            {
                return View();
            }
            else
            {
                return AccessDeniedView();
            }
        }

        /// <summary>
        /// GET: UserMaster/GetDashBoardData - AJAX endpoint for loading user data with search
        /// </summary>
        /// <param name="searchtext">Optional search text to filter users</param>
        /// <returns>JSON array of user master records</returns>
        public IActionResult GetDashBoardData(string searchtext)
        {
            try
            {
                var data = _userMasterService.GetUserMasterDashBoardData(searchtext);
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        item.EncryptedParam = _encryptionService.EncryptString(item.Id.ToString(), "Encrypt");
                    }
                }
                return Json(data);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
                return Json(new List<UserMasterEntity>());
            }
        }

        /// <summary>
        /// GET: UserMaster/Create - Display form to create new user or edit existing user
        /// </summary>
        /// <param name="id">Optional encrypted user ID for editing</param>
        /// <returns>Create view with user form</returns>
        public IActionResult Create(string id)
        {
            try
            {
                var entity = new UserMasterEntity();

                if (!string.IsNullOrEmpty(id))
                {
                    // Incoming id may have spaces instead of '+' from URLs — restore before decrypt
                    var encrypted = id.Replace(" ", "+");
                    var decrypted = _encryptionService.DecryptString(encrypted, "Encrypt");
                    if (long.TryParse(decrypted, out var parsedId))
                    {
                        var data = _userMasterService.GetUserMasterByUserId(parsedId);
                        if (data != null)
                        {
                            entity = (UserMasterEntity)HtmlEncodeDecodeHelper.Decode(data);
                            entity.Id = parsedId;
                            entity.EncryptedParam = encrypted;
                        }
                    }
                }

                PopulateDropdowns(entity);
                return View(entity);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
                throw;
            }
        }

        /// <summary>
        /// POST: UserMaster/Create - Create or update user master record via stored procedure
        /// </summary>
        /// <param name="model">User master entity with form data</param>
        /// <returns>JSON response with CREATE/UPDATE/FAIL status</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserMasterEntity model)
        {
            try
            {
                if (base._workContext.CurrentUser.DefaultPermissions.Contains(PermissionEnum.UserMaster.ToString()))
                {
                    if (!ModelState.IsValid)
                    {
                        PopulateDropdowns(model);
                        return View(model);
                    }

                    if (ModelState.IsValid)
                    {
                        model.CreatedBy = _workContext.CurrentUser.Id;

                        // Encode sensitive data before saving
                        UserMasterEntity encodedEntity = (UserMasterEntity)HtmlEncodeDecodeHelper.Encode(model);

                        // Call stored procedure to insert/update
                        var result = _userMasterService.InsertAndUpdateUserMaster(encodedEntity);

                        string bootBox = "FAIL";
                        if (model.Id == 0)
                        {
                            bootBox = "CREATE";
                        }
                        else if (model.Id > 0)
                        {
                            bootBox = "UPDATE";
                        }

                        return Json(new { message = bootBox, success = (bootBox != "FAIL") });
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    return AccessDeniedView();
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
                ModelState.AddModelError(string.Empty, "An error occurred while saving the record.");
                PopulateDropdowns(model);
                return View(model);
            }
        }

        /// <summary>
        /// GET: UserMaster/GetUsersByRole - Returns users filtered by role (AJAX)
        /// </summary>
        /// <param name="roleId">Role ID to filter users</param>
        /// <returns>JSON array of users</returns>
        [HttpGet]
        public IActionResult GetUsersByRole(long roleId)
        {
            try
            {
                var users = _userMasterService.GetUsersByRoleId(roleId) ?? new List<UserMasterEntity>();

                // Return a DTO with user information
                var result = users.Select(u => new
                {
                    Id = u.Id,
                    UserName = string.IsNullOrWhiteSpace(u.FirstName)
                        ? (u.EmailId ?? u.LoginId ?? $"User-{u.Id}")
                        : (u.FirstName + (string.IsNullOrWhiteSpace(u.LastName) ? "" : " " + u.LastName)),
                    Value = u.Id.ToString(),
                    Text = string.IsNullOrWhiteSpace(u.FirstName)
                        ? (u.EmailId ?? u.LoginId ?? $"User-{u.Id}")
                        : (u.FirstName + (string.IsNullOrWhiteSpace(u.LastName) ? "" : " " + u.LastName))
                }).ToList();

                return Json(result);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
                return Json(new List<object>());
            }
        }

        /// <summary>
        /// Helper method to populate dropdowns for roles and users
        /// </summary>
        /// <param name="model">User master entity to pre-select values</param>
        private void PopulateDropdowns(UserMasterEntity model)
        {
            // Populate Roles dropdown
            var rolesObj = _userMasterService.GetRoles();
            if (rolesObj is IEnumerable<SelectListItem> roleItems)
            {
                ViewBag.Roles = roleItems;
            }
            else if (rolesObj is IEnumerable<object> roleList)
            {
                ViewBag.Roles = roleList.Select(r =>
                {
                    var val = r.GetType().GetProperty("Id")?.GetValue(r)?.ToString() ?? r.GetType().GetProperty("Value")?.GetValue(r)?.ToString() ?? "";
                    var text = r.GetType().GetProperty("RoleName")?.GetValue(r)?.ToString() ?? r.GetType().GetProperty("Text")?.GetValue(r)?.ToString() ?? val;
                    var selected = (long.TryParse(val, out var idVal) && model != null && model.RoleId == idVal);
                    return new SelectListItem(text, val, selected);
                }).ToList();
            }
            else
            {
                ViewBag.Roles = Enumerable.Empty<SelectListItem>();
            }

            // Populate Users dropdown by role (if editing) or empty for create
            if (model != null && model.RoleId > 0)
            {
                var usersObj = _userMasterService.GetUsersByRoleId(model.RoleId);
                if (usersObj is IEnumerable<SelectListItem> userItems)
                {
                    ViewBag.Users = userItems;
                }
                else if (usersObj is IEnumerable<object> userList)
                {
                    ViewBag.Users = userList.Select(u =>
                    {
                        var val = u.GetType().GetProperty("Id")?.GetValue(u)?.ToString() ?? "";
                        var text = u.GetType().GetProperty("UserName")?.GetValue(u)?.ToString() ?? u.GetType().GetProperty("Text")?.GetValue(u)?.ToString() ?? val;
                        var selected = (long.TryParse(val, out var idVal) && model != null && model.Id == idVal);
                        return new SelectListItem(text, val, selected);
                    }).ToList();
                }
                else
                {
                    ViewBag.Users = Enumerable.Empty<SelectListItem>();
                }
            }
            else
            {
                ViewBag.Users = Enumerable.Empty<SelectListItem>();
            }
        }

        #endregion
    }
