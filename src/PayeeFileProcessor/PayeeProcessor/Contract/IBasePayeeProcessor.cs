using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayeeProcessor.Contract
{
    public interface IBasePayeeProcessor
    {
        void ProcessPayeeFile(string fileName, string filePath);

        void StopProcessingFile();
         
    }
}
