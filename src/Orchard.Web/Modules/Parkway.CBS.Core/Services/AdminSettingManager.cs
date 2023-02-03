using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Services
{
    public class AdminSettingManager : BaseManager<ExpertSystemSettings>, IAdminSettingManager<ExpertSystemSettings>
    {
        private readonly IRepository<ExpertSystemSettings> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }

        public AdminSettingManager(IRepository<ExpertSystemSettings> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
            _user = user;
            _transactionManager = _orchardServices.TransactionManager;
        }

        /// <summary>
        /// Check if the collection of expert systems has a root system
        /// </summary>
        /// <returns>ExpertSystemSettings | null</returns>
        public ExpertSystemSettings HasRootExpertSystem()
        {
            return _repository.Fetch(xpt => xpt.IsRoot).SingleOrDefault();
        }


        /// <summary>
        /// Get a list of expert systems
        /// </summary>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns>IEnumerable{ExpertSystemSettings}</returns>
        public IEnumerable<ExpertSystemSettings> GetExpertSystemList(int take, int skip)
        {
            try
            {
                ExpertSystemSettings result = null;
               return _orchardServices.TransactionManager.GetSession().SessionFactory.OpenSession()
                        .QueryOver<ExpertSystemSettings>()
                        .Select(Projections.ProjectionList().Add(Projections.Property<ExpertSystemSettings>(xpt => xpt.AddedBy).WithAlias(() => result.AddedBy))
                        .Add(Projections.Property<ExpertSystemSettings>(xpt =>xpt.ClientId).WithAlias(() => result.ClientId ))
                        .Add(Projections.Property<ExpertSystemSettings>(xpt =>xpt.BillingSchedulerIdentifier).WithAlias(() => result.BillingSchedulerIdentifier ))
                        .Add(Projections.Property<ExpertSystemSettings>(xpt => xpt.CompanyName).WithAlias(() => result.CompanyName))
                        .Add(Projections.Property<ExpertSystemSettings>(xpt => xpt.Id).WithAlias(() => result.Id))
                        .Add(Projections.Property<ExpertSystemSettings>(xpt => xpt.URLPath).WithAlias(() => result.URLPath)))
                        .Where(xpt => xpt.Id != 0).Skip(skip).Take(take).UnderlyingCriteria.SetTimeout(120).SetResultTransformer(Transformers.AliasToBean<ExpertSystemSettings>()).List<ExpertSystemSettings>();
            }
            catch (Exception exception)
            { Logger.Error(exception, exception.Message); }
            return new List<ExpertSystemSettings> { };
        }


        /// <summary>
        /// Get list of expert systems. Returns a partially populated list of objects
        /// </summary>
        /// <returns>List{ExpertSystemSettings}</returns>
        public List<ExpertSystemSettings> GetExpertSystemsMDADropDown()
        {
            try
            {
                ExpertSystemSettings result = null;
                return _orchardServices.TransactionManager.GetSession().SessionFactory.OpenSession()
                         .QueryOver<ExpertSystemSettings>()
                         .Select(Projections.ProjectionList()
                         .Add(Projections.Property<ExpertSystemSettings>(xpt => xpt.BillingSchedulerIdentifier).WithAlias(() => result.BillingSchedulerIdentifier))
                         .Add(Projections.Property<ExpertSystemSettings>(xpt => xpt.CompanyName).WithAlias(() => result.CompanyName)))
                         .Where(xpt => xpt.Id != 0 && xpt.IsActive).UnderlyingCriteria.SetTimeout(120).SetResultTransformer(Transformers.AliasToBean<ExpertSystemSettings>()).List<ExpertSystemSettings>().ToList();
            }
            catch (Exception exception)
            { Logger.Error(exception, exception.Message); }
            return new List<ExpertSystemSettings> { };
        }

        /// <summary>
        /// Get client secret by client Id
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public string GetClientSecretByClientId(string clientId)
        {
            try
            {
                return _orchardServices.TransactionManager.GetSession().QueryOver<ExpertSystemSettings>() 
                    .Where(xpt => xpt.ClientId == clientId)
                    .Select(xpt => xpt.ClientSecret)                   
                    .List<string>()
                    .FirstOrDefault();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message + " Error getting client secret for client Id "+ clientId);
                return null;
            }
        }

        public void Flush()
        {
            _orchardServices.TransactionManager.GetSession().Flush();
        }



        /// <summary>
        /// Get root expert system
        /// <para>Returns the future instance</para>
        /// </summary>
        /// <returns>IEnumerable<ExpertSystemVM></returns>
        public IEnumerable<ExpertSystemVM> GetRootExpertSystem()
        {
            return _transactionManager.GetSession().Query<ExpertSystemSettings>()
          .Where(x => x.IsRoot)
          .Select(x => new ExpertSystemVM
          {
             Id = x.Id,
             StateId = x.TenantCBSSettings.StateId,
          }).ToFuture();
        }


        /// <summary>
        /// Get expert system with specified Id
        /// </summary>
        /// <param name="expertSystemId"></param>
        /// <returns>IEnumerable<ExpertSystemVM></returns>
        public IEnumerable<ExpertSystemVM> GetExpertSystemById(int expertSystemId)
        {
            return _transactionManager.GetSession().Query<ExpertSystemSettings>().Where(xpt => xpt.Id == expertSystemId).Select(xpt => new ExpertSystemVM
            {
                Id = xpt.Id,
                CompanyName = xpt.CompanyName
            }).ToFuture();
        }

    }
}