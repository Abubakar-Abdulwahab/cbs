using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel;
using System.Reflection;

namespace Parkway.CBS.Police.Core.Models.Enums
{
    /// <summary>
    /// this is used to indicate the direction of the request
    /// if this request is directed to generateinvoice, then an invoice will be generated
    /// else if this request is tagged to anotherapproval then the request is setup for another round of review
    /// else this request is tagged for nofurtheraction, that is it has reached the final stage of reviews or action
    /// and status of the request can be shown as approved
    /// </summary>
    public enum RequestDirection
    {
        None = 0,
        GenerateInvoice = 1,
        Approval = 2,
        NoFurtherAction = 3,
        InitiatePaymentRequest = 4,
        PaymentApproval = 5,
        PaymentReportViewer = 6
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PaymentRequestStatus
    {
        [Description("All")]
        ALL = 0,

        [Description("Awaiting Approval")]
        AWAITINGAPPROVAL = 1,

        [Description("Under Approval")]
        UNDERAPPROVAL = 2,

        [Description("Declined")]
        DECLINED = 3,

        [Description("Waiting")]
        WAITING = 4,

        [Description("In Progress")]
        INPROGRESS = 5,

        [Description("Paid")]
        PAID = 6,

        [Description("Failed")]
        FAILED = 7,
    }

    public enum DayType
    {
        [Description("Half Day")]
        H = 1,

        [Description("Full Day")]
        F = 2
    }

    public enum EGSCommandType
    {
        [Description("Default")]
        Default = 1,

        [Description("Tactical")]
        T = 2,

        [Description("Conventional")]
        C = 3
    }

    public enum StatusFilter
    {
        [Description("All")]
        All = 0,

        [Description("Active")]
        Active = 1,

        [Description("Inactive")]
        Inactive = 2,
    }

    public enum DefinitionType
    {
        Request = 0,
        Payment = 1,
    }

    public enum POSSAPCachePrefix
    {
        ServiceCategory,
        Category,
        Subcategory,
        Subsubcategory,
        Extractsubcategory,
        ExtractAllsubcategory,
        InquiryReasons,
        AreaDivisionCommand,
        AreaDivisionLGACommand,
        ServiceType,
        Caveat,
        FileNumber,
        ServiceNumber,
        FeeParties,
        Commands,
        Services,
        TaxEntityCategorySettings,
        ServiceStateCommandList,
        ServiceStateCommand,
        ServiceOptions,
        CaveatPartial,
        ServiceOptionValue
    }

    public enum PSSCommandWalletStatementStatus
    {
        [Description("All")]
        None = 0,
        [Description("Pending")]
        Pending = 1,
        [Description("Successful")]
        Successful = 2,
    }

    /// <summary>
    /// Request
    /// </summary>
    public enum PSSRequestStatus
    {
        [Description("All")]
        None = 0,
        [Description("Pending")]
        Pending = 1,
        [Description("Approved")]
        Approved = 2,
        [Description("Rejected")]
        Rejected = 3,
        [Description("Pending Approval")]
        PendingApproval = 4,
        [Description("Confirmed")]
        Confirmed = 5,
        [Description("Pending Invoice Payment")]
        PendingInvoicePayment = 6,
        [Description("Requery")]
        Requery = 7
        //[Description("All")]
        //None = 0,
        //[Description("Pending Processing Fee Payment")]
        //PendingApplicationFeePayment = 1,
        //[Description("Pending Approval")]
        //PendingApproval = 2,
        //[Description("Approved")]
        //Approved = 3,
        //[Description("Rejected")]
        //Rejected = 4,
        //[Description("Paid")]
        //Paid = 5,
        //[Description("Pending Service Fee Payment")]
        //PendingRequestFeePayment = 6,
    }

    public enum PSSPulseTemplateFileNames
    {
        [Description("POSSAP <no-reply@possap.gov.ng>")]
        Sender,

        [Description("POSSAP.ContactUsRequest")]
        ContactUsNotification,

        [Description("POSSAP.SubUserPasswordNotification")]
        SubUserPasswordNotification,

        [Description("POSSAP.AdminUserCreationNotification")]
        AdminUserCreationNotification,

        [Description("CBS.AccountVerification")]
        AccountVerification,

        [Description("POSSAP.EgsRegularizationRecurringInvoiceNotification")]
        EGSRegularizationInvoiceNotification,
    }


    /// <summary>
    /// Handles staging for invoice generation from front end
    /// </summary>
    public enum PSSUserRequestGenerationStage
    {
        RequestUserFormStage,
        ConfirmUserProfile,
        PSSExtractRequest,
        PSSRequestConfirmation,
        PSSRequest,
        PSSInvestigationReportRequest,
        PSSCharacterCertificateRequest,
        ServiceOptions,
    }


    /// <summary>
    /// Stage to determine what revenue head is picked for invoice generation
    /// </summary>
    public enum PSSRevenueServiceStep
    {
        ProcessingFee = 1,
        RequestFee = 2,
    }


    public enum PSSRequestSettingsName
    {
        PssRequests,
        PssCollectionReport,
        PssDeployedOfficersReport,
        PSSCommandStatementReport,
        PssOfficersReport,
        PssSettlementReportSummary,
        PssSettlementReportInvoices,
        PSSWalletPaymentReport,
        PssSettlementReportBreakdown,
        PSSBranchOfficers,
        RegularizationGenerateRequestWithoutOfficers,
        PSSDeploymentAllowancePaymentReport,
    }


    public enum PSBillingType
    {
        None = 0,
        [Description("One Off")]
        OneOff = 1,
        [Description("Daily")]
        Daily = 2,
        [Description("Weekly")]
        Weekly = 3,
        [Description("Monthly")]
        Monthly = 4,
    }


    public enum PSSServiceTypeDefinition
    {
        [Description("Police Extract")]
        Extract = 1,
        [Description("Police Escort Services")]
        Escort = 2,
        [Description("Other Police Services")]
        GenericPoliceServices = 3,
        [Description("Police Character Certificate")]
        CharacterCertificate = 4,
        [Description("Police Escort Unknown Police Officer Deployment Regularization")]
        EscortRegularization = 5
    }


    public enum PSSCommandCategoryLevel
    {
        Force = 1,
        Zonal = 2,
        State = 3,
        Area = 4,
        Divisional = 5,
        Station = 6,
        Outpost = 7
    }

    public enum DeploymentStatus
    {
        None,
        Pending,
        Running,
        Completed,
        Terminated
    }

    public enum Gender
    {
        Male = 1,
        Female = 2
    }

    public enum USSDProcessingStage
    {
        RequestType = 1,
        FileNumber = 2,
        OperationType = 3
    }

    public enum USSDOperationType
    {
        Approve = 1,
        Decline = 2
    }

    public enum USSDPSSServiceTypeDefinition
    {
        [Description("Extract")]
        Extract = 1,
        [Description("Escort & Guard Services")]
        Escort = 2,
        [Description("Character Certificate")]
        CharacterCertificate = 3,
        [Description("Others")]
        Others = 4
    }

    public enum USSDRequestType
    {
        [Description("Request Verification")]
        Verification = 1,
        [Description("Request Validation")]
        Validation = 2,
        [Description("Request Approval")]
        Approval = 10
    }

    public enum DeploymentAllowanceStatus
    {
        [Description("All")]
        None = 0,
        [Description("Pending Approval")]
        PendingApproval = 1,
        [Description("In Progress")]
        Waiting = 2,
        [Description("Declined")]
        Declined = 3,
        [Description("Failed")]
        Failed = 4,
        [Description("Paid")]
        Paid = 5,
    }

    public enum PSSTenantConfigKeys
    {
        None = 0,
        PSSAllowanceSettlementRuleCode,
        PSSAllowanceCallBackURL,
        PSSOfficerAllowanceDeduction,
        PSSOfficerAllowanceMobilizationFee,
        PSSOfficerAllowanceMobilizationBalanceFee,
        PoliceMedianRankID,
        ExternalDataSourceURL,
        ExtractFilePath,
        RegistrationIdentificationFilePath,
        CharacterCertificateFilePath,
        ContactUsEmailAddress,

        [Description("npf_logo.png")]
        PoliceLogo,

        [Description("PCCBackgroundBorders.png")]
        CharacterCertificateBG,

        [Description("PCC_stripe.png")]
        CertificateStrip,

        [Description("possap_logo.png")]
        PossapLogo,

        [Description("EXTBackgroundBorders.jpg")]
        ExtractDocumentBG,

        ApprovalNumberRegexPattern,
        SMSShortCode,
        PCCBiometricCaptureDueDay,
        PSSAdminSignaturesFilePath,
        PSSAdminSignaturesFileSize,

        [Description("semi_transparent_possap_bg.jpg")]
        DispatchNoteBG,
        ValidateDocumentURL,

        [Description("PCCRejectionBackgroundBorders.png")]
        RejectedCharacterCertificateBG,

        POSSAPSettlementClientCode,
        POSSAPSettlementSecret,
        SettlementEngineAuthTokenURL,
        SettlementEnginePaymentURL,
        SettlementEngineAccountVerifyURL,
        SettlementEnginePaymentCallbackURL,

        [Description("pcc_police_logo.png")]
        PccPoliceLogo,
        [Description("pccStamp.png")]
        PccEStampUrl,

        LatestBiometricAppVersion,
        UsingVersion,
        POSSAPAnalyticsEncryptionKey,
        AnalyticsDashboardURL,
        PSSAdminBranchSubUserFilePath,
        PSSAdminBranchSubUserFileSize,
        PSSAdminBranchOfficerFilePath,
        PSSAdminBranchOfficerFileSize,
        CPCCR_DB_Command_Identifier,
        PSSEGSRegularizationInvoiceGenerationPeriodMonthValue,
        PSSEGSRegularizationInvoiceGenerationPeriodYearValue,
        PSSEGSGenerateRequestWithoutOfficersUploadFilePath,
        PSSEGSGenerateRequestWithoutOfficersUploadFileSize,
        DeploymentAllowanceSettlementEnginePaymentCallbackURL
    }

    public enum PSSAllowancePaymentStage
    {
        MobilizationFee = 1,
        MobilizationBalanceFee = 2,
        OneOffFee = 3,
        MonthlyFee = 4
    }

    public enum PoliceRank
    {
        [Description("Inspector-General of Police")]
        InspectorGeneral = 1,
        [Description("Deputy Inspector-General of Police")]
        DeputyInspectorGeneral = 2,
        [Description("Assistant Inspector-General of Police")]
        AssistantInspectorGeneral = 3,
        [Description("Commissioner of Police")]
        Commissioner = 4,
        [Description("Deputy Commissioner of Police")]
        DeputyCommissioner = 5,
        [Description("Assistant Commissioner of Police")]
        AssistantCommissioner = 6,
        [Description("Chief Superintendent of Police")]
        ChiefSuperintendent = 7,
        [Description("Superintendent of Police")]
        Superintendent = 8,
        [Description("Deputy Superintendent of Police")]
        DeputySuperintendent = 9,
        [Description("Assistant Superintendent of Police")]
        AssistantSuperintendent = 10,
        [Description("Inspector of Police")]
        Inspector = 11,
        [Description("Sergeant Major")]
        SergeantMajor = 12,
        [Description("Sergeant")]
        Sergeant = 13,
        [Description("Corporal")]
        Corporal = 14,
        [Description("Lance Corporal")]
        LanceCorporal = 15
    }

    public enum DeductionShareType
    {
        [Description("Percentage")]
        Percentage = 1,

        [Description("Flat")]
        Flat = 2
    }

    public enum PCCRequestType
    {
        [Description("Domestic")]
        Domestic = 1,
        [Description("International")]
        International = 2
    }


    public enum AdminUserType
    {
        Approver = 1,
        Viewer = 2
    }

    public enum CommandTypeId
    {
        Default = 1,
        Tactical = 2,
        Conventional = 3
    }

    public enum RequestPhase
    {
        [Description("All")]
        None = 0,
        [Description("New")]
        New = 1,
        [Description("Ongoing")]
        Ongoing = 2
    }


    public enum ErrorType
    {
        None = 0,

        Application = 1,

        SettlementEngine = 2,
    }


    public enum WalletIdentifierType
    {
        CommandWalletDetails = 1,
        PSSFeeParty = 2
    }


    public enum PSSBranchSubUserUploadStatus
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

    public enum PSSBranchOfficersUploadStatus
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

    public enum GenerateRequestWithoutOfficersUploadStatus
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

    public enum SettlementAccountType
    {
        [Description("All")]
        All = 0,

        [Description("Command Settlement")]
        CommandSettlement = 1,

        [Description("Deployment Allowance Settlement")]
        DeploymentAllowanceSettlement = 2
    }

}