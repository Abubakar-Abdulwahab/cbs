using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers.BillingImpl;
using Parkway.CBS.OSGOF.Web.Controllers.Handlers.BillingImpl.Contracts;

namespace Parkway.CBS.OSGOF.Web.Controllers.Handlers.BillingImpl
{
    public class OSGOFDirectAssessmentBillingImpl : DirectAssessmentBillingImpl, IOSGOFBillingImpl
    {
        public OSGOFDirectAssessmentBillingImpl(IPAYEBatchRecordStagingManager<PAYEBatchRecordStaging> payeBatchRepo, ICoreCollectionService coreCollectionService) : base(payeBatchRepo, coreCollectionService)
        { }
    }
}