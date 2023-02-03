using System;

namespace Parkway.CBS.Core.HelperModels
{
    public class CreateCustomerResponse : BaseRequestResponse
    {
        public virtual Int64 Id { get; set; }

        public virtual String Name { get; set; }

        public virtual String Address { get; set; }
        public virtual Int32 CountryID { get; set; }
        public virtual Int32 StateID { get; set; }
        //public virtual TaxEntityType Type { get; set; }
        public virtual Contact PryContact { get; set; }

        public virtual Int64 PrimaryContactId { get; set; }

        public class Contact
        {
            public virtual Int64 Id { get; set; }
            public virtual String Name { get; set; }
            public virtual String Email { get; set; }

        }
    }
}