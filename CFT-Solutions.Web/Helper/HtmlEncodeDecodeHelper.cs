
using System.Text.RegularExpressions;
using System.Web;

namespace CFT_Solutions.Web.Helper
{
    public static class HtmlEncodeDecodeHelper
    {

        /// <summary>
        /// Encode 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="AllowHtmlProperties"></param>
        /// <param name="IgnoreProperties"></param>
        /// <returns></returns>
        public static object Encode(object obj, List<string> AllowHtmlProperties = null, List<string> IgnoreProperties = null)
        {
            if (AllowHtmlProperties == null)
                AllowHtmlProperties = new List<string>();

            if (IgnoreProperties == null)
                IgnoreProperties = new List<string>();

            //Get all Property
            var Properties = obj.GetType().GetProperties();

            //Loop each property
            foreach (var item in Properties)
            {
                try
                {
                    //if property is not in ignore list then process further
                    if (!IgnoreProperties.Contains(item.Name))
                    {
                        //value to set to property
                        string PropertyValue = string.Empty;
                        bool IsAllowHtmlAttribute = false;

                        //Get Type of current property
                        Type propType = item.PropertyType;

                        //check for specific data type
                        if (propType.Name.ToLower() == "string".ToLower())
                        {
                            //check if property has AllowHtmlAttribute - this we can get for Model Properties
                            //20-07-20222
                            // var hasAllowHtmlAttribute = item.GetCustomAttributes(typeof(AllowHtmlAttribute), false).FirstOrDefault() == null ? false : true;
                            // if (hasAllowHtmlAttribute)
                            //     IsAllowHtmlAttribute = true;
                            // else if (AllowHtmlProperties.Contains(item.Name)) //this we need to check for Entity Properties
                            //     IsAllowHtmlAttribute = true;
                            //20-07-20222
                            // try to get the value
                            //PropertyValue = Convert.ToString(obj.GetType().GetProperty(item.Name).GetValue(obj, null));
                            PropertyValue = Convert.ToString(item.GetValue(obj, null));

                            // if value is not null or empty set the value to the property
                            if (!string.IsNullOrEmpty(PropertyValue))
                            {
                                object updatedValue;

                                if (!IsAllowHtmlAttribute)
                                {
                                    updatedValue = HttpUtility.HtmlEncode(PropertyValue);
                                }
                                else
                                {
                                    //need to decode here bcoz rich text editor gives encoded text
                                    var decodedHTML = HttpUtility.HtmlDecode(PropertyValue);

                                    //below is to remove tags like script, iframe - will give only plain html text
                                    //https://github.com/mganss/HtmlSanitizer
                                    var sanitizer = new Ganss.Xss.HtmlSanitizer();
                                    var sanitized = sanitizer.Sanitize(decodedHTML);
                                    updatedValue = sanitized;
                                }

                                //item.SetValue(ob, Convert.ChangeType(PropertyValue, propType), null);
                                item.SetValue(obj, updatedValue, null);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            return obj;
        }

        /// <summary>
        /// Decode
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="AllowHtmlProperties"></param>
        /// <param name="IgnoreProperties"></param>
        /// <returns></returns>
        public static object Decode(object obj, List<string> AllowHtmlProperties = null, List<string> IgnoreProperties = null)
        {
            if (AllowHtmlProperties == null)
                AllowHtmlProperties = new List<string>();

            if (IgnoreProperties == null)
                IgnoreProperties = new List<string>();

            //Get all Property
            var Properties = obj.GetType().GetProperties();

            //Loop each property
            foreach (var item in Properties)
            {
                try
                {
                    //if property is not in ignore list then process further
                    if (!IgnoreProperties.Contains(item.Name))
                    {
                        //value to set to property
                        string PropertyValue = string.Empty;
                        bool IsAllowHtmlAttribute = false;

                        //Get Type of current property
                        Type propType = item.PropertyType;

                        //check for specific data type
                        if (propType.Name.ToLower() == "string".ToLower())
                        {
                            //20-07-2022
                            //check if property has AllowHtmlAttribute - this we can get for Model Properties
                            // var hasAllowHtmlAttribute = item.GetCustomAttributes(typeof(AllowHtmlAttribute), false).FirstOrDefault() == null ? false : true;
                            // if (hasAllowHtmlAttribute)
                            //     IsAllowHtmlAttribute = true;
                            // else if (AllowHtmlProperties.Contains(item.Name)) //this we need to check for Entity Properties
                            //     IsAllowHtmlAttribute = true;
                            //20-07-2022
                            // try to get the value
                            //PropertyValue = Convert.ToString(obj.GetType().GetProperty(item.Name).GetValue(obj, null));
                            PropertyValue = Convert.ToString(item.GetValue(obj, null));

                            // if value is not null or empty set the value to the property
                            if (!string.IsNullOrEmpty(PropertyValue))
                            {
                                object updatedValue;

                                if (!IsAllowHtmlAttribute)
                                {
                                    updatedValue = HttpUtility.HtmlDecode(PropertyValue);
                                }
                                else
                                {
                                    updatedValue = PropertyValue;
                                }

                                //item.SetValue(ob, Convert.ChangeType(PropertyValue, propType), null);
                                item.SetValue(obj, updatedValue, null);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return obj;
        }

        /// <summary>
        /// Validate Special Chars
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="IgnoreProperties"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ValidateSpecialChars(object obj, List<string> IgnoreProperties = null, string InputDisallowedChars = null)
        {
            Dictionary<string, string> modelErrors = new Dictionary<string, string>();
            //string InputDisallowedChars = System.Configuration.ConfigurationManager.AppSettings["InputDisallowedChars"]+","+"<"+"@";
            var DisallowedChars = InputDisallowedChars.Split(',').ToList();
            //DisallowedChars.Add("<");
            //DisallowedChars.Add(">");
            //DisallowedChars.Add("/");
            var errorMsg = "Following special characters are not allowed in {0} " + String.Join(" ", DisallowedChars);
            var regEx = new Regex("[" + String.Join("\\", DisallowedChars) + "]");

            if (IgnoreProperties == null)
                IgnoreProperties = new List<string>();

            //Get all Property
            var Properties = obj.GetType().GetProperties();

            //Loop each property
            foreach (var item in Properties)
            {
                try
                {
                    //if property is not in ignore list then process further
                    if (!IgnoreProperties.Contains(item.Name))
                    {
                        //value to set to property
                        string PropertyValue = string.Empty;
                        bool IsAllowHtmlAttribute = false;

                        //Get Type of current property
                        Type propType = item.PropertyType;

                        //check for specific data type
                        if (propType.Name.ToLower() == "string".ToLower())
                        {
                            //20-07-2022
                            //check if property has AllowHtmlAttribute - this we can get for Model Properties
                            // var hasAllowHtmlAttribute = item.GetCustomAttributes(typeof(AllowHtmlAttribute), false).FirstOrDefault() == null ? false : true;
                            // if (hasAllowHtmlAttribute)
                            //     IsAllowHtmlAttribute = true;
                            //20-07-2022

                            // try to get the value
                            PropertyValue = Convert.ToString(item.GetValue(obj, null));

                            // if value is not null or empty set the value to the property
                            if (!string.IsNullOrEmpty(PropertyValue))
                            {
                                if (!IsAllowHtmlAttribute)
                                {
                                    MatchCollection matchCount = regEx.Matches(PropertyValue.Trim());
                                    if (matchCount.Count > 0)
                                    {
                                        var _errorMsg = errorMsg;
                                        _errorMsg = _errorMsg.Replace("{0}", item.Name);
                                        modelErrors.Add(item.Name, _errorMsg);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return modelErrors;
        }

        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
    }
}
