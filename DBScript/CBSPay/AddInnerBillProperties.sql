use cbspaymentengine
go

ALTER TABLE EIRSPaymentRequest
  ADD TemplateType nvarchar(255),
	SettlementStatusName nvarchar(255),
	ReferenceNotes nvarchar(255),
	TotalAmount decimal(19,2),
	TotalOutstandingAmount decimal(19,2),
	TotalAmountToPay decimal(19,2),
	SettlementMethod int,
	ReferenceDate datetime,
	AddNotes nvarchar (255);

ALTER TABle EIRSPaymentRequestItem
ADD TaxYear int,
	RefRuleID bigint,
	RuleName nvarchar(255),
	RuleID bigint,
	RuleAmount decimal(19,2),
	SettledAmount decimal(19,2),
	OutstandingAmount decimal(19,2),
	RuleAmountToPay decimal(19,2),
	RuleItemRef nvarchar(255),
	RuleItemID nvarchar(255),
	RuleItemName nvarchar(255),
	RuleComputation nvarchar (255);
