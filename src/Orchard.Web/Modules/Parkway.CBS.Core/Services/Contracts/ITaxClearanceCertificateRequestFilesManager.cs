using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface ITaxClearanceCertificateRequestFilesManager<TaxClearanceCertifcateRequestFiles> : IDependency, IBaseManager<TaxClearanceCertifcateRequestFiles>
    {
        /// <summary>
        /// Save bundle of tcc files upload
        /// </summary>
        /// <param name="collections"></param>
        /// <returns>bool</returns>
        bool SaveBundleUnCommit(List<TaxClearanceCertificateRequestFiles> collections);
    }
}
