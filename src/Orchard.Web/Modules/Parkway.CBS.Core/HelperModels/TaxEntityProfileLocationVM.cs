using System;

namespace Parkway.CBS.Core.HelperModels
{
    public class TaxEntityProfileLocationVM
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CreatedAtParsed => CreatedAt.ToString("dd/MM/yyyy");

        public string Name { get; set; }

        public int State { get; set; }

        public string StateName { get; set; }

        public TaxEntityViewModel TaxEntity { get; set; }

        public int LGA { get; set; }

        public string LGAName { get; set; }

        public string Address { get; set; }

        public Int64 TaxEntityId { get; set; }

        public string Code { get; set; }
    }
}