using System.Threading.Tasks;
using System.Web;
using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Client.Web.Controllers.Handlers.Contracts
{
    public interface IPAYEFileUploadHandler : IDependency
    {
        /// <summary>
        /// Validates file input
        /// </summary>
        /// <param name="file"></param>
        /// <param name="errorMessage"></param>
        /// <exception cref="FileNotFoundException">File not found</exception>
        /// <exception cref="Exception">Invalid file type </exception>
        void ValidateFileUpload(HttpPostedFileBase file, ref string errorMessage);

        /// <summary>
        /// Process file upload for PAYE schedule
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="userDetailsModel"></param>
        /// <param name="httpPostedFileBase"></param>
        void DoProcessingForPAYEFileUpload(GenerateInvoiceStepsModel processStage, UserDetailsModel userDetailsModel, HttpPostedFileBase httpPostedFileBase);

    }
}
