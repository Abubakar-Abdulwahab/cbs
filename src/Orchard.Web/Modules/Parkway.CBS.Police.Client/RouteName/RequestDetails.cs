using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Client.RouteName
{
    public class RequestDetails
    {
        public static String ShowRequestDetails
        {
            get { return "P.Request.Details"; }
        }

        public static string ShowBranchRequestDetails
        {
            get { return "P.Request.Subuser.Details"; }
        }

        /// <summary>
        /// P.Request.Details.Document
        /// </summary>
        public static string ViewServiceDocument { get { return "P.Request.Details.Document"; } }
    }
}