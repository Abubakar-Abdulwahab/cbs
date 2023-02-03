using Parkway.Cashflow.Ng.API;
using Parkway.Cashflow.Ng.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientServices.Services.Contracts
{
    public interface IInvoicingService
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
