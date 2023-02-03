using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CentralBillingScheduler.DAO.Models
{
    public class CBSTenantSettings : CBSModel
    {
        public virtual string ClientId { get; set; }
        public virtual string ClientSecret { get; set; }

        /// <summary>
        /// TSA bank Id
        /// </summary>
        public virtual int TSA { get; set; }

        /// <summary>
        /// Bank code
        /// </summary>
        public virtual string BankCode { get; set; }

        /// <summary>
        /// TSA bank account number
        /// </summary>
        public virtual string TSABankNumber { get; set; }

        public virtual string CompanyEmail { get; set; }

        public virtual string CompanyAddress { get; set; }

        public virtual int StateID { get; set; }

        public virtual string StateName { get; set; }

        public virtual int CountryID { get; set; }

        public virtual string CompanyName { get; set; }

        public virtual string CompanyMobile { get; set; }

        public virtual string BusinessNature { get; set; }

        public virtual string LogoPath { get; set; }

        public virtual string SignaturePath { get; set; }

        /// <summary>
        /// Reference data source type. this is the source type e.g adapter
        /// </summary>
        public virtual string ReferenceDataSourceType { get; set; }

        /// <summary>
        /// Reference data source name, this is the name of the reference data
        /// </summary>
        public virtual string ReferenceDataSourceName { get; set; }

        /// <summary>
        /// if the reference data source type is of adapter type. you are required to provide this field with the class full name got from the config file
        /// </summary>
        public virtual string AdapterClassName { get; set; }

        /// <summary>
        /// Scheduler identifier
        /// </summary>
        public virtual string BillingSchedulerIdentifier { get; set; }

        public virtual string SiteName { get; set; }
    }
}
