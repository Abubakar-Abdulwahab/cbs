using Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System;

namespace Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl
{
    public class ReferenceDataTaxEntityStagingDAOManager : Repository<ReferenceDataTaxEntityStaging>, IReferenceDataTaxEntityStagingDAOManager
    {
        public ReferenceDataTaxEntityStagingDAOManager(IUoW uow) : base(uow)
        {

        }


        /// <summary>
        /// Create the Tax Entity records where the Tax Entity Category and PhoneNumber does not match what is in the Reference Data records.
        /// </summary>
        /// <param name="batchId"></param>
        public void CreateTaxEntityWithReferenceDataRecords(long batchId)
        {
            try
            {
                var queryText = $"INSERT INTO Parkway_CBS_Core_TaxEntity (TaxPayerIdentificationNumber, Recipient, PhoneNumber, Email, TaxEntityType, TaxEntityCategory_Id, Address, LGA, CreatedAtUtc, UpdatedAtUtc)" +
                    $" SELECT rdt.TIN, CONCAT(rdt.Surname, ' ', rdt.Firstname, ' ', rdt.Middlename) AS Recepient, rdt.PhoneNumber, rdt.EmailAddress, rdt.TaxEntityCategory_Id, rdt.TaxEntityCategory_Id, CONCAT(rdt.HouseNo, ' ', rdt.StreetName, ' ', rdt.City) AS Address," +
                    $" DbLGAId, :dateSaved, :dateSaved  FROM Parkway_CBS_Core_ReferenceDataTaxEntityStaging rdt INNER JOIN (SELECT PhoneNumber, TaxEntityCategory_Id, MIN(Id) as Id FROM Parkway_CBS_Core_ReferenceDataTaxEntityStaging" +
                    $" WHERE ReferenceDataBatch_Id = :batch_Id AND OperationType_Id = :OperationType_Id GROUP BY PhoneNumber, TaxEntityCategory_Id) AS rdtg ON rdt.Id = rdtg.Id";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("dateSaved", DateTime.Now.ToLocalTime());
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("OperationType_Id", (int)ReferenceDataOperationType.Create);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchId"></param>
        public void MoveTaxEntityStagingRecordsToWithHoldingTaxOnRent(long batchId)
        {
            try
            {
                var queryText = $"INSERT INTO Parkway_CBS_Core_ReferenceDataWithHoldingTaxOnRent (ReferenceDataTaxEntityStaging_Id, ReferenceDataBatch_Id, TaxEntity_Id, PropertyRentAmount," +
                    $" CreatedAtUtc, UpdatedAtUtc) SELECT rdt.Id, rdt.ReferenceDataBatch_Id, rdt.TaxEntity_Id, rdt.PropertyRentAmount, :dateSaved, :dateSaved FROM Parkway_CBS_Core_ReferenceDataTaxEntityStaging rdt" +
                    $" INNER JOIN (SELECT TaxEntity_Id , MIN(Id) as Id FROM Parkway_CBS_Core_ReferenceDataTaxEntityStaging" +
                    $" WHERE ReferenceDataBatch_Id = :batch_Id AND IsTaxPayerLandlord = :IsLandlord GROUP BY TaxEntity_Id) AS rdtg ON rdt.Id = rdtg.Id";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("dateSaved", DateTime.Now.ToLocalTime());
                query.SetParameter("IsLandlord", true);
                query.SetParameter("batch_Id", batchId);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }

        /// <summary>
        /// Update the Tax Entity Staging records operationTypeId where the Tax Entity Category and PhoneNumber matches what is in the Reference Data records.
        /// </summary>
        /// <param name="batchId"></param>
        public void UpdateReferenceDataTaxEntityStagingRecordsOperationType(long batchId)
        {
            try
            {
                var queryText = $"UPDATE rdt SET rdt.OperationType_Id = :OperationType_Id, rdt.TaxEntity_Id = t.Id FROM Parkway_CBS_Core_ReferenceDataTaxEntityStaging rdt" +
                    $" INNER JOIN Parkway_CBS_Core_TaxEntity as t ON t.TaxEntityCategory_Id = rdt.TaxEntityCategory_Id AND t.PhoneNumber = rdt.PhoneNumber" +
                    $" WHERE rdt.ReferenceDataBatch_Id = :batch_Id";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("OperationType_Id", (int)ReferenceDataOperationType.Update);
                query.SetParameter("batch_Id", batchId);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }

        public void UpdateReferenceDataTaxEntityStagingRecordsTaxEntityId(long batchId)
        {
            try
            {
                var queryText = $"UPDATE rdt SET rdt.TaxEntity_Id = t.Id FROM Parkway_CBS_Core_ReferenceDataTaxEntityStaging rdt" +
                    $" INNER JOIN Parkway_CBS_Core_TaxEntity as t ON t.TaxEntityCategory_Id = rdt.TaxEntityCategory_Id AND t.PhoneNumber = rdt.PhoneNumber" +
                    $" WHERE rdt.ReferenceDataBatch_Id = :batch_Id AND rdt.OperationType_Id = :OperationType_Id";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("OperationType_Id", (int)ReferenceDataOperationType.Create);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }

        /// <summary>
        /// Update the Tax Entity records where the Tax Entity Category and PhoneNumber matches what is in the Reference Data records.
        /// </summary>
        /// <param name="batchId"></param>
        public void UpdateTaxEntityWithReferenceDataRecords(long batchId)
        {
            try
            {
                var queryText = $"UPDATE t SET t.LGA = rdt.DbLGAId, t.Recipient = CONCAT(rdt.Surname, ' ', rdt.Firstname, ' ', rdt.Middlename), t.Address = CONCAT(rdt.HouseNo, ' ', rdt.StreetName, ' ', rdt.City) FROM Parkway_CBS_Core_TaxEntity t" +
                    $" INNER JOIN Parkway_CBS_Core_ReferenceDataTaxEntityStaging as rdt ON t.Id = rdt.TaxEntity_Id" +
                    $" WHERE rdt.ReferenceDataBatch_Id = :batch_Id AND rdt.OperationType_Id = :OperationType_Id";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("OperationType_Id", (int)ReferenceDataOperationType.Update);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }

        public void MoveRecordsToAssets(long batchId)
        {
            try
            {
                var queryText = $"INSERT INTO Parkway_CBS_Core_ReferenceDataAsset (AssetType_Id, Fullname, TaxEntity_Id, Address, TIN, PhoneNumber, LGA_Id, Email, CreatedAtUtc, UpdatedAtUtc) SELECT :assetTypeId, CONCAT(Surname, ' ', Firstname, ' ', Middlename) AS Fullname, TaxEntity_Id, CONCAT(HouseNo, ' ', StreetName, ' ', City) AS Address, TIN, PhoneNumber, DbLGAId, EmailAddress, :dateSaved, :dateSaved  FROM Parkway_CBS_Core_ReferenceDataTaxEntityStaging WHERE ReferenceDataBatch_Id = :batch_Id AND IsTaxPayerLandlord = :IsLandlord";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("dateSaved", DateTime.Now.ToLocalTime());
                query.SetParameter("IsLandlord", true);
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("assetTypeId", 2);

                query.ExecuteUpdate();


                var queryText1 = $"INSERT INTO Parkway_CBS_Core_ReferenceDataBuildingProperties (ReferenceDataAsset_Id, Purpose, Address, RentAmount, LGA_Id, CreatedAtUtc, UpdatedAtUtc) SELECT RDA.Id, :purpose, RDA.Address, :amount, RDA.LGA_Id, :dateSaved, :dateSaved  FROM Parkway_CBS_Core_ReferenceDataAsset as RDA INNER JOIN Parkway_CBS_Core_TaxEntity as t ON t.Id = RDA.TaxEntity_Id WHERE t.TaxEntityCategory_Id = :taxCategoryId";
                var query1 = _uow.Session.CreateSQLQuery(queryText1);
                query1.SetParameter("dateSaved", DateTime.Now.ToLocalTime());
                query1.SetParameter("taxCategoryId", 1);
                //query1.SetParameter("batch_Id", batchId);
                query1.SetParameter("purpose", 1);
                query1.SetParameter("amount", 100000);

                query1.ExecuteUpdate();


                var queryText2 = $"INSERT INTO Parkway_CBS_Core_ReferenceDataBusinessPremises (ReferenceDataAsset_Id, OrganizationType, NoofEmployees, CommencementDate, CreatedAtUtc, UpdatedAtUtc) SELECT RDA.Id, :OrganizationType, :NoofEmployees, :dateSaved, :dateSaved, :dateSaved  FROM Parkway_CBS_Core_ReferenceDataAsset as RDA INNER JOIN Parkway_CBS_Core_TaxEntity as t ON t.Id = RDA.TaxEntity_Id WHERE t.TaxEntityCategory_Id = :taxCategoryId";
                var query2 = _uow.Session.CreateSQLQuery(queryText2);
                query2.SetParameter("dateSaved", DateTime.Now.ToLocalTime());
                query2.SetParameter("taxCategoryId", 2);
                //query2.SetParameter("batch_Id", batchId);
                query2.SetParameter("OrganizationType", 1);
                query2.SetParameter("NoofEmployees", 10);

                query2.ExecuteUpdate();

            }
            catch (Exception)
            { throw; }
        }

        public void MoveWithHoldingTaxOnRentRecordsToInvoiceStaging(long batchId)
        {
            try
            {
                var queryText = $"INSERT INTO Parkway_CBS_Core_ReferenceDataRecordsInvoice (ReferenceDataBatch_Id, TaxEntity_Id, TaxEntityCategory_Id, InvoiceUniqueKey, CreatedAtUtc, UpdatedAtUtc)" +
                    $" SELECT rdt.ReferenceDataBatch_Id, rdt.TaxEntity_Id, rdt.TaxEntityCategory_Id, rwt.Id, :dateSaved, :dateSaved FROM Parkway_CBS_Core_ReferenceDataWithHoldingTaxOnRent as rwt" +
                    $" INNER JOIN Parkway_CBS_Core_ReferenceDataTaxEntityStaging as rdt ON rwt.ReferenceDataTaxEntityStaging_Id = rdt.Id" +
                    $" WHERE rwt.ReferenceDataBatch_Id = :batch_Id";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("dateSaved", DateTime.Now.ToLocalTime());

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }

        public void UpdateTaxEntityWithCashflowInvoiceResponse(long batchId)
        {
            try
            {
                var queryText = $"UPDATE t SET t.PrimaryContactId = rdi.PrimaryContactId, t.CashflowCustomerId = rdi.CashflowCustomerId FROM Parkway_CBS_Core_TaxEntity t" +
                    $" INNER JOIN Parkway_CBS_Core_ReferenceDataRecordsInvoice as rdi ON t.Id = rdi.TaxEntity_Id AND t.TaxEntityCategory_Id = rdi.TaxEntityCategory_Id" +
                    $" WHERE rdi.ReferenceDataBatch_Id = :batch_Id";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }

        public void MoveDevelopmentLevyRecordToInvoiceStagingTable(long batchId)
        {
            try
            {
                var queryText = $"INSERT INTO Parkway_CBS_Core_ReferenceDataRecordsInvoice (ReferenceDataBatch_Id, TaxEntity_Id, TaxEntityCategory_Id, InvoiceUniqueKey, CreatedAtUtc, UpdatedAtUtc)" +
                    $" SELECT :batch_Id, tEntity.Id, tEntity.TaxEntityCategory_Id, tEntity.Id, :dateSaved, :dateSaved FROM Parkway_CBS_Core_TaxEntity as tEntity" +
                    $" WHERE tEntity.TaxEntityCategory_Id != :FederalAgenciesCategory_Id AND tEntity.TaxEntityCategory_Id != :StateAgenciesCategory_Id";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);
                //query.SetParameter("FederalAgenciesCategory_Id", (int)TaxEntityCategoryEnum.FederalAgency);
                //query.SetParameter("StateAgenciesCategory_Id", (int)TaxEntityCategoryEnum.StateAgency);
                query.SetParameter("dateSaved", DateTime.Now.ToLocalTime());

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }
    }
}
