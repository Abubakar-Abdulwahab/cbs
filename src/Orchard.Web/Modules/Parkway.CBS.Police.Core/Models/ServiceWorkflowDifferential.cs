using Parkway.CBS.Core.Models;


namespace Parkway.CBS.Police.Core.Models
{
    public class ServiceWorkflowDifferential : CBSModel
    {
        public virtual PSService Service { get; set; }

        public virtual string Name { get; set; }

        /// <summary>
        /// this would hold the model name for the type name
        /// that the differential value maps to
        /// For example the differential model name here for escort will be
        /// the model name for the model that hold the value of the diff value PSSEscortServiceCategory
        /// with this we are guarantee that there will be no clashing between model that have the same diff value
        /// </summary>
        public virtual string DifferentialModelName { get; set; }

        /// <summary>
        /// this would hold the value that differentiates the service and the flow definitions
        /// for each value that needs to be held.
        /// For example the escort service requires that based on the category the workflow 
        /// should be different, here the differentiates value will be the ID of the category
        /// </summary>
        public virtual int DifferentialValue { get; set; }


        public virtual bool IsActive { get; set; }

        /// <summary>
        /// this describes the flow to approval for requests made on this service
        /// </summary>
        public virtual PSServiceRequestFlowDefinition FlowDefinition { get; set; }

    }
}