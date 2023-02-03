using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl.Contracts;
using Parkway.CBS.Payee.ReferenceDataImplementation;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl
{
    public class ReferenceDataRecordsDAOManager : Repository<ReferenceDataRecords>, IReferenceDataBatchRecordsDAOManager
    {
        private static ILGAMapping _lgaMapping;
        public ReferenceDataRecordsDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// Match the Reference Data records to the Revenue Head Mapping using the Serial Number and Batch Id.
        /// </summary>
        /// <param name="batchId"></param>
        public void MatchReferenceDataRecordsToTypeOfTaxpaid(long batchId)
        {
            try
            {
                var queryText = $"UPDATE rm SET rm.ReferenceDataRecord_Id = rd.Id FROM Parkway_CBS_Core_ReferenceDataTypeOfTaxPaidMapping rm INNER JOIN Parkway_CBS_Core_ReferenceDataRecords as rd ON rd.SerialNumberId = rm.SerialNumberId AND rd.ReferenceDataBatch_Id = rm.ReferenceDataBatch_Id WHERE rd.ReferenceDataBatch_Id = :batch_Id";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }


        /// <summary>
        /// Move the records for tax entity creation to a staging table Parkway_CBS_Core_ReferenceDataTaxEntityStaging
        /// When data ref data is added to the ref data records table, we need to add this staging table with data we need to create or update a tax payer record
        /// </summary>
        /// <param name="batchId"></param>
        public void MoveReferenceDataToTaxEntityStaging(long batchId)
        {
            try
            {
                var queryText = $"INSERT INTO Parkway_CBS_Core_ReferenceDataTaxEntityStaging (ReferenceDataRecord_Id, ReferenceDataBatch_Id, Surname, Firstname, Middlename, TIN, HouseNo, StreetName, DbLGAId, City, EmailAddress, PhoneNumber, TaxEntityCategory_Id, IsTaxPayerLandlord, IsEvidenceProvided, OperationType_Id, PropertyRentAmount, CreatedAtUtc, UpdatedAtUtc)" +
                    $" SELECT Id, ReferenceDataBatch_Id, Surname, Firstname, Middlename, TIN, HouseNo, StreetName, DbLGAId, City, EmailAddress, PhoneNumber1, TaxEntityCategory_Id, IsTaxPayerLandlord, IsEvidenceProvided, :OperationType_Id, PropertyRentAmount, :dateSaved, :dateSaved  FROM Parkway_CBS_Core_ReferenceDataRecords WHERE ReferenceDataBatch_Id = :batch_Id";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("dateSaved", DateTime.Now.ToLocalTime());
                //initially all records are marked for creation on the tax entity table
                query.SetParameter("OperationType_Id", (int)ReferenceDataOperationType.Create);
                query.SetParameter("batch_Id", batchId);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }

        public int SaveReferenceDataRecords(string tenantName, string LGAId, long recordId, ConcurrentStack<ReferenceDataLineRecordModel> referenceDataLineRecords)
        {
            _lgaMapping = new LGAMapping();
            int chunkSize = 500000;
            var dataSize = referenceDataLineRecords.Count;

            double pageSize = ((double)dataSize / (double)chunkSize);
            int pages = 0;

            if (pageSize < 1 && dataSize >= 1) { pages = 1; }
            else { pages = (int)Math.Ceiling(pageSize); }
            int stopper = 0;
            int skip = 0;
            try
            {
                #region data column

                var dataTable = new DataTable("Parkway_CBS_Core_" + typeof(Core.Models.ReferenceDataRecords).Name);
                dataTable.Columns.Add(new DataColumn("ReferenceDataBatch_Id", typeof(Int64)));
                dataTable.Columns.Add(new DataColumn("RowId", typeof(string)));
                dataTable.Columns.Add(new DataColumn("Completed", typeof(string)));
                dataTable.Columns.Add(new DataColumn("CompletedBy", typeof(string)));
                dataTable.Columns.Add(new DataColumn("Started", typeof(string)));
                dataTable.Columns.Add(new DataColumn("Received", typeof(string)));
                dataTable.Columns.Add(new DataColumn("CompletedAt", typeof(string)));
                dataTable.Columns.Add(new DataColumn("Title", typeof(string)));
                dataTable.Columns.Add(new DataColumn("Sex", typeof(string)));
                dataTable.Columns.Add(new DataColumn("Surname", typeof(string)));
                dataTable.Columns.Add(new DataColumn("Firstname", typeof(string)));
                dataTable.Columns.Add(new DataColumn("Middlename", typeof(string)));
                dataTable.Columns.Add(new DataColumn("Nationality", typeof(string)));
                dataTable.Columns.Add(new DataColumn("PhoneNumber1", typeof(string)));
                dataTable.Columns.Add(new DataColumn("PhoneNumber2", typeof(string)));
                dataTable.Columns.Add(new DataColumn("EmailAddress", typeof(string)));
                dataTable.Columns.Add(new DataColumn("TIN", typeof(string)));
                dataTable.Columns.Add(new DataColumn("HouseNo", typeof(string)));
                dataTable.Columns.Add(new DataColumn("StreetName", typeof(string)));
                dataTable.Columns.Add(new DataColumn("City", typeof(string)));
                dataTable.Columns.Add(new DataColumn("LGA", typeof(string)));
                dataTable.Columns.Add(new DataColumn("EmploymentStatus", typeof(string)));
                dataTable.Columns.Add(new DataColumn("EmployerName", typeof(string)));
                dataTable.Columns.Add(new DataColumn("EmployerAddress", typeof(string)));
                dataTable.Columns.Add(new DataColumn("PropertyType", typeof(string)));
                dataTable.Columns.Add(new DataColumn("PropertyStructure", typeof(string)));
                dataTable.Columns.Add(new DataColumn("PropertyStructureNumber", typeof(string)));
                dataTable.Columns.Add(new DataColumn("PropertyAddress", typeof(string)));
                dataTable.Columns.Add(new DataColumn("PropertyRentAmount", typeof(decimal)));
                dataTable.Columns.Add(new DataColumn("RentStartDate", typeof(string)));
                dataTable.Columns.Add(new DataColumn("RentEndDate", typeof(string)));
                dataTable.Columns.Add(new DataColumn("OwnerTitle", typeof(string)));
                dataTable.Columns.Add(new DataColumn("OwnerSex", typeof(string)));
                dataTable.Columns.Add(new DataColumn("OwnerSurname", typeof(string)));
                dataTable.Columns.Add(new DataColumn("OwnerFirstname", typeof(string)));
                dataTable.Columns.Add(new DataColumn("OwnerMiddlename", typeof(string)));
                dataTable.Columns.Add(new DataColumn("OwnerNationality", typeof(string)));
                dataTable.Columns.Add(new DataColumn("OwnerPhoneNumber1", typeof(string)));
                dataTable.Columns.Add(new DataColumn("OwnerPhoneNumber2", typeof(string)));
                dataTable.Columns.Add(new DataColumn("OwnerEmailAddress", typeof(string)));
                dataTable.Columns.Add(new DataColumn("OwnerTIN", typeof(string)));
                dataTable.Columns.Add(new DataColumn("OwnerEmploymentStatus", typeof(string)));
                dataTable.Columns.Add(new DataColumn("OwnerEmployerName", typeof(string)));
                dataTable.Columns.Add(new DataColumn("OwnerEmployerAddress", typeof(string)));
                dataTable.Columns.Add(new DataColumn("TypeOfTaxPaid", typeof(string)));
                dataTable.Columns.Add(new DataColumn("IsEvidenceProvided", typeof(string)));
                dataTable.Columns.Add(new DataColumn("IsTaxPayerLandlord", typeof(string)));
                dataTable.Columns.Add(new DataColumn("BusinessName", typeof(string)));
                dataTable.Columns.Add(new DataColumn("BusinessRegOffice", typeof(string)));
                dataTable.Columns.Add(new DataColumn("CorporateHouseAddress", typeof(string)));
                dataTable.Columns.Add(new DataColumn("CorporateStreetName", typeof(string)));
                dataTable.Columns.Add(new DataColumn("CorporateCity", typeof(string)));
                dataTable.Columns.Add(new DataColumn("CorporateLGA", typeof(string)));
                dataTable.Columns.Add(new DataColumn("CorporateOrganizationType", typeof(string)));
                dataTable.Columns.Add(new DataColumn("CorporateTin", typeof(string)));
                dataTable.Columns.Add(new DataColumn("CorporateContactName", typeof(string)));
                dataTable.Columns.Add(new DataColumn("CorporatePhoneNumber", typeof(string)));
                dataTable.Columns.Add(new DataColumn("BusinessCommencement", typeof(string)));
                dataTable.Columns.Add(new DataColumn("NumberEmployees", typeof(string)));
                dataTable.Columns.Add(new DataColumn("DirectorTitle", typeof(string)));
                dataTable.Columns.Add(new DataColumn("DirectorSex", typeof(string)));
                dataTable.Columns.Add(new DataColumn("DirectorSurname", typeof(string)));
                dataTable.Columns.Add(new DataColumn("DirectorFirstName", typeof(string)));
                dataTable.Columns.Add(new DataColumn("DirectorMiddleName", typeof(string)));
                dataTable.Columns.Add(new DataColumn("DirectorNationality", typeof(string)));
                dataTable.Columns.Add(new DataColumn("DirectorPhone1", typeof(string)));
                dataTable.Columns.Add(new DataColumn("DirectorPhone2", typeof(string)));
                dataTable.Columns.Add(new DataColumn("DirectorEmail", typeof(string)));
                dataTable.Columns.Add(new DataColumn("DirectorAddress", typeof(string)));
                dataTable.Columns.Add(new DataColumn("DirectorCity", typeof(string)));
                dataTable.Columns.Add(new DataColumn("CorporateIncomeYr1", typeof(string)));
                dataTable.Columns.Add(new DataColumn("CorporateIncomeYr2", typeof(string)));
                dataTable.Columns.Add(new DataColumn("CorporateIncomeYr3", typeof(string)));
                dataTable.Columns.Add(new DataColumn("SolProSouIncome", typeof(string)));
                dataTable.Columns.Add(new DataColumn("AnnualGross", typeof(string)));
                dataTable.Columns.Add(new DataColumn("TaxEntityCategory_Id", typeof(string)));
                dataTable.Columns.Add(new DataColumn("CreatedAtUtc", typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn("UpdatedAtUtc", typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn("SerialNumberId", typeof(Int64)));
                dataTable.Columns.Add(new DataColumn("DbLGAId", typeof(string)));

                var dataTableTypeOfTaxPaid = new DataTable("Parkway_CBS_Core_" + typeof(ReferenceDataTypeOfTaxPaidMapping).Name);
                dataTableTypeOfTaxPaid.Columns.Add(new DataColumn("ReferenceDataBatch_Id", typeof(Int64)));
                dataTableTypeOfTaxPaid.Columns.Add(new DataColumn("ReferenceDataTypeOfTaxPaid", typeof(int)));
                dataTableTypeOfTaxPaid.Columns.Add(new DataColumn("CreatedAtUtc", typeof(DateTime)));
                dataTableTypeOfTaxPaid.Columns.Add(new DataColumn("UpdatedAtUtc", typeof(DateTime)));
                dataTableTypeOfTaxPaid.Columns.Add(new DataColumn("SerialNumberId", typeof(int)));
                #endregion

                Int64 counter = 0;
                while (stopper < pages)
                {
                    counter = (chunkSize * stopper);

                    referenceDataLineRecords.Skip(skip).Take(chunkSize).ToList().ForEach(x =>
                    {
                        var row = dataTable.NewRow();
                        row["ReferenceDataBatch_Id"] = recordId;
                        row["RowId"] = x.Row_Id.Value;
                        row["Completed"] = x.Completed.Value;
                        row["CompletedBy"] = x.CompletedBy.Value;
                        row["Started"] = x.Started.Value;
                        row["Received"] = x.Received.Value;
                        row["CompletedAt"] = x.CompletedAt.Value;
                        row["Title"] = x.Title.Value;
                        row["Sex"] = x.Sex.Value;
                        row["Surname"] = x.Surname.Value;
                        row["Firstname"] = x.Firstname.Value;
                        row["Middlename"] = x.Middlename.Value;
                        row["Nationality"] = x.Nationality.Value;
                        row["PhoneNumber1"] = x.PhoneNumber1.Value;
                        row["PhoneNumber2"] = x.PhoneNumber2.Value;
                        row["EmailAddress"] = x.EmailAddress.Value;
                        row["TIN"] = x.TIN.Value;
                        row["HouseNo"] = x.HouseNo.Value;
                        row["StreetName"] = x.StreetName.Value;
                        row["City"] = x.City.Value;
                        row["LGA"] = x.LGA.Value;
                        row["EmploymentStatus"] = x.EmploymentStatus.Value;
                        row["EmployerName"] = x.EmployerName.Value;
                        row["EmployerAddress"] = x.EmployerAddress.Value;
                        row["PropertyType"] = x.PropertyType.Value;
                        row["PropertyStructure"] = x.PropertyStructure.Value;
                        row["PropertyStructureNumber"] = x.PropertyStructureNumber.Value;
                        row["PropertyAddress"] = x.PropertyAddress.Value;
                        row["PropertyRentAmount"] = x.PropertyRentAmount;
                        row["RentStartDate"] = x.RentStartDate.Value;
                        row["RentEndDate"] = x.RentEndDate.Value;
                        row["OwnerTitle"] = x.OwnerTitle.Value;
                        row["OwnerSex"] = x.OwnerSex.Value;
                        row["OwnerSurname"] = x.OwnerSurname.Value;
                        row["OwnerFirstname"] = x.OwnerFirstname.Value;
                        row["OwnerMiddlename"] = x.OwnerMiddlename.Value;
                        row["OwnerNationality"] = x.OwnerNationality.Value;
                        row["OwnerPhoneNumber1"] = x.OwnerPhoneNumber1.Value;
                        row["OwnerPhoneNumber2"] = x.OwnerPhoneNumber2.Value;
                        row["OwnerEmailAddress"] = x.OwnerEmailAddress.Value;
                        row["OwnerTIN"] = x.OwnerTIN.Value;
                        row["OwnerEmploymentStatus"] = x.OwnerEmploymentStatus.Value;
                        row["OwnerEmployerName"] = x.OwnerEmployerName.Value;
                        row["OwnerEmployerAddress"] = x.OwnerEmployerAddress.Value;
                        row["TypeOfTaxPaid"] = x.TypeOfTaxPaid.Value;
                        row["IsEvidenceProvided"] = x.EvidenceProvided;
                        row["IsTaxPayerLandlord"] = x.IsTaxPayerLandlord;
                        row["BusinessName"] = x.BusinessName.Value;
                        row["BusinessRegOffice"] = x.BusinessRegOffice.Value;
                        row["CorporateHouseAddress"] = x.CorporateHouseAddress.Value;
                        row["CorporateStreetName"] = x.CorporateStreetName.Value;
                        row["CorporateCity"] = x.CorporateLGA.Value;
                        row["CorporateLGA"] = x.CorporateLGA.Value;
                        row["CorporateOrganizationType"] = x.CorporateOrganizationType.Value;
                        row["CorporateTin"] = x.CorporateTin.Value;
                        row["CorporateContactName"] = x.CorporateContactName.Value;
                        row["CorporatePhoneNumber"] = x.CorporatePhoneNumber.Value;
                        row["BusinessCommencement"] = x.BusinessCommencement.Value;
                        row["NumberEmployees"] = x.NumberEmployees.Value;
                        row["DirectorTitle"] = x.DirectorTitle.Value;
                        row["DirectorSex"] = x.DirectorSex.Value;
                        row["DirectorSurname"] = x.DirectorSurname.Value;
                        row["DirectorFirstName"] = x.DirectorFirstName.Value;
                        row["DirectorMiddleName"] = x.DirectorMiddleName.Value;
                        row["DirectorNationality"] = x.DirectorNationality.Value;
                        row["DirectorPhone1"] = x.DirectorPhone1.Value;
                        row["DirectorPhone2"] = x.DirectorPhone2.Value;
                        row["DirectorEmail"] = x.DirectorEmail.Value;
                        row["DirectorAddress"] = x.DirectorAddress.Value;
                        row["DirectorCity"] = x.DirectorCity.Value;
                        row["CorporateIncomeYr1"] = x.CorporateIncomeYr1.Value;
                        row["CorporateIncomeYr2"] = x.CorporateIncomeYr2.Value;
                        row["CorporateIncomeYr3"] = x.CorporateIncomeYr3.Value;
                        row["SolProSouIncome"] = x.SolProSouIncome.Value;
                        row["AnnualGross"] = x.AnnualGross.Value;
                        row["TaxEntityCategory_Id"] = x.TaxEntityCategory.Value;
                        row["CreatedAtUtc"] = DateTime.Now.ToLocalTime();
                        row["UpdatedAtUtc"] = DateTime.Now.ToLocalTime();
                        row["SerialNumberId"] = ++counter;
                        row["DbLGAId"] = string.IsNullOrEmpty(x.LGA.Value) ? LGAId : _lgaMapping.GetLGADatabaseId(tenantName, x.LGA.Value);

                        dataTable.Rows.Add(row);

                        if (x.TypeOfTaxPaidMappingList != null)
                        {
                            x.TypeOfTaxPaidMappingList.ToList().ForEach(t =>
                            {
                                var rowTypeOfTaxPaid = dataTableTypeOfTaxPaid.NewRow();
                                rowTypeOfTaxPaid["ReferenceDataBatch_Id"] = recordId;
                                rowTypeOfTaxPaid["ReferenceDataTypeOfTaxPaid"] = t.ReferenceDataTypeOfTaxPaid;
                                rowTypeOfTaxPaid["CreatedAtUtc"] = DateTime.Now.ToLocalTime();
                                rowTypeOfTaxPaid["UpdatedAtUtc"] = DateTime.Now.ToLocalTime();
                                rowTypeOfTaxPaid["SerialNumberId"] = counter;
                                dataTableTypeOfTaxPaid.Rows.Add(rowTypeOfTaxPaid);
                            });
                        }
                    });

                    if (!SaveBundle(dataTableTypeOfTaxPaid, "Parkway_CBS_Core_" + typeof(ReferenceDataTypeOfTaxPaidMapping).Name))
                    { throw new Exception("Error saving details for batch Id " + recordId); }

                    if (!SaveBundle(dataTable, "Parkway_CBS_Core_" + typeof(Core.Models.ReferenceDataRecords).Name))
                    { throw new Exception("Error saving details for batch Id " + recordId); }

                    skip += chunkSize;
                    stopper++;
                }
            }
            catch (Exception ex)
            { throw ex; }

            return dataSize;

        }
    }
}
