using Orchard.Users.Models;
using System;

namespace Parkway.CBS.Core.HelperModels
{
    public class ExternalPaymentProviderVM
    {
        public int Id { get; set; }

        public int Identifier { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public string ClassImplementation { get; set; }

        public string ClientID { get; set; }

        public string ClientSecret { get; set; }

        public DateTime CreatedAt { get; set; }

        public string AddedBy { get ; set; }

        /// <summary>
        /// this value will hold the agent fee 
        /// that was deducted from the amount the user has paid
        /// </summary>
        public bool AllowAgentFeeAddition { get; set; }
    }
}