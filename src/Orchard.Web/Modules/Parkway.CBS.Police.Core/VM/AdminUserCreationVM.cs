using Parkway.CBS.Police.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.VM
{
    public class AdminUserCreationVM
    {
        public string Fullname { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public int RoleTypeId { get; set; }

        public int AdminUserId { get; set; }

        public int CommandCategoryId { get; set; }

        public int OfficerSectionId { get; set; }

        public int OfficerSubSectionId { get; set; }

        public int CommandId { get; set; }

        public List<UserRoleVM> RoleTypes { get; set; }

        public IEnumerable<CommandCategoryVM> CommandCategories { get; set; }

        public List<CommandVM> Commands { get; set; }

        public List<CommandVM> OfficerSectionCommands { get; set; }

        public List<CommandVM> OfficerSubSectionCommands { get; set; }

        /// <summary>
        /// This will hold the selected commands on the interface
        /// </summary>
        public ICollection<CommandVM> SelectedCommands { get; set; }

        public ICollection<CommandVM> AddedCommands { get; set; }

        public ICollection<CommandVM> RemovedCommands { get; set; }

        /// <summary>
        /// This holds the user type that will deteremine the flow either approver or viewer
        /// <see cref="Models.Enums.AdminUserType"/>
        /// </summary>
        public int AdminUserType { get; set; }

        public IEnumerable<DTO.PSServiceVM> ServiceTypes { get; set; }

        /// <summary>
        /// This holds the selected service ids
        /// </summary>
        public ICollection<int> SelectedServiceTypes { get; set; }

        public ICollection<int> RemovedServiceTypes { get; set; }

        public List<DTO.PSServiceRequestFlowDefinitionDTO> FlowDefinitions { get; set; }

        /// <summary>
        /// This holds the selected flow definition ids
        /// </summary>
        public ICollection<int> SelectedFlowDefinitions { get; set; }

        public ICollection<int> RemovedFlowDefinitions { get; set; }

        public List<List<DTO.PSServiceRequestFlowDefinitionLevelDTO>> FlowDefinitionLevels { get; set; }

        /// <summary>
        /// This holds the Ids of the selected flow definition levels
        /// </summary>
        public ICollection<int> SelectedFlowDefinitionLevels { get; set; }

        public ICollection<int> RemovedFlowDefinitionLevels { get; set; }
    }
}