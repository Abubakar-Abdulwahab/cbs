using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class PAYEDirectAssessmentType : CBSModel
    {
        public virtual string Name { get; set; }
        public virtual bool IsActive { get; set; }

    }
}