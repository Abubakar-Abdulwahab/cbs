using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSRequestEditPSServiceFormFields : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PSSRequestEdit RequestEdit { get; set; }

        public virtual PSServiceFormFields PSServiceFormFields { get; set; }
    }
}