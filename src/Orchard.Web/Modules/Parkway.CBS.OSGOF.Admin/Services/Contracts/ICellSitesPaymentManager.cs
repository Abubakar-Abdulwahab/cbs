using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.FileUpload;
using Parkway.CBS.OSGOF.Admin.Models;
using Parkway.CBS.OSGOF.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.OSGOF.Admin.Services.Contracts
{
    public interface ICellSitesPaymentManager<CellSitesPayment> : IDependency, IBaseManager<CellSitesPayment>
    {
        /// <summary>
        /// Get the list of cell sites for this record I
        /// </summary>
        /// <param name="record"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns>CellSiteReportQueryObj</returns>
        CellSiteReportQueryObj GetRecords(CellSiteClientPaymentBatch record, int take, int skip, bool getStats = false);


        /// <summary>
        /// Delete all the cell site (child) records for this batch, 
        /// </summary>
        /// <param name="record"></param>
        void Delete(CellSiteClientPaymentBatch record);
        

        void SaveCellSites(long id, CellSitesBreakDown cellSitesObj);


        void RunComparisonForCellSites(long id);


        /// <summary>
        /// Get the total amount due on the given schedule
        /// </summary>
        /// <param name="record"></param>
        /// <returns>decimal</returns>
        decimal GetTotalAmountForSchedule(CellSiteClientPaymentBatch record);
    }
}
