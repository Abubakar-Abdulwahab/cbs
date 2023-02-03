using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Payee.PayeeAssessments.Contracts
{
    public interface IPayeeAssessmentInterface : IDependency
    {
        /// <summary>
        /// Generate assessments
        /// </summary>
        void GenerateAssessments();
             
    }
}
