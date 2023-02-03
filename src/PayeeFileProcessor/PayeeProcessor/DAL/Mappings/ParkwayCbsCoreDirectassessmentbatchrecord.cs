using System; 
using System.Collections.Generic; 
using System.Text; 
using System; 


namespace PayeeProcessor.DAL.Model {
    
    public class ParkwayCbsCoreDirectassessmentbatchrecord {
        public ParkwayCbsCoreDirectassessmentbatchrecord() { }
        public virtual long Id { get; set; }
        public virtual string Tin { get; set; }
        public virtual string Filepath { get; set; }
        public virtual string Adaptervalue { get; set; }
        public virtual string Rules { get; set; }
        public virtual string Batchref { get; set; }
        public virtual System.Nullable<decimal> Amount { get; set; }
        public virtual System.DateTime Createdatutc { get; set; }
        public virtual System.DateTime Updatedatutc { get; set; }
        public virtual System.Nullable<double> Percentageprogress { get; set; }
        public virtual System.Nullable<int> Totalnoofrowsprocessed { get; set; }
    }
}
