using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IPAYEDirectAssessmentTypeManager<PAYEDirectAssessmentType> : IDependency, IBaseManager<PAYEDirectAssessmentType>
    {
        /// <summary>
        /// Gets all the Direct assessment types
        /// </summary>
        /// <returns>A list of all active direct assessment type name and Id</returns>
        IEnumerable<PAYEDirectAssessmentTypeVM> GetAll();
    }
}
