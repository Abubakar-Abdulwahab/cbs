using Parkway.Cashflow.Ng;
using Parkway.Cashflow.Ng.API;
using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.Services.Implementations.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Services.Implementations
{
    public class InvoicingService : IInvoicingService
    {

        #region Cashflow

        public CashFlowRequestContext StartInvoicingService(Dictionary<string, dynamic> credentials)
        {
            return CashFlow.Initializer(credentials);
        }

        public StateServices StateService(CashFlowRequestContext context)
        {
            return new StateServices(context);
        }

        public CustomerService CustomerServices(CashFlowRequestContext context)
        {
            return new CustomerService(context);
        }

        public BankServices BankService(CashFlowRequestContext context)
        {
            return new BankServices(context);
        }

        public CompanyServices CompanyServices(CashFlowRequestContext context)
        {
            return new CompanyServices(context);
        }

        public ProductServices ProductServices(CashFlowRequestContext context)
        {
            return new ProductServices(context);
        }

        public InvoiceService InvoiceService(CashFlowRequestContext context)
        {
            return new InvoiceService(context);
        }

        #endregion
    }
}
