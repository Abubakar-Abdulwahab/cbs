using System;

namespace Parkway.CBS.Core.HelperModels
{
    public class RegisterUserResponse
    {
        /// <summary>
        /// CBSUser Id
        /// </summary>
        public Int64 CBSUserId { get; set; }

        /// <summary>
        /// Tax entity details
        /// </summary>
        public TaxEntityViewModel TaxEntityVM { get; set; }
        
        /// <summary>
        /// CBS user details
        /// </summary>
        public CBSUserVM CBSUser { get; set; }
    }
}