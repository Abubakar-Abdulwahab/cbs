using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSEscortOfficerDetails : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PSSRequest Request { get; set; }

        public virtual PoliceRanking PoliceRanking { get; set; }

        public virtual int Quantity { get; set; }

        public virtual decimal RankAmountRate { get; set; }

        public virtual decimal TotalAmount { get; set; }

        public Command Command { get; set; }

        public string IPPISNumber { get; set; }

        public string Name { get; set; }
    }   
}