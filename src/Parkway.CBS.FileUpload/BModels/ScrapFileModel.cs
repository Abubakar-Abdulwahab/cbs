using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.FileUpload.BModels
{
    public class ScrapFileModel
    {
        public int LineNumber { get; set; }

        public string Message { get; set; }

        public string TIN { get; set; } //tin

        public string Prev_TIN { get; set; }//

        public string CombinedName { get; set; }//

        public string Sname { get; set; }//

        public string Fname { get; set; }//

        public string MName { get; set; }//

        public DateTime? BOD { get; set; }//

        public string Gender { get; set; }//

        public DateTime? RegDate { get; set; }//

        public string Address { get; set; }

        public string HouseNo { get; set; }//

        public string Street { get; set; }//

        public string City { get; set; }//

        public string Lga { get; set; }//

        public string Ward { get; set; }//

        public string Email { get; set; }//

        public string Phone { get; set; }//

        public string Occupation { get; set; }//

        public string Source { get; set; }//

        public string Nationality { get; set; } //

        public string State { get; set; }//

        public string StateO { get; set; }//

        public string TaxAuth { get; set; }//
    }
}
