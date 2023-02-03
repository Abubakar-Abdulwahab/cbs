using Orchard.Users.Models;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class EscortAmountChartSheet : CBSModel
    {
        public virtual PoliceRanking Rank { get; set; }

        public virtual decimal Rate { get; set; }

        public virtual PSSEscortServiceCategory PSSEscortServiceCategory { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual UserPartRecord AddedBy { get; set; }

        public virtual LGA LGA { get; set; }

        public virtual StateModel State { get; set; }

        public virtual CommandType CommandType { get; set; }

        public virtual PSSEscortDayType PSSEscortDayType { get; set; }
    }
}