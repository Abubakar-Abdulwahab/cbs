using System;
using Parkway.CBS.Core.Models;
using Parkway.CBS.ClientRepository.Repositories.Contracts;

namespace Parkway.CBS.ClientRepository.Repositories
{
    public class DirectAssessmentPayeeRecordDAOManager : Repository<DirectAssessmentPayeeRecord>, IDirectAssessmentPayeeRecordDAOManager
    {
        public DirectAssessmentPayeeRecordDAOManager(IUoW uow) : base(uow)
        { }


        public void MigrateIPPISEmployeeRecordsToDirectAssessmentPayeeRecordTable(long batchId, int month, string monthVal, int year)
        {
            try
            {
                var queryText = $"INSERT INTO Parkway_CBS_Core_DirectAssessmentPayeeRecord (GrossAnnual, Exemptions, HasErrors, IncomeTaxPerMonth, IncomeTaxPerMonthValue, Month, Year, Email, PhoneNumber, PayeeName, Address, DirectAssessmentBatchRecord_Id, AssessmentDate, CreatedAtUtc, UpdatedAtUtc, GradeLevel, Step) SELECT  :zeroVal, :zeroVal, :noErr, ip.TaxStringValue, ip.Tax, :monthVal, :year, ip.Email, ip.PhoneNumber, ip.PayeeName, ip.Address, br.Id, :assementDate, :dateSaved, :dateSaved, ip.GradeLevel, ip.Step FROM Parkway_CBS_Core_IPPISBatchRecords ip INNER JOIN Parkway_CBS_Core_DirectAssessmentBatchRecord br ON  ip.IPPISBatch_Id = :batch_Id AND ip.TaxPayerCode = br.TaxPayerCode";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("zeroVal", 0);
                query.SetParameter("noErr", false);
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("monthVal", monthVal);
                query.SetParameter("year", year);
                query.SetParameter("assementDate", new DateTime(year, month, 1));
                query.SetParameter("dateSaved", DateTime.Now.ToLocalTime());

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                throw;
            }
        }
    }
}
