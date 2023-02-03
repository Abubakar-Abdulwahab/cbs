using Parkway.CBS.Payee.PayeeAdapters;
using Parkway.CBS.Payee.PayeeAdapters.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Payee
{
    public class DirectAssessmentPayee : IDirectAssessmentPayee
    {
        public IPayeeAdapter GetAdapter(string className)
        {
            var adapterSplit = className.Split(',');
            var implementingClass = Activator.CreateInstance(adapterSplit[1].Trim(), adapterSplit[0].Trim());
            return (IPayeeAdapter)implementingClass.Unwrap();
        }

        
        public GetPayeResponse ReadFile(string filePath, string LGAFilePath, string stateName)
        {
            return new FileReader().ReadFile(filePath, LGAFilePath, stateName);
        }
    }

    public interface IDirectAssessmentPayee
    {
        GetPayeResponse ReadFile(string filePath, string LGAFilePath, string stateName);

        IPayeeAdapter GetAdapter(string className);
    }
}
