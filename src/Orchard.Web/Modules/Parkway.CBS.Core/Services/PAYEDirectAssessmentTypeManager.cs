using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Services
{
    public class PAYEDirectAssessmentTypeManager : BaseManager<PAYEDirectAssessmentType>, IPAYEDirectAssessmentTypeManager<PAYEDirectAssessmentType>
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;
        private readonly IRepository<PAYEDirectAssessmentType> _payeDirectAssessmentTypeRepository;

        public PAYEDirectAssessmentTypeManager(IRepository<PAYEDirectAssessmentType> payeDirectAssessmentTypeRepository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(payeDirectAssessmentTypeRepository, user, orchardServices)
        {
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            _payeDirectAssessmentTypeRepository = payeDirectAssessmentTypeRepository;
        }

        /// <summary>
        /// Gets all the Direct assessment types
        /// </summary>
        /// <returns>A list of all active direct assessment types</returns>
        public IEnumerable<PAYEDirectAssessmentTypeVM> GetAll()
        {
            return _transactionManager.GetSession().Query<PAYEDirectAssessmentType>().Where(directAssessment => directAssessment.IsActive).Select(x => new PAYEDirectAssessmentTypeVM {Name = x.Name, Id = x.Id }).ToFuture();
        }
    }
}