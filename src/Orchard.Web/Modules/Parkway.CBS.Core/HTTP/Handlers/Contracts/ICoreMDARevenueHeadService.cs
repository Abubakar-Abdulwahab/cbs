using Orchard;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;
using Parkway.CBS.Core.Validations;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreMDARevenueHeadService : IDependency
    {
        /// <summary>
        /// Trim name and code values. Collection type must inherit from MDARevenueHead.
        /// </summary>
        /// <typeparam name="M">MDARevenueHead</typeparam>
        /// <param name="collection">collection of MDARevenueHead</param>
        void TrimString<M>(ICollection<M> collection) where M : MDARevenueHead;

        List<ErrorModel> ValidateUniqueness<M>(List<UniqueValidationModel> dataValues) where M : CBSModel;

        /// <summary>
        /// Set orchard compliant slug value for the corresponding items in the collection
        /// </summary>
        /// <typeparam name="M">MDARevenueHead <see cref="MDARevenueHead"/></typeparam>
        /// <param name="collection">Collection of MDARevenueHead</param>
        void SetSlug<M>(ICollection<M> collection) where M : MDARevenueHead;
    }
}
