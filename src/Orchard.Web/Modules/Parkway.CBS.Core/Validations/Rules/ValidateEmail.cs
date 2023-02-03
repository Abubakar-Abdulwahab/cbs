using Parkway.CBS.Core.Validations.Rules.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Core.Validations.Rules
{
    public class ValidateEmail : BaseCollectionValidate, ICollectionValidator
    {
        private static readonly Lazy<string> _name = new Lazy<string>(() => { return "Email"; });

        public string ValidationName() { return _name.Value; }

        public C Validate<C>(dynamic data) where C : Controller
        {
            C callBack = data.CallBack;
            string variableName = data.HTMLName;
            string value = data.Value;
            string props = data.Properties;
            var values = props.Split(',');
            var required = values[0];
            string smaxLength = values[1].Split(':')[1];
            string sminLength = values[2].Split(':')[1];

            int maxLength = 250; int minLength = 2;
            Int32.TryParse(smaxLength, out maxLength);
            Int32.TryParse(sminLength, out minLength);

            Required(callBack, variableName, value);
            //
            CheckEmail(variableName, value).CheckLength(callBack, variableName, value, maxLength, minLength);
            //CheckEmail(callBack, variableName, value).CheckLength(callBack, variableName, value, maxLength, minLength);

            return callBack;
        }

        private ValidateEmail CheckEmail(string variableName, string value)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(value, Orchard.Users.Models.UserPart.EmailPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                // http://haacked.com/archive/2007/08/21/i-knew-how-to-validate-an-email-address-until-i.aspx    
                //callBack.ModelState.AddModelError(variableName, ("You must specify a valid email address."));
            }
            return this;
        }        
    }
}