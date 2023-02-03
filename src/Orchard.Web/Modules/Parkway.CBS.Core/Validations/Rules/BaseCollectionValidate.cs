using System.Web.Mvc;

namespace Parkway.CBS.Core.Validations.Rules
{
    public abstract class BaseCollectionValidate
    {
        /// <summary>
        /// Check the max and min length
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="varibaleName"></param>
        /// <param name="value"></param>
        /// <param name="maxLength"></param>
        /// <param name="minLength"></param>
        /// <returns>BaseCollectionValidate</returns>
        protected BaseCollectionValidate CheckLength<C>(C callBack, string varibaleName, string value, int maxLength, int minLength) where C : Controller
        {
            if (value.Length > maxLength || value.Length < minLength)
            {
                callBack.ModelState.AddModelError(varibaleName, string.Format("This value can only be {0} long and {1} short", maxLength, minLength));
            }
            return this;
        }

        /// <summary>
        /// Check that the value is not null or empty
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="varibaleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected BaseCollectionValidate Required<C>(C callBack, string varibaleName, string value) where C : Controller
        {
            if (string.IsNullOrEmpty(value)) { callBack.ModelState.AddModelError(varibaleName, "This field is required"); }
            return this;
        }
    }
}