using Orchard;
using Orchard.Autoroute.Services;
using Orchard.FileSystems.Media;
using Orchard.Localization;
using Orchard.MediaLibrary.Services;
using Orchard.Modules.Services;
using Orchard.Security;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.HTTP.RemoteClient.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Validations;
using Parkway.CBS.Core.Validations.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreMDARevenueHeadService : CoreBaseService, ICoreMDARevenueHeadService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IAuthorizer _authorizer;
        public Localizer T { get; set; }
        private readonly IValidator _validator;
        private readonly ISlugService _slugService;

        public CoreMDARevenueHeadService(IOrchardServices orchardServices, ISlugService slugService, IValidator validator,
            IMediaLibraryService mediaManagerService, IMimeTypeProvider mimeTypeProvider)
            : base(orchardServices, mediaManagerService, mimeTypeProvider)
        {
            _orchardServices = orchardServices;
            _authorizer = orchardServices.Authorizer;
            T = NullLocalizer.Instance;
            _validator = validator;
            _slugService = slugService;
        }

        /// <summary>
        /// Trim a collection of Ms from MDARevenueHeadBaseModel
        /// </summary>
        /// <typeparam name="M">Model</typeparam>
        /// <param name="collection">model</param>
        /// <returns>typeparam name="T"<typeparamref name="MDARevenueHeadHandler"/></returns>
        public void TrimString<M>(ICollection<M> collection) where M : MDARevenueHead
        {
            foreach (var model in collection)
            {
                model.Name = model.Name.Trim().ToLower().ToUpper();
                model.Code = model.Code.Trim().ToLower().ToUpper();
            }
        }

        /// <summary>
        /// Transform the collection of MDARevenueHeadBaseModel model into a collection of
        /// UniqueValidationModel and check if there are duplicate SelectDataValue
        /// </summary>
        /// <typeparam name="C">Controller type</typeparam>
        /// <typeparam name="M">MDARevenueHeadBase</typeparam>
        /// <typeparam name="T">Subclass of MDARevenueHeadHandler</typeparam>
        /// <param name="callback">Controller</param>
        /// <param name="collection">ICollection{M}</param>
        /// <exception cref="DirtyFormDataException"></exception>
        /// <returns><typeparamref name="T"/>Subclass of MDARevenueHeadHandler</returns>
        protected List<ErrorModel> ValidateInnerCollection<M>(ICollection<M> collection) where M : MDARevenueHead
        {
            if (!collection.Any()) return new List<ErrorModel>();
            ICollection<UniqueValidationModel> dataValues = new List<UniqueValidationModel>();
            int counter = 0;

            foreach (var model in collection)
            {
                dataValues.Add(new UniqueValidationModel() { Identifier = "RevenueHeadsCollection[" + counter + "].Name", Name = "Name", SelectDataValue = model.Name, InclusiveClauses = new string[] { }, ErrorMessage = "This form value has been duplicated" });
                dataValues.Add(new UniqueValidationModel() { Identifier = "RevenueHeadsCollection[" + counter++ + "].Code", Name = "Code", SelectDataValue = model.Code, InclusiveClauses = new string[] { }, ErrorMessage = "This form value has been duplicated" });
            }
            var validator = _validator.BundleCollectionUnique<M>(dataValues);
            return validator.HasValidationErrors();
        }

        /// <summary>
        /// Validate that the records in datavalues are unique
        /// </summary>
        /// <typeparam name="M">MDARevenueHead</typeparam>
        /// <param name="dataValues">List{UniqueValidationModel}</param>
        /// <returns>List{ErrorModel}</returns>
        public List<ErrorModel> ValidateUniqueness<M>(List<UniqueValidationModel> dataValues) where M : CBSModel
        {
            if (!dataValues.Any()) return new List<ErrorModel>();
            var validator = _validator.BundleUnique<M>(dataValues);
            return validator.HasValidationErrors();
        }

        /// <summary>
        /// Set orchard compliant slug value for the corresponding items in the collection
        /// </summary>
        /// <typeparam name="M">MDARevenueHead <see cref="MDARevenueHead"/></typeparam>
        /// <param name="collection">Collection of MDARevenueHead</param>
        public void SetSlug<M>(ICollection<M> collection) where M : MDARevenueHead
        {
            foreach (var model in collection) { model.Slug = _slugService.Slugify(String.Format("{0} {1}", model.Name, model.Code)); }
        }
    }
}