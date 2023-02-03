-- START REVISION 1.00
--
-- script for database view 16/8/2018
-- This creates a view that gives the amount due on an invoice
-- Id here is the invoice Id on the invoice table and AmountDue is the amount due on that invoice.
-- The AmountDue column value is the sum of all credits for that invoice substracted for the invoice amount the value is now the Amount due for that invoice 
-- subject to the result gotten from the GetAmountDue function, which takes in the value of the substraction

-- For example: A tax payer has generated an invoice of 50 Naira, when the view is queried, the invoice Id of that invoice is gotten along with the amount for that invoice.
-- A query for the sum of credit for that invoice Id is done on the TransactionLog table.
-- This value is substracted from the invoice Amount then put into the GetAmountDue function, to get the actual amount due.
-- If the sum of credits is greater than the invoice amount, a value 0 is returned that is the invoice has no amount due.
-- Still with our example if 20 Naira was paid twice against that invoice, the sum of credits = 40 Naira, substracting that from the 
-- invoice Amount would give 10 Naira, which is the amount due on this invoice.
-- On this same invoice if another payment of 20 Naira is made, substracting a sum of all my credits for this invoice would give -10 Naira.
-- Passing this -10 Naira value to the GetAmountDue function would give me a value of 0 Naira, which invariably indicates that no amount is due on this invoice
---
CREATE VIEW Parkway_CBS_Core_InvoiceAmountDueSummary AS

SELECT dbo.Parkway_CBS_Core_Invoice.Id as Id, 
dbo.GetAmountDue(COALESCE(dbo.Parkway_CBS_Core_Invoice.Amount - SUM(dbo.Parkway_CBS_Core_TransactionLog.AmountPaid), dbo.Parkway_CBS_Core_Invoice.Amount)) as AmountDue

FROM dbo.Parkway_CBS_Core_Invoice LEFT JOIN dbo.Parkway_CBS_Core_TransactionLog 
ON dbo.Parkway_CBS_Core_Invoice.Id = dbo.Parkway_CBS_Core_TransactionLog.Invoice_Id and dbo.Parkway_CBS_Core_TransactionLog.TypeID != '1' GROUP BY dbo.Parkway_CBS_Core_Invoice.Id, dbo.Parkway_CBS_Core_Invoice.Amount;
--- 
--
--- END REVISION 1.00