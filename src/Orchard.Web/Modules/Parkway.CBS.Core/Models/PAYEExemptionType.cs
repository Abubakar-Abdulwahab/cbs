using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class PAYEExemptionType : CBSBaseModel
    {
        public virtual int Id { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual string Name { get; set; }
    }
}