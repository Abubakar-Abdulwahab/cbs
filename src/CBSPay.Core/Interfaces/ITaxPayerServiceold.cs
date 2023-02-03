using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.Interfaces
{
    public interface ITaxPayerServiceold
    {
        void RetrieveTaxPayerInfo(string assessmentNumber, string phoneNumber);
    }
}
