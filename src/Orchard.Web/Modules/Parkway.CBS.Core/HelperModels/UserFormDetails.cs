using System;

namespace Parkway.CBS.Core.HelperModels
{
    public class UserFormDetails
    {
        /// <summary>
        /// the control Id on the form revenue head table
        /// </summary>
        public Int64 ControlIdentifier { get; set; }

        public string FriendlyName { get; set; }

        public string FormValue { get; set; }

        public int RevenueHeadId { get; set; }

    }
}