using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Validations
{
    public class CollectionValidationModel
    {
        public int Id { get; set; }
        public int ControlTypeNumber { get; set; }
        public string Name { get; set; }
        public string Validators { get; set; }
    }
}