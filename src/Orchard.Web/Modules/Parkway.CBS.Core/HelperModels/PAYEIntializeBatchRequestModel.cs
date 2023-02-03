using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class PAYEIntializeBatchRequestModel
    {
        /// <summary>
        /// An identifier for the PAYE items to be sent. Should be unique
        /// per request
        /// </summary>
        public string BatchIdentifier { get; set; }

        /// <summary>
        /// The employer State TIN Id
        /// </summary>
        public string EmployerPayerId { get; set; }

        /// <summary>
        /// The POST endpoint URL where the response will be posted
        /// after validation
        /// </summary>
        public string CallbackURL { get; set; }
    }
}