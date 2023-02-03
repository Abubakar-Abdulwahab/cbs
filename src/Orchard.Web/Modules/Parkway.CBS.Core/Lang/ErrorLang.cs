using System;
using Orchard.Localization;

namespace Parkway.CBS.Core.Lang
{
    public abstract class ErrorLang
    {
        private static Localizer _t = NullLocalizer.Instance;

        public static LocalizedString errorvalidatingcollectionform(string message = "")
        {
            return T("Error validating collection form");
        }


        /// <summary>
        /// Error validating form.
        /// </summary>
        public static LocalizedString validationerror()
        {
            return T("Error validating form.");
        }

        private static Localizer T { get { return _t; } }



        /// <summary>
        /// Invoice amount is too small
        /// </summary>
        /// <returns></returns>
        public static LocalizedString invoiceamountistoosmall(decimal amount)
        { return T(string.Format("Invoice amount {0} is too small.", amount.ToString())); }


        /// <summary>
        /// Invoice amount is too small
        /// </summary>
        /// <returns></returns>
        public static LocalizedString invoiceamountistoosmall()
        { return T("Invoice amount is too small."); }



        /// <summary>
        /// TIN could not be found.
        /// </summary>
        public static LocalizedString tinnumbernotfound()
        {
            return T("TIN could not be found.");
        }



        /// <summary>
        /// This field value must be between {0} and {1} characters
        /// </summary>
        /// <param name="minLength"></param>
        public static LocalizedString inputlengthvalidation(int minLength, int maxLength)
        {
            return T(string.Format("This field value must be between {0} and {1} characters", minLength, maxLength));
        }


        /// <summary>
        /// Please register to use start your application request.
        /// </summary>
        /// <returns></returns>
        public static LocalizedString pleaseregister()
        {
            return T("Please register to use start your application request.");
        }


        /// <summary>
        /// Business type not found
        /// </summary>
        public static LocalizedString businesstype404()
        {
            return T("Business type not found.");
        }

        /// <summary>
        /// Business size not found
        /// </summary>
        public static LocalizedString businesssize404()
        {
            return T("Business size not found.");
        }

        public static LocalizedString givemelocalizedmessage(string message)
        {
            return T(message);
        }


        /// <summary>
        /// To perform that action you are required to sign in
        /// </summary>
        public static LocalizedString signinrequired()
        {
            return T("To perform that action you are required to sign in");
        }


        /// <summary>
        /// Error validating form
        /// </summary>
        public static LocalizedString errorvalidatingform()
        {
            return T("Error validating form.");
        }


        /// <summary>
        /// This field is required
        /// </summary>
        public static LocalizedString fieldrequired()
        {
            return T("This field is required");
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
        /// Another entry already has this {0} value
        /// </summary>
        public static LocalizedString notunique_vl_text(string param = "")
        {
            return T(string.Format("Another entry already has this {0} value", param));
        }


        public static LocalizedString nosearchparamforsearchfortaxpayer()
        {
            return T(string.Format("Please enter either the Tax Payer's TIN or the Phone Number"));
        }


        /// <summary>
        /// Exceeded the number of resends.
        /// </summary>
        public static LocalizedString exceededcoderetry()
        {
            return T(string.Format("Exceeded the number of resends."));
        }


        /// <summary>
        /// Please confirm email, before resetting email
        /// </summary>
        public static LocalizedString pleaseconfirmemail()
        {
            return T(string.Format("Please confirm email, before resetting email."));
        }

        /// <summary>
        /// Error saving customer record {0} value
        /// </summary>
        public static LocalizedString errorsavingcustomerrecord(string param = "")
        {
            return T(string.Format("Error saving customer record {0} value", param));
        }


        /// <summary>
        /// Record not found
        /// </summary>
        public static LocalizedString record404()
        {
            return T(string.Format("Record not found"));

        }


        /// <summary>
        /// A settlement rule with the MDA, Revenue Head, and Payment Provider combination already exists.
        /// </summary>
        public static LocalizedString mdarevenueprovidercombinationalreadyexists()
        {
            return T(string.Format("A settlement rule with the MDA, Revenue Head, and Payment Provider combination already exists."));
        }


        /// <summary>
        /// A settlement rule with the MDA, Revenue Head, and Payment Provider combination already exists.
        /// </summary>
        public static LocalizedString mdarevenueprovidercombinationalreadyexists(string revenueHeadName)
        {
            return T(string.Format("A settlement rule with the MDA, Revenue Head {0}, and Payment Provider combination already exists.", revenueHeadName));
        }


        /// <summary>
        /// Could not complete authentication process
        /// <para>Could not compute the expected MAC</para>
        /// </summary>
        public static LocalizedString macauthfailed()
        {
            return T(string.Format("Could not complete authentication process"));
        }

        /// <summary>
        /// Payment reference is required.
        /// </summary>
        public static LocalizedString paymentrefisrequired()
        {
            return T(string.Format("Payment reference is required."));
        }


        /// <summary>
        /// No token found
        /// </summary>
        /// <returns></returns>
        public static LocalizedString notokenfound()
        {
            return T(string.Format("No token found."));
        }


        /// <summary>
        /// Could not verify IP
        /// </summary>
        /// <returns></returns>
        public static LocalizedString couldnotverifyIP()
        {
            return T(string.Format("Could not verify IP"));
        }


        /// <summary>
        /// For recurring bills, a valid duration is required
        /// </summary>
        public static LocalizedString durationisrequired()
        {
            return T(string.Format("For recurring bills, a valid duration is required"));
        }


        /// <summary>
        /// Could not find user with payer Id.
        /// </summary>
        public static LocalizedString couldnotfinduserwithpayerid()
        {
            return T(string.Format("Could not find user with payer Id."));
        }


        /// <summary>
        /// Paye period not valid. Valid month and year fields are required
        /// </summary>
        public static LocalizedString payeperiodnotvalid(string month, string year)
        {
            return T(string.Format("Paye period not valid. Valid month and year fields are required. Month value {0}, Year value {1}", month, year));
        }


        /// <summary>
        /// Could not find tax payer.
        /// </summary>
        public static LocalizedString taxpayer404()
        {
            return T(string.Format("Could not find tax payer."));
        }


        /// <summary>
        /// Enter a valid amount.
        /// </summary>
        public static LocalizedString addvalidamount()
        {
            return T(string.Format("Enter a valid amount."));
        }


        /// <summary>
        /// Reversal amount do not match Initial amount : {0}, Reversal Amount: {1}
        /// </summary>
        public static LocalizedString reversalamountdoesnotmatch()
        {
            return T(string.Format("Reversal amount do not match Initial amount"));
        }


        /// <summary>
        /// Amount too small.
        /// </summary>
        public static LocalizedString amounttoosmall()
        {
            return T(string.Format("Amount is too small."));
        }


        /// <summary>
        /// Your account is not authorized to make {0}. Please contact Parkway admin
        /// </summary>
        /// <param name="permission"></param>
        public static LocalizedString apiactionforbidden(string permission)
        {
            return T(string.Format("Your account is not authorized to make {0}. Please contact Parkway admin", permission));
        }

        /// <summary>
        /// Reversal payment ref mismatch
        /// </summary>
        public static LocalizedString transactionlogrefdonotmatch()
        {
            return T(string.Format("Reversal payment ref mismatch"));
        }


        /// <summary>
        /// Verification message
        /// </summary>
        public static LocalizedString verificationmessage()
        {
            return T(string.Format("Verification message."));
        }


        /// <summary>
        /// No matching code found
        /// </summary>
        public static LocalizedString nomatchingcodefound()
        {
            return T(string.Format("No matching code found."));
        }


        /// <summary>
        /// We could not find your client ID in the request header
        /// </summary>
        public static LocalizedString noclientidinheader(params object[] parameters)
        {
            return T(string.Format("We could not find your client ID in the request header", parameters));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static LocalizedString custommessage(string message)
        {
            return T(message);
        }

        /// <summary>
        /// No record found
        /// </summary>
        public static LocalizedString norecord404()
        {
            return T(string.Format("No record found"));
        }

        /// <summary>
        /// No payment record found
        /// </summary>
        public static LocalizedString nopaymentrecordfound()
        {
            return T(string.Format("No payment items found"));
        }


        /// <summary>
        /// No record found
        /// </summary>
        public static LocalizedString norecord404(string msg)
        {
            return T(msg);
        }


        /// <summary>
        /// Could not find next schedule date
        /// </summary>
        public static LocalizedString couldnotgetnextscheduledate()
        {
            return T(string.Format("Could not find next schedule date"));
        }

        /// <summary>
        /// Form field not found {0} value
        /// </summary>
        public static LocalizedString collectionfieldnotfound(string param = "")
        {
            return T(string.Format("Form field not found {0} value", param));
        }

        /// <summary>
        /// We could not compute signature
        /// </summary>
        public static LocalizedString couldnotcomputehash(params object[] parameters)
        {
            return T(string.Format("We could not compute signature", parameters));
        }


        /// <summary>
        /// {fieldName} value is required
        /// </summary>
        public static LocalizedString valuerequired(string fieldName)
        {
            return T(string.Format($"{fieldName} value is required"));
        }


        /// <summary>
        /// Not Ok
        /// </summary>
        public static LocalizedString remitapaymentnotificationnotok()
        {
            return T(string.Format("Not Ok"));
        }


        /// <summary>
        /// Selected template {selectedTemplate} not found
        /// </summary>
        /// <param name="selectedTemplate"></param>
        public static LocalizedString notemplatefound(string selectedTemplate)
        {
            return T(string.Format($"Selected template {selectedTemplate} not found"));
        }


        /// <summary>
        /// Please create the default settlement rule
        /// </summary>
        public static LocalizedString createdefaultsettlement()
        {
            return T(string.Format("Please create the default settlement rule"));
        }


        /// <summary>
        /// Selected template implementation {selectedTemplateImpl} not found
        /// </summary>
        /// <param name="selectedTemplate"></param>
        public static LocalizedString notemplateimplfound(string selectedTemplateImpl)
        {
            return T(string.Format($"Selected template implementation {selectedTemplateImpl} not found"));
        }


        /// <summary>
        /// No Expert system found
        /// </summary>
        public static LocalizedString expertsys404()
        {
            return T(string.Format("No Expert system found"));
        }


        /// <summary>
        /// Error occured while getting {0} report
        /// </summary>
        /// <param name="reportName"></param>
        public static LocalizedString errorgettingreport(string reportName)
        {
            return T(string.Format("Error occured while getting {0} report", reportName));
        }

        /// <summary>
        /// File sizes must not have content exceeding 2Mb
        /// </summary>
        public static LocalizedString filesizeexceeded(string param = "")
        {
            return T("File sizes must not have content exceeding 2Mb");
        }

        /// <summary>
        /// File types are supported for upload.
        /// </summary>
        public static LocalizedString unsupportedfiletype(string param)
        {
            return T(string.Format("Unsupported filetype {0}. ", param));
        }

        /// <summary>
        /// Only {0} image types are supported for upload.
        /// </summary>
        public static LocalizedString filetypenotallowed(string param = "")
        {
            return T(string.Format("Only {0} image types are supported for upload. ", param));
        }

        public static LocalizedString tenantstatesettingshasalreadybeenset(string stateName = null)
        {
            return T(string.Format("Tenant state {0} has already been chosen. Please contact Parkway admin. ", stateName ?? ""));
        }



        /// <summary>
        /// Billing is not allowed for this revenue head
        /// </summary>
        public static LocalizedString billingisnotallowed()
        {
            return T(string.Format("Billing is not allowed for this revenue head."));
        }


        /// <summary>
        /// Username already exists
        /// </summary>
        public static LocalizedString usernamealreadyexists()
        {
            return T(string.Format("Username is already being used by another user. Try another one."));
        }


        /// <summary>
        /// A profile with these details already exists. You can try logging in on the signin page.
        /// </summary>
        public static LocalizedString profilealreadyexists()
        {
            return T(string.Format("A profile with these details already exists. You can try logging in on the signin page."));
        }

        /// <summary>
        /// Billing type not found.
        /// </summary>
        public static LocalizedString billingtype404()
        {
            return T(string.Format("Billing type not found."));
        }


        /// <summary>
        /// Transaction has already been reversed
        /// </summary>
        /// <returns></returns>
        public static LocalizedString transactionhasalreadybeenreversed()
        {
            return T(string.Format("Transaction has already been reversed"));
        }


        /// <summary>
        /// "Start time must be greater must be greater than present date."
        /// </summary>
        public static LocalizedString datefrompast(params string[] parameters)
        {
            return T(string.Format("start date has past. Start Date and Time must be greater must be greater than present date."));
        }


        /// <summary>
        /// No request type found
        /// </summary>
        public static LocalizedString paydirectrequesttype404()
        {
            return T(string.Format("No request type found"));
        }


        /// <summary>
        /// Email is already in use by another user. Try another email.
        /// </summary>
        public static LocalizedString emailalreadyinuse()
        {
            return T(string.Format("Email is already in use by another user. Try another email."));

        }


        /// <summary>
        /// Could not find any bank with the given bank name
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static LocalizedString bank404(params object[] parameters)
        {
            return T(string.Format("Could not find any bank with the given bank name"));
        }

        /// <summary>
        /// {0} is required.
        /// </summary>
        public static LocalizedString filerequired(string param = "")
        {
            return T(string.Format("{0} is required. ", param));
        }

        /// <summary>
        /// Could not save record
        /// </summary>
        public static LocalizedString couldnotsaverecord()
        {
            return T(string.Format("Could not save record."));
        }

        /// <summary>
        /// {0} is required.
        /// </summary>
        public static LocalizedString fieldrequired(string param = "")
        {
            return T(string.Format("{0} is required. ", param));
        }

        /// <summary>
        /// Revenue head for paye cannot be changed. Please contact Parkway admin.
        /// </summary>
        public static LocalizedString onlyonepayeecanexists()
        {
            return T(string.Format("Revenue head for paye cannot be changed.Please contact Parkway admin. "));
        }

        /// <summary>
        /// Already has form controls, try editing.
        /// </summary>
        /// <param name="parameters"></param>
        public static LocalizedString hasformcontrols(params object[] parameters)
        {
            return T(string.Format("Already has form controls, try editing. ", parameters));
        }


        /// <summary>
        /// Part payments not allowed for this invoice. Please pay the invoice amount in full.
        /// </summary>
        /// <param name="parameters"></param>
        public static LocalizedString nopartpaymentsallow()
        {
            return T("Part payments not allowed for this invoice. Please pay the invoice amount in full");
        }


        /// <summary>
        /// Error deserializing pay direct request stream
        /// </summary>
        public static LocalizedString errordeserializingpaydirectrequest()
        {
            return T(string.Format("Error deserializing pay direct request stream"));
        }

        /// <summary>
        /// Bad request
        /// </summary>
        public static LocalizedString badrequest()
        {
            return T(string.Format("Bad request"));
        }


        /// <summary>
        /// No service URL found
        /// </summary>
        public static LocalizedString noserviceurl404()
        {
            return T(string.Format("No service URL found"));
        }

        /// <summary>
        /// Billing cannot be created for revenue head, because it has sub revenue heads.
        /// </summary>
        public static LocalizedString revenueheadhassubrevenueheads(params object[] parameters)
        {
            return T(string.Format("Billing cannot be created for revenue head, because it has sub revenue heads."));
        }

        /// <summary>
        /// Date/time value is in the wrong format. Expected format is dd/MM/yyyy HH:mm:ss.
        /// </summary>
        public static LocalizedString dateandtimecouldnotbeparsed(string value)
        {
            return T(string.Format("Date/time value is in the wrong format. Expected format is dd/MM/yyyy HH:mm:ss, value given {0}.", value));
        }

        /// <summary>
        /// Date/time value is in the wrong format. Expected format is dd/MM/yyyy HH:mm:ss.
        /// </summary>
        public static LocalizedString dateandtimecouldnotbeparsed()
        {
            return T(string.Format("Date/time value is in the wrong format. Expected format is dd/MM/yyyy HH:mm:ss."));
        }


        /// <summary>
        /// Error saving tax entity record.
        /// </summary>
        public static LocalizedString couldnotsavetaxentityrecord()
        {
            return T(string.Format("Error saving tax entity record."));
        }

        public static LocalizedString couldnotcbsuser()
        {
            return T(string.Format("Error saving user record."));
        }


        /// <summary>
        /// Merchant reference mismatch {0}
        /// </summary>
        public static LocalizedString merchantrefmismatch(string merchantReference)
        {
            return T(string.Format("Merchant reference mismatch {0}", merchantReference));
        }


        /// <summary>
        /// No frequncy type found.
        /// </summary>
        public static LocalizedString notfrequencytypefound(params object[] parameters)
        {
            return T(string.Format("No frequncy type found."));
        }


        /// <summary>
        /// Could not get merchant reference {0}
        /// </summary>
        public static LocalizedString couldnotgetmerchantref(string merchantReference)
        {
            return T(string.Format("Could not get merchant reference {0}.", merchantReference));
        }


        /// <summary>
        /// cannot find state with Id
        /// </summary>
        /// <returns></returns>
        public static LocalizedString cannotfindstate()
        {
            return T(string.Format("Cannot find tenant state."));
        }


        /// <summary>
        /// "No matching reference data source found."
        /// </summary>
        public static LocalizedString couldnotfindmatchingrefdata()
        {
            return T(string.Format("No matching reference data source found."));
        }

        /// <summary>
        /// No year(s) attached to the frequency.
        /// </summary>
        public static LocalizedString noyearsinfrequencymodel(params object[] parameters)
        {
            return T(string.Format("No year(s) attached to the frequency."));
        }

        /// <summary>
        /// No category found.
        /// </summary>
        public static LocalizedString categorynotfound()
        {
            return T(string.Format("No category found."));
        }

        public static LocalizedString nobillingtypefound(params object[] parameters)
        {
            return T(string.Format("No billing type specified/or found."));
        }

        /// <summary>
        /// No demand notice type specified/or found.
        /// </summary>
        /// <returns>LocalizedString</returns>
        public static LocalizedString demandnoticetype()
        {
            return T(string.Format("No demand notice type specified/or found."));
        }

        public static LocalizedString errorreadingpayefile()
        {
            return T(string.Format("Error reading paye file upload."));
        }

        public static LocalizedString errorreadingosgofschedulefile()
        {
            return T(string.Format("Error reading cell site schedule. Please use the approriate header values."));
        }

        /// <summary>
        /// Demand notice period interval must be greater than 0.
        /// </summary>
        /// <param name="parameters"></param>
        public static LocalizedString demandnoticefrom(params object[] parameters)
        {
            return T(string.Format("Demand notice period interval must be greater than 0."));
        }


        /// <summary>
        /// Paye assessment is still processing. Please wait..
        /// </summary>
        public static LocalizedString payeassessmentstillprocessing()
        {
            return T(string.Format("Paye assessment is still processing. Please wait.."));
        }


        /// <summary>
        /// No Paye found
        /// </summary>
        public static LocalizedString payees404()
        {
            return T(string.Format("No Paye found"));
        }

        /// <summary>
        /// Error processing paye assessment. Please contact Parkway admin.
        /// </summary>
        public static LocalizedString errorprocessingfile()
        {
            return T(string.Format("Error processing paye assessment. Please contact Parkway admin."));
        }


        /// <summary>
        /// Cannot save setting. Please contact Parkway admin.
        /// </summary>
        public static LocalizedString cannotsavetenantsettingsinfo()
        {
            return T(string.Format("Cannot save setting. Please contact Parkway admin."));
        }

        /// <summary>
        /// Cannot update setting. Please contact Parkway admin.
        /// </summary>
        public static LocalizedString cannotupdatetenantsettingsinfo()
        {
            return T(string.Format("Cannot update setting. Please contact Parkway admin."));
        }


        /// <summary>
        /// No day(s) attached to the frequency.
        /// </summary>
        public static LocalizedString daysintervalinfrequencymodelistoosmall(params object[] parameters)
        {
            return T(string.Format("Days interval is too small, must be greater than 0."));
        }


        /// <summary>
        /// Amount generated from schedule is too small.
        /// </summary>
        public static LocalizedString scheduleamountistoosmall()
        {
            return T(string.Format("Amount generated from schedule is too small."));
        }

        /// <summary>
        /// No days found
        /// </summary>
        public static LocalizedString nodaysinfrequencymodel()
        {
            return T(string.Format("No days found"));
        }

        /// <summary>
        /// No institution Id found {0}
        /// </summary>
        /// <param name="institutionId"></param>
        public static LocalizedString institutionId404(string institutionId)
        {
            return T(string.Format("No institution Id found {0}", institutionId));
        }

        /// <summary>
        /// Payment reference and invoice mismatch
        /// </summary>
        public static LocalizedString datamismatch()
        {
            return T(string.Format("Payment reference and invoice mismatch"));
        }

        /// <summary>
        /// Due date value given must be greater than 0 or check the Due on next bill Date if bill is recurring.
        /// </summary>
        public static LocalizedString invalidduedate(params object[] parameters)
        {
            return T(string.Format("Due date value given must be greater than 0 or check the Due on next bill Date if bill is recurring."));
        }


        /// <summary>
        /// Invoice not found
        /// </summary>
        public static LocalizedString invoice404(string invoiceNumber = null)
        {
            if (string.IsNullOrEmpty(invoiceNumber))
            {
                return T(string.Format("Invoice not found"));
            }
            else
            {
                return T(string.Format("Invoice {0} not found", invoiceNumber));
            }
        }

        /// <summary>
        /// Payment Reference not found
        /// </summary>
        public static LocalizedString paymentreference404(string paymentReferenceNumber = null)
        {
            if (string.IsNullOrEmpty(paymentReferenceNumber))
            {
                return T(string.Format("Payment Reference not found"));
            }
            else
            {
                return T(string.Format("Payment Reference {0} not found", paymentReferenceNumber));
            }
        }


        /// <summary>
        /// No billing info found
        /// </summary>
        public static LocalizedString billinginfo404()
        {
            return T(string.Format("No billing info found"));
        }

        public static LocalizedString startdateisgreaterthanenddate(string message)
        {
            return T(message);
        }

        /// <summary>
        /// A paye revenue head already exists.
        /// </summary>
        public static LocalizedString payealreadyexists()
        {
            return T(string.Format("A paye revenue head already exists."));
        }

        /// <summary>
        /// The amount paid is a mismatch to the existing payment log record
        /// </summary>
        public static LocalizedString amountmismatchforexistingpaymentlogid()
        {
            return T("The amount paid is a mismatch to the existing payment log record");
        }


        /// <summary>
        /// Could not find bank code
        /// </summary>
        public static LocalizedString bankCode404(string bankCode)
        {
            return T(string.Format("Could not find bank code {0}.", bankCode));
        }

        /// <summary>
        /// The amount paid is a mismatch to the existing payment ref record. Supplied amount: {0}, Stored amount: {1}
        /// </summary>
        public static LocalizedString amountmismatchforexistingpaymentref(string amount, string existingAmount)
        {
            return T(string.Format("The amount paid is a mismatch to the existing payment ref record. Supplied amount: {0}, Stored amount: {1}", amount, existingAmount));
        }

        /// <summary>
        /// Due date type could not be found or check the Due on next bill Date if bill is recurring.
        /// </summary>
        public static LocalizedString duedatetype404(params object[] parameters)
        {
            return T(string.Format("Due date type could not be found or check the Due on next bill Date if bill is recurring."));
        }


        /// <summary>
        /// There was a problem with the provided link.
        /// </summary>
        public static LocalizedString errorinexternalinvoicegeneration(string url)
        {
            return T(string.Format("There was a problem with the provided link {0}.", url));
        }

        /// <summary>
        /// The receipt number is a mismatch to the existing payment record. Supplied receipt number: {0}, Stored receipt number: {1}
        /// </summary>
        /// <param name="receiptNo"></param>
        /// <param name="thirdPartyReceiptNumber"></param>
        public static LocalizedString receiptmismatchforexistingpaymentid(string receiptNo, string thirdPartyReceiptNumber)
        {
            return T(string.Format("The receipt number is a mismatch to the existing payment record. Supplied receipt number: {0}, Stored receipt number: {1}", receiptNo, thirdPartyReceiptNumber));
        }


        /// <summary>
        /// There was a problem redirecting to the link <b>{0}</b> for invoice generation. <br />It could be the link is broken, the site is down or your session for this request has ended. Please try again from the invoice generation page.<br /> Hit the button below to get started.
        /// </summary>
        public static LocalizedString externalredirect404(string url)
        {
            return T(string.Format("There was a problem redirecting to the link <b>{0}</b> for invoice generation. <br />It could be the link is broken, the site is down or your session for this request has ended. Please try again from the invoice generation page.<br /> Hit the button below to get started.", url));
        }


        /// <summary>
        /// We encountered an error redirecting you to the third party website for invoice generation. Please contact Parkway admin.
        /// </summary>
        public static LocalizedString errorredireting()
        {
            return T(string.Format("We encountered an error redirecting you to the third party website for invoice generation. Please contact Parkway admin."));
        }

        /// <summary>
        /// The payment ref value is a mismatch to the existing payment log id record
        /// </summary>
        /// <param name="refVal"></param>
        /// <param name="existingRefVal"></param>
        public static LocalizedString paymentrefmismatchforexistingpaymentid()
        {
            return T("The payment ref value is a mismatch to the existing payment log id record");
        }


        /// <summary>
        /// The payment lod Id value is a mismatch to the existing payment log id record. Supplied ref: {0}, Stored ref: {1}
        /// </summary>
        /// <param name="refVal"></param>
        /// <param name="existingRefVal"></param>
        public static LocalizedString paymentlogidmismatchforexistingpaymentref(string refVal, string existingRefVal)
        {
            return T(string.Format("The payment ref value is a mismatch to the existing payment ref record. Supplied ref: {0}, Stored ref: {1}", refVal, existingRefVal));
        }

        public static LocalizedString penalty404(params object[] parameters)
        {
            return T(string.Format("Penalty type could not be found."));
        }

        public static LocalizedString billingdurationexception(string message)
        {
            return T(message);
        }
        

        /// <summary>
        /// Add a valid number.
        /// </summary>
        public static LocalizedString penaltybadvalue(params object[] parameters)
        {
            return T(string.Format("Add a valid number."));
        }


        /// <summary>
        /// No week(s) attached to the frequency.
        /// </summary>
        /// <returns></returns>
        public static LocalizedString noweeksinfrequencymodel()
        {
            return T(string.Format("No week(s) attached to the frequency."));
        }        


        /// <summary>
        /// A valid adapter value is required for file uploads.
        /// </summary>
        public static LocalizedString forfileuploadsavalidadapterisrequired()
        {
            return T(string.Format("A valid adapter value is required for file uploads."));
        }

        /// <summary>
        /// To proceed you are required to sign in.
        /// </summary>
        public static LocalizedString requiressignin()
        {
            return T(string.Format("To proceed you are required to sign in."));
        }


        /// <summary>
        /// Tenant identifier has not been set.
        /// </summary>
        public static LocalizedString tenantidentifierhasnotbeenset()
        {
            return T(string.Format("Tenant settings has not been set. Please contact Parkway admin."));
        }

        /// <summary>
        /// No month(s) attached to the frequency.
        /// </summary>
        /// <returns></returns>
        public static LocalizedString nomonthsinfrequencymodel()
        {
            return T(string.Format("No month(s) attached to the frequency."));
        }


        /// <summary>
        /// Payment Notification request for this payment ref {0} has already been received and processed
        /// </summary>
        /// <param name="paymentReference"></param>
        public static LocalizedString paymentrefalreadyprocessed(string paymentReference)
        {
            return T(string.Format("Payment Notification request for this payment ref {0} has already been received and processed", paymentReference));
        }

        /// <summary>
        /// The parent revenue head cannot have sub-revenue heads, because it already has been set up as a billable revenue head
        /// </summary>
        public static LocalizedString revenueheadcannothaveasubrevenuehead()
        {
            return T(string.Format("The parent revenue head cannot have sub-revenue heads, because it already has been set up as a billable revenue head"));
        }

        public static LocalizedString mdasmekycouldnotbefound()
        {
            return T(string.Format("MDA company key code could not be found"));
        }

        /// <summary>
        /// This revenue head already has billing info. You can always edit this info.
        /// </summary>
        public static LocalizedString alreadyhasbillinginfo()
        {
            return T(string.Format("This revenue head already has billing info. You can always edit this info."));
        }

        /// <summary>
        /// Sorry you are not authorized to perform that action. Please contact Parkway admin.
        /// </summary>
        public static LocalizedString usernotauthorized(params object[] parameters)
        {
            return T(string.Format("Sorry you are not authorized to perform that action. Please contact Parkway admin."));
        }

        /// <summary>
        /// Sorry this revenue head has sub-revenueheads, therefore billing cannot be created for it. Please contact Parkway admin.
        /// </summary>
        public static LocalizedString hassubrevenueheads(params object[] parameters)
        {
            return T(string.Format("Sorry this revenue head {0} has sub-revenueheads, therefore billing cannot be created for it. Please contact Parkway admin.", parameters[0]));
        }

        /// <summary>
        /// Payment Log Id is not valid
        /// </summary>
        public static LocalizedString paymentlogidisnotvalid()
        {
            return T(string.Format("Payment Log Id is not valid"));
        }


        /// <summary>
        /// Transaction amount {0} is not negative.
        /// </summary>
        public static LocalizedString transactionreversalisnotnegative(string amountValue)
        {
            return T(string.Format("Transaction amount {0} is not negative.", amountValue));
        }

        /// <summary>
        /// Could not find user
        /// </summary>
        /// <param name="param"></param>
        public static LocalizedString usernotfound(string param = "")
        {
            return T(string.Format("Could not find user"));
        }

        /// <summary>
        /// No Revenue Head found
        /// </summary>
        /// <param name="param"></param>
        public static LocalizedString revenueheadnotfound(string param = "")
        {
            return T(string.Format("No Revenue Head found"));
        }

        /// <summary>
        /// Specified amount is too small
        /// </summary>
        internal static LocalizedString amountistoosmall()
        {
            return T(string.Format("Specified amount is too small"));
        }

        /// <summary>
        /// Another Revenue Head under this MDA already has this name value
        /// </summary>
        /// <param name="param"></param>
        public static LocalizedString revenueheadmdanotunique(string param = "")
        {
            return T(string.Format("Another Revenue Head under this MDA already has this name value"));
        }

        /// <summary>
        /// Another Revenue Head under this MDA already has this code value
        /// </summary>
        /// <param name="param"></param>
        public static LocalizedString revenueheadcodenotunique(string param = "")
        {
            return T(string.Format("Another Revenue Head under this MDA already has this code value"));
        }

        /// <summary>
        /// We could not find MDA record
        /// </summary>
        /// <param name="param"></param>
        public static LocalizedString mdacouldnotbefound(string param = "")
        {
            return T(string.Format("We could not find that MDA record"));
        }

        /// <summary>
        /// We could not find RevenueHead record
        /// </summary>
        public static LocalizedString revenuehead404(string param = "")
        {
            return T(string.Format("We could not find Revenue Head record"));
        }

        /// <summary>
        /// Cannot connect to invoicing service at the moment. Please contact Parkway admin
        /// </summary>
        public static LocalizedString cannotconnettoinvoicingservice(string param = "")
        {
            return T(string.Format("Cannot connect to invoicing service at the moment. Please contact Parkway admin"));
        }

        /// <summary>
        /// Error occured while saving MDA
        /// </summary>
        public static LocalizedString couldnotsavemdarecord(string param = "")
        {
            return T(string.Format("Error occured while saving MDA"));
        }

        public static LocalizedString modulenotenabled(params object[] parameters)
        {
            return T(string.Format("The requested module has not been enabled for your system. Please contact Parkway admin"));
        }

        /// <summary>
        /// Sorry something went wrong while processing your request. Please try again later or contact admin
        /// </summary>
        public static LocalizedString genericexception(string param = "")
        {
            return T(string.Format("Sorry something went wrong while processing your request. Please try again later or contact admin.", param));
        }


        /// <summary>
        /// Problem processing your upload file. Please try again later or contact admin.
        /// </summary>
        public static LocalizedString couldnotsavedirectassessmentbatchrecord()
        {
            return T(string.Format("Problem processing your upload file. Please try again later or contact admin."));
        }

        /// <summary>
        /// Revenue Head(s) could not be saved. Please try again later or contact Parkway admin.
        /// </summary>
        /// <param name="parameters"></param>
        public static LocalizedString revenueheadscouldnotbesaved(params object[] parameters)
        {
            return T(string.Format("Revenue Head(s) could not be saved. Please try again later or contact Parkway admin.", parameters));
        }


        /// <summary>
        /// 9999
        /// </summary>
        public static LocalizedString bankcollecterrorcode()
        {
            return T(string.Format("9999"));
        }

        /// <summary>
        /// 9999
        /// </summary>
        public static LocalizedString netpayerrorcode()
        {
            return T(string.Format("9999"));
        }


        /// <summary>
        /// Invalid input type
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>LocalizedString</returns>
        public static LocalizedString invalidinputtype(params object[] parameters)
        {
            return T(string.Format("Invalid input type {0}", parameters));
        }

        /// <summary>
        /// MDA setting for mda - {0}:{1} could not be found
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static LocalizedString mdasetting404(params object[] parameters)
        {
            return T(string.Format("MDA setting for mda - {0}:{1} could not be found", parameters));
        }

        /// <summary>
        /// MDA bank details for mda - {0}:{1} is null
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static LocalizedString bankdetails404(params object[] parameters)
        {
            return T(string.Format("MDA bank details for mda - {0}:{1} is null", parameters));
        }

        /// <summary>
        /// not ok
        /// </summary>
        public static LocalizedString remitavalidationerrormsg()
        {
            return T(string.Format("not ok"));
        }

        /// <summary>
        /// Tenant info could not be found, please contact Parkway admin
        /// </summary>
        /// <returns></returns>
        public static LocalizedString tenant404(params object[] parameters)
        {
            return T(string.Format("Tenant info has not been set up, please contact Parkway admin.", parameters));
        }


        /// <summary>
        /// Could not save billing information
        /// </summary>
        /// <param name="parameters"></param>
        public static LocalizedString cannotsavebillinginfo(params object[] parameters)
        {
            return T(string.Format("Could not save billing information"));
        }


        /// <summary>
        /// Only recurring bills can have their next bill date as the due date.
        /// </summary>
        public static LocalizedString nextbilldateforduedateisonlyvalidforrecurringbills()
        {
            return T(string.Format("Only recurring bills can have their next bill date as the due date."));
        }


        /// <summary>
        /// Billing has ended.
        /// </summary>
        public static LocalizedString billinghasended()
        {
            return T(string.Format("Billing has ended."));
        }


        /// <summary>
        /// Phone number is already being used by another user.
        /// </summary>
        public static LocalizedString phonenumberalreadyexists()
        {
            return T(string.Format("Phone number is already being used by another user."));
        }


        /// <summary>
        /// Your session could not be continued. Please fill in your details and proceed to start new session.
        /// </summary>
        public static LocalizedString sessionended()
        {
            return T(string.Format("Your session could not be continued. Please fill in your details and proceed to start new session."));
        }

        /// <summary>
        /// An already existing tax profile was found for the provided TIN. This tax profile requires you to signin.
        /// </summary>
        /// <returns></returns>
        public static LocalizedString retrievedcategoryrequiressignin(string TIN)
        {
            return T(string.Format("An already existing tax profile was found for the provided TIN {0}. This tax profile requires you to signin.", TIN));
        }


        /// <summary>
        /// A tax profile for a Payer ID {0} has been created for you but the provided category {1} for this tax profile requires you to signin.
        /// </summary>
        /// <param name="payerId"></param>
        /// <param name="categoryName"></param>
        public static LocalizedString createdtaxentityrequiredlogin(string payerId, string categoryName)
        {
            return T(string.Format("A tax profile for a Payer ID {0} has been created for you but the provided category {1} for this tax profile requires you to signin.", payerId, categoryName));
        }

        /// <summary>
        /// No tax payer with the Payer ID {0} was found.
        /// </summary>
        /// <param name="payerId"></param>
        public static LocalizedString notaxpayerrecord404(string payerId)
        {
            return T(string.Format("No tax payer with the Payer ID {0} was found.", payerId));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="LGAId"></param>
        /// <returns></returns>
        public static LocalizedString nolgarecord404(string LGAId)
        {
            return T(string.Format("No LGA with the LGA ID {0} was found.", LGAId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="LGAId"></param>
        /// <returns></returns>
        public static LocalizedString nocellsiterecord404(string cellSiteId)
        {
            return T(string.Format("No cell site with the ID {0} was found.", cellSiteId));
        }

        /// <summary>
        /// Receipt not found
        /// </summary>
        public static LocalizedString receipt404(string receiptNumber = null)
        {
            if (string.IsNullOrEmpty(receiptNumber))
            {
                return T(string.Format("Receipt not found"));
            }
            else
            {
                return T(string.Format("Receipt {0} not found", receiptNumber));
            }
        }


        /// <summary>
        /// No product information was found.
        /// </summary>
        public static LocalizedString productinfo404()
        {
            return T(string.Format("No product information was found."));
        }


        /// <summary>
        /// The maximum report selection months exceeded.
        /// </summary>
        public static LocalizedString maximumreportmonthsrexceeded()
        {
            return T(string.Format("Please specify maximum of 6 months date range."));
        }

        /// <summary>
        /// Specified state does not exist
        /// </summary>
        /// <returns></returns>
        public static LocalizedString statenotfound() {

            return T(string.Format("State not found"));
        }


        /// <summary>
        /// No settlement code found
        /// </summary>
        public static LocalizedString nosettlementCode()
        {
            return T(string.Format("No settlement code found"));
        }

        /// <summary>
        /// Unable to verify the status of netpay transaction for a specific reference number
        /// </summary>
        /// <param name="referenceNUmber"></param>
        /// <returns></returns>
        public static LocalizedString unabletoverifynetpayreference(string referenceNUmber)
        {
            return T(string.Format($"Unable to verify payment status for {referenceNUmber} at the moment, please contact support."));
        }

        /// <summary>
        /// Reference number not found
        /// </summary>
        /// <param name="referenceNUmber"></param>
        /// <returns></returns>
        public static LocalizedString netpayreferencenumber404(string referenceNUmber)
        {
            return T(string.Format($"No record is found for Reference Number {referenceNUmber}"));
        }

        /// <summary>
        /// No callback URL found.
        /// </summary>
        public static LocalizedString callbackurl404()
        {
            return T(string.Format("No callback URL found."));
        }

        public static LocalizedString surchargeAmtNotValid() {
            return T(string.Format("Surcharge amount is not valid"));
        }

        /// <summary>
        /// Tenant info could not be found, please contact Parkway admin
        /// </summary>
        /// <returns></returns>
        public static LocalizedString invoicehaspayment(string invoiceNumber)
        {
            return T($"Invoice number {invoiceNumber} has been fully paid.");
        }

        /// <summary>
        /// Invoice already fully paid
        /// </summary>
        /// <returns></returns>
        public static LocalizedString invoiceFullyPaid(string invoiceNumber)
        {
            return T($"Invoice number {invoiceNumber} has been fully paid.");
        }

        /// <summary>
        /// Invoice already fully paid
        /// </summary>
        /// <returns></returns>
        public static LocalizedString invoiceFullyPaid()
        {
            return T($"Invoice has been fully paid.");
        }

        /// <summary>
        /// Receipt not found
        /// </summary>
        public static LocalizedString stateTIN404(string stateTIN = null)
        {
            if (string.IsNullOrEmpty(stateTIN))
            {
                return T(string.Format("State TIN not found"));
            }
            else
            {
                return T(string.Format("State TIN {0} not found", stateTIN));
            }
        }

        /// <summary>
        /// Unable to get utilized receipts for schedule with batch ref {batchRef}
        /// </summary>
        public static LocalizedString utilizedreceiptsnotfound(string batchRef = "")
        {
            return T(string.Format("Unable to get utilized receipts for schedule with batch ref {0}.",batchRef));
        }

        /// <summary>
        /// Existing batch Identifier with page number already exist.
        /// </summary>
        public static LocalizedString batchidentifierandpageexist(int pageNumber)
        {
            return T(string.Format($"Batch Identifier with page number: {pageNumber} already exist."));
        }

        /// <summary>
        /// Batch limit exceeded.
        /// </summary>
        public static LocalizedString batchlimitexceeded(int batchLimit)
        {
            return T(string.Format($"Batch limit exceeded. Limit: {batchLimit}"));
        }

        /// <summary>
        /// Could not save PAYE Items.
        /// </summary>
        /// <returns></returns>
        public static LocalizedString couldnotsavepayeitem()
        {
            return T(string.Format("Could not save PAYE Items."));
        }

        /// <summary>
        /// Specified lga does not exist
        /// </summary>
        /// <returns></returns>
        public static LocalizedString lganotfound()
        {

            return T(string.Format("Invalid LGA entry"));
        }


        /// <summary>
        /// Model is empty
        /// </summary>
        public static LocalizedString modelisempty()
        {
            return T(string.Format($"Model is empty"));
        }

        /// <summary>
        /// Biller code is required
        /// </summary>
        public static LocalizedString billercode(string message = "")
        {
            if (string.IsNullOrEmpty(message))
            {
                return T(string.Format("Biller code is required"));
            }
            else
            {
                return T(string.Format("{0}", message));
            }

        }



        /// <summary>
        /// This field value must be between {0} and {1} years
        /// </summary>
        /// <param name="minYear"></param>
        /// <param name="maxYear"></param>
        /// <returns></returns>
        public static LocalizedString yearrangevalidation(int minYear, int maxYear)
        {
            return T(string.Format("This field value must be between {0} and {1}", minYear, maxYear));
        }

        public static LocalizedString requesthasnotyetbeenapproved
        {
            get { return T("Request has not yet been approved by anyone"); }
        }


        /// <summary>
        /// 0094
        /// </summary>
        public static LocalizedString readycasherrorcodeforalreadypaidforinvoice
        {
            get { return T(string.Format("0094")); }
        }

        public static LocalizedString emailnotificationnotenable
        {
            get { return T("Unable to send email, Please contact Parkway admin"); }
        }

    }
}
