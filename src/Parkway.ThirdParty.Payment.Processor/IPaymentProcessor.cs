using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.ThirdParty.Payment.Processor
{
    public interface IPaymentProcessor
    {
        void GetProcessor(string requestStream);
    }
}
