using System;

namespace Parkway.CBS.Police.Core.DTO
{
    public class EscortDetailsDTO
    {
        public Int64 Id { get; set; }

        public int NumberOfOfficers { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string CustomerName { get; set; }

        public string FileRefNumber { get; set; }

        public string Address { get; set; }

        public int LGAId { get; set; }

        public string LGAName { get; set; }

        public int StateId { get; set; }

        public string StateName { get; set; }

        public bool OfficersHaveBeenAssigned { get; set; }

        public int SubSubTaxCategoryId { get; set; }

        public int PSSEscortServiceCategoryId { get; set; }

        public int NumberOfOfficersAssigned { get; set; }

        public int CommandTypeId { get; set; }

        public string OriginLGAName { get; set; }

        public int OriginStateId { get; set; }

        public string OriginStateName { get; set; }

        public string OriginAddress { get; set; }
    }
}