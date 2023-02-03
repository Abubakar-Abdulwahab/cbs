using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Data;
using Orchard.Users.Models;
using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Services
{
    public class FormControlsManager : BaseManager<FormControl>, IFormControlsManager<FormControl>
    {
        private readonly IRepository<FormControl> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;

        public FormControlsManager(IRepository<FormControl> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _user = user;

        }
    }
}
