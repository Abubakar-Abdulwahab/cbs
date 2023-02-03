using System; 
using System.Collections.Generic; 
using System.Text; 
using System; 


namespace PayeeProcessor.DAL.Model {
    
    public class ParkwayCbsCoreDirectassessmentpayeerecord {
        public ParkwayCbsCoreDirectassessmentpayeerecord() { }
        public virtual long Id { get; set; }
        public virtual string Grossannual { get; set; }
        public virtual string Exemptions { get; set; }
        public virtual string Incometaxpermonth { get; set; }
        public virtual System.Nullable<decimal> Incometaxpermonthvalue { get; set; }
        public virtual string Tin { get; set; }
        public virtual string Month { get; set; }
        public virtual string Year { get; set; }
        public virtual string Email { get; set; }
        public virtual string Phonenumber { get; set; }
        public virtual string Payeename { get; set; }
        public virtual bool Haserrors { get; set; }
        public virtual string Errormessages { get; set; }
        public virtual long DirectassessmentbatchrecordId { get; set; }
        public virtual System.DateTime Createdatutc { get; set; }
        public virtual System.DateTime Updatedatutc { get; set; }
    }
}
