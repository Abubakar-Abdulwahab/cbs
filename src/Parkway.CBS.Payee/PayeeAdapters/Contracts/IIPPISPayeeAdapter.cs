using Parkway.CBS.Payee.Models;
using Parkway.CBS.Payee.PayeeAdapters.IPPIS;
using System.Collections.Generic;

namespace Parkway.CBS.Payee.PayeeAdapters.Contracts
{
    public interface IIPPISPayeeAdapter
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


        ///// <summary>
        ///// Group the line records by whatever implementation lies on the other side
        ///// </summary>
        ///// <typeparam name="G">G : LineRecordModel</typeparam>
        ///// <param name="payees">ConcurrentStack{G}</param>
        ///// <returns>IEnumerable{BG}</returns>
        //IEnumerable<BG> GetPayeesGroup<G, BG>(ConcurrentStack<G> payees) where G : LineRecordModel where BG : BatchGroup;


        List<IPPISPayeeAmountMinistrySummary> GetIPPISPayeeMinistrySummary(List<IPPISAssessmentLineRecordModel> payees);

        /// <summary>
        /// Convert excel file to CSV
        /// </summary>
        /// <param name="processingFilePath"></param>
        /// <param name="destinationFilePath"></param>
        /// <returns>HeaderValidateObject</returns>
        HeaderValidateObject ConvertExcelToCSV(string processingFilePath, string destinationFilePath, SettlementDetails settlementDetails);

    }

}
