using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSServiceOptionsVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int ServiceOptionId { get; set; }

        public string OptionType { get; set; }
    }
}