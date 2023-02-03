using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface ITINFormManager<TINRegistrationForm> : IDependency, IBaseManager<TINRegistrationForm>
    {
       IEnumerable<TINApplicantReportModel> TINApplicantSearch(string firstname, string lastName, string phone, DateTime? startDate, DateTime? endDate);
        void SaveAssetsRecord(Asset assetsRecord);
        void SaveAddressRecord(Address businessAddress);
        void SaveApplicantRecord(Applicant appliantFormData);
        IEnumerable<TINRegistrationForm> GetAllTINApplicants();
        void SaveTINRecord(Models.TINRegistrationForm tinRegistrationFormData);
        TaxEntity SaveTaxEntityRecord(TaxEntity taxEntity);
        bool UpdateTINApplicantRecord(long tINId, string TINValue);
        TaxEntityCategory GetTaxCategory(string individual);
    }
}
