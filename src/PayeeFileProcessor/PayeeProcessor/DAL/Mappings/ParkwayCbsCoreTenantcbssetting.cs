using System; 
using System.Collections.Generic; 
using System.Text; 
using System; 


namespace PayeeProcessor.DAL.Model {
    
    public class ParkwayCbsCoreTenantcbssetting {
        public ParkwayCbsCoreTenantcbssetting() { }
        public virtual int Id { get; set; }
        public virtual string Identifier { get; set; }
        public virtual string Statename { get; set; }
        public virtual int Stateid { get; set; }
        public virtual string Countryid { get; set; }
        public virtual string Countryname { get; set; }
        public virtual int AddedbyId { get; set; }
        public virtual int LastupdatedbyId { get; set; }
        public virtual System.DateTime Createdatutc { get; set; }
        public virtual System.DateTime Updatedatutc { get; set; }
    }
}
