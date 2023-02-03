using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.Validations.FormValidators.Contracts
{
    public interface IFormValidator
    {

        void ValidateFormFields(ref List<ErrorModel> errors, FormControlViewModel formField);
    }
}
