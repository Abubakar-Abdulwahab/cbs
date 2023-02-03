using System.Collections.Generic;
using Parkway.CBS.Police.Core.DTO;

namespace Parkway.CBS.Police.Core.VM
{
    public class GenericRequestDetails : RequestTypeVM
    {
        public IEnumerable<PSSServiceRequestDTO> ServiceRequests { get; set; }

    }
}