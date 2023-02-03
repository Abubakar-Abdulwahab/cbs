using System.Collections.Generic;
using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.PSSServiceType.Approval.EscortApprovalViewComposition.Contracts
{
    public interface IEscortViewComposition : IDependency
    {
        int StageIdentifier { get; }

        /// <summary>
        /// Set the transaction manager to aid database queries
        /// </summary>
        /// <param name="transactionManager"></param>
        void SetTransactionManagerForDBQueries(Orchard.Data.ITransactionManager transactionManager);

        dynamic SetPartialData(EscortPartialVM partialComp);

        /// <summary>
        /// Do validation on partial
        /// </summary>
        /// <param name="item"></param>
        /// <param name="objUserInput"></param>
        /// <param name="errors"></param>
        /// <returns>bool | has eror</returns>
        bool DoValidation(EscortPartialVM item, EscortRequestDetailsVM objUserInput, ref List<ErrorModel> errors);


        /// <summary>
        /// Saves model records to appropriate table
        /// </summary>
        /// <param name="item"></param>
        /// <param name="objUserInput"></param>
        /// <param name="errors"></param>
        void SaveRecords(EscortPartialVM item, EscortRequestDetailsVM objUserInput, EscortDetailsDTO escortDetails, ref List<ErrorModel> errors);


        RequestApprovalResponse OnSubmit(int approverId, long requestId, int commandTypeId);

        /// <summary>
        /// Approval for request
        /// </summary>
        /// <param name="partials"></param>
        /// <param name="requestId"></param>
        /// <param name="escort"></param>
        /// <param name="userPartId"></param>
        EscortApprovalMessage Approval(List<EscortPartialVM> partials, long requestId, EscortRequestDetailsVM escort, int userPartId);


        CanApproveEscortVM CanApprove(List<EscortPartialVM> partials, long requestId, EscortRequestDetailsVM escort, int adminUserId);


        /// <summary>
        /// Routes the request to escort stage of the implementation
        /// </summary>
        /// <param name="item"></param>
        /// <param name="objUserInput"></param>
        SecretariatRoutingApprovalResponse RouteToThisEscortStage(EscortPartialVM item, EscortRequestDetailsVM objUserInput);


    }
}
