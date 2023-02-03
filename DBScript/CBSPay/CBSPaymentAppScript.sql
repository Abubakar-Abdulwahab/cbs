
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKF47A7CD333EA6E77]') AND parent_object_id = OBJECT_ID('EIRSPaymentRequestItem'))
alter table EIRSPaymentRequestItem  drop constraint FKF47A7CD333EA6E77


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK172993FDFBC0326B]') AND parent_object_id = OBJECT_ID('PaymentHistoryItem'))
alter table PaymentHistoryItem  drop constraint FK172993FDFBC0326B


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK5CA6B3ED9AF40982]') AND parent_object_id = OBJECT_ID('AssessmentRuleItem'))
alter table AssessmentRuleItem  drop constraint FK5CA6B3ED9AF40982


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKD5ADBD87DDEBF1A]') AND parent_object_id = OBJECT_ID('ServiceBillItem'))
alter table ServiceBillItem  drop constraint FKD5ADBD87DDEBF1A


    if exists (select * from dbo.sysobjects where id = object_id(N'AssessmentDetails') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table AssessmentDetails

    if exists (select * from dbo.sysobjects where id = object_id(N'EIRSPaymentRequestItem') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table EIRSPaymentRequestItem

    if exists (select * from dbo.sysobjects where id = object_id(N'EIRSPaymentRequest') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table EIRSPaymentRequest

    if exists (select * from dbo.sysobjects where id = object_id(N'PaymentHistory') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table PaymentHistory

    if exists (select * from dbo.sysobjects where id = object_id(N'PaymentHistoryItem') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table PaymentHistoryItem

    if exists (select * from dbo.sysobjects where id = object_id(N'AssessmentRuleItem') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table AssessmentRuleItem

    if exists (select * from dbo.sysobjects where id = object_id(N'ServiceBillItem') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table ServiceBillItem

    if exists (select * from dbo.sysobjects where id = object_id(N'ServiceBill') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table ServiceBill

    if exists (select * from dbo.sysobjects where id = object_id(N'TaxPayerDetails') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table TaxPayerDetails

    create table AssessmentDetails (
        Id BIGINT IDENTITY NOT NULL,
       AssessmentID BIGINT null,
       AssessmentRefNo NVARCHAR(255) null,
       AssessmentDate DATETIME null,
       TaxPayerTypeID BIGINT null,
       TaxPayerTypeName NVARCHAR(255) null,
       TaxPayerID BIGINT null,
       TaxPayerName NVARCHAR(255) null,
       TaxPayerRIN NVARCHAR(255) null,
       AssessmentAmount DECIMAL(19,5) null,
       SetlementAmount DECIMAL(19,5) null,
       SettlementStatusID BIGINT null,
       SettlementDueDate DATETIME null,
       SettlementStatusName NVARCHAR(255) null,
       SettlementDate DATETIME null,
       AssessmentNotes NVARCHAR(255) null,
       Active BIT null,
       ActiveText NVARCHAR(255) null,
       PhoneNumber NVARCHAR(255) null,
       DateCreated DATETIME not null,
       DateModified DATETIME not null,
       IsDeleted BIT not null,
       primary key (Id)
    )

    create table EIRSPaymentRequestItem (
        Id BIGINT IDENTITY NOT NULL,
       ItemId BIGINT null,
       EIRSPaymentRequestId BIGINT null,
       ItemDescription NVARCHAR(255) null,
       ItemAmount DECIMAL(19,5) null,
       AmountPaid DECIMAL(19,5) null,
       TotalAmountPaid DECIMAL(19,5) null,
       DateCreated DATETIME not null,
       DateModified DATETIME not null,
       IsDeleted BIT null,
       primary key (Id)
    )

    create table EIRSPaymentRequest (
        Id BIGINT IDENTITY NOT NULL,
       ReferenceNumber NVARCHAR(255) null,
       TotalAmountPaid DECIMAL(19,5) null,
       Description NVARCHAR(255) null,
       TaxPayerName NVARCHAR(255) null,
       PaymentIdentifier NVARCHAR(255) null,
       PhoneNumber NVARCHAR(25) null,
       TaxPayerRIN NVARCHAR(255) null,
       TaxPayerTIN NVARCHAR(255) null,
       ReferenceID BIGINT null,
       IsPaymentSuccessful BIT null,
       DateCreated DATETIME not null,
       DateModified DATETIME not null,
       IsDeleted BIT not null,
       primary key (Id)
    )

    create table PaymentHistory (
        Id BIGINT IDENTITY NOT NULL,
       ReferenceNumber NVARCHAR(255) null,
       TaxPayerMobileNumber NVARCHAR(255) null,
       TaxPayerName NVARCHAR(255) null,
       TaxPayerTIN NVARCHAR(255) null,
       TaxPayerRIN NVARCHAR(255) null,
       PaymentChannel NVARCHAR(255) null,
       UpdatedPaymentChannel NVARCHAR(255) null,
       AmountPaid DECIMAL(19,5) null,
       TotalAmountPaid DECIMAL(19,5) null,
       ReferenceAmount DECIMAL(19,5) null,
       PaymentIdentifier NVARCHAR(255) null,
       IsCustomerDeposit BIT null,
       IsRepeated BIT null,
       PaymentLogId INT null,
       PaymentReference NVARCHAR(255) null,
       CustReference NVARCHAR(255) null,
       AlternateCustReference NVARCHAR(255) null,
       PaymentMethod NVARCHAR(255) null,
       ChannelName NVARCHAR(255) null,
       Location NVARCHAR(255) null,
       IsReversal BIT null,
       PaymentDate DATETIME not null,
       SettlementDate DATETIME null,
       InstitutionId NVARCHAR(255) null,
       InstitutionName NVARCHAR(255) null,
       BranchName NVARCHAR(255) null,
       BankName NVARCHAR(255) null,
       FeeName NVARCHAR(255) null,
       ReceiptNo NVARCHAR(255) null,
       PaymentCurrency INT null,
       OriginalPaymentLogId INT null,
       OriginalPaymentReference NVARCHAR(255) null,
       Teller NVARCHAR(255) null,
       ReferenceID BIGINT null,
       IsSyncedWithEIRS BIT null,
       DateCreated DATETIME not null,
       DateModified DATETIME not null,
       IsDeleted BIT not null,
       primary key (Id)
    )

    create table PaymentHistoryItem (
        Id BIGINT IDENTITY NOT NULL,
       ItemId BIGINT null,
       ItemDescription NVARCHAR(255) null,
       ItemAmount DECIMAL(19,5) null,
       AmountPaid DECIMAL(19,5) null,
       PaymentHistoryId BIGINT null,
       DateCreated DATETIME not null,
       DateModified DATETIME not null,
       IsDeleted BIT not null,
       primary key (Id)
    )

    create table AssessmentRuleItem (
        Id BIGINT IDENTITY NOT NULL,
       AAIID BIGINT null,
       AARID BIGINT null,
       AssessmentRuleID BIGINT null,
       AssessmentRuleName NVARCHAR(255) null,
       AssessmentItemReferenceNo NVARCHAR(255) null,
       AssessmentItemID BIGINT null,
       AssessmentItemName NVARCHAR(255) null,
       ComputationName NVARCHAR(255) null,
       PaymentStatusID BIGINT null,
       PaymentStatusName NVARCHAR(255) null,
       TaxAmount DECIMAL(19,5) null,
       AmountPaid DECIMAL(19,5) null,
       SettlementAmount DECIMAL(19,5) null,
       PendingAmount DECIMAL(19,5) null,
       AssessmentDetailsId BIGINT null,
       DateCreated DATETIME not null,
       DateModified DATETIME not null,
       IsDeleted BIT null,
       primary key (Id)
    )

    create table ServiceBillItem (
        Id BIGINT IDENTITY NOT NULL,
       SBSIID BIGINT null,
       SBSID BIGINT null,
       MDAServiceID BIGINT null,
       MDAServiceName NVARCHAR(255) null,
       MDAServiceItemReferenceNo NVARCHAR(255) null,
       MDAServiceItemID BIGINT null,
       MDAServiceItemName NVARCHAR(255) null,
       ComputationName NVARCHAR(255) null,
       PaymentStatusID BIGINT null,
       PaymentStatusName NVARCHAR(255) null,
       ServiceAmount DECIMAL(19,5) null,
       AmountPaid DECIMAL(19,5) null,
       SettlementAmount DECIMAL(19,5) null,
       PendingAmount DECIMAL(19,5) null,
       ServiceBillId BIGINT null,
       DateCreated DATETIME not null,
       DateModified DATETIME not null,
       IsDeleted BIT null,
       primary key (Id)
    )

    create table ServiceBill (
        Id BIGINT IDENTITY NOT NULL,
       ServiceBillID BIGINT null,
       ServiceBillRefNo NVARCHAR(255) null,
       ServiceBillDate DATETIME null,
       TaxPayerID BIGINT null,
       TaxPayerName NVARCHAR(255) null,
       TaxpayerRIN NVARCHAR(255) null,
       ServiceBillAmount DECIMAL(19,5) null,
       SetlementAmount DECIMAL(19,5) null,
       SettlementStatusID BIGINT null,
       SettlementDueDate DATETIME null,
       SettlementStatusName NVARCHAR(255) null,
       SettlementDate DATETIME null,
       Active BIT null,
       ActiveText NVARCHAR(255) null,
       PhoneNumber NVARCHAR(255) null,
       DateCreated DATETIME not null,
       DateModified DATETIME not null,
       IsDeleted BIT not null,
       primary key (Id)
    )

    create table TaxPayerDetails (
        Id BIGINT IDENTITY NOT NULL,
       TaxPayerID BIGINT null,
       TaxPayerTypeID BIGINT null,
       TaxPayerRIN NVARCHAR(255) null,
       TaxPayerTIN NVARCHAR(255) null,
       TaxPayerName NVARCHAR(255) null,
       TaxPayerMobileNumber NVARCHAR(255) null,
       TaxPayerAddress NVARCHAR(255) null,
       DateCreated DATETIME not null,
       DateModified DATETIME not null,
       IsDeleted BIT null,
       primary key (Id)
    )

    alter table EIRSPaymentRequestItem 
        add constraint FKF47A7CD333EA6E77 
        foreign key (EIRSPaymentRequestId) 
        references EIRSPaymentRequest

    alter table PaymentHistoryItem 
        add constraint FK172993FDFBC0326B 
        foreign key (PaymentHistoryId) 
        references PaymentHistory

    alter table AssessmentRuleItem 
        add constraint FK5CA6B3ED9AF40982 
        foreign key (AssessmentDetailsId) 
        references AssessmentDetails

    alter table ServiceBillItem 
        add constraint FKD5ADBD87DDEBF1A 
        foreign key (ServiceBillId) 
        references ServiceBill
