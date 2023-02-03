using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Core.Services
{
    public class WebPaymentRequestManager : BaseManager<WebPaymentRequest>, IWebPaymentRequestManager<WebPaymentRequest>
    {
        private readonly IRepository<WebPaymentRequest> _webPayRequestRepository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public WebPaymentRequestManager(IRepository<WebPaymentRequest> webPayRequestRepository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(webPayRequestRepository, user, orchardServices)
        {
            _transactionManager = orchardServices.TransactionManager;
            _webPayRequestRepository = webPayRequestRepository;
            _orchardServices = orchardServices;
            _user = user;
            Logger = NullLogger.Instance;
        }
    }    
}