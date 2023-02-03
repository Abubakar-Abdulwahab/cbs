using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.HelperModels
{
    public class PenaltyModel
    {
        public PenaltyValueType PenaltyValueType { get; set; }
        public EffectiveFromType EffectiveFromType { get; set; }
        public decimal Value { get; set; }
        public int EffectiveFrom { get; set; }
    }
}