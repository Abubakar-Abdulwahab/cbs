using Orchard;
using Parkway.Cashflow.Ng.Models;
using System.Collections.Generic;
using Parkway.Cashflow.Ng.API;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IInvoicingService : IDependency
    {
        CashFlowRequestContext StartInvoicingService(Dictionary<string, dynamic> credentials);
        StateServices StateService(CashFlowRequestContext context);
        BankServices BankService(CashFlowRequestContext context);
        CompanyServices CompanyServices(CashFlowRequestContext context);
        ProductServices ProductServices(CashFlowRequestContext context);
        CustomerService CustomerServices(CashFlowRequestContext context);
        InvoiceService InvoiceService(CashFlowRequestContext context);
    }


}