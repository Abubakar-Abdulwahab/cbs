using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Parkway.CBS.Core.HelperModels
{
    public class CollectionFormControlsViewModel
    {
        public ControlTypes ControlType { get; set; }
        public string PlaceHolderText { get; set; }
        public string Value { get; set; }
        public string ControlName { get; set; }
        public int ControlId { get; set; }
        public string FriendlyName { get; set; }
        public string LabelText { get; set; }
        public string ControlTypeString { get; set; }
    }


    public class CollectionFormVM
    {
        public string FormValue { get; set; }
        public int FormIdentifier { get; set; }
        public string FormLabel { get; set; }
        public string FormControlTypeString { get; set; }
    }

    public class CollectionForm
    {
        public bool IsDirectAssessment { get; set; }

        public bool HasErrors { get; set; }

        public List<CollectionFormVM> Forms { get; set; } 

        public string ErrorMessage { get; set; }
    }

    public class DirectAssessmentCollection
    {
        [StringLength(30, ErrorMessage = "RCNumber value must be between 5 and 30 characters", MinimumLength =5)]
        public string RCNumber { get; set; }

        [StringLength(250, ErrorMessage = "Company name must be between 2 and 250 characters", MinimumLength =2)]
        public string CompanyName { get; set; }

        [StringLength(50, ErrorMessage = "Email value must be between 5 and 50 characters", MinimumLength = 5)]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(15, ErrorMessage = "Add a valid phone number", MinimumLength = 5)]
        public string PhoneNumber { get; set; }

        [StringLength(250, ErrorMessage = "Address must be between 5 and 250 characters", MinimumLength = 5)]
        public string Address { get; set; }

        public string FilePath { get; set; }

        public string AdapterValue { get; set; }

        public string BatchToken { get; set; }

        public FileProcessModel BatchTokenObj { get; set; }

    }
}