using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class ReferenceDataAsset : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual int AssetTypeId { get; set; }

        public virtual string Fullname { get; set; }

        public virtual string Address { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual string Email { get; set; }

        public virtual string TIN { get; set; }

        public virtual LGA LGA { get; set; }

        public virtual TaxEntity TaxEntity { get; set; }

        public AssetType GetAssetType()
        {
            return (AssetType)this.AssetTypeId;
        }


    }
}