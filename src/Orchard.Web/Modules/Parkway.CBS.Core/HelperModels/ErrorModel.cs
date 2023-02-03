using Newtonsoft.Json;

namespace Parkway.CBS.Core.HelperModels
{
    /// <summary>
    /// Error model
    /// </summary>
    public class ErrorModel
    {
        /// <summary>
        /// Error field name
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}