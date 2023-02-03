using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Core.Models.Enums
{
    public enum CachePrefix
    {
        States,
        Countries,
        AllowPartPayment,
        NibssCredential
    }
    public enum FlashType
    {
        Warning,
        Success,
        Error,
        Info
    }

    public enum IPPISProcessingStages
    {
        NotProcessed = 0,
        FileValidationProcessed = 1,
        CategorizationOfTaxPayerByCode = 2,
        MapTaxPayerCodeToTaxProfile = 3,
        MapUnknownTaxPayerCodeToUnknownTaxProfile = 4,
        GenerateInvoices = 5,
        CreateDirectAssessments = 6,
        CreateDirectAssessmentRecords = 7,
        CreateInvoices = 8,
        CreateTransactionLogs = 9,
        ConfirmDirectAssessmentInvoices = 10,
        Processed = 11,
        MovedCSVToSummaryPath = 12,
        ConvertExcelToCSV = 13,
    }

    public enum ReferenceDataProcessingStages
    {
        [Description("Not Processed")]
        NotProcessed = 0,
        [Description("File Saved")]
        FileSaved = 1,
        [Description("Records Moved to Tax Entity Staging")]
        RecordsMovedToTaxEntityStaging = 2,
        [Description("Records Matched to Type of Tax Paid")]
        MatchedRecordsToTypeOfTaxPaid = 3,
        [Description("Update TaxEntityStaging OperationType")]
        UpdateTaxEntityStagingOperationType = 4,
        [Description("Tax Payer Created")]
        TaxPayerCreated = 5,
        [Description("Existing Tax Payer Records Updated")]
        UpdateExistingTaxPayerRecords = 6,
        [Description("Tax Entity Staging Entity Id Updated")]
        UpdateReferenceDataTaxEntityStaging = 7,
        [Description("Records Moved to Withholding Tax on Rent Table")]
        MoveTaxEntityStagingRecordsToWithHoldingTaxOnRent = 8,
        [Description("Records Moved to Invoice Staging Table")]
        MoveWithHoldingTaxOnRentToInvoiceStaging = 9,
        [Description("Sent Bulk Invoice request to CashFlow")]
        SentBulkInvoiceRequestToCashFlow = 10,
        [Description("Save Cashflow Batch Invoice response")]
        SavedCashflowBatchInvoiceResponse = 11,
        [Description("Update Invoice staging with CashFlow response")]
        UpdateInvoiceStagingWithCashFlowResponse = 12,
        [Description("Move Invoice staging records to main Invoice table")]
        MoveInvoiceStagingRecordsToInvoiceTable = 13,
        [Description("Move relevants columns in Invoice staging records to Transaction Logs table")]
        MoveInvoiceStagingRecordsToTransactionsLogTable = 14,
        [Description("Update the Tax Entity table with CashFlow customer details")]
        UpdateTaxEntityWithCashFlowCustomerDetails = 15,
        [Description("Completed")]
        Completed = 16
    }

    public enum NagisDataProcessingStages
    {
        [Description("Not Processed")]
        NotProcessed = 0,
        [Description("File Saved")]
        FileSaved = 1,
        [Description("Update TaxEntityStaging OperationType")]
        UpdateNagisOldInvoicesStagingOperationType = 2,
        [Description("Tax Payer Created")]
        TaxPayerCreated = 3,
        [Description("Existing Tax Payer Records Updated")]
        UpdateExistingTaxPayerRecords = 4,
        [Description("NAGIS Old Invoices Staging Entity Id Updated")]
        UpdateNagisDataTaxEntityStaging = 5,
        [Description("Categorize By NAGIS Invoice Number")]
        CategorizeByNAGISInvoiceNumber = 6,
        [Description("NAGIS Old Invoices Staging Invoice Summary Id Updated")]
        UpdateNagisDataInvoiceSummaryStaging = 7,
        [Description("Create Customer on Cashflow")]
        CreateCustomerOnCashflow = 8,
        [Description("Sent Bulk Invoice request to CashFlow")]
        SentBulkInvoiceRequestToCashFlow = 9,
        [Description("Save Cashflow Batch Invoice response")]
        SavedCashflowBatchInvoiceResponse = 10,
        [Description("Update Invoice staging with CashFlow response")]
        UpdateInvoiceStagingWithCashFlowResponse = 11,
        [Description("Move Invoice staging records to main Invoice table")]
        MoveInvoiceStagingRecordsToInvoiceTable = 12,
        [Description("Move Invoice staging records to main Invoice table")]
        MoveInvoiceItemsRecordsToInvoiceItemsTable = 13,
        [Description("Move relevants columns in Invoice staging records to Transaction Logs table")]
        MoveInvoiceStagingRecordsToTransactionsLogTable = 14,
        [Description("Update the Tax Entity table with CashFlow customer details")]
        UpdateTaxEntityWithCashFlowCustomerDetails = 15,
        [Description("Completed")]
        Completed = 16
    }


    public enum ReferenceDataOperationType
    {
        Create = 1,
        Update = 2
    }

    public enum AssetType
    {
        Land = 1,
        Building = 2,
        Vehicle = 3,
        Business = 4
    }



    public enum UserIsLoggedInResult
    {
        AlreadyLoggedIn = 0,
        RequiresLogin = 1,
        DoesNotRequireLogin = 2,
    }

    /// <summary>
    /// specify the channels that should get payment notifications
    /// </summary>
    public enum PaymentNotificationChannel
    {
        [Description("None")]
        None = 0,
        [Description("Cashflow")]
        Cashflow = 1,
        [Description("Eregistry")]
        Eregistry = 2,
        [Description("ThirdParty")]
        ThirdParty = 3,
    }


    /// <summary>
    /// describes where web paymenet request is coming from
    /// </summary>
    public enum WebPaymentRequestSource
    {
        [Description("All")]
        None = 0,
        [Description("Eregistry")]
        Eregistry = 1,
    }

    public enum AppSettingEnum
    {
        None = 0,
        PaymentNotificationResponseCode = 1,
        UnknownTaxPayerCodeId = 2,
        ChunkSizeForIPPISInvoiceGeneration = 3,
        DBChunkSize = 4,
        NumberOfRetriesForMovingSummaryFileProcess = 5,
        RetryWaitTimeForSummaryFileMovement = 6,
        ChunkSizeForBatchInvoiceGenerationResponse = 7,
        IsLive = 8,
        ReadyCashExchangeKey = 9,
        NetPaySuccessCode = 10,
        NetPayURL = 11,
        HashKey = 12,
        VerificationResendCodeLimit = 13,
        VerificationCodeExpiryInMinutes = 14,
        EmailProvider = 15,
        SMSProvider = 16,
        PayeApiBatchlimit = 17,
        HangfireConnectionStringName = 18,
        OK = 19,
        InternalError = 20,
        BVNValidationEndpoint = 21,
        PayeExcelSaveChunkSize = 22,
        PayeExcelFileSize = 23,
        TINValidationEndpoint = 24,
        TINValidationUsername = 25,
        TINValidationPassword = 26,
        NINValidationUsername = 27,
        NINValidationPassword = 28,
        NINValidationOrgid = 29,
        NINValidationExponent = 30,
        NINValidationModulus = 31,
        NINValidationRESTEndpoint = 32,
        NINValidationRESTEndpointKey = 33,
        RetrieveEmailVerificationResendCodeLimit = 34,
        NINPCCRevalidationIntervalDays = 35,
        AccountWalletPaymentVerificationResendCodeLimit = 36,
        FourCoreBVNValidationEndpoint = 37,
        FourCoreBVNValidationUsername = 38,
        FourCoreBVNValidationSecret = 39
    }

    /// <summary>
    /// Config keys for state settings
    /// </summary>
    public enum TenantConfigKeys
    {
        None = 0,
        [Description("SiteNameOnFile")]
        SiteNameOnFile = 1,
        [Description("PaymentNotificationResponseCode")]
        PaymentNotificationResponseCode = 2,
        [Description("IsTINUnique")]
        IsTINUnique = 3,
        [Description("UnreconciledRevenueHeadId")]
        UnreconciledRevenueHeadId = 4,
        [Description("LogoPath")]
        LogoPath = 5,
        [Description("/images/")]
        ThemeImages,
        [Description("longstrip.svg")]
        ReceiptThemeLongStrip,
        [Description("shortstrip.svg")]
        ReceiptThemeShortStrip,
        [Description("logo.png")]
        TenantThemeLogo,

        [Description("payeReceiptPlainBg.png")]
        PAYEReceiptWatermark,

        /// <summary>
        /// infogrid exchange key
        /// </summary>
        InfoGridExchangeValue,
        /// <summary>
        /// key for readycash interacts
        /// </summary>
        ReadycashExchangeKeyValue,
        /// <summary>
        /// Cashflow vendor code
        /// </summary>
        CashflowVendorCode,
        GatewayFee,
        HRSystemBaseURL,
        HRSystemUsername,
        HRSystemKey,
        NetPayMerchantKey,
        NetPayMode,
        NetPayCurrencyCode,
        NetPayColorCode,
        NetPayMerchantSecretId,
        MerchantSite,
        BaseURL,
        ReceiptPrefix,
        CashflowCompanyCode,
        MerchantCallBackURL,
        RequestFeeAPICallBack,
        TCCFilePath,
        AuthorizedApprovalNumbers,
        IsEmailEnabled,
        IsSMSEnabled,
        AdminBaseURL,
        ClientBaseURL,
        StateDisplayName,
        IsEnabledFormControlActionFilter,
        DevelopmentLevyRevenueHeadID,
        PulseSMSTemplateName,

        [Description("TccPlainBg.png")]
        TCCCertificateBG,

        [Description("receipt_watermark.png")]
        ReceiptWatermark,
        PhoneNumberTrimIndex,
        SendPaymentNotification,
    }


    public enum EmailTemplateFileNames
    {
        [Description("CBS <no-reply@cbs.ng>")]
        Sender,
        [Description("CBS.InvoiceGeneration")]
        InvoiceGeneration,
        [Description("CBS.UserRegistration")]
        UserRegistration,
        [Description("CBS.AccountVerification")]
        AccountVerification,
        [Description("CBS.ExtractApprovalNotification")]
        ExtractApprovalNotification,
        [Description("CBS.ExtractRejectionNotification")]
        ExtractRejectionNotification,
        [Description("CBS.EscortApprovalNotification")]
        EscortApprovalNotification,
        [Description("CBS.EscortRejectionNotification")]
        EscortRejectionNotification,
        [Description("CBS.GenericApprovalNotification")]
        GenericApprovalNotification,
        [Description("CBS.GenericRejectionNotification")]
        GenericRejectionNotification,
        [Description("CBS.InvestigationReportApprovalNotification")]
        InvestigationReportApprovalNotification,
        [Description("CBS.InvestigationReportRejectionNotification")]
        InvestigationReportRejectionNotification,
        [Description("CBS.CharacterCertificateApprovalNotification")]
        CharacterCertificateApprovalNotification,
        [Description("CBS.CharacterCertificateRejectionNotification")]
        CharacterCertificateRejectionNotification,
        [Description("CBS.NibssSecretKeyAndIVNotification")]
        NibssSecretKeyAndIVNotification,

    }

    public enum PAYEAPIProcessingStages
    {
        [Description("Not Processed")]
        NotProcessed = 0,

        [Description("Batch Initialization")]
        BatchInitialized = 1,

        [Description("Batch Items Saving")]
        BatchItemsSaved = 2,

        [Description("Batch Validation")]
        BatchValidated = 3,

        [Description("Batch Confirmation")]
        BatchConfirmed = 4,

        [Description("Processing Completed")]
        Completed = 5
    }

    public enum PAYEBatchItemsProcessStages
    {
        [Description("Batch Items Saving")]
        BatchItemsSaving = 0,

        [Description("Items Validation")]
        BatchValidation = 1,

        [Description("exemption Items Validation")]
        BatchExemptionValidation = 2
    }

    public enum SettingsFileNames
    {
        None = 0,
        [Description("Banks.json")]
        Banks = 1,
        //Assessment report
        AssessmentReport = 2,
        CollectionReport = 3,
        PaymentReceipt = 4,
        TCCRequestReport = 5,
        PAYEReceiptUtilizationsReport = 6,
        DirectAssessmentReport = 7
    }


    public enum CollectionPaymentDirection
    {
        [Description("Payment Date")]
        PaymentDate = 1,
        [Description("Settlement Date")]
        TransactionDate = 2,
        [Description("Synchronization Date")]
        CreatedAtUtc = 3,
    }

    public enum FilterDate
    {
        [Description("Invoice Date")]
        InvoiceDate = 0,
        [Description("Due Date")]
        DueDate = 1,
    }


    public enum EnumExpertSystemPermissions
    {
        None = 0,
        [Description("Can process PAYE payments")]
        CanMakePayePayments = 1,
        [Description("Can generate invoice")]
        GenerateInvoice = 2
    }


    public enum PaymentProvider
    {
        [Description("All")]
        None = 0,
        [Description("Bank3D (Parkway Projects)")]
        Bank3D = 1,
        [Description("Remita (System Specs)")]
        Remita = 2,
        [Description("Pay Direct (Interswitch)")]
        PayDirect = 3,
        [Description("Ebills Pay (NIBSS)")]
        NIBSS = 4,
        [Description("Readycash Bank (Parkway Projects)")]
        Readycash = 5,
        [Description("ITEX (GTB)")]
        ITEX = 7,
        [Description("CITI-SERVE (UBA)")]
        CITISERVE = 8,
        [Description("Remita Single Product (SystemSpecs)")]
        RemitaSingleProduct = 9,
        [Description("ITEX (ZENITH)")]
        ITEXZENITH = 10,
        [Description("POS (FCMB)")]
        FCMBPOS = 11,
        [Description("POS (PAUSHAR)")]
        PAUSHAR = 12,
        [Description("TELLER POINT (COURTS)")]
        TELLERPOINT = 13
    }


    public enum PaymentMethods
    {
        [Description("All")]
        None = 0,
        [Description("Cash")]
        Cash = 1,
        [Description("Cheque")]
        Cheque = 2,
        [Description("Debit Card")]
        DebitCard = 3,
        [Description("Internal Transfer")]
        InternalTransfer = 4,
        [Description("Other Bank Cheque")]
        OtherBankCheque = 5,
        [Description("Own Bank Cheque")]
        OwnBankCheque = 6,
        [Description("Bank Transfer")]
        BankTransfer = 4,
        [Description("Other payment methods")]
        OtherPaymentMethods = 7
    }


    public enum PaymentChannel
    {
        [Description("All")]
        None = 0,
        [Description("ATM")]
        ATM = 1,
        [Description("POS")]
        POS = 2,
        [Description("Web (Internet Banking)")]
        Web = 3,
        [Description("Mobile")]
        MOB = 4,
        [Description("Kiosk")]
        Kiosk = 5,
        [Description("Voice")]
        Voice = 6,
        [Description("Bank Branch")]
        BankBranch = 7,
        [Description("Vendor/Merchant Web Portal")]
        VendorMerchantWebPortal = 8,
        [Description("Third–Party Payment Platform")]
        ThirdPartyPaymentPlatform = 9,
        [Description("Other Channels")]
        OtherChannels = 10,
        [Description("Agency Banking")]
        AgencyBanking = 11,
    }


    /// <summary>
    /// short code for onscreen PAO, PAF, PAFIPPIS
    /// </summary>
    public enum PayeAssessmentType
    {
        None,
        //[Description("base file path: /App_data/Media/DirectAssessments/")]
        [Description("PAF")]
        FileUpload,
        [Description("PAO")]
        OnScreen,
        //[Description("base file path: /App_data/Media/DirectAssessments/ProcessedFile/")]
        [Description("PAFP")]
        ProcessedFileUpload,
        //[Description("This value is for file uploads from API. Base file path: /App_data/Media/DirectAssessments/API/")]
        [Description("PAFPA")]
        FileUploadFromAPI,
        //[Description("This is the value for IPPIS file uploads")]
        [Description("PAFIPPIS")]
        FileUploadForIPPIS,

        // [Description("PFAPI")]
        FromAPI
    }

    public enum EnumerationScheduleUploadType 
    {
        None,

        [Description("ESF")]
        FileUpload,

        [Description("ESO")]
        OnScreen
    }


    public enum CallTypeEnum
    {
        MDA = 0,
        RevenueHead = 1,
        Billing = 2,
        Register = 3,
        Invoice = 4,
        InvoiceForUnknownPaye = 5,
    }

    public enum RepresentativeType
    {
        TaxAgent,
        Auditor,
        Lawyer
    }

    public enum SourceOfIncome
    {
        Employed,
        SelfEmployed,
        Owner_Partner,
        None
    }

    public enum TaxType
    {
        PersonalIncome,
        CompanyTax
    }

    public enum IdentificationType
    {
        None,
        InternationalPassport,
        NationalIDCard,
        DriversLicense,
        VoterRegistration,
        TaxIdentification,
    }

    public enum MaritalStatus
    {
        Married,
        Single,
        Divorced,
        Widowed,
        Null
    }

    public enum ScheduleInvoiceProcessingStatus
    {
        AquiredData = 0,
        ErrorInGettingJoinerWithTaxEntity = 1,
        ErrorGettingDistinctRefDataEntitesWithoutCashflowRecord = 2,
        //Processing = ,
        //Failed = 2,
        //Completed = 3,
    }

    public enum PAYEDirection
    {
        None = 0,
        PAYEWithNoSchedule = 1,
        PAYEWithSchedule = 2
    }

    /// <summary>
    /// Lists the type of billing frequency a revenue head can have.
    /// Fixed, means that the billing frequency is the same for all tax payers subscribed to the revenue head
    /// Variable, means that the billing frequency varies for each tax payers
    /// </summary>
    public enum BillingType
    {
        None = 0,
        Fixed = 1,
        Variable = 2,
        OneOff = 3,
        DirectAssessment = 4,
        SelfAssessment = 5,
        FileUpload = 6,
        FixedAmountAssessment = 7,
    }

    /// <summary>
    /// Defines the frequency for the billing
    /// </summary>
    public enum FrequencyType
    {
        None = 0,
        Daily = 1,
        Weekly = 2,
        Monthly = 3,
        Yearly = 4,
    }

    public enum DueDateAfter
    {
        None = 0,
        Days = 1,
        Weeks = 2,
        Months = 3,
        Years = 4,
    }

    public enum MDAOrder
    {
        UpdatedAtUtc,
        CreatedAtUtc,
        Name,
        Code,
    }

    public enum MDAFilter
    {
        All,
        Enabled,
        Disabled,
    }


    public enum Direction
    {
        Ascending,
        Descending,
    }


    public enum TaxPayerType
    {
        Individual = 0,
        Corporate = 1,
        General = 2,
        Dealer = 3
    }

    public enum ControlTypes
    {
        TextBox = 0,
        CheckBoxList = 1,
        DropDownList = 2,
        TextArea = 3,
        DateTime = 4,
    }

    public enum ElementTypeEnum
    {
        Text = 0,
        Number = 1,
        Password = 2,
    }

    public enum ControlTypeDropDownType
    {
        State = 0,
        Gender = 1,
        Country = 2
    }

    public enum Gender
    {
        None = 0,
        Male = 1,
        Female = 2,
    }

    [Obsolete]
    public enum InvoiceStatusList
    {
        All = 90,
        Paid = 1,
        Unpaid = 0,
        WriteOff = 3,
        Overdue = 2,
        PartPaid = 5,
    }

    public enum ReceiptStatus
    {
        All = 0,
        UnPaid = 1,
        Paid = 2,
    }


    /// <summary>
    /// This is used to describe the invoice that was generated
    /// example: for normal invoices that donot have any additional processes tied to them
    /// we will assign standard to them, if not apply the applicable type
    /// </summary>
    public enum InvoiceType
    {
        Standard = 0,
        DirectAssessment = 1,
        OSGOF = 2
    }


    /// <summary>
    /// ALWAY USE THIS ENUM VALUES
    /// </summary>
    public enum InvoiceStatus
    {
        [Description("Unpaid")]
        Unpaid = 0,
        [Description("Paid")]
        Paid = 1,
        [Description("Overdue")]
        Overdue = 2,
        [Description("Write Off")]
        WriteOff = 3,
        [Description("Demand Notice")]
        DemandNotice = 4,
        [Description("Part Paid")]
        PartPaid = 5,
        [Description("Processing On Cashflow")]
        ProcessingOnCashflow = 6,
        /// <summary>
        /// Do not use this value to set invoice status, only use for report purposes
        /// </summary>
        [Description("All")]
        All = 1959917,
    }

    public enum CompanyStatus
    {
        Pending_Activation = 0,
        Activated = 1,
        Blocked = 2,
        Suspended = 3
    }

    public enum Days
    {
        None = 0,
        SUN = 1,
        MON = 2,
        TUE = 3,
        WED = 4,
        THU = 5,
        FRI = 6,
        SAT = 7,
    }

    public enum Months
    {
        None = 0,
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12,
    }

    public enum ScheduleStatus
    {
        NotStarted = 0,
        Running = 1,
        Stopped = 2,
        HasExpired = 3,
    }

    public enum MDAOrderCriteria
    {
        MDA = 1,
        Earnings = 0,
    }

    public enum PaymentStatus
    {
        Successful,
        Pending,
        Failed,
        Declined
    }

    public enum PaymentType
    {
        All = 0,
        Bill = 1,
        Debit = 2,
        Credit = 3,
    }

    public enum ExportFormat
    {
        None = 0,
        PDF = 1,
        Excel = 2,
    }

    public enum NotificationMessageType
    {
        Email = 1,
        SMS = 2
    }

    public enum TaxEntityCategoryEnum
    {
        Individual = 1,
        Corporate = 2,
        Dealer = 3,
        FederalAgency = 4,
        StateAgency = 5
    }


    public enum AccessType
    {
        None = 0,
        InvoiceAssessmentReport = 1,
        CollectionReport = 2,
        Dashboard = 3
    }

    public enum BusinessType
    {
        SoleProprietorship = 1,
        LLC = 2
    }


    public enum VerificationState
    {
        Unused = 0,
        Used = 1
    }


    public enum SettlementType
    {
        None = 0,
        Flat = 1,
        Percentage = 2,
    }

    public enum AuditType
    {
        Edit_Billing = 1
    }

    public enum EmailProvider
    {
        [Description("Gmail")]
        Gmail = 1,
        [Description("Pulse")]
        Pulse = 2,
    }

    public enum TCCExemptionType
    {
        [Description("None")]
        None = 0,

        [Description("Not Exempted")]
        NotExempted = 1,

        [Description("Wholly Exempted")]
        WhollyExempted = 2,

        [Description("House Wife without occupation")]
        HouseWife = 3,

        [Description("Student")]
        Student = 4,
    }

    public enum TCCRequestStatus
    {
        [Description("All")]
        None = 0,
        [Description("Pending Approval")]
        PendingApproval = 1,
        [Description("Approved")]
        Approved = 2,
        [Description("Rejected")]
        Rejected = 3,
    }

    public enum TCCFileUploadType
    {
        [Description("Exemption Certificate")]
        ExemptionCertificate = 1,

        [Description("School Leaving Certificate")]
        SchoolLeavingCertificate = 2,

        [Description("Account Statement")]
        AccountStatement = 3,
    }

    public enum TCCAuthorizedSignatories
    {
        [Description("Revenue Officer")]
        RevenueOfficer = 1,

        [Description("Director of Revenue")]
        DirectorOfRevenue = 2
    }

    public enum SMSProvider
    {
        [Description("Pulse")]
        Pulse = 1,
    }

    public enum PAYEReceiptUtilizationStatus
    {
        [Description("None")]
        None = 0,

        [Description("Unutilized")]
        Unutilized = 1,

        [Description("Partly Utilized")]
        PartlyUtilized = 2,

        [Description("Fully Utilized")]
        FullyUtilized = 3
    }

    public enum TCCApprovalLevel
    {
        [Description("Awaiting First Level Approver")]
        FirstLevelApprover = 1,

        [Description("Awaiting Second Level Approver")]
        SecondLevelApprover = 2,

        [Description("Completed")]
        Completed = 3
    }

    public enum TaxPayerEnumerationProcessingStages
    {
        [Description("Not Processed")]
        NotProcessed = 0,

        [Description("Batch Initialization")]
        BatchInitialized = 1,

        [Description("Batch Items Saved")]
        BatchItemsSaved = 2,

        [Description("Batch Validation")]
        BatchValidation = 3,

        [Description("Batch Validated")]
        BatchValidated = 4,

        [Description("Processing Completed")]
        Completed = 5,

        [Description("Processing Failed Due to Invalid Headers")]
        Fail = 6
    }

    public enum TransactionType
    {
        [Description("All")]
        None = 0,

        [Description("Credit")]
        Credit = 1,

        [Description("Debit")]
        Debit = 2
    }

    public enum ActivityType
    {
        [Description("Tenant")]
        Tenant = 1,

        [Description("RevenueHead")]
        RevenueHead = 2,

        [Description("MDA")]
        MDA = 3,

        [Description("ExpertSystem")]
        ExpertSystem = 4
    }

    public enum CBSPermissionName
    {
        [Description("This is to indicate the preference on allowing invoice part payment")]
        Allow_Part_Payment
    }

    public enum VerificationType
    {
        AccountVerification = 1,
        ForgotPassword = 2,
        RetrieveEmail = 3,
        AccountWalletPayment = 4,
    }


    public static class EnumHelper
    {

        /// <summary>
        /// Get Enum name
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns>string</returns>
        public static string GetEnumName(this Enum enumValue)
        {
            return Enum.GetName(enumValue.GetType(), enumValue);
        }

        /// <summary>
        /// Get description attribute
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns>string</returns>
        public static string GetDescription(this Enum enumValue)
        {
            FieldInfo fieldInfo = enumValue.GetType().GetField(Enum.GetName(enumValue.GetType(), enumValue));
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : Enum.GetName(enumValue.GetType(), enumValue);
        }


        [Obsolete]
        /// <summary>
        /// Use GetDescription, this method is not performant
        /// <para>google enum ToString performance <see cref="https://medium.com/@fran6_ca/performance-analysis-of-net-enum-tostring-vs-nameof-c2b7f99567af"/></para>
        /// </summary>
        public static string ToDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }


        public static SelectList ToSelectList<TEnum>(this TEnum obj, object selectedValue) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return new SelectList(Enum.GetValues(typeof(TEnum)).OfType<Enum>()
                .Select(x =>
                    new SelectListItem
                    {
                        Text = Enum.GetName(typeof(TEnum), x),
                        Value = (Convert.ToInt32(x)).ToString()
                    }), "Value", "Text", selectedValue);
        }

    }
}