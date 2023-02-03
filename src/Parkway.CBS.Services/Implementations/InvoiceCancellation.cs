using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.HangFireInterface.Configuration;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.Services.Implementations.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Services.Implementations
{
    public class InvoiceCancellation : IInvoiceCancellation
    {
        private static readonly ILogger log = new Log4netLogger();

        public IUoW UoW { get; set; }

        public IInvoiceDAOManager InvoiceDAOManager { get; set; }

        protected void SetUnitofWork(string tenantName)
        {
            if (UoW == null)
            {
                UoW = new UoW(tenantName.Replace(" ", "") + "_SessionFactory", "PSSAllowanceJob");
            }
        }

        private void SetInvoiceDAOManager()
        {
            if (InvoiceDAOManager == null) { InvoiceDAOManager = new InvoiceDAOManager(UoW); }
        }

        [ProlongExpirationTime]
        public string ProcessInvoiceCancellation(string tenantName)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetInvoiceDAOManager();
                UoW.BeginTransaction();
                string response = InvoiceDAOManager.ProcessInvoiceCancellation();
                UoW.Commit();
                return response;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }
    }
}
