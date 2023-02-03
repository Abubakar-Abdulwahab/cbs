using CBSPay.Core.Entities;
using CBSPay.Core.Interfaces;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.Services
{
    /// <summary>
    /// Save Details of EIRS API models that dont change regularly to the db
    /// </summary>
    public class ConstantAPIModelService : IConstantAPIModelService
    {
        private readonly IBaseRepository<TaxPayerType> _taxpayerTypeRepo;
        private readonly IBaseRepository<RevenueStream> _revenueStreamRepo;
        private readonly IBaseRepository<RevenueSubStream> _revenueSubStreamRepo;
        private readonly IBaseRepository<TaxPayerDetails> _taxPayerRepo;
        private readonly IBaseRepository<EconomicActivities> _economicActivitiesRepo;
        private readonly IRestService _restService;

        public ConstantAPIModelService()
        {
            _taxpayerTypeRepo = new Repository<TaxPayerType>();
            _revenueStreamRepo = new Repository<RevenueStream>();
            _revenueSubStreamRepo = new Repository<RevenueSubStream>();
            _taxPayerRepo = new Repository<TaxPayerDetails>();
            _economicActivitiesRepo = new Repository<EconomicActivities>();
            _restService = new RestService();
        }
        private ILog Logger { get { return LogManager.GetLogger("CBSPay"); } }

        /// <summary>
        /// Fetch all active revenue Stream from the Db
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RevenueStream> FetchRevenueStreamList()
        {
            try
            {
                Logger.Debug("About to fetch revenue streams from the db");
                var revStreams = _revenueStreamRepo.Fetch(x => x.Active == true);
                Logger.Debug("Successfully fetched revenue streams from the db");
                return revStreams;
            }
            catch (Exception ex)
            {
                Logger.Error("Could not fetch revenue streams from the db");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }

        /// <summary>
        /// Fetch all active revenue sub stream from the Db
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RevenueSubStream> FetchRevenueSubStreamList()
        {
            try
            {
                Logger.Debug("About to fetch revenue sub streams from the db");
                var revSubStream = _revenueSubStreamRepo.Fetch(x => x.Active == true);
                Logger.Debug("Successfully fetched  revenue sub streams from the db");
                return revSubStream;
            }
            catch (Exception ex)
            {
                Logger.Error("Could not fetch  revenue sub streams from the db");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }

        /// <summary>
        /// Fetch all active economic activities based in the taxpayertype from the Db
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EconomicActivities> FetchEconomicActivitiesList(string taxPayerType)
        {
            try
            {
                Logger.Debug("About to fetch economic activities from the db");
                var economicActivities = _economicActivitiesRepo.Fetch(x => x.Active == true && x.TaxPayerTypeName == taxPayerType);
                Logger.Debug("Successfully fetched economic activities from the db");
                return economicActivities;
            }
            catch (Exception ex)
            {
                Logger.Error("Could not fetch economic activities from the db");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }

        /// <summary>
        /// get Revenue Stream from the API and save into the DB
        /// </summary>
        public void SaveRevenueStreamList()
        {
            try
            {
                Logger.Debug("Trying to save revenue stream list into DB");
                var Stream = _restService.GetRevenueStreamList();
                IEnumerable<RevenueStream> deserializedRevStream = JsonConvert.DeserializeObject<IEnumerable<RevenueStream>>(Stream.Result);
                if (deserializedRevStream.Count() > 0)
                {
                    foreach (var item in deserializedRevStream)
                    {
                        item.DateCreated = DateTime.Now;
                        item.DateModified = DateTime.Now;
                        item.IsDeleted = false;
                    }
                   _revenueStreamRepo.SaveBundle(deserializedRevStream);
                    Logger.Debug("Successfully saved revenue stream list into DB");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Could not save revenue stream list into DB");
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        /// <summary>
        /// get Revenue Sub Stream from the API and save into the DB
        /// </summary>
        public void SaveRevenueSubStreamList()
        {
            try
            {
                Logger.Debug("Trying to save revenue sub stream list into DB");
                var SubStream = _restService.GetRevenueSubStreamList();
                IEnumerable<RevenueSubStream> deserializedRevSubStream = JsonConvert.DeserializeObject<IEnumerable<RevenueSubStream>>(SubStream.Result);
                if (deserializedRevSubStream.Count() > 0)
                {
                    foreach (var item in deserializedRevSubStream)
                    {
                        item.DateCreated = DateTime.Now;
                        item.DateModified = DateTime.Now;
                        item.IsDeleted = false;
                    }
                    _revenueSubStreamRepo.SaveBundle(deserializedRevSubStream);
                    Logger.Debug("Successfully saved revenue sub stream list into DB");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Could not save revenue sub stream list into DB");
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        /// <summary>
        /// Get Economic Activities from the API and save into the DB
        /// </summary>
        public void SaveEconomicActivities(int taxPayer)
        {
            try
            {
                Logger.Debug($"Trying to save economic activities list for TaxPayerTypeId- {taxPayer} - into DB");
                var economicActivities = _restService.GetEconomicActivitiesList(taxPayer);
                IEnumerable<EconomicActivities> deserializedEconomicActivities = JsonConvert.DeserializeObject<IEnumerable<EconomicActivities>>(economicActivities.Result);
                if (deserializedEconomicActivities.Count() > 0)
                {
                    foreach (var item in deserializedEconomicActivities)
                    {
                        item.DateCreated = DateTime.Now;
                        item.DateModified = DateTime.Now;
                        item.IsDeleted = false;
                    }
                    _economicActivitiesRepo.SaveBundle(deserializedEconomicActivities);
                    Logger.Debug("Successfully saved economic activities list into DB");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Could not save economic activities list into DB");
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        /// <summary>
        /// get TaxPayersList from the API and save into the DB
        /// </summary>
        public void SaveTaxPayersTypeList()
        {
            try
            {
                Logger.Debug("Trying to save tax payer type list into DB");
                var taxPayerTypes = _restService.GetTaxPayerTypeList();
                IEnumerable<TaxPayerType> deserializedTaxPayerTypes = JsonConvert.DeserializeObject<IEnumerable<TaxPayerType>>(taxPayerTypes.Result);
                if (deserializedTaxPayerTypes.Count() > 0)
                {
                    foreach (var item in deserializedTaxPayerTypes)
                    {
                        item.DateCreated = DateTime.Now;
                        item.DateModified = DateTime.Now;
                        item.IsDeleted = false;
                    }
                    _taxpayerTypeRepo.SaveBundle(deserializedTaxPayerTypes);
                    Logger.Debug("Successfully saved revenue sub stream list into DB");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Could not save revenue stream list into DB");
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }

        public IEnumerable<TaxPayerType> FetchTaxPayersTypeList()
        {
            try
            {
                Logger.Debug("About to fetch Tax Payer Types from the db");
                var taxPayers = _taxpayerTypeRepo.Fetch(x => x.Active == true);
                Logger.Debug("Successfully fetched Tax Payer Types from the db");
                return taxPayers;
            }
            catch (Exception ex)
            {
                Logger.Error("Could not fetch Tax Payer Types from the db");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }
    }
}
