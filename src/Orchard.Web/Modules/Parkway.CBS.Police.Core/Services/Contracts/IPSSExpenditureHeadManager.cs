using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSExpenditureHeadManager<PSSExpenditureHead> : IDependency, IBaseManager<PSSExpenditureHead>
    {
        /// <summary>
        /// Check if the expenditure head exist using <paramref name="expenditureHeadId"/>
        /// </summary>
        /// <param name="expenditureHeadId"></param>
        /// <returns></returns>
        bool CheckIExpenditureHeadExist(int expenditureHeadId);

        /// <summary>
        /// Get expenditure head by <paramref name="expenditureHeadId"/>
        /// </summary>
        /// <param name="expenditureHeadId"></param>
        /// <returns></returns>
        ExpenditureHeadVM GetExpenditureHeadById(int expenditureHeadId);

        /// <summary>
        /// Gets all active expenditure head
        /// </summary>
        /// <returns></returns>
        List<ExpenditureHeadVM> GetActiveExpenditureHead();

        /// <summary>
        /// Sets IsActive to false
        /// </summary>
        /// <param name="expenditureHeadId"></param>
        /// <param name="isActive"></param>
        /// <param name="lastUpdatedById"></param>
        void ToggleIsActiveExpenditureHead(int expenditureHeadId, bool isActive, int lastUpdatedById);

        /// <summary>
        /// Updates certain columns in PSSExpenditureHead
        /// </summary>
        /// <param name="model"></param>
        /// <param name="lastUpdatedById"></param>
        void UpdateExpenditureHead(AddExpenditureHeadVM model, int lastUpdatedById);

        /// <summary>
        /// Checks if the Name and Code Already exist
        /// </summary>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        bool CheckIfNameOrCodeAlreadyExist(string name, string code);

        /// <summary>
        /// Checks if the Name and Code Already does not exist for <paramref name="expenditureHeadId"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <param name="expenditureHeadId"></param>
        /// <returns></returns>
        bool CheckIfNameOrCodeAlreadyExist(string name, string code, int expenditureHeadId);
    }
}
