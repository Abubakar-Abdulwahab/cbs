using NHibernate.Criterion;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using NHibernate.Linq;
using System.Data.SqlClient;
using NHibernate.Engine;
using Parkway.CBS.Core.Exceptions;
using NHibernate;

namespace Parkway.CBS.Core.Services
{
    public class InvoiceManager : BaseManager<Invoice>, IInvoiceManager<Invoice>
    {
        private readonly IRepository<Invoice> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;
        private readonly IRoleUserManager<AccessRoleUser> _accessRoleUserRepo;

        public InvoiceManager(IRepository<Invoice> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices, IRoleUserManager<AccessRoleUser> accessRoleUserRepo) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _transactionManager = orchardServices.TransactionManager;
            _accessRoleUserRepo = accessRoleUserRepo;
        }


        public List<Invoice> GetRevenueCollectionPerMDA(MDA mda, DateTime startDate, DateTime endDate, int take, int skip)
        {
            try
            {
                var revenueHeads = mda.RevenueHeads.Skip(skip).Take(take);
                var session = _transactionManager.GetSession();
                var arrayOfRevenueHeads = revenueHeads.Select(r => r.Id).ToArray();

                var invoicesForEachRevenueHead =
                    session.QueryOver<Invoice>().Where(val => (val.RevenueHead.Id.IsIn(arrayOfRevenueHeads))
                                                        && (val.CreatedAtUtc >= startDate && val.CreatedAtUtc <= endDate)).List<Invoice>();
                return invoicesForEachRevenueHead.ToList();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return null;
            }
        }


        public Dictionary<int, IEnumerable<Invoice>> GetRevenueInvoiceCollectionForRevenueHead(RevenueHead revenueHead, DateTime startDate, DateTime endDate, int take, int skip)
        {
            var invoicesSent = revenueHead.Invoices;
            var invoices = invoicesSent.Where(invc => (invc.CreatedAtUtc >= startDate) && (invc.CreatedAtUtc <= endDate));
            Dictionary<int, IEnumerable<Invoice>> val = new Dictionary<int, IEnumerable<Invoice>>();
            int count = invoices.Count();
            invoices = invoices.Skip(skip).Take(take);

            val.Add(count, invoices);
            return val;
        }


        public MDAExpectationViewModel ExpectationPerMDA(MDA mda, DateTime startDate, DateTime endDate, int skip, int take, bool direction, string orderBy)
        {
            try
            {
                int pageSize = mda.RevenueHeads.Count();
                IEnumerable<RevenueHeadAndInvoicesHelper> revandinvoices = new List<RevenueHeadAndInvoicesHelper>();
                //
                var summary = mda.RevenueHeads.Where(m => m.Invoices.Any(invc => (invc.CreatedAtUtc >= startDate) && (invc.CreatedAtUtc <= endDate))).Select(invc => invc.Invoices);

                //get revenue heads that have invoices
                var revenueHeads = mda.RevenueHeads.Where(m => m.Invoices.Any()).Select(rh => rh).Skip(skip).Take(take);

                //get the invoices of these revenue heads that are within the date range
                if (direction)
                    revandinvoices = revenueHeads.Where(rh => rh.Invoices.Any(invc => ((rh.Id == invc.RevenueHead.Id) && (invc.CreatedAtUtc >= startDate) && (invc.CreatedAtUtc <= endDate))))
                                            .Select(rh => new RevenueHeadAndInvoicesHelper() { RevenueHeadName = rh.Name, Invoices = rh.Invoices }).OrderBy(keySelector => keySelector.RevenueHeadName);
                else
                    revandinvoices = revenueHeads.Where(rh => rh.Invoices.Any(invc => ((rh.Id == invc.RevenueHead.Id) && (invc.CreatedAtUtc >= startDate) && (invc.CreatedAtUtc <= endDate))))
                                            .Select(rh => new RevenueHeadAndInvoicesHelper() { RevenueHeadName = rh.Name, Invoices = rh.Invoices }).OrderByDescending(keySelector => keySelector.RevenueHeadName);

                IEnumerable<ExpectationReport> reports = revandinvoices.Select(rhinvc => new ExpectationReport()
                {
                    ActualIncome = (long)rhinvc.Invoices.Where(invc => invc.Status == (int)InvoiceStatusList.Paid).Sum(invc => invc.Amount),
                    ExpectedIncome = (long)rhinvc.Invoices.Sum(invc => invc.Amount),
                    NumberOfInvoices = rhinvc.Invoices.Count(),
                    NumberOfInvoicesPaid = rhinvc.Invoices.Where(invc => invc.Status == (int)InvoiceStatusList.Paid).Count(),
                    RevenueHeadName = rhinvc.RevenueHeadName
                });

                long amount = 0;
                long counter = 0;

                foreach (var item in revandinvoices)
                {
                    foreach (var i in item.Invoices)
                    {
                        if (i.Status == (int)InvoiceStatusList.Paid)
                        {
                            amount += (long)i.Amount;
                            counter++;
                        }
                    }
                }

                MDAExpectationViewModel model = new MDAExpectationViewModel()
                {
                    MDAName = mda.Name,
                    NumberOfRecords = pageSize,
                    ExpectationReport = reports,
                    TotalAmount = (long)summary.Select(invc => invc.Sum(s => s.Amount)).Sum(inv => inv),
                    TotalNumberOfInvoices = summary.Select(invc => invc.Select(v => v).Count()).Sum(),
                    TotalActualIncome = amount,
                    TotalNumberOfInvoicesPaid = counter,
                };

                return model;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return null;
            }
        }


        public MDAMonthlyPaymentViewModel GetMonthlyMDAsPayment(IEnumerable<MDA> mdas, DateTime startDate, DateTime endDate)
        {
            List<PaymentReport> reports = new List<PaymentReport>();
            decimal total = 0;

            foreach (var mda in mdas)
            {
                if (mda.Invoices.Any())
                {
                    var invoices = mda.Invoices.Where(invc => ((invc.CreatedAtUtc >= startDate) && (invc.CreatedAtUtc <= endDate)));
                    reports.Add(new PaymentReport()
                    {
                        MDAName = mda.Name,
                        MDASlug = mda.Slug,
                        ActualIncome = (long)invoices.Where(invc => invc.Status == (int)InvoiceStatusList.Paid).Sum(invc => invc.Amount),
                        ExpectedIncome = (long)invoices.Sum(invc => invc.Amount),
                        NumberOfInvoices = invoices.Count()
                    });
                }
                else
                {
                    reports.Add(new PaymentReport()
                    {
                        MDAName = mda.Name,
                        MDASlug = mda.Slug,
                    });
                }
            }

            MDAMonthlyPaymentViewModel model = new MDAMonthlyPaymentViewModel()
            {
                PaymentReport = reports,
            };
            return model;
        }


        public MDAMonthlyPaymentPerRevenueViewModel GetMonthlyMDAsPaymentPerRevenueHead(IEnumerable<RevenueHead> strippedRevenHeads, DateTime endDate, DateTime startDate)
        {
            List<PaymentPerRevenueReport> reports = new List<PaymentPerRevenueReport>();
            foreach (var revenueHead in strippedRevenHeads)
            {
                if (revenueHead.Invoices.Any())
                {
                    var invoices = revenueHead.Invoices.Where(invc => ((invc.CreatedAtUtc >= startDate) && (invc.CreatedAtUtc <= endDate)));
                    if (invoices != null)
                    {
                        reports.Add(new PaymentPerRevenueReport()
                        {
                            MDAName = revenueHead.Mda.Name,
                            RevenueHeadName = revenueHead.Name,
                            RevenueHeadSlug = revenueHead.Slug,
                            ActualIncome = (long)invoices.Where(invc => invc.Status == (int)InvoiceStatusList.Paid).Sum(invc => invc.Amount),
                            ExpectedIncome = (long)invoices.Sum(invc => invc.Amount),
                            NumberOfInvoices = invoices.Count(),
                            RevenueHeadId = revenueHead.Id
                        });
                    }
                    else
                    {
                        reports.Add(new PaymentPerRevenueReport()
                        {
                            MDAName = revenueHead.Mda.Name,
                            RevenueHeadName = revenueHead.Name,
                            RevenueHeadSlug = revenueHead.Slug,
                        });
                    }
                }
                else
                {
                    reports.Add(new PaymentPerRevenueReport()
                    {
                        MDAName = revenueHead.Mda.Name,
                        RevenueHeadName = revenueHead.Name,
                        RevenueHeadSlug = revenueHead.Slug,
                    });
                }

            }

            MDAMonthlyPaymentPerRevenueViewModel model = new MDAMonthlyPaymentPerRevenueViewModel()
            {
                PaymentPerRevenueReport = reports,
            };
            return model;
        }


        /// <summary>
        /// Check for invoice
        /// </summary>
        /// <param name="uniqueInvoiceIdentifier"></param>
        /// <param name="taxPayerIdentificationNumber"></param>
        /// <param name="revenueHead"></param>
        /// <param name="category"></param>
        /// <returns>Models.Invoice | null</returns>
        public InvoiceGeneratedResponseExtn CheckInvoice(string uniqueInvoiceIdentifier, long taxEntityId, RevenueHead revenueHead, TaxEntityCategory category)
        {
            return _transactionManager.GetSession()
                .Query<Invoice>()
                .Where(inv => ((inv.TaxPayer == new TaxEntity { Id = taxEntityId }) && (inv.RevenueHead == revenueHead) && (inv.TaxPayerCategory == category) && (inv.CashflowInvoiceIdentifier == uniqueInvoiceIdentifier)))
                .Select(inv => new InvoiceGeneratedResponseExtn { InvoicePreviewUrl = inv.InvoiceURL, InvoiceNumber = inv.InvoiceNumber, AmountDue = inv.InvoiceAmountDueSummary.AmountDue, Email = inv.TaxPayer.Email, PhoneNumber = inv.TaxPayer.PhoneNumber, ExternalRefNumber = inv.ExternalRefNumber, MDAName = inv.Mda.Name, RevenueHeadName = revenueHead.Name, TIN = inv.TaxPayer.TaxPayerIdentificationNumber, Recipient = inv.TaxPayer.Recipient, StatusValue = inv.Status, Description = inv.InvoiceDescription, PayerId = inv.TaxPayer.PayerId }).ToList()
                .FirstOrDefault();
        }


        /// <summary>
        /// Get invoice belonging to this entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="revenueHead"></param>
        /// <returns>InvoiceGeneratedResponseExtn | null</returns>
        public InvoiceGeneratedResponseExtn CheckInvoice(TaxEntity entity, RevenueHead revenueHead)
        {
            var invoiceDetails = _transactionManager.GetSession()
                .Query<Invoice>()
                .Where(inv => ((inv.TaxPayer == entity) && (inv.RevenueHead == revenueHead))).Take(1)
                .Select(inv => new InvoiceGeneratedResponseExtn
                {
                    InvoicePreviewUrl = inv.InvoiceURL,
                    InvoiceNumber = inv.InvoiceNumber,
                    AmountDue = inv.InvoiceAmountDueSummary.AmountDue,
                    Email = entity.Email,
                    PhoneNumber = entity.PhoneNumber,
                    ExternalRefNumber = inv.ExternalRefNumber,
                    MDAName = inv.Mda.Name,
                    RevenueHeadName = revenueHead.Name,
                    TIN = entity.TaxPayerIdentificationNumber,
                    Recipient = entity.Recipient,
                    StatusValue = inv.Status,
                    PayerId = entity.PayerId,
                    CustomerId = inv.TaxPayer.CashflowCustomerId,
                    CustomerPrimaryContactId = inv.TaxPayer.PrimaryContactId,
                    Description = inv.InvoiceDescription,
                    InvoiceId = inv.Id
                }).ToList()
                .FirstOrDefault();

            return SetInvoiceStatusEnum(invoiceDetails);
        }


        private InvoiceGeneratedResponseExtn SetInvoiceStatusEnum(InvoiceGeneratedResponseExtn invoiceDetails)
        {
            if (invoiceDetails != null)
            {
                invoiceDetails.InvoiceStatus = (InvoiceStatus)invoiceDetails.StatusValue;
            }
            return invoiceDetails;
        }


        /// <summary>
        /// Get details pertaining to this invoice by invoiceType and resourceTypeId
        /// </summary>
        /// <param name="invoiceType"></param>
        /// <param name="invoiceTypeId"></param>
        /// <returns>InvoiceDetailsHelperModel</returns>
        public InvoiceDetailsHelperModel GetInvoiceDetails(InvoiceType invoiceType, long invoiceTypeId)
        {
            try
            {
                var result = _transactionManager.GetSession().Query<Invoice>()
                            .Where(inv => ((inv.InvoiceType == (int)invoiceType) && (inv.InvoiceTypeId == invoiceTypeId)))
                            .Select(inv => new InvoiceDetailsHelperModel()
                            {
                                Invoice = inv,
                                TaxCategoryId = inv.TaxPayerCategory.Id,
                                TaxEntityId = inv.TaxPayer.Id,
                                TaxEntityAccount = inv.TaxPayer.TaxEntityAccount,
                                RevenueHeadId = inv.RevenueHead.Id,
                                MDAId = inv.RevenueHead.Mda.Id,
                                AmountDue = inv.InvoiceAmountDueSummary.AmountDue,
                                ExpertSystemClientSecret = inv.ExpertSystemSettings.ClientSecret,
                                APIRequestReference = inv.APIRequest != null ? inv.APIRequest.RequestIdentifier : null,
                                InvoiceItems = inv.InvoiceItems.Select(itm => new InvoiceItemsSummary
                                {
                                    Id = itm.Id,
                                    TotalAmount = itm.TotalAmount,
                                    RevenueHeadId = itm.RevenueHead.Id,
                                    MDAId = itm.Mda.Id
                                }).ToList()
                            }).ToList().SingleOrDefault();
                return result;
            }
            catch (Exception exception)
            { Logger.Error(exception, exception.Message); throw; }
        }


        /// <summary>
        /// Get details pertaining to this invoice
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>InvoiceDetailsHelperModel</returns>
        public InvoiceDetailsHelperModel GetInvoiceDetails(string invoiceNumber)
        {
            try
            {
                if (invoiceNumber.Length < 10)
                {
                    return _transactionManager.GetSession().Query<Invoice>()
                            .Where(inv => (inv.NAGISInvoiceNumber == invoiceNumber))
                            .Select(inv => new InvoiceDetailsHelperModel()
                            {
                                Invoice = inv,
                                TaxCategoryId = inv.TaxPayerCategory.Id,
                                TaxEntityId = inv.TaxPayer.Id,
                                TaxEntityAccount = inv.TaxPayer.TaxEntityAccount,
                                RevenueHeadId = inv.RevenueHead.Id,
                                MDAId = inv.RevenueHead.Mda.Id,
                                MDAName = inv.Mda.Name,
                                RevenueHead = inv.RevenueHead.Name,
                                AmountDue = inv.InvoiceAmountDueSummary.AmountDue,
                                ExpertSystemClientSecret = inv.ExpertSystemSettings.ClientSecret,
                                APIRequestReference = inv.APIRequest != null ? inv.APIRequest.RequestIdentifier : null,
                                InvoiceItems = inv.InvoiceItems.Select(itm => new InvoiceItemsSummary
                                {
                                    Id = itm.Id,
                                    TotalAmount = itm.TotalAmount,
                                    RevenueHeadId = itm.RevenueHead.Id,
                                    MDAId = itm.Mda.Id,
                                    MDAName = itm.Mda.Name
                                }).ToList()
                            }).ToList().SingleOrDefault();
                }

                return _transactionManager.GetSession().Query<Invoice>()
                            .Where(inv => (inv.InvoiceNumber == invoiceNumber)).Take(1)
                            .Select(inv => new InvoiceDetailsHelperModel()
                            {
                                Invoice = inv,
                                TaxCategoryId = inv.TaxPayerCategory.Id,
                                TaxEntityId = inv.TaxPayer.Id,
                                TaxEntityAccount = inv.TaxPayer.TaxEntityAccount,
                                RevenueHeadId = inv.RevenueHead.Id,
                                MDAId = inv.RevenueHead.Mda.Id,
                                MDAName = inv.Mda.Name,
                                RevenueHead = inv.RevenueHead.Name,
                                AmountDue = inv.InvoiceAmountDueSummary.AmountDue,
                                ExpertSystemClientSecret = inv.ExpertSystemSettings.ClientSecret,
                                APIRequestReference = inv.APIRequest != null ? inv.APIRequest.RequestIdentifier : null,
                                InvoiceItems = inv.InvoiceItems.Select(itm => new InvoiceItemsSummary
                                {
                                    Id = itm.Id,
                                    TotalAmount = itm.TotalAmount,
                                    RevenueHeadId = itm.RevenueHead.Id,
                                    MDAId = itm.Mda.Id,
                                    MDAName = itm.Mda.Name
                                }).ToList()
                            }).ToList().SingleOrDefault();
            }
            catch (Exception exception)
            { Logger.Error(exception, exception.Message); throw; }
        }


        public InvoiceDetailsHelperModel GetInvoiceDetails(long invoiceId)
        {
            try
            {
                var result = _transactionManager.GetSession().Query<Invoice>()
                            .Where(inv => (inv.Id == invoiceId))
                            .Select(inv => new InvoiceDetailsHelperModel()
                            {
                                Invoice = inv,
                                TaxCategoryId = inv.TaxPayerCategory.Id,
                                TaxEntityId = inv.TaxPayer.Id,
                                TaxEntityAccount = inv.TaxPayer.TaxEntityAccount,
                                RevenueHeadId = inv.RevenueHead.Id,
                                MDAId = inv.RevenueHead.Mda.Id,
                                MDAName = inv.Mda.Name,
                                RevenueHead = inv.RevenueHead.Name,
                                AmountDue = inv.InvoiceAmountDueSummary.AmountDue,
                                ExpertSystemClientSecret = inv.ExpertSystemSettings.ClientSecret,
                                APIRequestReference = inv.APIRequest != null ? inv.APIRequest.RequestIdentifier : null,
                                InvoiceItems = inv.InvoiceItems.Select(itm => new InvoiceItemsSummary
                                {
                                    Id = itm.Id,
                                    TotalAmount = itm.TotalAmount,
                                    RevenueHeadId = itm.RevenueHead.Id,
                                    MDAId = itm.Mda.Id,
                                    MDAName = itm.Mda.Name
                                }).ToList()
                            }).ToList().SingleOrDefault();
                return result;
            }
            catch (Exception exception)
            { Logger.Error(exception, exception.Message); throw; }
        }



        public bool SaveBundleBatchInvoiceResponse(DataTable listOfDataTables, string tableName)
        {
            try
            {
                using (var connection = (SqlConnection)(((ISessionFactoryImplementor)_transactionManager.GetSession().SessionFactory).ConnectionProvider.GetConnection()))
                {
                    using (SqlTransaction tranx = connection.BeginTransaction())
                    {
                        using (var copy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, tranx))
                        {
                            try
                            {
                                copy.BulkCopyTimeout = 10000;
                                copy.DestinationTableName = tableName;
                                foreach (DataColumn column in listOfDataTables.Columns) { copy.ColumnMappings.Add(column.ColumnName, column.ColumnName); }

                                copy.WriteToServer(listOfDataTables);
                                tranx.Commit();
                            }
                            catch (Exception exception)
                            {
                                tranx.Rollback();
                                throw exception;
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }



        /// <summary>
        /// Get invoice URL
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>string</returns>
        public string GetInvoiceURL(string invoiceNumber)
        {
            if (invoiceNumber.Length < 10)
            {
                return _transactionManager.GetSession().Query<Invoice>().Where(inv => inv.NAGISInvoiceNumber == invoiceNumber).Select(invurl => invurl.InvoiceURL).ToList().SingleOrDefault();
            }
            else
            {
                return _transactionManager.GetSession().Query<Invoice>().Where(inv => inv.InvoiceNumber == invoiceNumber).Take(1).Select(invurl => invurl.InvoiceURL).ToList().SingleOrDefault();
            }
        }


        /// <summary>
        /// Get invoice URL
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>string</returns>
        public Int64 GetInvoiceId(string invoiceNumber)
        {
            if (invoiceNumber.Length < 10)
            {
                return _transactionManager.GetSession().Query<Invoice>().Where(inv => inv.NAGISInvoiceNumber == invoiceNumber).Select(invurl => invurl.Id).ToList().SingleOrDefault();
            }
            else
            {
                return _transactionManager.GetSession().Query<Invoice>().Where(inv => inv.InvoiceNumber == invoiceNumber).Take(1).Select(invurl => invurl.Id).ToList().SingleOrDefault();
            }
        }


        /// <summary>
        /// Get the in5voice details for an invoice given the invoice number for make payment page
        /// <para>Will return null if invoice not found</para>
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>InvoiceGeneratedResponseExtn</returns>
        public InvoiceGeneratedResponseExtn GetInvoiceDetailsForPaymentView(string invoiceNumber)
        {
            try
            {
                InvoiceGeneratedResponseExtn invoiceDetails = null;
                //do check if invoice number length is under 10, for nagis purposes
                if (invoiceNumber.Length < 10)
                {
                    try
                    {
                        invoiceDetails = _transactionManager.GetSession().Query<Invoice>()
                                            .Where(inv => (inv.NAGISInvoiceNumber == invoiceNumber && inv.Status != (int)InvoiceStatus.WriteOff))
                                            .Select(inv => new InvoiceGeneratedResponseExtn()
                                            {
                                                DueDate = inv.DueDate,
                                                PayerId = inv.TaxPayer.PayerId,
                                                Email = inv.TaxPayer.Email,
                                                PhoneNumber = inv.TaxPayer.PhoneNumber,
                                                Recipient = inv.TaxPayer.Recipient,
                                                TIN = inv.TaxPayer.TaxPayerIdentificationNumber,
                                                AmountDue = inv.InvoiceAmountDueSummary.AmountDue,
                                                ExternalRefNumber = inv.ExternalRefNumber,
                                                InvoiceNumber = inv.InvoiceNumber,
                                                MDAName = inv.Mda.Name,
                                                RevenueHeadName = inv.RevenueHead.Name,
                                                InvoicePreviewUrl = inv.InvoiceURL,
                                                ShowRemitta = inv.TaxPayerCategory.GetSettings().IsFederalAgency,
                                                StatusValue = inv.Status,
                                                RevenueHeadID = inv.RevenueHead.Id,
                                                InvoiceId = inv.Id,
                                                InvoiceTitle = inv.InvoiceTitle,
                                                InvoiceDesc = inv.InvoiceDescription,
                                                MDAId = inv.Mda.Id,
                                                HasPaymentProviderValidationConstraint = inv.Mda.HasPaymentProviderValidationConstraint,
                                                MDASettlementCode = inv.Mda.SettlementCode,
                                                RevenueHeadSettlementCode = inv.RevenueHead.SettlementCode,
                                                MDASettlementType = inv.Mda.SettlementType,
                                                RevenueHeadSettlementType = inv.RevenueHead.SettlementType,
                                                RevenueHeadServiceId = inv.RevenueHead.ServiceId
                                            }).ToList().SingleOrDefault();
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception, string.Format("error getting invoice details {0}", invoiceNumber));
                    }
                }
                else
                {
                    invoiceDetails = _transactionManager.GetSession().Query<Invoice>()
                           .Where(inv => (inv.InvoiceNumber == invoiceNumber) && inv.Status != (int)InvoiceStatus.WriteOff).Take(1)
                           .Select(inv => new InvoiceGeneratedResponseExtn()
                           {
                               PayerId = inv.TaxPayer.PayerId,
                               Email = inv.TaxPayer.Email,
                               PhoneNumber = inv.TaxPayer.PhoneNumber,
                               Recipient = inv.TaxPayer.Recipient,
                               TIN = inv.TaxPayer.TaxPayerIdentificationNumber,
                               AmountDue = inv.InvoiceAmountDueSummary.AmountDue,
                               ExternalRefNumber = inv.ExternalRefNumber,
                               InvoiceNumber = inv.InvoiceNumber,
                               MDAName = inv.Mda.Name,
                               RevenueHeadName = inv.RevenueHead.Name,
                               InvoicePreviewUrl = inv.InvoiceURL,
                               ShowRemitta = inv.TaxPayerCategory.GetSettings().IsFederalAgency,
                               StatusValue = inv.Status,
                               RevenueHeadID = inv.RevenueHead.Id,
                               InvoiceId = inv.Id,
                               InvoiceTitle = inv.InvoiceTitle,
                               InvoiceDesc = inv.InvoiceDescription,
                               MDAId = inv.Mda.Id,
                               HasPaymentProviderValidationConstraint = inv.Mda.HasPaymentProviderValidationConstraint,
                               MDASettlementCode = inv.Mda.SettlementCode,
                               RevenueHeadSettlementCode = inv.RevenueHead.SettlementCode,
                               MDASettlementType = inv.Mda.SettlementType,
                               RevenueHeadSettlementType = inv.RevenueHead.SettlementType,
                               RevenueHeadServiceId = inv.RevenueHead.ServiceId
                           }).ToList().FirstOrDefault();
                }
                return SetInvoiceStatusEnum(invoiceDetails);
            }
            catch (Exception exception)
            { Logger.Error(exception, exception.Message); throw; }
        }



        /// <summary>
        /// Get the receipts that belong to this invoice
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>InvoiceGeneratedResponseExtn</returns>
        public InvoiceGeneratedResponseExtn GetReceiptsBelongingToInvoiceNumber(string invoiceNumber)
        {
            try
            {
                if (invoiceNumber.Length < 10)
                {
                    InvoiceGeneratedResponseExtn invoiceDetails = _transactionManager.GetSession().Query<Invoice>()
                            .Where(inv => (inv.NAGISInvoiceNumber == invoiceNumber))
                            .Select(inv => new InvoiceGeneratedResponseExtn()
                            {
                                PayerId = inv.TaxPayer.PayerId,
                                Email = inv.TaxPayer.Email,
                                PhoneNumber = inv.TaxPayer.PhoneNumber,
                                Recipient = inv.TaxPayer.Recipient,
                                TIN = inv.TaxPayer.TaxPayerIdentificationNumber,
                                AmountDue = inv.InvoiceAmountDueSummary.AmountDue,
                                ExternalRefNumber = inv.ExternalRefNumber,
                                InvoiceNumber = inv.InvoiceNumber,
                                MDAName = inv.Mda.Name,
                                RevenueHeadName = inv.RevenueHead.Name,
                                InvoicePreviewUrl = inv.InvoiceURL,
                                StatusValue = inv.Status,
                                Transactions = inv.Payments.Where(p => p.TypeID == (int)PaymentType.Credit)
                                .DefaultIfEmpty().Select(p => new TransactionLogGroup
                                {
                                    TotalAmountPaid = p.AmountPaid,
                                    PaymentReference = p.PaymentReference,
                                    ReceiptNumber = p.Receipt.ReceiptNumber,
                                    TransactionDate = p.CreatedAtUtc
                                }).ToList()
                            }).ToList().SingleOrDefault();


                    invoiceDetails = SetInvoiceStatusEnum(invoiceDetails);
                    if (invoiceDetails != null && invoiceDetails.Transactions.Any())
                    {
                        invoiceDetails.Transactions = invoiceDetails.Transactions
                            .GroupBy(t => t.ReceiptNumber, t => t,
                            (key, tranx) => new TransactionLogGroup
                            {
                                ReceiptNumber = key,
                                TotalAmountPaid = tranx.Sum(s => s.TotalAmountPaid),
                                PaymentReference = tranx.ElementAt(0).PaymentReference,
                                TransactionDate = tranx.ElementAt(0).TransactionDate,
                            }).ToList();
                    }

                    return invoiceDetails;
                }
                else
                {
                    InvoiceGeneratedResponseExtn invoiceDetails = _transactionManager.GetSession().Query<Invoice>()
                            .Where(inv => (inv.InvoiceNumber == invoiceNumber))
                            .Select(inv => new InvoiceGeneratedResponseExtn()
                            {
                                PayerId = inv.TaxPayer.PayerId,
                                Email = inv.TaxPayer.Email,
                                PhoneNumber = inv.TaxPayer.PhoneNumber,
                                Recipient = inv.TaxPayer.Recipient,
                                TIN = inv.TaxPayer.TaxPayerIdentificationNumber,
                                AmountDue = inv.InvoiceAmountDueSummary.AmountDue,
                                ExternalRefNumber = inv.ExternalRefNumber,
                                InvoiceNumber = inv.InvoiceNumber,
                                MDAName = inv.Mda.Name,
                                RevenueHeadName = inv.RevenueHead.Name,
                                InvoicePreviewUrl = inv.InvoiceURL,
                                StatusValue = inv.Status,
                                Transactions = inv.Payments.Where(p => p.TypeID == (int)PaymentType.Credit)
                                .DefaultIfEmpty().Select(p => new TransactionLogGroup
                                {
                                    TotalAmountPaid = p.AmountPaid,
                                    PaymentReference = p.PaymentReference,
                                    ReceiptNumber = p.Receipt.ReceiptNumber,
                                    TransactionDate = p.CreatedAtUtc
                                }).ToList()
                            }).ToList().FirstOrDefault();


                    invoiceDetails = SetInvoiceStatusEnum(invoiceDetails);
                    if (invoiceDetails != null && invoiceDetails.Transactions.Any())
                    {
                        invoiceDetails.Transactions = invoiceDetails.Transactions
                            .GroupBy(t => t.ReceiptNumber, t => t,
                            (key, tranx) => new TransactionLogGroup
                            {
                                ReceiptNumber = key,
                                TotalAmountPaid = tranx.Sum(s => s.TotalAmountPaid),
                                PaymentReference = tranx.ElementAt(0).PaymentReference,
                                TransactionDate = tranx.ElementAt(0).TransactionDate,
                            }).ToList();
                    }

                    return invoiceDetails;
                }

            }
            catch (Exception exception)
            { Logger.Error(exception, exception.Message); throw; }
        }


        /// <summary>
        /// Get transaction for each individual invoice items for this invoice number
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>InvoiceDetails</returns>
        public InvoiceDetails GetInvoiceTransactions(string invoiceNumber)
        {
            try
            {
                InvoiceDetails invoiceDetails = _transactionManager.GetSession().Query<Invoice>()
                        .Where(inv => (inv.InvoiceNumber == invoiceNumber))
                        .Select(inv => new InvoiceDetails()
                        {
                            CallBackURL = inv.CallBackURL,
                            RevenueHeadCallBackURL = inv.RevenueHead.CallBackURL,
                            RequestRef = inv.APIRequest != null ? inv.APIRequest.RequestIdentifier : null,
                            InvoiceNumber = inv.InvoiceNumber,
                            ExpertSystemKey = inv.ExpertSystemSettings.ClientSecret,
                            Transactions = inv.Payments.Where(p => p.TypeID != (int)PaymentType.Bill)
                            .DefaultIfEmpty().Select(p => new TransactionLogVM
                            {
                                PaymentDate = p.PaymentDate,
                                BankCode = p.BankCode,
                                BankBranch = p.BankBranch,
                                Bank = p.Bank,
                                Channel = p.Channel,
                                PaymentMethod = p.PaymentMethod,
                                PaymentProvider = p.PaymentProvider,
                                AmountPaid = p.AmountPaid,
                                PaymentReference = p.PaymentReference,
                                TransactionDate = p.TransactionDate
                            }).ToList()
                        }).ToList().SingleOrDefault();


                if (invoiceDetails == null || !invoiceDetails.Transactions.Any()) { throw new NoRecordFoundException { }; }

                if (invoiceDetails.Transactions.Count() == 1)
                {
                    invoiceDetails.Transactions.ElementAt(0).InvoiceNumber = invoiceDetails.InvoiceNumber;
                    return invoiceDetails;
                }

                //invoiceDetails.Transactions =
                var transformedTranLogs = invoiceDetails.Transactions
                .GroupBy(t => new
                {
                    t.PaymentProvider,
                    t.PaymentReference,
                    t.Channel
                }, t => t,
                (key, tranx) => new TransactionLogVM
                {
                    InvoiceNumber = invoiceDetails.InvoiceNumber,
                    PaymentReference = tranx.ElementAt(0).PaymentReference,
                    PaymentDate = tranx.ElementAt(0).PaymentDate,
                    BankCode = tranx.ElementAt(0).BankCode,
                    BankBranch = tranx.ElementAt(0).BankBranch,
                    AmountPaid = tranx.Sum(s => s.AmountPaid),
                    Bank = tranx.ElementAt(0).Bank,
                    Channel = tranx.ElementAt(0).Channel,
                    TransactionDate = tranx.ElementAt(0).TransactionDate,
                    PaymentMethod = tranx.ElementAt(0).PaymentMethod,
                    PaymentProvider = tranx.ElementAt(0).PaymentProvider,
                }).ToList();

                //
                invoiceDetails.Transactions = transformedTranLogs;

                return invoiceDetails;


            }
            catch (Exception exception)
            { Logger.Error(exception, exception.Message); throw; }
        }


        /// <summary>
        /// Get details for an invoice by invoice id
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns>InvoiceGeneratedResponseExtn</returns>
        public InvoiceGenerationResponse GetInvoiceByInvoiceIdForDuplicateRecordsX(Int64 invoiceId)
        {
            return _transactionManager.GetSession().Query<Invoice>()
                .Where(inv => (inv.Id == invoiceId)).Take(1)
                .Select(inv => new InvoiceGenerationResponse
                {
                    InvoiceTitle = inv.InvoiceTitle,
                    InvoiceDescription = inv.InvoiceDescription,
                    AmountDue = inv.InvoiceAmountDueSummary.AmountDue,
                    ExternalRefNumber = inv.ExternalRefNumber,
                    InvoiceNumber = inv.InvoiceNumber,
                    TIN = inv.TaxPayer.TaxPayerIdentificationNumber,
                    PhoneNumber = inv.TaxPayer.PhoneNumber,
                    InvoicePreviewUrl = inv.InvoiceURL,
                    Recipient = inv.TaxPayer.Recipient,
                    Email = inv.TaxPayer.Email,
                    StatusValue = inv.Status,
                    PayerId = inv.TaxPayer.PayerId,
                    CustomerId = inv.TaxPayer.CashflowCustomerId,
                    CustomerPrimaryContactId = inv.TaxPayer.PrimaryContactId,
                    Description = inv.InvoiceDescription,
                    InvoiceId = invoiceId,
                    IsDuplicateRequestReference = true,
                    InvoiceItemsSummaries = inv.InvoiceItems.Select(itm => new InvoiceItemsSummary
                    {
                        MDAName = itm.Mda.Name,
                        Quantity = itm.Quantity,
                        RevenueHeadId = itm.RevenueHead.Id,
                        RevenueHeadName = itm.RevenueHead.Name,
                        UnitAmount = itm.UnitAmount
                    }).ToList()
                }).ToList().SingleOrDefault();
        }


        /// <summary>
        /// Get details for an invoice by invoice id
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns>InvoiceGenerationResponse</returns>
        public InvoiceGeneratedResponseExtn GetInvoiceByInvoiceIdForDuplicateRecords(long invoiceId)
        {
            return _transactionManager.GetSession().Query<Invoice>()
                .Where(inv => (inv.Id == invoiceId)).Take(1)
                .Select(inv => new InvoiceGeneratedResponseExtn
                {
                    AmountDue = inv.InvoiceAmountDueSummary.AmountDue,
                    ExternalRefNumber = inv.ExternalRefNumber,
                    InvoiceNumber = inv.InvoiceNumber,
                    TIN = inv.TaxPayer.TaxPayerIdentificationNumber,
                    MDAName = inv.Mda.Name,
                    RevenueHeadName = inv.RevenueHead.Name,
                    PhoneNumber = inv.TaxPayer.PhoneNumber,
                    InvoicePreviewUrl = inv.InvoiceURL,
                    Recipient = inv.TaxPayer.Recipient,
                    Email = inv.TaxPayer.Email,
                    StatusValue = inv.Status,
                    PayerId = inv.TaxPayer.PayerId,
                    CustomerId = inv.TaxPayer.CashflowCustomerId,
                    CustomerPrimaryContactId = inv.TaxPayer.PrimaryContactId,
                    Description = inv.InvoiceDescription,
                    InvoiceId = invoiceId,
                    IsDuplicateRequestReference = true,
                }).ToList().SingleOrDefault();
        }


        /// <summary>
        /// Get the list of payment references and their providers for this invoice number
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>InvoiceDetails</returns>
        public InvoiceDetails GetPaymentRefs(string invoiceNumber)
        {
            try
            {
                return _transactionManager.GetSession().Query<Invoice>()
                .Where(inv => (inv.InvoiceNumber == invoiceNumber))
                .Select(inv => new InvoiceDetails
                {
                    InvoiceNumber = inv.InvoiceNumber,
                    PaymentReferenceVMs = inv.PaymentProviderPaymentReferences
                    .Select(pr => new PaymentReferenceVM
                    {
                        PaymentProvider = (PaymentProvider)pr.PaymentProvider,
                        ReferenceNumber = pr.ReferenceNumber,
                        DateGenerated = pr.CreatedAtUtc,
                    }).ToList()
                }).ToList().SingleOrDefault();
            }
            catch (Exception exception)
            {
                throw;
            }
        }


        public ICriteria GetCriteria(CollectionSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<Invoice>("invoice");
            criteria
                .CreateAlias("invoice.Mda", "Mda")
                .CreateAlias("invoice.RevenueHead", "RevenueHead")
                .Add(Restrictions.Eq("InvoiceNumber", searchParams.InvoiceNumber));

            var armCriteria = DetachedCriteria.For<AccessRoleMDARevenueHead>("arm")
            .Add(Restrictions.Disjunction()
            .Add(Restrictions
                .And(Restrictions.EqProperty("MDA.Id", "invoice.Mda.Id"), Restrictions.IsNull("RevenueHead.Id")))
            .Add(Restrictions.EqProperty("RevenueHead.Id", "invoice.RevenueHead.Id")))
            .SetProjection(Projections.Constant(1));

            var aruCriteria = DetachedCriteria.For<AccessRoleUser>("aru")
                .Add(Restrictions.Eq("User.Id", searchParams.AdminUserId))
                .Add(Restrictions.EqProperty("AccessRole.Id", "arm.AccessRole.Id"))
                .SetProjection(Projections.Constant(1));


            var arCriteria = DetachedCriteria.For<AccessRole>("ar")
                .Add(Restrictions.Eq("AccessType", (int)AccessType.CollectionReport))
                .Add(Restrictions.EqProperty("Id", "aru.AccessRole.Id"))
                .SetProjection(Projections.Constant(1));

            aruCriteria.Add(Subqueries.Exists(arCriteria));
            armCriteria.Add(Subqueries.Exists(aruCriteria));
            criteria.Add(Subqueries.Exists(armCriteria));

            return criteria;
        }


        /// <summary>
        /// Get status of an invoice with all the payment transactions on it.
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public Invoice GetInvoiceStatus(CollectionSearchParams searchParams)
        {
            bool hasAccess = _accessRoleUserRepo.UserHasAcessTypeRole(searchParams.AdminUserId, AccessType.CollectionReport);
            if (!hasAccess)
            {
                throw new UserNotAuthorizedForThisActionException($"User Id {searchParams.AdminUserId } is not authorised to view status of Invoice number {searchParams.InvoiceNumber}.");
            }

            var query = GetCriteria(searchParams);

            return query.Future<Invoice>().FirstOrDefault();
        }

        /// <summary>
        /// Confirm the existence of a paid development levy invoice using the invoice number and development levy revenue head
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <param name="developmentLevyRevenueHeadId"></param>
        /// <returns>int</returns>
        public long CheckDevelopmentLevyInvoice(string invoiceNumber, int developmentLevyRevenueHeadId)
        {
            return _transactionManager.GetSession()
                .Query<Invoice>()
                .Where(x => x.InvoiceNumber == invoiceNumber && x.RevenueHead.Id == developmentLevyRevenueHeadId && x.Status == (int)InvoiceStatus.Paid).Select(x => x.Id).Single();
        }

        /// <summary>
        /// Returns InvoiceStatusVM of invoice if it exists
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>InvoiceStatusVM</returns>
        public InvoiceStatusVM CheckDevelopmentLevyInvoice(string invoiceNumber)
        {
            return _transactionManager.GetSession()
                .Query<Invoice>()
                .Where(x => x.InvoiceNumber == invoiceNumber).Select(x => new InvoiceStatusVM { Id = x.Id, RevenueHeadId = x.RevenueHead.Id, Status = x.Status }).SingleOrDefault();
        }

        /// <summary>
        /// Get invoice details using the specified invoice number
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>ValidateInvoiceVM</returns>
        public ValidateInvoiceVM GetInvoiceInfo(string invoiceNumber)
        {
            return _transactionManager.GetSession().Query<Invoice>().Where(x => x.InvoiceNumber == invoiceNumber).Select(inv => new ValidateInvoiceVM
            {
                InvoiceNumber = inv.InvoiceNumber,
                ApplicantName = inv.TaxPayer.Recipient,
                InvoiceAmount = string.Format("{0:n2}", inv.Amount),
                AmountPaid = string.Format("{0:n2}", (inv.Amount - inv.InvoiceAmountDueSummary.AmountDue)),
                Status = (InvoiceStatus)inv.Status
            }).SingleOrDefault();
        }

        /// <summary>
        /// Checks if the invoice status is paid
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>boolean</returns>
        public bool IsInvoicePaid(string invoiceNumber)
        {
            try
            {
                if (invoiceNumber.Length < 10)
                {
                    return _transactionManager.GetSession().Query<Invoice>()
                                   .Count(inv => inv.NAGISInvoiceNumber == invoiceNumber && inv.Status == (int)InvoiceStatus.Paid) > 0;
                }

                return _transactionManager.GetSession().Query<Invoice>()
                                    .Count(inv => inv.InvoiceNumber == invoiceNumber && inv.Status == (int)InvoiceStatus.Paid) > 0;
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

    }

    public class InvoicesStatHolder
    {
        public decimal AmountExpected { get; set; }
        public Int64 Count { get; set; }
        public int Status { get; set; }
    }
}