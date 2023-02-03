using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.Validations.FormValidators.Properties
{
    public class FormValidatorModel
    {
        public bool IsComplexValidator { get; set; }

        public string ValidatorClass { get; set; }

        public string ValidatorAssembly { get; set; }


        public string ValidatorAssemblyAndClass
        {
            get
            {
                return this.ValidatorAssembly + this.ValidatorClass;
            }
        }
    }


    public class FormControlVMFormValidatorModel
    {
        public FormControlViewModel FormControlViewModel { get; set; }

        public FormValidatorModel FormValidatorModel { get; set; }
    }

}