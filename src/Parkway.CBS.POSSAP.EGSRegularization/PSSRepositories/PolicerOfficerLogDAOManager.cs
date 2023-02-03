using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts;
using System;
using System.Collections.Generic;
using System.Data;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories
{
    public class PolicerOfficerLogDAOManager : Repository<PolicerOfficerLog>, IPolicerOfficerLogDAOManager
    {
        public PolicerOfficerLogDAOManager(IUoW uow) : base(uow)
        {

        }

        public void Save(PolicerOfficerLog policerOfficerLog)
        {
            try
            {
                var queryText = $"INSERT INTO Parkway_CBS_Police_Core_PolicerOfficerLog (Name, Gender, PhoneNumber,Rank_Id, IdentificationNumber, IPPISNumber, Command_Id, BankCode, AccountNumber, CreatedAtUtc, UpdatedAtUtc) VALUES (:name, :gender, :phoneNumber, :rank, :identificationNumber, :ippisNumber, :command, :bankCode, :accountNumber, :createdAtUtc, :updatedAtUtc)";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("name", policerOfficerLog.Name);
                query.SetParameter("phoneNumber", policerOfficerLog.PhoneNumber);
                query.SetParameter("rank", policerOfficerLog.Rank.Id);
                query.SetParameter("identificationNumber", policerOfficerLog.IdentificationNumber);
                query.SetParameter("ippisNumber", policerOfficerLog.IPPISNumber);
                query.SetParameter("gender", policerOfficerLog.Gender);
                query.SetParameter("command", policerOfficerLog.Command.Id);
                query.SetParameter("accountNumber", policerOfficerLog.AccountNumber);
                query.SetParameter("bankCode", policerOfficerLog.BankCode);
                query.SetParameter("createdAtUtc", DateTime.Now.ToLocalTime());
                query.SetParameter("updatedAtUtc", DateTime.Now.ToLocalTime());
                query.ExecuteUpdate();
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// Bulk save
        /// </summary>
        /// <param name="policerOfficerLogs"></param>
        public void SaveBundle(IEnumerable<PolicerOfficerLog> policerOfficerLogs)
        {
            try
            {
                DataTable dataTable = new DataTable("Parkway_CBS_Police_Core_" + typeof(PolicerOfficerLog).Name);
                dataTable.Columns.Add(new DataColumn(nameof(PolicerOfficerLog.Name), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PolicerOfficerLog.PhoneNumber), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PolicerOfficerLog.Rank) + "_Id", typeof(int)));
                dataTable.Columns.Add(new DataColumn(nameof(PolicerOfficerLog.Command) + "_Id", typeof(int)));
                dataTable.Columns.Add(new DataColumn(nameof(PolicerOfficerLog.IdentificationNumber), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PolicerOfficerLog.IPPISNumber), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PolicerOfficerLog.Gender), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PolicerOfficerLog.AccountNumber), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PolicerOfficerLog.BankCode), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PolicerOfficerLog.CreatedAtUtc), typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn(nameof(PolicerOfficerLog.UpdatedAtUtc), typeof(DateTime)));

                foreach(var item in policerOfficerLogs)
                {
                    DataRow row = dataTable.NewRow();
                    row[nameof(PolicerOfficerLog.Name)] = item.Name;
                    row[nameof(PolicerOfficerLog.PhoneNumber)] = item.PhoneNumber;
                    row[nameof(PolicerOfficerLog.Rank) + "_Id"] = item.Rank.Id;
                    row[nameof(PolicerOfficerLog.Command) + "_Id"] = item.Command.Id;
                    row[nameof(PolicerOfficerLog.IdentificationNumber)] = item.IdentificationNumber;
                    row[nameof(PolicerOfficerLog.IPPISNumber)] = item.IPPISNumber;
                    row[nameof(PolicerOfficerLog.Gender)] = item.Gender;
                    row[nameof(PolicerOfficerLog.AccountNumber)] = item.AccountNumber;
                    row[nameof(PolicerOfficerLog.BankCode)] = item.BankCode;
                    row[nameof(PolicerOfficerLog.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                    row[nameof(PolicerOfficerLog.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                    dataTable.Rows.Add(row);
                }

                SaveBundle(dataTable, "Parkway_CBS_Police_Core_" + typeof(PolicerOfficerLog).Name);

            }
            catch (Exception) { throw; }
        }
    }
}
