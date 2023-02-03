using Newtonsoft.Json;
using Orchard;
using Orchard.Users.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Data;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreRevenueHeadPermissionService : ICoreRevenueHeadPermissionService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ICoreSettingsService _settingsService;
        private readonly IRevenueHeadManager<RevenueHead> _revHeadRepo;
        private readonly IRevenueHeadPermissionConstraintsManager<RevenueHeadPermissionConstraints> _revHeadPermissionConstraintsRepo;

        public CoreRevenueHeadPermissionService(IOrchardServices orchardServices, ICoreSettingsService settingsService, IRevenueHeadManager<RevenueHead> revHeadRepo, IRevenueHeadPermissionConstraintsManager<RevenueHeadPermissionConstraints> revHeadPermissionConstraintsRepo)
        {
            _orchardServices = orchardServices;
            _settingsService = settingsService;
            _revHeadRepo = revHeadRepo;
            _revHeadPermissionConstraintsRepo = revHeadPermissionConstraintsRepo;
        }      


        /// <summary>
        /// Assign revenue head constraints to selected expert system
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="admin"></param>
        public void TryAssignRevenueHeadPermissionsToExpertSystem(AssignRevenueHeadPermissionConstraintsVM userInput, UserPartRecord admin)
        {
            try
            {
                List<RevenueHeadLite> rhCollection = new List<RevenueHeadLite>();

                //First delete constraints for expert system with selected permission if any
                DeleteExistingExpertSystemRecords(userInput.ExpertSystem.Id);

                var rhAndMdaCollection = JsonConvert.DeserializeObject<Dictionary<int, IEnumerable<int>>>(userInput.SelectedRhAndMdas);

                var dataTable = new DataTable("Parkway_CBS_Core_" + typeof(RevenueHeadPermissionConstraints).Name);
                dataTable.Columns.Add(new DataColumn("MDA_Id", typeof(int)));
                dataTable.Columns.Add(new DataColumn("RevenueHead_Id", typeof(int)));
                dataTable.Columns.Add(new DataColumn("ExpertSystem_Id", typeof(int)));
                dataTable.Columns.Add(new DataColumn("RevenueHeadPermission_Id", typeof(int)));
                dataTable.Columns.Add(new DataColumn("LastUpdatedBy_Id", typeof(int)));
                dataTable.Columns.Add(new DataColumn("CreatedAtUtc", typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn("UpdatedAtUtc", typeof(DateTime)));

                foreach (var mda in rhAndMdaCollection)
                {
                    foreach (var rh in mda.Value)
                    {
                        if (rh != 0)
                        {
                            if (!_revHeadRepo.CheckIfRevenueHeadAndExistsWithMDA(rh, mda.Key)) { throw new NoRecordFoundException(string.Format("No match for rev {0} and MDA {1} ", rh, mda.Key)); }
                            var row = dataTable.NewRow();
                            row["MDA_Id"] = mda.Key;
                            row["RevenueHead_Id"] = rh;
                            row["ExpertSystem_Id"] = userInput.ExpertSystem.Id;
                            row["RevenueHeadPermission_Id"] = userInput.SelectedPermissionIdParsed;
                            row["LastUpdatedBy_Id"] = admin.Id;
                            row["CreatedAtUtc"] = DateTime.Now.ToLocalTime();
                            row["UpdatedAtUtc"] = DateTime.Now.ToLocalTime();
                            dataTable.Rows.Add(row);
                        }
                        else
                        {
                            var row = dataTable.NewRow();
                            row["MDA_Id"] = mda.Key;
                            row["RevenueHead_Id"] = DBNull.Value;
                            row["ExpertSystem_Id"] = userInput.ExpertSystem.Id;
                            row["RevenueHeadPermission_Id"] = userInput.SelectedPermissionIdParsed;
                            row["LastUpdatedBy_Id"] = admin.Id;
                            row["CreatedAtUtc"] = DateTime.Now.ToLocalTime();
                            row["UpdatedAtUtc"] = DateTime.Now.ToLocalTime();
                            dataTable.Rows.Add(row);
                        }
                    }
                }

                if (!_revHeadPermissionConstraintsRepo.SaveBundle(dataTable, "Parkway_CBS_Core_" + typeof(RevenueHeadPermissionConstraints).Name))
                {
                    throw new Exception($"Unable to assign revenue head constraints with permission id {userInput.SelectedPermissionIdParsed} to expert systems {userInput.ExpertSystem.CompanyName}");
                }
            }
            catch (Exception)
            {
                _revHeadPermissionConstraintsRepo.RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// This method fetches existing revenue head constraints for the expert system with the specified Id.
        /// </summary>
        /// <param name="expertSystemId"></param>
        /// <param name="permissionId"></param>
        /// <returns>IEnumerable{RevenueHeadPermissionsConstraintsVM}</returns>
        public IEnumerable<RevenueHeadPermissionsConstraintsVM> GetExistingConstraints(int expertSystemId, int permissionId)
        {
            try
            {
                return _revHeadPermissionConstraintsRepo.GetExistingConstraints(expertSystemId, permissionId);
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// Delete existing records for expert system with specified Id and permission Id
        /// </summary>
        /// <param name="expertSystemId"></param>
        public void DeleteExistingExpertSystemRecords(int expertSystemId)
        {
            try
            {
                _revHeadPermissionConstraintsRepo.DeleteExpertSystemRecords(expertSystemId);
            }
            catch (Exception)
            {
                _revHeadPermissionConstraintsRepo.RollBackAllTransactions();
                throw;
            }
        }
    }
}