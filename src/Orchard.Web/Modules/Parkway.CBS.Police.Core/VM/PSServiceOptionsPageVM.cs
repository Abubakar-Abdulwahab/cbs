using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSServiceOptionsPageVM : RequestDumpVM
    {
        public IEnumerable<PSServiceOptionsVM> Options { get; set; }

        public int SelectedOption { get; set; }
    }
}