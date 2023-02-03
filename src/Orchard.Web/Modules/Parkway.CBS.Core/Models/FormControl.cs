using Orchard.Users.Models;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.Models
{
    public class FormControl : CBSModel
    {
        public virtual string Name { get; set; }
        public virtual string TechnicalName { get; set; }
        public virtual int ControlTypeNumber { get; set; }
        public virtual int ControlTypeDropDownNumber { get; set; }
        public virtual string FriendlyName { get; set; }
        public virtual string LabelText { get; set; }
        public virtual string HintText { get; set; }
        public virtual string PlaceHolderText { get; set; }
        public virtual string Validators { get; set; }
        public virtual bool DefaultStatus { get; set; }
        public virtual int ElementType { get; set; }

        public virtual string PartialName { get; set; }


        public virtual bool IsComplexValidator { get; set; }

        public virtual string PartialModelProvider { get; set; }

        public virtual UserPartRecord LastUpdatedBy { get; set; }

        /// <summary>
        /// LastUpdatedBy id of the admin that added the form record.
        /// Only for audit/reference purposes, always use the LastUpdatedBy property if you want to know
        /// the user that performed the last update, which should suffice for most cases.
        /// </summary>
        public virtual UserPartRecord AddedBy { get; set; }


        public string ValidationProps { get; set; }

        public ControlTypes ControlType(int controlNumber)
        {
            return (ControlTypes)controlNumber;
        }

        public ControlTypeDropDownType ControlTypeDropDown(int controlNumber)
        {
            return (ControlTypeDropDownType)ControlTypeDropDownNumber;
        }

        public ElementTypeEnum FormElementType(int elementType)
        {
            return (ElementTypeEnum)elementType;
        }
    }    
}