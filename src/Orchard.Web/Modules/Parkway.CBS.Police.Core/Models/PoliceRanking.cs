using System;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PoliceRanking : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string RankName { get; set; }

        public virtual int RankLevel { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual string ExternalDataRankId { get; set; }

        public virtual string ExternalDataCode { get; set; }
    }
}