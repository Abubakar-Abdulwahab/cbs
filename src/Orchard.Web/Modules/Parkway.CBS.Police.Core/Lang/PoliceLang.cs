using Orchard.Localization;

namespace Parkway.CBS.Police.Core.Lang
{
    public abstract class PoliceLang
    {
        private static Localizer _t = NullLocalizer.Instance;

        private static Localizer T { get { return _t; } }

        /// <summary>
        /// Sorry you are not authorized to perform that action. Please contact admin.
        /// </summary>
        public static LocalizedString request_initiation_with_invoice
        {
            get { return T("Request initiation. A request for this service has been logged. Invoice payment is expected before your request can proceed to the next stage for approval."); }
        }


        public static LocalizedString requesthasnotyetbeenapproved
        {
            get { return T("Request has not yet been approved by anyone"); }
        }

        public static LocalizedString cannotendofficerdeployment
        {
            get { return T("Police officer deployment cannot end because the deployment is no longer running"); }
        }

        public static LocalizedString officerdeploymentendedsuccessfully
        {
            get { return T("Officer deployment ended successfully."); }
        }

        public static LocalizedString escortestimateamount(string rankName)
        {
            return T($"NB: This estimate is based on {rankName} rank");
        }

        public static LocalizedString escortestimateamount(int numberOfOfficers)
        {
            return T($"NB: This is an estimate of the amount that would be paid for {numberOfOfficers} officer(s)");
        }

        public static LocalizedString savesuccessfully
        {
            get { return T("Record saved successfully"); }
        }

        public static LocalizedString updatesuccessfull
        {
            get { return T("Record updated successfully"); }
        }

        public static LocalizedString updatesuccessfully(string message)
        {
            return T($"{message} updated successfully");
        }

        public static LocalizedString applicantinvitedsuccessfully(string message)
        {
            return T(string.Format("{0}", message));
        }

        public static LocalizedString pssadminsignaturesavedsuccessfully
        {
            get { return T("Signature saved successfully."); }
        }

        public static LocalizedString nowalletidfound
        {
            get { return T("Account wallet id not found."); }
        }

        public static LocalizedString noselectedusers
        {
            get { return T("Kindly select a user(s)."); }
        }

        public static LocalizedString ToLocalizeString(string message)
        {
            return T(string.Format("{0}", message));
        }
    }
}