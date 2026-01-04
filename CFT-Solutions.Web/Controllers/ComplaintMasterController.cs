using CFT_Solutions.Core;
using CFT_Solutions.Core.Entity.ComplaintMaster;
using CFT_Solutions.Core.Entity.UserMaster;
using CFT_Solutions.Core.Enum;
using CFT_Solutions.Core.Helper;
using CFT_Solutions.Service.ComplaintMster;
using CFT_Solutions.Service.CustomerMaster;
using CFT_Solutions.Service.Security;
using CFT_Solutions.Service.User;
using CFT_Solutions.Service.UserMaster;
using CFT_Solutions.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CFT_Solutions.Web.Controllers
{
    public class ComplaintMasterController : BaseController
    {
        #region Fields
        private readonly IComplaintService _complaintasterService;
        private readonly IEncryptionService _encryptionService;
        private IWorkContext _workContextRepository;
        private ICustomerMasterService _customerMasterService;
        #endregion

        #region Constructor
        public ComplaintMasterController(IComplaintService complaintMasterService, IEncryptionService encryptionService, IUserService userService, IWorkContext workContextRepository, IHttpContextAccessor httpContextAccessor,ICustomerMasterService customerMasterService) : base(workContextRepository, httpContextAccessor, userService)
        {
            _complaintasterService = complaintMasterService;
            _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
            _workContextRepository = workContextRepository;
            _customerMasterService = customerMasterService;
        }
        #endregion

        public IActionResult Index()
        {
            if (base._workContext.CurrentUser.DefaultPermissions.Contains(PermissionEnum.ComplaintMaster.ToString()))
            {
                ViewBag.CurruntUserRoleId = _workContext.CurrentUser.DefaultRoleId;
                return View();
            }
            else
            {
                return AccessDeniedView();
            }
        }
        public IActionResult GetDashBoardData(string searchtext)
        {
            try
            {
                var CurruntUserId = _workContext.CurrentUser.Id; ;
                var data =_complaintasterService.GetComplaintMasterDashBoardData(searchtext, CurruntUserId);
                if (data != null)
                {
                    foreach (var item in data)
                    {

                        if (item.CreatedDate != null)
                        {
                            item.CreatedDateStr = item.CreatedDate.Value.ToString("dd-MM-yyyy");
                        }
                        else
                        {
                            item.CreatedDateStr = "";
                        }
                        if (item.ModifiedDate != null)
                        {
                            item.ModifiedDateStr = item.ModifiedDate.Value.ToString("dd-MM-yyyy");
                        }
                        else
                        {
                            item.ModifiedDateStr = "";
                        }
                        if (item.CompletionDate != null)
                        {
                            item.CompletionDatestr = item.CompletionDate.Value.ToString("dd-MM-yyyy");
                        }
                        else
                        {
                            item.CompletionDatestr = "";
                        }


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
        public IActionResult Create(string id,bool IsView=false)
        {
            try
            {
                var model = new ComplaintMasterEntity();

                if (!string.IsNullOrEmpty(id))
                {
                    var encrypted = id.Replace(" ", "+");
                    var decrypted = _encryptionService.DecryptString(encrypted, "Encrypt");

                    if (long.TryParse(decrypted, out var complaintId))
                    {
                        var data = _complaintasterService.GetComplaintMasterDataById(complaintId);

                        if (data != null)
                        {
                            model.Id = data.Id;
                            model.CustomerId = data.CustomerId;
                            model.NatureOfComplaintId = data.NatureOfComplaintId;
                            model.CustomerName = data.CustomerName;
                            model.MobileNumber = data.MobileNumber;
                            model.CustomerEmail = data.CustomerEmail;
                            model.Address = data.Address;
                            model.ComplaintDetails = data.ComplaintDetails;
                            model.IsActive = data.IsActive;
                            model.EngineerId = data.EngineerId;
                            model.StatusId = data.StatusId;
                            model.Remark = data.Remark;
                        }
                    }
                }
                ViewBag.IsView = IsView;
        
                model.CustomersList = _complaintasterService.GetCustomer().Select(x => new ComplaintMasterEntity
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                   .ToList();

                model.NatureOfComplaintsList = _complaintasterService.GetNatureOfComplaint().Select(x => new ComplaintMasterEntity
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                   .ToList();

                model.EngineerList = _complaintasterService.GetEngineerList().Select(x => new ComplaintMasterEntity
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                  .ToList();
                Dictionary<int, string> statusDictionary = new Dictionary<int, string>();
                statusDictionary.Add(3, "Completed");

                model.StatusList = statusDictionary.Select(x => new ComplaintMasterEntity
                {
                    Id = x.Key,
                    Name = x.Value
                }).ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
                throw;
            }
        }

        [HttpGet]
        public IActionResult GetCustomerById(long customerId)
        {
            try
            {
                var data = _customerMasterService.GetCustomerMasterByUserId(customerId);

                if (data == null)
                {
                    return Json(null);
                }

                return Json(new
                {
                    customerName = $"{data.FirstName} {data.LastName}".Trim(),
                    mobileNumber = data.MobileNo,
                    email = data.EmailId,
                    address = data.Address
                });
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
                return Json(null);
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult Create(ComplaintMasterEntity model)
        {
            try
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

                        if(model.ActionType=="SaveDraft")
                        {
                            model.StatusId = 1;
                        }
                        else
                        {
                            model.StatusId = 2;
                        }
                            // Encode sensitive data before saving
                            ComplaintMasterEntity encodedEntity = (ComplaintMasterEntity)HtmlEncodeDecodeHelper.Encode(model);
                       
                        var result = _complaintasterService.InsertUpdateComplaintMaster(encodedEntity);

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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public IActionResult UpdateComplaintStatusById(Int64 ComplaintId,string Remark)
        {
            try
            {
                var CurruntUserId = _workContext.CurrentUser.Id;
                var data = _complaintasterService.UpdateComplaintStatusById(ComplaintId,CurruntUserId,Remark);

                return Json(new
                {            
                    status = "Sucess"
                });
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
                return Json(null);
            }
        }

    }
}
