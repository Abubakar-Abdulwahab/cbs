using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.Core.Services.Contracts;
using System.Collections.Generic;
using CashFlowAPI = Parkway.Cashflow.Ng;
using Parkway.Cashflow.Ng.API;

namespace Parkway.CBS.Core.Services
{
    public class InvoicingService : IInvoicingService
    {
        #region Cashflow
        public CashFlowRequestContext StartInvoicingService(Dictionary<string, dynamic> credentials)
        {
            return CashFlowAPI.CashFlow.Initializer(credentials);
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