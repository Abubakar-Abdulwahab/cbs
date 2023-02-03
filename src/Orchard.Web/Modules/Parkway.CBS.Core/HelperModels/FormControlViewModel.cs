using Parkway.CBS.Core.FormControlsComposition;
using Parkway.CBS.Core.Validations.FormValidators.Properties;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class FormControlViewModel
    {

        /// <summary>
        /// the control Id on the form revenue head table
        /// </summary>
        public Int64 ControlIdentifier { get; set; }

        public  string Name { get; set; }

        public  string TechnicalName { get; set; }

        public  int ControlTypeNumber { get; set; }

        public  int ControlTypeDropDownNumber { get; set; }

        public  string FriendlyName { get; set; }

        public  string LabelText { get; set; }

        public  string HintText { get; set; }

        public  string PlaceHolderText { get; set; }

        public  bool DefaultStatus { get; set; }

        public  int ElementType { get; set; }

        public string PartialName { get; set; }

        public bool IsCompulsory { get; set; }

        /// <summary>
        /// this prop hold the data for the partial view
        /// </summary>
        public dynamic PartialModel { get; set; }

        public int TaxEntityCategoryId { get; set; }

        public string FormValue { get; set; }

        public int Position { get; set; }


        /// <summary>
        /// this hold the JSON string of the class to be instantiated 
        /// to execute code that give the partial view its data
        /// </summary>
        public string PartialProvider { get; set; }


        /// <summary>
        /// JSON string of the validators for this form prop
        /// </summary>
        public string Validators { get; set; }


        /// <summary>
        /// Validation props are the properties you might likely need to ensure proper
        /// validations
        /// </summary>
        public string ValidationProps { get; set; }


        public int FormIndex { get; set; }

        public int RevenueHeadId { get; set; }

        public int FormId { get; internal set; }

        public int Index { get; set; }

        /// <summary>
        /// list of validators
        /// </summary>
        public List<FormValidatorModel> ValidatorsModels { get; internal set; }

        internal FormControlsCompositionModel FormPartialCompostionModel { get; set; }
    }
}