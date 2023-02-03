using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientFileServices.Implementations.Models
{
    public class ValidateFileResponse
    {
        public bool ErrorOccurred { get; internal set; }

        public Int64 BatchId { get; internal set; }

        public string ErrorMessage { get; internal set; }
    }
}
