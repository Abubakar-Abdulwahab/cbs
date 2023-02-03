using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    /// <summary>
    /// Validation response model for remita
    /// <para>
    /// {
    ///     "Status":"ok",
    ///     "uid":"0112233445",
    ///     "amount":"50000",
    ///     "name":"Tobi Amira",
    ///     "email":"amira@systemspecs.com.ng",
    ///     "phoneNumber":"09037775694"
    /// }
    ///</para>
    /// </summary>
    public class RemitaValidationResponseModel
    {
        public string Status { get; set; }

        /// <summary>
        /// Unique reference for each customer on your application/database
        /// </summary>
        //this would be the invoice number
        public string uuid { get; set; }
        public string amount { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }

        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }
    }
}