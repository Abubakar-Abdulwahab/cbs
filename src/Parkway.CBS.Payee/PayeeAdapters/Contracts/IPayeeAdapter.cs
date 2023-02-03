using Parkway.CBS.Payee.Models;
using Parkway.CBS.Payee.PayeeAdapters.IPPIS;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Parkway.CBS.Payee.PayeeAdapters.Contracts
{
    public interface IPayeeAdapter
    {

        /// <summary>
        /// Get the response to reading the file and processing the file from the file path
        /// </summary>
        /// <typeparam name="PR">the Payee model 
        /// <see cref="IPPISPayeeResponse"/>
        /// <see cref="PayeeResponseModel"/>
        /// </typeparam>
        /// <param name="filePath"></param>
        /// <param name="LGAFilePath"></param>
        /// <param name="stateName"></param>
        /// <returns>IR</returns>
        IR GetPayeeResponseModels<IR>(string filePath, string LGAFilePath, string stateName, int month = 0, int year = 0) where IR : GetPayeResponse;


        GetPayeResponse GetPayeeModels(string filePath, string LGAFilePath, string stateName);


        PayeeAmountAndBreakDown GetRequestBreakDown(List<PayeeAssessmentLineRecordModel> payees);


        List<PayeeAssessmentLineRecordModel> GetPAYEModels<T>(ICollection<T> lines, string LGAFilePath, string stateName);

    }
}
