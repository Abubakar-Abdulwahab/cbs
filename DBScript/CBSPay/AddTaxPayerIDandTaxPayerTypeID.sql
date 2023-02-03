ALTER TABLE paymenthistory
  ADD TaxPayerID bigint,
      TaxPayerTypeID bigint;

ALTER TABLE EIRSPaymentRequest
  ADD TaxPayerID bigint,
      TaxPayerTypeID bigint;