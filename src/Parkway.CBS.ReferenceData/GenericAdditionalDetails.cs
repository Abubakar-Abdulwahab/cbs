using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ReferenceData
{
    internal class GenericAdditionalDetails
    {
        /// <summary>
        /// Identifier name
        /// </summary>
        public virtual string IdentifierName { get; set; }

        /// <summary>
        /// Identifier technical name
        /// </summary>
        public virtual string IdentifierTechnicalName { get; set; }

        /// <summary>
        /// Identifier value
        /// </summary>
        public virtual string IdentifierValue { get; set; }

        /// <summary>
        /// Is this field compulsory
        /// </summary>
        public virtual bool IsCompulsory { get; set; }
    }
}
