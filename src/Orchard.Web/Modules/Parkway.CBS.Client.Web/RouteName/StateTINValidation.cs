using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Client.Web.RouteName
{
    public class StateTINValidation
    {
        /// <summary>
        /// C.PAYE.Tax.Receipt.Validate
        /// </summary>
        public static string ValidateStateTIN { get { return "C.ValidateStateTin"; } }

        /// <summary>
        /// C.PAYE.Tax.Receipt.Details
        /// </summary>
        public static string StateTINDetails { get { return "C.State.TIN.Details"; } }
    }
}