using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class PSSAdminUserAssignEscortProcessFlowAJAXHandler : IPSSAdminUserAssignEscortProcessFlowAJAXHandler
    {
        private readonly IEscortProcessStageDefinitionManager<EscortProcessStageDefinition> _escortProcessStageDefinitionManager;
        private readonly IPSSAdminUsersManager<PSSAdminUsers> _pssAdminUsersManager;
        private readonly IEscortProcessFlowManager<EscortProcessFlow> _escortProcessFlowManager;
        ILogger Logger { get; set; }

        public PSSAdminUserAssignEscortProcessFlowAJAXHandler(IPSSAdminUsersManager<PSSAdminUsers> pssAdminUsersManager, IEscortProcessStageDefinitionManager<EscortProcessStageDefinition> escortProcessStageDefinitionManager, IEscortProcessFlowManager<EscortProcessFlow> escortProcessFlowManager)
        {
            _escortProcessStageDefinitionManager = escortProcessStageDefinitionManager;
            _pssAdminUsersManager = pssAdminUsersManager;
            _escortProcessFlowManager = escortProcessFlowManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Gets details of admin user with specified username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public APIResponse GetAdminUser(string username)
        {
            try
            {
                AdminUserVM adminUser = _pssAdminUsersManager.GetAdminUser(username);
                if (adminUser == null) { return new APIResponse { ResponseObject = "User not found", StatusCode = System.Net.HttpStatusCode.NotFound }; }
                if(_escortProcessFlowManager.Count(x => x.AdminUser.Id == adminUser.Id) > 0) { return new APIResponse { ResponseObject = "User already assigned" }; }
                return new APIResponse { ResponseObject = adminUser, StatusCode = System.Net.HttpStatusCode.OK };

            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text };
            }
        }


        /// <summary>
        /// Gets active escort process stage definitions for specified command type
        /// </summary>
        /// <param name="commandTypeId"></param>
        /// <returns></returns>
        public APIResponse GetEscortProcessStageDefinitions(int commandTypeId)
        {
            try
            {
                IEnumerable<EscortProcessStageDefinitionDTO> escortProcessStages = _escortProcessStageDefinitionManager.GetAllEscortProcessStageDefinitions(commandTypeId);
                if (escortProcessStages == null || !escortProcessStages.Any()) { return new APIResponse { ResponseObject = "No process stages found", StatusCode = System.Net.HttpStatusCode.NotFound }; }
                return new APIResponse { ResponseObject = escortProcessStages, StatusCode = System.Net.HttpStatusCode.OK };
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text };
            }
        }
    }
}