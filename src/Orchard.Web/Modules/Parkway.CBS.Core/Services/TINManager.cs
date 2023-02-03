using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Services
{
    public class TINManager : BaseManager<TIN>, ITINManager<TIN>
    {
        private readonly IRepository<TIN> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;

        public TINManager(IRepository<TIN> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
        }
    }
}