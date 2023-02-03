using Parkway.CBS.Entities.DTO;
using Parkway.CBS.FileUpload.OSGOFImplementation.Models;
using System.Collections.Generic;

namespace Parkway.CBS.FileUpload.OSGOFImplementation.Contracts
{
    public interface IOSGOFCellSitesAdapter
    {
        /// <summary>
        /// Get cell sites response model
        /// </summary>
        /// <param name="statesAndLGAs"></param>
        /// <param name="filePath"></param>
        /// <returns>OSGOFCellSitesResponse</returns>
        OSGOFCellSitesResponse GetCellSitesResponseModels(List<StatesAndLGAs> statesAndLGAs, string filePath);

    }
}
