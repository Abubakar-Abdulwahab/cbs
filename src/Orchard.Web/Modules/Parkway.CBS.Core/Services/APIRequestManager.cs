using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Services
{
    public class APIRequestManager : BaseManager<APIRequest>, IAPIRequestManager<APIRequest>
    {
        private readonly IRepository<APIRequest> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;


        public APIRequestManager(IRepository<APIRequest> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _user = user;
        }

        /// <summary>
        /// get resource identifier
        /// </summary>
        /// <param name="requestIdentifier">string</param>
        /// <param name="expertSystem">ExpertSystemSettings</param>
        /// <param name="callerType">CallTypeEnum</param>
        /// <returns>Int64</returns>
        public Int64 GetResourseIdentifier(string requestIdentifier, ExpertSystemSettings expertSystem, CallTypeEnum callerType)
        {
            using (var sess = _orchardServices.TransactionManager.GetSession().SessionFactory.OpenSession())
            {
                return sess.QueryOver<APIRequest>().Where(req => (req.RequestIdentifier == requestIdentifier) && (req.ExpertSystemSettings == expertSystem) && (req.CallType == (short)callerType)).Select(req => req.ResourceIdentifier).List<Int64>().FirstOrDefault();
            }
        }
    }
}