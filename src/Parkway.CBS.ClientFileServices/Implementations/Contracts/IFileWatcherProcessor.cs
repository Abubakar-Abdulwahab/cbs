using System;
using System.IO;
using Parkway.CBS.Entities.VMs;
using Parkway.CBS.ClientFileServices.Implementations.Models;


namespace Parkway.CBS.ClientFileServices.Implementations.Contracts
{
    public interface IFileWatcherProcessor
    {

        /// <summary>
        /// Get the service details
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="directoryInfo"></param>
        /// <param name="sstateId"></param>
        /// <param name="sunknownTaxPayerCodeId"></param>
        /// <param name="filePath"></param>
        /// <returns>FileServiceHelper</returns>
        FileServiceHelper GetFileServiceHelper(string tenantName, DirectoryInfo directoryInfo, string sstateId, string sunknownTaxPayerCodeId, string filePath, string summaryCSVProcessedFilePath, string summaryFilePath);


        ValidateFileResponse GetValidationObject(int v);


        /// <summary>
        /// Do file processing 
        /// </summary>
        /// <returns>ValidateFileResponse object containing the batch Id and other props indicating whether an error has occurred or not</returns>
        /// <exception cref="Exception">Throw an exception if something doesn't get saved or header values are incorrect</exception>
        ValidateFileResponse ValidateTheFile(FileServiceHelper serviceProperties, string fiilePath);


        /// <summary>
        /// Do a sort of the IPPIS records by tax payer code, do a summation of all tax amount with the value and a count also of each 
        /// record that falls into the category
        /// </summary>
        /// <param name="batchId">the batch recor</param>
        void CategorizeByTaxPayerCode(FileServiceHelper serviceProperties, Int64 batchId);


        /// <summary>
        /// Match the tax payer code to the corrresponding tax entity agency code.
        /// Match identifier would be the Id of the TaxEntity
        /// </summary>
        /// <param name="batchId"></param>
        void MatchTaxEntityToTaxPayerCode(FileServiceHelper serviceProperties, long batchId);


        /// <summary>
        /// when the tax payer code have been matched with the corresponding tax entity
        /// there might be some tax payer codes that do not have a corresponding tax entity
        /// this method would tie these tax payer code to a default value
        /// </summary>
        /// <param name="batchId"></param>
        void AttachUnknownTaxPayerCodeToUnknownTaxEntity(FileServiceHelper serviceProperties, long batchId);


        /// <summary>
        /// Generate invoices for the batch records
        /// </summary>
        /// <param name="batchId"></param>
        void GenerateInvoices(FileServiceHelper serviceProperties, long batchId);


        /// <summary>
        /// Once invoices have been generated for the batch, lets create the direct assessments for each record
        /// </summary>
        /// <param name="batchId"></param>
        void CreateDirectAssessments(FileServiceHelper serviceProperties, long batchId);


        /// <summary>
        /// We have created the direct assessment batch record,
        /// now lets move the records for assesment into the direct assessment batch record table
        /// </summary>
        /// <param name="batchId"></param>
        void MoveDirectAssessmentsBatchRecords(FileServiceHelper serviceProperties, long batchId);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchId"></param>
        void CreateInvoices(FileServiceHelper serviceProperties, long batchId);


        /// <summary>
        /// We have created the invoices for direct assessment generation
        /// now we confirm these invoices
        /// </summary>
        /// <param name="batchId"></param>
        void ConfirmDirectAssessments(FileServiceHelper serviceProperties, long batchId);


        /// <summary>
        /// move CSV to summary file path
        /// </summary>
        /// <param name="serviceProperties"></param>
        /// <param name="batchId"></param>
        void MoveCSVToSummaryPath(FileServiceHelper serviceProperties, long batchId);


        /// <summary>
        /// This method convert's the IPPIS summary file to CSV
        /// now we confirm these invoices
        /// </summary>
        /// <param name="serviceProperties"></param>
        /// <param name="batchId"></param>
        void ConvertIPPISSummaryFileToCSV(FileServiceHelper details);
    }
}
