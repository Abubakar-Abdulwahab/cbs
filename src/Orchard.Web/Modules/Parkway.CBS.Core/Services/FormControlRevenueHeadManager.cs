using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Core.Services
{
    public class FormControlRevenueHeadManager : BaseManager<FormControlRevenueHead>, IFormControlRevenueHeadManager<FormControlRevenueHead>
    {
        private readonly IRepository<FormControlRevenueHead> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public FormControlRevenueHeadManager(IRepository<FormControlRevenueHead> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _transactionManager = orchardServices.TransactionManager;
        }



        /// <summary>
        /// Get the form controls for the given params
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="categoryId"></param>
        public IEnumerable<FormControlViewModel> GetDBForms(int revenueHeadId, int categoryId)
        {
            return _transactionManager.GetSession().Query<FormControlRevenueHead>()
           .Where(x => (x.RevenueHead == new RevenueHead { Id = revenueHeadId }) && (x.TaxEntityCategory == new TaxEntityCategory { Id = categoryId }))
           .Select(f => new FormControlViewModel
           {
               Name = f.Form.Name,
               PartialName = f.Form.PartialName,
               ControlIdentifier = f.Id,
               HintText = f.Form.HintText,
               PlaceHolderText = f.Form.PlaceHolderText,
               FriendlyName = f.Form.FriendlyName,
               LabelText = f.Form.LabelText,
               PartialProvider = f.Form.PartialModelProvider,
               TaxEntityCategoryId = f.TaxEntityCategory.Id,
               TechnicalName = f.Form.TechnicalName,
               Position = f.Position,
               IsCompulsory = f.IsComplusory,
               Validators = f.Form.Validators,
               ValidationProps = f.Form.ValidationProps,
               RevenueHeadId = f.RevenueHead.Id,
           }).ToFuture();
        }


        /// <summary>
        /// Get the form controls for the given params
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="categoryId"></param>
        public IEnumerable<FormControlViewModel> GetFormsForView(int revenueHeadId, int categoryId)
        {
            return _transactionManager.GetSession().Query<FormControlRevenueHead>()
           .Where(x => (x.RevenueHead == new RevenueHead { Id = revenueHeadId }) && (x.TaxEntityCategory == new TaxEntityCategory { Id = categoryId }))
           .Select(f => new FormControlViewModel
           {
               PartialName = f.Form.PartialName,
               ControlIdentifier = f.Id,
               HintText = f.Form.HintText,
               PlaceHolderText = f.Form.PlaceHolderText,
               FriendlyName = f.Form.FriendlyName,
               LabelText = f.Form.LabelText,
               PartialProvider = f.Form.PartialModelProvider,
               TaxEntityCategoryId = f.TaxEntityCategory.Id,
               TechnicalName = f.Form.TechnicalName,
               Position = f.Position,
               IsCompulsory = f.IsComplusory,
               Validators = f.Form.Validators,
               ValidationProps = f.Form.ValidationProps,
               RevenueHeadId = f.RevenueHead.Id,
           }).ToFuture();
        }

    }
}