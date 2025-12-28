using CFT_Solutions.Core;
using CFT_Solutions.Core.Entity.CustomerMaster;
using CFT_Solutions.Core.Entity.UserMaster;
using CFT_Solutions.Core.Enum;
using CFT_Solutions.Core.Helper;
using CFT_Solutions.Service.CustomerMaster;
using CFT_Solutions.Service.Security;
using CFT_Solutions.Service.User;
using CFT_Solutions.Service.UserMaster;
using CFT_Solutions.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CFT_Solutions.Web.Controllers
{
    public class CustomerMasterController : BaseController
    {
        #region Fields
        private readonly ICustomerMasterService _customerMasterService;
        private readonly IEncryptionService _encryptionService;
        private IWorkContext _workContextRepository;
        #endregion

        #region Constructor
        public CustomerMasterController(ICustomerMasterService customerMasterService, IEncryptionService encryptionService, IUserService userService, IWorkContext workContextRepository, IHttpContextAccessor httpContextAccessor) : base(workContextRepository, httpContextAccessor, userService)
        {
            _customerMasterService = customerMasterService;
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
            if (base._workContext.CurrentUser.DefaultPermissions.Contains(PermissionEnum.CustomerMaster.ToString()))
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
                var MasterTypeId = 2;
                var data = _customerMasterService.GetCustomerMasterDashBoardData(searchtext);
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
                var entity = new CustomerMasterEntity();

                if (!string.IsNullOrEmpty(id))
                {
                    // Incoming id may have spaces instead of '+' from URLs — restore before decrypt
                    var encrypted = id.Replace(" ", "+");
                    var decrypted = _encryptionService.DecryptString(encrypted, "Encrypt");
                    if (long.TryParse(decrypted, out var parsedId))
                    {
                        var data = _customerMasterService.GetCustomerMasterByUserId(parsedId);
                        if (data != null)
                        {
                            entity = (CustomerMasterEntity)HtmlEncodeDecodeHelper.Decode(data);
                            entity.Id = parsedId;
                            entity.EncryptedParam = encrypted;
                        }
                    }
                }

                //PopulateDropdowns(entity);

                return Json(new
                {
                    id = entity.Id,
                    firstName = entity.FirstName,
                    lastName = entity.LastName,
                    emailId = entity.EmailId,
                    mobileNumber = entity.MobileNo,
                    isActive = entity.IsActive,
                    address=entity.Address
                });
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
        public IActionResult Create(CustomerMasterEntity model)
        {
            try
            {
                if (base._workContext.CurrentUser.DefaultPermissions.Contains(PermissionEnum.CustomerMaster.ToString()))
                {
                    List<string> IgnoreProperties = new List<string> { "LoginId", "MobileNo", "Password", "RoleName", "MiddleName" };
                    var SpecialCharactor = "{,},`,^,>,<";

                    Dictionary<string, string> modelErrors = HtmlEncodeDecodeHelper.ValidateSpecialChars(model, IgnoreProperties, SpecialCharactor);


                    if (modelErrors.Count > 0)
                    {
                        foreach (var item in modelErrors)
                        {
                            ModelState.AddModelError(item.Key, item.Value);
                        }

                        string errorlist = string.Empty;
                        foreach (var item in ModelState)
                        {
                            foreach (var error in item.Value.Errors)
                            {
                                errorlist = errorlist + "#" + item.Key + "," + error.ErrorMessage;
                            }
                        }

                        return Json(new { Message = "MODELERROR", errorlist });

                    }

                    if (modelErrors.Count == 0)
                    {
                        model.CreatedBy = _workContext.CurrentUser.Id;

                        // Encode sensitive data before saving
                        CustomerMasterEntity encodedEntity = (CustomerMasterEntity)HtmlEncodeDecodeHelper.Encode(model);
                        encodedEntity.EmployeeType = 2;
                        // Call stored procedure to insert/update
                        var result = _customerMasterService.InsertAndUpdateCustomerMaster(encodedEntity);

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
                throw ex;
            }
        }     
       
        #endregion

    }
}
