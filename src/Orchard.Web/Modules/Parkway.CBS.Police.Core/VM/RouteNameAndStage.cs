using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.Core.VM
{
    public class RouteNameAndStage
    {
        public string RouteName { get; set; }

        public PSSUserRequestGenerationStage Stage { get; set; }
    }
}