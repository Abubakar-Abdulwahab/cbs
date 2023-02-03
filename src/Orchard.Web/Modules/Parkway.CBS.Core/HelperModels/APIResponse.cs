using Newtonsoft.Json;
using System;
using System.Net;

namespace Parkway.CBS.Core.HelperModels
{

    /// <summary>
    /// 
    /// </summary>
    public class APIResponse
    {
        /// <summary>
        /// Does the response have any errors.
        /// <para>If the response has any error the bool value is true.</para>
        /// </summary>
        public bool Error { get; set; }
        
        /// <summary>
        /// Error code
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// This contains the response object.
        /// <para>Return object is the result of the request, if error is true, returned value is a list of error model <see cref="ErrorModel"/></para>
        /// </summary>
        public dynamic ResponseObject { get; set; }

        /// <summary>
        /// Status code
        /// </summary>
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }
    }


    public class PayDirectAPIResponseObj : APIResponse
    {
        public string ReturnType { get; set; }
    }
}