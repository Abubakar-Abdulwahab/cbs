using Orchard.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class PAYEDirectAssessmentRecord : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual UserPartRecord AssessedBy { get; set; }

        public virtual ExpertSystemSettings AssessedByExternalExpertSystem { get; set; }

        public virtual PAYEBusinessType PAYEBusinessType { get; set; }

        public virtual int Month { get; set; }

        public virtual int Year { get; set; }

        public virtual PAYEBusinessSize PAYEBusinessSize { get; set; }

        public virtual Invoice Invoice { get; set; }

        public virtual decimal IncomeTaxPerMonth { get; set; }
       
        public virtual PAYEDirectAssessmentType AssessmentType { get; set; }

        /// <summary>
        /// Tax officer comment for best of judgment direct assessment
        /// </summary>
        public virtual string Comment { get; set; }
    }
}