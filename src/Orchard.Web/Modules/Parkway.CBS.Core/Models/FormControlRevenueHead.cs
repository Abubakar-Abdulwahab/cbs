using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class FormControlRevenueHead : CBSBaseModel
    {
        public virtual long Id { get; set; }

        public virtual TaxEntityCategory TaxEntityCategory { get; set; }

        public virtual string MetaData { get; set; }
        
        public virtual RevenueHead RevenueHead { get; set; }

        public virtual FormControl Form { get; set; }

        public virtual bool IsComplusory { get; set; }

        public virtual int Position { get; set; }
    }   
}