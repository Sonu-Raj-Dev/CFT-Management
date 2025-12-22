using CFT_Solutions.Core;
using CFT_Solutions.Core.Entity.User;
using CFT_Solutions.Service.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CFT_Solutions.Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        #region Fields

        //protected internal readonly IWorkContext _workContext;
        public readonly IWorkContext _workContext;
        protected internal readonly IHttpContextAccessor _httpContextAccessor;
        public readonly IUserService _userService;
        #endregion

        #region Constructor

        public BaseController(IWorkContext workContext,
            IHttpContextAccessor httpContextAccessor,
            IUserService userService)
        {
            this._workContext = workContext;
            this._httpContextAccessor = httpContextAccessor;
            this._userService = userService;
            //this._commonService = EngineContext.Current.Resolve<ICommonService>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Access denied view
        /// </summary>
        /// <returns>Access denied view</returns>
        protected virtual ActionResult AccessDeniedView()
        {
            //return new HttpUnauthorizedResult();
            return RedirectToAction("AccessDenied", "Home");
        }


        protected void SetRegistrationStepDone(Int64 TransactionId, int stepID)
        {
            //_commonService.SetStepCompletionStatus(TransactionId, stepID);
        }

        protected void SetVerificationStatus(Int64 TransactionId, int stepID, int VerificationStatus, string comments)
        {
            //  _commonService.SetVerificationStatus(TransactionId, stepID, VerificationStatus, comments);
        }

        /// <summary>
        /// GetUserContext
        /// </summary>
        /// <returns></returns>
        public UserEntity GetUserContext()
        {
            List<Claim> UserClaims = new List<Claim>();
            UserEntity userEntity = null;
            try
            {
                UserClaims = _httpContextAccessor.HttpContext.User.Claims.ToList();
                foreach (var item in UserClaims)
                {
                    if (item.Type.ToLower() == "LoginID".ToLower())
                    {
                        if (!string.IsNullOrEmpty(item.Value))
                        {
                            //Get user Details
                            userEntity = _userService.GetUserByLoginId(item.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return userEntity;
        }

        protected string DecryptStringAES(string cipherText)
        {
            //var keybytes = Encoding.UTF8.GetBytes("8080808080808080");
            //var iv = Encoding.UTF8.GetBytes("8080808080808080");

            var keybytes = Encoding.UTF8.GetBytes("9920053264969936");
            var iv = Encoding.UTF8.GetBytes("9920053264969936");

            var encrypted = Convert.FromBase64String(cipherText);
            var decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);
            return string.Format(decriptedFromJavascript);
        }

        [NonAction]
        private string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.  
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            // Declare the string used to hold  
            // the decrypted text.  
            string plaintext = null;

            // Create an RijndaelManaged object  
            // with the specified key and IV.  
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings  
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.  
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                try
                {
                    // Create the streams used for decryption.  
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {

                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream  
                                // and place them in a string.  
                                plaintext = srDecrypt.ReadToEnd();

                            }

                        }
                    }
                }
                catch
                {
                    plaintext = "keyError";
                }
            }

            return plaintext;
        }

        #endregion
    }
}
