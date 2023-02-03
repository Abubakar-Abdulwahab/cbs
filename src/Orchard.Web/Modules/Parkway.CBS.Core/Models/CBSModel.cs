using System;

namespace Parkway.CBS.Core.Models
{
    public abstract class CBSModel : CBSBaseModel
    {
        public virtual int Id { get; set; }       
    }
}