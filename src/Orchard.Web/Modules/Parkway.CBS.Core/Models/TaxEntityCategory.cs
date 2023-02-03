using Newtonsoft.Json;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class TaxEntityCategory : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual int Identifier { get; set; }

        public virtual string StringIdentifier { get; set; }

        public virtual bool Status { get; set; }

        public virtual bool RequiresLogin { get; set; }

        /// <summary>
        /// this gives a json representation of the settings that this category has
        /// <see cref="TaxEntityCategorySettings"/>
        /// </summary>
        public virtual string JSONSettings { get; set; }


        public TaxEntityCategorySettings GetSettings()
        {
            if (string.IsNullOrEmpty(this.JSONSettings)) { return new TaxEntityCategorySettings { }; }
            return JsonConvert.DeserializeObject<TaxEntityCategorySettings>(this.JSONSettings);
        }
    }
}