Use CBSPaymentEngine

create table AssessmentRule (
        Id BIGINT IDENTITY(1,1) NOT NULL,
       AARID BIGINT null,
	   AssetTypeId BIGINT null,
	   AssetTypeName NVARCHAR(255) null,
	   AssetID BIGINT null,
	   AssetRIN  NVARCHAR(255) null,
	   ProfileID BIGINT null,
	   ProfileDescription  NVARCHAR(255) null,
       AssessmentRuleID BIGINT null,
       AssessmentRuleName NVARCHAR(255) null,
	   TaxYear INT null,
	   AssessmentRuleAmount DECIMAL(15,2) null,
	   SettledAmount DECIMAL(15,2) null,
	   AssessmentDetailsId [bigint] NULL,
       Active BIT null,
       ActiveText NVARCHAR(255) null,
       DateCreated DATETIME not null,
       DateModified DATETIME not null,
       IsDeleted BIT not null,
       primary key (Id),
	   CONSTRAINT fk_AssAssesmentRules FOREIGN KEY (AssessmentDetailsId)
REFERENCES AssessmentDetails(Id)
    )
	
	create table MDAService (
        Id BIGINT IDENTITY(1,1) NOT NULL,
       SBSID BIGINT null,
	   MDAServiceID BIGINT null,
	   MDAServiceName NVARCHAR(255) null,	   
	   TaxYear INT null,
	   ServiceAmount DECIMAL(15,2) null,
	   SettledAmount DECIMAL(15,2) null,
	   ServiceBillId [bigint] NULL,
       Active BIT null,
       ActiveText NVARCHAR(255) null,
       DateCreated DATETIME not null,
       DateModified DATETIME not null,
       IsDeleted BIT not null,
       primary key (Id),
	   CONSTRAINT fk_SBMDAService FOREIGN KEY (ServiceBillId)
REFERENCES ServiceBill(Id)
    )
