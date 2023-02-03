using System;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.POSSAP.Services.PSSRepositories;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using Parkway.CBS.POSSAP.Services.Implementations.PSSWalletStatementRequest.Processes.Contracts;


namespace Parkway.CBS.POSSAP.Services.Implementations.PSSWalletStatementRequest.Processes
{
    public class MoveWalletStatementsFromStagingToMainTable
    {
        private static readonly ILogger log = new Log4netLogger();

        public IUoW UoW { get; set; }

        public IProcessComp _processCompo { get; set; }

        public IWalletStatementDAOManager _walletStatementDAOManager { get; set; }


        protected void SetUnitofWork(string tenantName)
        {
            if (UoW == null)
            {
                UoW = new UoW(tenantName.Replace(" ", "") + "_SessionFactory", "PSSWalletStatementRequestJob");
            }
        }


        private void SetWalletStatementDAOManager()
        {
            if (_walletStatementDAOManager == null) { _walletStatementDAOManager = new WalletStatementDAOManager(UoW); }
        }


        public void MoveWalletStatementsFromStagingToMain(string tenantName, string reference)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetWalletStatementDAOManager();
                UoW.BeginTransaction();
                _walletStatementDAOManager.MoveStatementsFromStagingToMain(reference);
                UoW.Commit();
            }
            catch (Exception exception)
            {
                log.Error($"Error moving POSSAP wallet statements from WalletStatementStaging to WalletStatement for statements with referencce {reference}");
                log.Error(exception.Message, exception);
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                _walletStatementDAOManager = null;
                _processCompo = null;
            }
        }
    }
}
