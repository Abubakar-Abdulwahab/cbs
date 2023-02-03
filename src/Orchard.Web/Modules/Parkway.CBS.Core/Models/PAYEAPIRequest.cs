using System;
using System.Collections.Generic;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.Models
{
    public class PAYEAPIRequest : CBSModel
    {
        public virtual TaxEntity TaxEntity { get; set; }


        public virtual ExpertSystemSettings RequestedByExpertSystem { get; set; }

        public virtual PAYEBatchRecordStaging PAYEBatchRecordStaging { get; set; }

        /// <summary>
        /// Batch identifier sent by the caller party
        /// </summary>
        public virtual string BatchIdentifier { get; set; }

        public virtual int BatchLimit { get; set; }

        /// <summary>
        /// URL where the validation notification to the caller will be sent
        /// </summary>
        public virtual string CallbackURL { get; set; }

        /// <summary>
        /// indicate the status of the request processing
        /// <see cref="PAYEAPIProcessingStages"/>
        /// </summary>
        public virtual int ProcessingStage { get; set; }

        /// <summary>
        /// Once the request processing for this record has been completed
        /// this flag is set to true
        /// </summary>
        public virtual bool ProcessingCompleted { get; set; }
        

    }
}