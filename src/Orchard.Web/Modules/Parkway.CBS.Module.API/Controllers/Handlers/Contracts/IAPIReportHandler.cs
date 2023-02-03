using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Module.API.Controllers.Handlers.Contracts
{
    public interface IAPIReportHandler : IDependency
    {
        APIResponse GetCollectionReport(ReportController collectionController, CollectionReportViewModel model, dynamic headerParams = null);
    }
}
