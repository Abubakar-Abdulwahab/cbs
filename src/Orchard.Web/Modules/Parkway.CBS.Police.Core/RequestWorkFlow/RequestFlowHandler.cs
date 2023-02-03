using System;
using System.Linq;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.RequestWorkFlow.Contracts;
using Parkway.CBS.Police.Core.RequestWorkFlow.Actions.Contracts;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.RequestWorkFlow
{
    public class RequestFlowHandler : IRequestFlowHandler
    {
        private readonly IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> _flowDefManager;
        private readonly IEnumerable<Lazy<IActionImpl>> _processFlows;


        public RequestFlowHandler(IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> flowDefManager, IEnumerable<Lazy<IActionImpl>> processFlows)
        {
            _flowDefManager = flowDefManager;
            _processFlows = processFlows;
        }


        /// <summary>
        /// Move request to next stage
        /// </summary>
        /// <param name="requestDeet"></param>
        public RequestFlowVM MoveRequestToNextStage(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeet)
        {
            //here we get the next level for this request as defined by the set definition level
            PSServiceRequestFlowDefinitionLevelDTO definedFlow = _flowDefManager.GetNextLevelDefinitionId(requestDeet.First().DefinitionId, requestDeet.First().DefinitionLevelIdPosition);
            //we find out what direction this flow is taking
            foreach (var item in _processFlows)
            {
                if (item.Value.GetRequestDirection == definedFlow.RequestDirectionValue)
                {
                    return item.Value.MoveToNextDefinitionLevel(requestDeet, definedFlow);
                }
            }
            throw new Exception("No defined process flow found");
        }

    }
}