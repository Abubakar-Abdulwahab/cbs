using Orchard;
using Orchard.Security.Permissions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IExpenditureHeadHandler : IDependency
    {
        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canAddExpenditureHead"></param>
        void CheckForPermission(Permission canAddExpenditureHead);

        /// <summary>
        /// Toggle isactive in expenditure head with the value from <paramref name="isActive"/>
        /// </summary>
        /// <param name="expenditureHeadId"></param>
        /// <param name="isActive"></param>
        void ToggleIsActiveExpenditureHead(int expenditureHeadId, bool isActive);

        /// <summary>
        /// Get the view model for adding an expenditure head
        /// </summary>
        /// <returns><see cref="AddExpenditureHeadVM"/></returns>
        AddExpenditureHeadVM GetAddExpenditureHeadVM();

        /// <summary>
        /// Validates and Creates expenditure head in <see cref="PSSExpenditureHead"/>
        /// </summary>
        /// <param name="errors"> Validation errors</param>
        /// <param name="model">User input model</param>
        /// <exception cref="DirtyFormDataException">Invalid data</exception>
        /// <exception cref="CouldNotSaveRecord"></exception>
        /// <exception cref="Exception"></exception>
        void AddExpenditureHead(ref List<ErrorModel> errors, AddExpenditureHeadVM model);

        /// <summary>
        /// Validates and edits expenditure head in <see cref="PSSExpenditureHead"/>
        /// </summary>
        /// <param name="errors"> Validation errors</param>
        /// <param name="model">User input model</param>
        /// <exception cref="DirtyFormDataException">Invalid data</exception>
        /// <exception cref="CouldNotSaveRecord"></exception>
        /// <exception cref="Exception"></exception>
        void EditExpenditureHead(ref List<ErrorModel> errors, AddExpenditureHeadVM model);

        /// <summary>
        /// Get the view model for editing an expenditure head
        /// </summary>
        /// <returns><see cref="AddExpenditureHeadVM"/></returns>
        AddExpenditureHeadVM GetEditExpenditureHeadVM(int expenditureHeadId);
    }
}
