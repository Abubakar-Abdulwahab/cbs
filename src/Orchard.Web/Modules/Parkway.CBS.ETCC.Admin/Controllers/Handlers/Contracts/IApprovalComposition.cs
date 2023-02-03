using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ETCC.Admin.Controllers.Handlers.Contracts
{
    public interface IApprovalComposition : IDependency
    {
        TCCApprovalLevel GetApprovalLevelDefinition { get; }

        /// <summary>
        /// Save request approval details
        /// </summary>
        /// <param name="requestDetailVM"></param>
        /// <param name="errors"></param>
        /// <returns>bool</returns>
        bool ProcessRequestApproval(TCCRequestDetailVM requestDetailVM, ref List<ErrorModel> errors);

        /// <summary>
        /// Save request rejection details
        /// </summary>
        /// <param name="requestDetailVM"></param>
        /// <param name="errors"></param>
        /// <returns>bool</returns>
        bool ProcessRequestRejection(TCCRequestDetailVM requestDetailVM, ref List<ErrorModel> errors);

    }
}
