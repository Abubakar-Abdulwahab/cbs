using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.FileUpload.Implementations.Contracts
{
    public interface IFileUploadImplementations
    {
        dynamic GetViewModelForFileUpload(string filePath, params object[] parameters);

        /// <summary>
        /// process cell sites for onscreen
        /// </summary>
        /// <param name="settingPath"></param>
        /// <param name="filePath"></param>
        /// <returns>CellSitesBreakDown</returns>
        CellSitesBreakDown ProcessOnScreenCellSitesForOSGOF(string xmlFilePath, ICollection<FileUploadCellSites> cellSites);

       
        /// <summary>
        /// Process cell sites for file upload
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <param name="fileContents"></param>
        /// <returns>CellSitesBreakDown</returns>
        CellSitesBreakDown ProcessFileUploadCellSitesForOSGOF(string xmlFilePath, CellSiteFileProcessResponse fileContents);
    }

}
