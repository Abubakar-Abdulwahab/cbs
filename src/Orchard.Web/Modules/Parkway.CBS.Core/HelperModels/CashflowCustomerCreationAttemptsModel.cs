using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class CashflowCustomerCreationAttemptsModel
    {
        public ConcurrentStack<CashFlowCustomer> ListOfSuccessfulAttempts { get; set; }

        public ConcurrentStack<FailedAtCashflowModel> ListOfUnSuccessfulAttempts { get; set; }

        public ConcurrentStack<TaxEntity> ListOfSuccessfulAttemptsTaxEntity { get; set; }
    }
}