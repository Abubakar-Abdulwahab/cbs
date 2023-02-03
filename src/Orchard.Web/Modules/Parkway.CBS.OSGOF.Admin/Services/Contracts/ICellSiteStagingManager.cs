using Orchard;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.FileUpload.OSGOFImplementation.Models;
using Parkway.CBS.OSGOF.Admin.Models;
using Parkway.CBS.OSGOF.Admin.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Parkway.CBS.OSGOF.Admin.Services.Contracts
{
    public interface ICellSiteStagingManager<CellSitesStaging> : IDependency, IBaseManager<CellSitesStaging>
    {
        void SaveRecords(ConcurrentStack<OSGOFCellSitesExcelModel> cellSitesFromFile, TaxEntity taxProfile, UserPartRecord adminUser, CBSUser loggedInUser, CellSitesScheduleStaging batchRecord);


        /// <summary>
        /// Get the IEnumerable of cell sites in the staging table with the given schedule ref
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns>IEnumerable{Models.CellSitesStaging}</returns>
        IEnumerable<Models.CellSitesStaging> GetCellSitesForScheduleStaging(Int64 scheduleId, int take, int skip);

        /// <summary>
        /// Get the number of records with errors
        /// </summary>
        /// <param name="id"></param>
        /// <returns>NHibernate.IFutureValue{Int32}</returns>
        NHibernate.IFutureValue<Int32> GetCellSitesWithErrors(long id);


        /// <summary>
        /// Move valid cell sites from staging to main table
        /// </summary>
        /// <param name="id"></param>
        /// <param name="approvedBy"></param>
        void DoTransferFromStagingToMainTable(CellSitesScheduleStagingVM scheduleVM, UserPartRecord approvedBy);
    }   
}
