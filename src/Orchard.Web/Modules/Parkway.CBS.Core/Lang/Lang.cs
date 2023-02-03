using System;
using Orchard.Localization;

namespace Parkway.CBS.Core.Lang
{
    public abstract class Lang
    {
        private static Localizer _t = NullLocalizer.Instance;

        private static Localizer T { get { return _t; } }

        /// <summary>
        /// Sorry you are not authorized to perform that action. Please contact admin.
        /// </summary>
        public static LocalizedString usernotauthorized_ex_text
        {
            get { return T("Sorry you are not authorized to perform that action. Please contact admin."); }
        }

        /// <summary>
        /// Sorry we cannot save your data right now. Please contact admin
        /// </summary>
        public static LocalizedString cannotsavemdarecord_ex_text
        {
            get { return T("Sorry we cannot save your data right now. Please contact the site admin."); }
        }

        /// <summary>
        /// An MDA with the name you specified is already taken.
        /// </summary>
        public static LocalizedString registrationsuccessful
        {
            get { return T("Registration successful. You can now login."); }
        }

        /// <summary>
        /// An invoice for this revenue head was previously generated for you.
        /// </summary>
        public static LocalizedString existinginvoicefoundforoneoffpayment()
        {
            return T(string.Format("An invoice for this revenue head was previously generated for you."));
        }


        public static LocalizedString operatorsavedsuccessfully(string categoryName, string name, string payerId)
        {
            return T(string.Format("{0} Operator {1} with Payer Id {2} has been created successfully, please add a file containing the schedule for cellsites", categoryName, name, payerId));
        }


        /// <summary>
        /// Invoice {0} has already been paid for
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        public static LocalizedString invoicealreadypaid(string invoiceNumber)
        {
            return T(string.Format("Invoice {0} has already been paid for ", invoiceNumber));
        }


        /// <summary>
        /// MDA(s) saved successfully
        /// </summary>
        public static LocalizedString mdasaved
        {
            get { return T("MDA saved successfully."); }
        }

        public static LocalizedString invoicepaymentadded(string invoiceNumber, string amountPaid)
        {
            var samount = string.Format("{0:0n}", amountPaid);
            return T(string.Format("₦{1} has been added to this invoice {0}.", invoiceNumber, samount));
        }

        /// <summary>
        /// MDA record has been updated successfully.
        /// </summary>
        public static LocalizedString mdaupdated
        {
            get { return T("MDA record has been updated successfully."); }
        }

        /// <summary>
        /// Sorry something went wrong while processing your request. Please try again later or contact admin
        /// </summary>
        public static LocalizedString genericexception_text
        {
            get { return T("Sorry something went wrong while processing your request. Please try again later or contact admin."); }
        }


        /// <summary>
        /// We could not find any record with those details.
        /// </summary>
        public static LocalizedString recordcouldnotbefound_text
        {
            get { return T("We could not find any record with those details."); }
        }


        /// <summary>
        /// Your previous code has expired. A new code has been sent to you.
        /// </summary>
        /// <returns></returns>
        public static LocalizedString yourprevcodeexpiredanewcodehasbeengeneratedforyou
        {
            get { return T("Your previous code has expired. A new code has been sent to you."); }
        }

        public static LocalizedString custommessage(string message)
        {
            return T(string.Format("{0}", message));
        }


        /// <summary>
        /// Your previous code has expired. Enter email for a new one.
        /// </summary>
        /// <returns></returns>
        public static LocalizedString yourprevcodeexpired
        {
            get { return T("Your previous code has expired. Enter email for a new one."); }
        }


        /// <summary>
        /// Your previous code has expired. Enter phone number for a new one.
        /// </summary>
        /// <returns></returns>
        public static LocalizedString yourprevcodeexpiredphonenumber
        {
            get { return T("Your previous code has expired. Enter phone number for a new one."); }
        }


        /// <summary>
        /// You need to add a billing information first.
        /// </summary>
        public static LocalizedString addbillingddetailsfirst
        {
            get { return T("You need to add a billing information first."); }
        }

        public static LocalizedString adminactivationemailnotificationsent
        {
            get { return T("Email notification has been sent."); }
        }

        public static LocalizedString couldnotsendadminactivationemail
        {
            get { return T("Could not send email notification has been sent."); }
        }

        public static LocalizedString passwordchangedsuccessfully
        {
            get { return T("Your password has been changed successfully."); }
        }

        /// <summary>
        /// Form fields have been saved.
        /// </summary>
        public static LocalizedString formdatahasbeensaved
        {
            get { return T("Form fields have been saved."); }
        }


        /// <summary>
        /// Settings saved successfully
        /// </summary>
        public static LocalizedString savesuccessfully
        {
            get { return T("Settings saved successfully."); }
        }

        
        /// <summary>
        /// Could not save form details.
        /// </summary>
        public static LocalizedString couldnotsaveformdetails
        {
            get { return T("Could not save form details."); }
        }

        /// <summary>
        /// The search parameters could not be found.
        /// </summary>
        public static LocalizedString searchcrriteriacouldnotbefound
        {
            get { return T("The search parameters could not be found."); }
        }        


        /// <summary>
        /// Ok
        /// </summary>
        public static LocalizedString remitapaymentnotificationok
        {
            get { return T("Ok"); }
        }

        /// <summary>
        /// Your account has already been verified
        /// </summary>
        public static LocalizedString youraccounthasalreadybeenverified
        {
            get { return T("Your account has already been verified."); }
        }

        /// <summary>
        /// Ok
        /// </summary>
        public static LocalizedString netpaypaymentnotificationok
        {
            get { return T("Ok"); }
        }



        /// <summary>
        /// Invoice has already been paid for.
        /// </summary>
        public static LocalizedString invoicealreadypaidfor
        { get { return T("Invoice has already been paid for."); } }


        /// <summary>
        /// A tax profile with the Payer Id {0} has been created for you. You can register later to view all your invoices, payments and receipts.
        /// </summary>
        public static LocalizedString taxentitycreated(string payerId)
        {  return T(string.Format("A tax profile with the Payer Id {0} has been created for you. You can register later to view all your invoices, payments and receipts.", payerId)); }


        /// <summary>
        /// 0000
        /// <para>When invoice details has been gotten successfully</para>
        /// </summary>
        public static LocalizedString bankcollectresponseokcode
        { get { return T("0000"); } }

        /// <summary>
        /// 0000
        /// <para>When invoice details has been gotten successfully</para>
        /// </summary>
        public static LocalizedString netpayresponseokcode
        { get { return T("0000"); } }


        /// <summary>
        /// Payment notification already processed
        /// </summary>
        public static LocalizedString paymentnotificationalreadyprocess
        { get { return T("Payment notification already processed."); } }


        /// <summary>
        /// No profile(s) for this category found.
        /// </summary>
        public static LocalizedString noprofiles404
        { get { return T("No profile(s) for this category found."); } }

        public static LocalizedString taxpayercodealreadyexist
        {
            get { return T("Tax Payer Code already exist for another Tax Payer."); }
        }

        /// <summary>
        /// Payment Notification Successful
        /// </summary>
        public static LocalizedString paymentnotificationsuccessful
        {
            get { return T("Payment Notification Successful"); }
        }


        /// <summary>
        ///  Token has expired, reloading this page to get you a new one.
        /// </summary>
        public static LocalizedString tokenexpiredredirecting
        {
            get { return T("Token has expired, reloading this page to get you a new one."); }
        }


        /// <summary>
        ///  Code has been resent.
        /// </summary>
        public static LocalizedString codehasbeenresent
        {
            get { return T("Code has been resent."); }
        }


        public static LocalizedString referencedatasavedsuccessfully(string batchRef)
        {
            return T(string.Format("Data saved successfully. You can check the status using the {0} Batch Reference ID.", batchRef));
        }

        public static LocalizedString pssrequestapprovedsuccessfully(string message)
        {
            return T(string.Format("{0}", message));
        }

        public static LocalizedString pssrequestrejectedsuccessfully(string message)
        {
            return T(string.Format("{0}", message));
        }

        public static LocalizedString passwordresetsuccessful
        {
            get { return T("Your password has been changed successfully."); }
        }

        public static LocalizedString policeofficerchangedsuccessfully
        {
            get { return T("Officer changed successfully."); }
        }

        public static LocalizedString cannotchangepoliceofficer
        {
            get { return T("Police officer cannot be changed because the deployment is no longer running"); }
        }

        public static LocalizedString extpaymentprovidersaved
        {
            get { return T("External Payment Provider Created Successfully."); }
        }

        public static LocalizedString payebatchvalidation
        {
            get { return T("Successful"); }
        }
    }
}