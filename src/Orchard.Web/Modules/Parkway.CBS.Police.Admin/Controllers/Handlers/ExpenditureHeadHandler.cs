using Orchard;
using Orchard.Logging;
using Orchard.Security.Permissions;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class ExpenditureHeadHandler : IExpenditureHeadHandler
    {
        private readonly IHandlerComposition _handlerComposition;
        private readonly IPSSExpenditureHeadManager<PSSExpenditureHead> _expenditureHeadManager;
        private readonly IOrchardServices _orchardServices;
        ILogger Logger { get; set; }

        public ExpenditureHeadHandler(IHandlerComposition handlerComposition, IPSSExpenditureHeadManager<PSSExpenditureHead> expenditureHeadManager, IOrchardServices orchardServices)
        {
            _handlerComposition = handlerComposition;
            _expenditureHeadManager = expenditureHeadManager;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
        }

        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canAddExpenditureHead"></param>
        public void CheckForPermission(Permission canAddExpenditureHead)
        {
            _handlerComposition.IsAuthorized(canAddExpenditureHead);
        }

        /// <summary>
        /// Get the view model for adding an expenditure head
        /// </summary>
        /// <returns><see cref="AddExpenditureHeadVM"/></returns>
        public AddExpenditureHeadVM GetAddExpenditureHeadVM()
        {
            return new AddExpenditureHeadVM();
        }

        /// <summary>
        /// Get the view model for editing an expenditure head
        /// </summary>
        /// <returns><see cref="AddExpenditureHeadVM"/></returns>
        public AddExpenditureHeadVM GetEditExpenditureHeadVM(int expenditureHeadId)
        {
            ExpenditureHeadVM expenditureHead = _expenditureHeadManager.GetExpenditureHeadById(expenditureHeadId) ?? throw new NoRecordFoundException();

            return new AddExpenditureHeadVM
            {
                Id = expenditureHeadId,
                Code = expenditureHead.Code,
                IsActive = expenditureHead.IsActive,
                Name = expenditureHead.Name
            };
        }

        /// <summary>
        /// Toggle isactive in expenditure head with the value from <paramref name="isActive"/>
        /// </summary>
        /// <param name="expenditureHeadId"></param>
        /// <param name="isActive"></param>
        public void ToggleIsActiveExpenditureHead(int expenditureHeadId, bool isActive)
        {
            try
            {
                _expenditureHeadManager.ToggleIsActiveExpenditureHead(expenditureHeadId, isActive, lastUpdatedById: _orchardServices.WorkContext.CurrentUser.Id);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _expenditureHeadManager.RollBackAllTransactions();
                throw;
            }
        }

        /// <summary>
        /// Validates and Creates expenditure head in <see cref="PSSExpenditureHead"/>
        /// </summary>
        /// <param name="errors"> Validation errors</param>
        /// <param name="model">User input model</param>
        /// <exception cref="DirtyFormDataException">Invalid data</exception>
        /// <exception cref="CouldNotSaveRecord"></exception>
        /// <exception cref="Exception"></exception>
        public void AddExpenditureHead(ref List<ErrorModel> errors, AddExpenditureHeadVM model)
        {
            try
            {
                ValidateUserInput(errors, model);

                if (_expenditureHeadManager.CheckIfNameOrCodeAlreadyExist(model.Name.Trim(), model.Code.Trim()))
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"Name or Code already exist", FieldName = nameof(model.Code) });
                    throw new DirtyFormDataException();
                }

                if (!_expenditureHeadManager.Save(new PSSExpenditureHead
                {
                    Code = model.Code.Trim(),
                    IsActive = true,
                    Name = model.Name.Trim(),
                    LastUpdatedBy = new Orchard.Users.Models.UserPartRecord { Id = _orchardServices.WorkContext.CurrentUser.Id }
                }))
                {
                    throw new CouldNotSaveRecord();
                };


            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _expenditureHeadManager.RollBackAllTransactions();
                throw;
            }
        }

        /// <summary>
        /// Validates and edits expenditure head in <see cref="PSSExpenditureHead"/>
        /// </summary>
        /// <param name="errors"> Validation errors</param>
        /// <param name="model">User input model</param>
        /// <exception cref="DirtyFormDataException">Invalid data</exception>
        /// <exception cref="CouldNotSaveRecord"></exception>
        /// <exception cref="Exception"></exception>
        public void EditExpenditureHead(ref List<ErrorModel> errors, AddExpenditureHeadVM model)
        {
            try
            {
                ValidateUserInput(errors, model);

                if (_expenditureHeadManager.CheckIfNameOrCodeAlreadyExist(model.Name.Trim(), model.Code.Trim(), model.Id))
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"Name or Code already exist", FieldName = nameof(model.Code) });
                    throw new DirtyFormDataException();
                }

                _expenditureHeadManager.UpdateExpenditureHead(model, lastUpdatedById: _orchardServices.WorkContext.CurrentUser.Id);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _expenditureHeadManager.RollBackAllTransactions();
                throw;
            }
        }

        private void ValidateUserInput(List<ErrorModel> errors, AddExpenditureHeadVM model)
        {
            if (string.IsNullOrEmpty(model.Name?.Trim()))
            {
                errors.Add(new ErrorModel { ErrorMessage = $"Kindly enter a name", FieldName = nameof(model.Name) });
                throw new DirtyFormDataException();
            }

            if (string.IsNullOrEmpty(model.Code?.Trim()))
            {
                errors.Add(new ErrorModel { ErrorMessage = $"Kindly enter a code", FieldName = nameof(model.Code) });
                throw new DirtyFormDataException();
            }

            if (model.Code.Trim().Length > 20 || model.Code.Trim().Length < 3)
            {
                errors.Add(new ErrorModel { ErrorMessage = "Code field must be between 3 to 20 characters long.", FieldName = nameof(model.Code) });
                throw new DirtyFormDataException();
            }

            if (model.Name.Trim().Length > 250 || model.Name.Trim().Length < 5)
            {
                errors.Add(new ErrorModel { ErrorMessage = $"Name field must be between 5 to 250 characters long.", FieldName = nameof(model.Name) });
                throw new DirtyFormDataException();
            }
        }
    }
}