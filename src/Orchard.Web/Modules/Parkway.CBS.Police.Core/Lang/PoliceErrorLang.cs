using System;
using Orchard.Localization;
using Parkway.CBS.Core.Lang;

namespace Parkway.CBS.Police.Core.Lang
{
    public abstract class PoliceErrorLang : ErrorLang
    {
        private static Localizer _t = NullLocalizer.Instance;

        private static Localizer T { get { return _t; } }


        /// <summary>
        /// This police officer is in active deployment
        /// </summary>
        public static LocalizedString police_officer_is_in_active_deployment
        {
            get { return T("This police officer is in active deployment"); }
        }


        /// <summary>
        /// This police officer is in active deployment
        /// </summary>
        public static LocalizedString police_officer_with_service_number_is_in_active_deployment(string serviceNumber)
        {
            return T(string.Format($"Police Officer with AP/Force Number {serviceNumber} is in an active deployment"));
        }


        /// <summary>
        /// The number of assigned officers does not match the requested number.
        /// </summary>
        public static LocalizedString escort_officers_assigned_mismatch(int numberAssigned, int numberRequested)
        {
            return T(string.Format($"Please assign the officer(s). Number Requested: {numberRequested}, Number Assigned: {numberAssigned}"));

        }


        public static LocalizedString Request_List_Months_Duration_Exceeded(int maximumListMonths)
        {
            return T($"Duration of months selected exceeds the acceptable range of {maximumListMonths} months.");
        }

        /// <summary>
        /// Sorry you are not authorized to perform that action. Please contact POSSAP admin.
        /// </summary>
        public static LocalizedString usernotauthorized(params object[] parameters)
        {
            return T(string.Format("Sorry you are not authorized to perform that action. Please contact POSSAP admin."));
        }


        /// <summary>
        /// Error creating UserPartRecord user
        /// </summary>
        public static LocalizedString unabletocreateuser()
        {
            return T(string.Format("Unable to create user."));
        }


        /// <summary>
        /// return localized string
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static LocalizedString ToLocalizeString(string errorMessage)
        {
            return T(errorMessage);
        }

        /// <summary>
        /// Selected command not found
        /// </summary>
        public static LocalizedString selected_command_404()
        {
            return T(string.Format("Selected command not found."));
        }

        /// <summary>
        /// Selected option not found.
        /// </summary>
        /// <returns></returns>
        public static LocalizedString selected_option_404()
        {
            return T(string.Format("Selected option not found."));
        }


        /// <summary>
        /// NIN not found.
        /// </summary>
        public static LocalizedString nin_not_found()
        {
            return T(string.Format("NIN not found."));
        }


        /// <summary>
        /// Cannot parse NIN date of birth format
        /// </summary>
        public static LocalizedString cannot_parse_nin_dob()
        {
            return T(string.Format("Cannot parse NIN date of birth format."));
        }


        /// <summary>
        /// Mismatch in date of birth from Identity system.
        /// </summary>
        public static LocalizedString mismatch_in_dateofbirth()
        {
            return T(string.Format("Mismatch in date of birth from Identity system."));
        }

    }
}