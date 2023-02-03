using System;
using System.Linq;
using System.Collections.Generic;
using Parkway.CBS.FileUpload.Implementations.Contracts;


namespace Parkway.CBS.FileUpload.Implementations
{
    public class OSGOFFileUploadViewImpl : BaseFileUploadImpl, IFileUploadImplementations
    {
        public dynamic GetViewModelForFileUpload(string filePath, params object[] parameters)
        {
            //here we want to get the entities for the page, like the states, lgas and cell site
            return GetCellSiteStates(filePath);
        }

        public CellSitesBreakDown ProcessOnScreenCellSitesForOSGOF(string xmlFilePath, ICollection<FileUploadCellSites> cellSites)
        {
            return ValidateCellSites(cellSites.ToList(), xmlFilePath);
        }


        /// <summary>
        /// Process cell sites for file upload
        /// </summary>
        /// <param name="settingPath"></param>
        /// <param name="fileContents"></param>
        /// <returns>CellSitesBreakDown</returns>
        public CellSitesBreakDown ProcessFileUploadCellSitesForOSGOF(string xmlFilePath, CellSiteFileProcessResponse fileContents)
        {
            return ValidateCellSites(fileContents.FileUploadCellSites, xmlFilePath);
        }        
    }
}
