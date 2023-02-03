using System;
using System.Collections.Generic;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSRequest : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PSService Service { get; set; }

        /// <summary>
        /// <see cref="Enums.PSSRequestStatus"/>
        /// </summary>
        public virtual int Status { get; set; }

        /// <summary>
        /// File Ref Number
        /// </summary>
        public virtual string FileRefNumber { get; set; }

        public virtual TaxEntity TaxEntity { get; set; }

        public virtual Command Command { get; set; }

        public virtual string ApprovalNumber { get; set; }

        public virtual string ServicePrefix { get; set; }

        public virtual string Reason { get; set; }


        public virtual IEnumerable<PoliceServiceRequest> ServiceRequests { get; set; }


        /// <summary>
        /// this field will hold the hash value of the request parameters for the callback endpoint
        /// <para>This is used to ascertain that the called endpoint indeed is the endpoint that should be called by comparing the hash
        /// value from the request body with the persisted value in the database</para>
        /// </summary>
        public virtual string ExpectedHash { get; set; }


        /// <summary>
        /// this tells us where this request is
        /// <para>It gives us the means to check if there are more invoice to be generated based on the 
        /// ApplicationRequestStage on the PSServiceRevenueHead.
        /// So if this value is 1, and there is a value for this same service with the ApplicationRequestStage 2
        /// then after the request status has been changed we will generate another invoice for this request
        /// </para>
        /// </summary>
        public virtual PSServiceRequestFlowDefinitionLevel FlowDefinitionLevel { get; set; }


        public virtual ICollection<PSSRequestInvoice> Invoices { get; set; }

        public virtual string ContactPersonName { get; set; }

        public virtual string ContactPersonEmail { get; set; }

        public virtual string ContactPersonPhoneNumber { get; set; }

        public virtual TaxEntityProfileLocation TaxEntityProfileLocation { get; set; }

        public virtual CBSUser CBSUser { get; set; }

    }
}