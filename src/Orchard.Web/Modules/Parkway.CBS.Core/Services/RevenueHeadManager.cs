using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Core.Services
{
    public class RevenueHeadManager : BaseManager<RevenueHead>, IRevenueHeadManager<RevenueHead>
    {
        private readonly IRepository<RevenueHead> _revenueHeaddRepository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public RevenueHeadManager(IRepository<RevenueHead> revenueHeaddRepository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(revenueHeaddRepository, user, orchardServices)
        {
            _revenueHeaddRepository = revenueHeaddRepository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }

        public IFutureValue<dynamic> CountRevenueHead()
        {
            var session = _orchardServices.TransactionManager.GetSession();
            return session.CreateCriteria<RevenueHead>().Add(!Restrictions.Eq("Id", 0))
                                                .SetProjection(Projections.Count(Projections.Id())).FutureValue<dynamic>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="revenueHeadSlug"></param>
        /// <param name="mdaId"></param>
        /// <returns></returns>
        public RevenueHead Get(string revenueHeadSlug, int mdaId)
        {
            try
            {
                return _revenueHeaddRepository.Fetch(r => (r.Slug == revenueHeadSlug && r.Mda.Id == mdaId)).FirstOrDefault();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return null;
        }

        /// <summary>
        /// Get revenue head with the slug and id
        /// </summary>
        /// <param name="revenueHeadSlug"></param>
        /// <param name="revenueHeadId"></param>
        /// <returns>RevenueHead</returns>
        public RevenueHead GetById(string slug, int Id)
        {
            try
            {
                return _revenueHeaddRepository.Get(r => ((r.Slug == slug) && (r.Id == Id)));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return null;
        }

        /// <summary>
        /// Return the revenue head that has the parent id and slug
        /// </summary>
        /// <param name="parentrevenueheadid"></param>
        /// <param name="revenueHeadSlug"></param>
        /// <returns>RevenueHead</returns>
        public RevenueHead GetSubRevenueHead(int parentrevenueheadid, string revenueHeadSlug)
        {
            try
            {
                return _revenueHeaddRepository.Get(r => ((r.Slug == revenueHeadSlug) && (r.Revenuehead.Id == parentrevenueheadid)));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return null;
        }

        public IEnumerable<RevenueHead> GetBillableCollection(string queryText)
        {
            var session = _orchardServices.TransactionManager.GetSession();
            return session.CreateCriteria<RevenueHead>()
                   .Add(Restrictions.InsensitiveLike("Code", queryText, MatchMode.Anywhere) || Restrictions.InsensitiveLike("Name", queryText, MatchMode.Anywhere))
                   .List<RevenueHead>();
        }


        /// <summary>
        /// Get a list of billable revenue heads
        /// </summary>
        /// <param name="queryText"></param>
        /// <returns>List{RevenueHeadLite}</returns>
        public List<RevenueHeadLite> GetBillableCollectionForAdminGenerateInvoice()
        {
            //all types of hackey, we do not want the admin user to be able to generate invoices for types that require file upload
            int directAssessmentType = (int)BillingType.DirectAssessment;
            int fileUploadType = (int)BillingType.FileUpload;

            return _transactionManager.GetSession().Query<RevenueHead>()
                .Where(revh => (revh.BillingModel != null) && (revh.IsVisible) && (revh.IsActive) && (revh.BillingModel.BillingType != directAssessmentType) && (revh.BillingModel.BillingType != fileUploadType)).Select(rev => new RevenueHeadLite { Code = rev.Code, Id = rev.Id, Name = rev.Name, MDAName = rev.Mda.Name, MDACode = rev.Mda.Code, BillingType = rev.BillingModel.BillingType }).ToList();
        }

        /// <summary>
        /// Save a bunch of Models of the same type
        /// </summary>
        /// <param name="revenueHead"></param>
        /// <returns>bool</returns>
        public bool SaveRevenueHeadWithRequestReference(RevenueHead revenueHead, ExpertSystemSettings expertSystem, string requestReference)
        {
            using (var session = _transactionManager.GetSession().SessionFactory.OpenStatelessSession())
            {
                using (var tranx = session.BeginTransaction())
                {
                    try
                    {
                        int id = (int)session.Insert(revenueHead);
                        session.Insert(new APIRequest { CallType = (int)CallTypeEnum.RevenueHead, ExpertSystemSettings = expertSystem, RequestIdentifier = requestReference, ResourceIdentifier = id });
                        tranx.Commit();
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception, string.Format("Could not save object ", Utilities.Util.SimpleDump(revenueHead)));
                        tranx.Rollback();
                        return false;
                    }
                }
            }
            Logger.Error("Revenue head saved");
            return true;
        }


        /// <summary>
        /// Get revenue head details
        /// <para>Gets the revenue head, mda, and billing info</para>
        /// </summary>
        /// <param name="id"></param>
        /// <returns>RevenueHeadDetails</returns>
        public RevenueHeadDetails GetRevenueHeadDetails(int id)
        {
            return _transactionManager.GetSession().Query<RevenueHead>()
                            .Where(revh => (revh.Id == id))
                            .Select(rev => new RevenueHeadDetails() { Billing = rev.BillingModel, RevenueHead = rev, Mda = rev.Mda, InvoiceGenerationRedirectURL = rev.InvoiceGenerationRedirectURL }).ToList().FirstOrDefault();
        }


        /// <summary>
        /// Get revenue head VM
        /// <para>Gets the revenue head VM</para>
        /// </summary>
        /// <param name="id">revenue head Id</param>
        /// <returns>RevenueHeadVM</returns>
        public RevenueHeadVM GetRevenueHeadVM(int id)
        {
            return _transactionManager.GetSession().Query<RevenueHead>()
                            .Where(revh => (revh.Id == id))
                            .Select(rev => new RevenueHeadVM() { Address = rev.Mda.MDASettings.CompanyAddress, Code = rev.Code, Name = rev.Name }).ToList().FirstOrDefault();
        }

        /// <summary>
        /// Get revenue head VM
        /// <para>Gets the revenue head VM</para>
        /// </summary>
        /// <param name="revenueHeadCode">revenueHeadCode</param>
        /// <returns>RevenueHeadVM</returns>
        public RevenueHeadVM GetRevenueHeadVMByCode(string revenueHeadCode)
        {
            return _transactionManager.GetSession().Query<RevenueHead>()
                            .Where(revh => (revh.Code == revenueHeadCode))
                            .Select(rev => new RevenueHeadVM() { Address = rev.Mda.MDASettings.CompanyAddress, Code = rev.Code, Name = rev.Name }).ToList().FirstOrDefault();
        }


        /// <summary>
        /// Get billing information
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <returns>BillingModel</returns>
        public BillingModel GetRevenueHeadBilling(int revenueHeadId)
        {
            return _transactionManager.GetSession().Query<RevenueHead>()
                            .Where(revh => (revh.Id == revenueHeadId))
                            .Select(rev => rev.BillingModel).ToList().FirstOrDefault();
        }


        /// <summary>
        /// Get revenue heads that belong to this MDA
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List{RevenueHeadVM}</returns>
        public IEnumerable<RevenueHeadVM> GetRevenueHeadsForMDA(int id)
        {
            return _transactionManager.GetSession().Query<RevenueHead>()
                            .Where(revh => (revh.Mda == new MDA { Id = id }))
                            .Select(rev => new RevenueHeadVM() { Name = rev.Name, Code = rev.Code, Id = rev.Id }).OrderBy(r => r.Name).ToFuture();
        }


        /// <summary>
        /// Get active and visible  billable revenue heads
        /// </summary>
        /// <param name="expertSysid"></param>
        /// <param name="mdaId"></param>
        /// <returns>IEnumerable{Models.RevenueHead}</returns>
        public IEnumerable<RevenueHeadLite> GetMDAActiveRevenueHeads(int expertSysid, int mdaId)
        {
            return _transactionManager.GetSession().Query<RevenueHead>()
                .Where(revh => (revh.Mda == new MDA { Id = mdaId }) && (revh.IsActive) && (revh.IsVisible) && (revh.BillingModel != null) && (revh.Mda.ExpertSystemSettings == new ExpertSystemSettings { Id = expertSysid })).Select(rev => new RevenueHeadLite { Id = rev.Id, Name = rev.Name, Code = rev.Code }).ToList();
        }


        /// <summary>
        /// Get details you would need for invoice generation from the revenue head
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <returns>RevenueHeadDetailsForInvoiceGeneration</returns>
        public RevenueHeadDetailsForInvoiceGeneration GetRevenueHeadDetailsForInvoiceGeneration(int revenueHeadId)
        {
            //interesting!
            return _transactionManager.GetSession().Query<RevenueHead>()
                .Where(revh => revh.Id == revenueHeadId).Select(rev => new RevenueHeadDetailsForInvoiceGeneration
                { RevenueHead = rev, Billing = rev.BillingModel, Mda = rev.Mda, Tenant = rev.Mda.ExpertSystemSettings.TenantCBSSettings, ExpertSystem = rev.Mda.ExpertSystemSettings }).ToList().FirstOrDefault();
        }

        /// <summary>
        /// Get the revenehead details
        /// </summary>
        /// <returns></returns>
        public RevenueHeadDetails GetRevenueHeadDetailsForPaye()
        {
            return _transactionManager.GetSession().Query<RevenueHead>()
                            .Where(revh => (revh.IsPayeAssessment))
                            .Select(rev => new RevenueHeadDetails() { Billing = rev.BillingModel, RevenueHead = rev, Mda = rev.Mda }).ToList().SingleOrDefault();
        }



        /// <summary>
        /// Get revenue head details
        /// <para>Gets the revenue head, mda, billing info and form details</para>
        /// </summary>
        /// <param name="id"></param>
        /// <returns>IEnumerable{RevenueHeadForInvoiceGenerationHelper}</returns>
        public IEnumerable<RevenueHeadForInvoiceGenerationHelper> GetRevenueHeadVMWithFormValidationDetails(int id)
        {
            return _transactionManager.GetSession().Query<RevenueHead>()
                            .Where(revh => (revh.Id == id))
                            .Select(rev => new RevenueHeadForInvoiceGenerationHelper()
                            {
                                BillingModelVM = new BillingModelVM
                                { Amount = rev.BillingModel.Amount, BillingType = rev.BillingModel.GetBillingType(), DueDate = rev.BillingModel.DueDate, StillRunning = rev.BillingModel.StillRunning, Id = rev.BillingModel.Id, PenaltyJSONModel = rev.BillingModel.Penalties, DiscountJSONModel = rev.BillingModel.Discounts, NextBillingDate = rev.BillingModel.NextBillingDate },
                                RevenueHeadVM = new RevenueHeadVM
                                { Code = rev.Code, Id = rev.Id, Name = rev.Name, InvoiceGenerationRedirectURL = rev.InvoiceGenerationRedirectURL, CashflowProductId = rev.CashFlowProductId },
                                MDAVM = new MDAVM
                                { Name = rev.Mda.Name, Id = rev.Mda.Id, Code = rev.Mda.Code, SMEKey = rev.Mda.SMEKey },
                                Forms = rev.FormControls.Select(fm => new FormControlViewModel
                                {
                                    ControlIdentifier = fm.Id,
                                    TaxEntityCategoryId = fm.TaxEntityCategory.Id,
                                    IsCompulsory = fm.IsComplusory,
                                    Validators = fm.Form.Validators,
                                    ValidationProps = fm.Form.ValidationProps,
                                    RevenueHeadId = rev.Id,
                                    FormId = fm.Form.Id
                                })
                            }).ToFuture();
        }


        /// <summary>
        /// Get revenue head details
        /// <para>Gets the revenue head, mda, and billing info</para>
        /// </summary>
        /// <param name="id"></param>
        /// <returns>IEnumerable{RevenueHeadForInvoiceGenerationHelper}</returns>
        public IEnumerable<RevenueHeadForInvoiceGenerationHelper> GetRevenueHeadVMForInvoiceGeneration(int id)
        {
            return _transactionManager.GetSession().Query<RevenueHead>()
                            .Where(revh => (revh.Id == id))
                            .Select(rev => new RevenueHeadForInvoiceGenerationHelper()
                            {
                                BillingModelVM = new BillingModelVM
                                { Amount = rev.BillingModel.Amount, BillingType = rev.BillingModel.GetBillingType(), DueDate = rev.BillingModel.DueDate, StillRunning = rev.BillingModel.StillRunning, Id = rev.BillingModel.Id, PenaltyJSONModel = rev.BillingModel.Penalties, DiscountJSONModel = rev.BillingModel.Discounts, NextBillingDate = rev.BillingModel.NextBillingDate },
                                RevenueHeadVM = new RevenueHeadVM
                                { Code = rev.Code, Id = rev.Id, Name = rev.Name, InvoiceGenerationRedirectURL = rev.InvoiceGenerationRedirectURL, CashflowProductId = rev.CashFlowProductId },
                                MDAVM = new MDAVM
                                { Name = rev.Mda.Name, Id = rev.Mda.Id, Code = rev.Mda.Code, SMEKey = rev.Mda.SMEKey },
                                Forms = rev.FormControls.Select(fm => new FormControlViewModel
                                {
                                    ControlIdentifier = fm.Id,
                                    RevenueHeadId = rev.Id,
                                    FormId = fm.Form.Id,
                                    TaxEntityCategoryId = fm.TaxEntityCategory.Id
                                })
                            }).ToFuture();
        }



        /// <summary>
        /// Get the group revenue head details
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns>RevenueHeadForInvoiceGenerationHelper</returns>
        public GenerateInvoiceRequestModel GetGroupRevenueHeadVMForInvoiceGeneration(int groupId)
        {
            try
            {
                var result = _transactionManager.GetSession().Query<RevenueHead>()
                                    .Where(revh => (revh.Id == groupId) && (revh.IsGroup))
                                    .Select(rev => new GenerateInvoiceRequestModel()
                                    {
                                        BillingModelVM = new BillingModelVM
                                        { BillingType = rev.BillingModel.GetBillingType(), DueDate = rev.BillingModel.DueDate, StillRunning = rev.BillingModel.StillRunning, Id = rev.BillingModel.Id, PenaltyJSONModel = rev.BillingModel.Penalties, DiscountJSONModel = rev.BillingModel.Discounts, NextBillingDate = rev.BillingModel.NextBillingDate },
                                        RevenueHeadVM = new RevenueHeadVM { Code = rev.Code, Id = rev.Id, Name = rev.Name, InvoiceGenerationRedirectURL = rev.InvoiceGenerationRedirectURL, CashflowProductId = rev.CashFlowProductId },
                                        MDAVM = new MDAVM { Name = rev.Mda.Name, Id = rev.Mda.Id, Code = rev.Mda.Code, SMEKey = rev.Mda.SMEKey },
                                        RevenueHeadGroupVM = rev.GroupParent.Select(gp => new RevenueHeadGroupVM { RevenueHeadsInGroup = gp.RevenueHead.Id }).ToList()
                                    }).ToList();

                if (result.Count() == 1)
                { return result.Single(); }
            }
            catch (Exception exception)
            {
                Logger.Error(string.Format("Error getting group id details {0}. Exception {1}", groupId, exception.Message), exception);
            }
            return null;
        }



        /// <summary>
        /// Get the list of revenue heads that this user has access and belongs to the given MDA Id
        /// </summary>
        /// <param name="adminUserId"></param>
        /// <param name="mdaId"></param>
        /// <param name="accessType"></param>
        /// <returns>IEnumerable{RevenueHeadDropDownListViewModel}</returns>
        public IEnumerable<RevenueHeadDropDownListViewModel> GetRevenueHeadsOnAccessListForMDA(int adminUserId, int mdaId, AccessType accessType, bool applyAccessRestrictions)
        {
            var session = _orchardServices.TransactionManager.GetSession();
            var criteria = session.CreateCriteria<RevenueHead>("RH")
                .Add(Restrictions.Eq("Mda.Id", mdaId));

            if (applyAccessRestrictions)
            {
                var armCriteria = DetachedCriteria.For<AccessRoleMDARevenueHead>("arm")
                .Add(Restrictions.Disjunction()
                .Add(Restrictions
                    .And(Restrictions.EqProperty("MDA.Id", "RH.Mda.Id"), Restrictions.IsNull("RevenueHead.Id")))
                .Add(Restrictions.EqProperty("RevenueHead.Id", "RH.Id")))
                .SetProjection(Projections.Constant(1));


                var aruCriteria = DetachedCriteria.For<AccessRoleUser>("aru")
                    .Add(Restrictions.Eq("User.Id", adminUserId))
                    .Add(Restrictions.EqProperty("AccessRole.Id", "arm.AccessRole.Id"))
                    .SetProjection(Projections.Constant(1));


                var arCriteria = DetachedCriteria.For<AccessRole>("ar")
                    .Add(Restrictions.Eq("AccessType", (int)accessType))
                    .Add(Restrictions.EqProperty("Id", "aru.AccessRole.Id"))
                    .SetProjection(Projections.Constant(1));


                aruCriteria.Add(Subqueries.Exists(arCriteria));
                armCriteria.Add(Subqueries.Exists(aruCriteria));
                criteria.Add(Subqueries.Exists(armCriteria));
            }

            criteria.SetProjection(
                 Projections.ProjectionList()
                .Add(Projections.Property<RevenueHead>(m => m.Name), "Name")
                .Add(Projections.Property<RevenueHead>(m => m.Code), "Code")
                .Add(Projections.Property<RevenueHead>(m => m.Id), "Id")
                ).SetResultTransformer(Transformers.AliasToBean<RevenueHeadDropDownListViewModel>());

            return criteria.AddOrder(Order.Asc("Name"))
                .Future<RevenueHeadDropDownListViewModel>();
        }


        public IEnumerable<RevenueHeadDropDownListViewModel> GetRevenueHeadsOnAccessListForMDA(int adminUserId, bool applyAccessRestrictions)
        {
            var session = _orchardServices.TransactionManager.GetSession();
            var criteria = session.CreateCriteria<RevenueHead>("RH");

            if (applyAccessRestrictions)
            {
                var armCriteria = DetachedCriteria.For<AccessRoleMDARevenueHead>("arm")
                .Add(Restrictions.Disjunction()
                .Add(Restrictions
                    .And(Restrictions.EqProperty("MDA.Id", "RH.Mda.Id"), Restrictions.IsNull("RevenueHead.Id")))
                .Add(Restrictions.EqProperty("RevenueHead.Id", "RH.Id")))
                .SetProjection(Projections.Constant(1));

                var aruCriteria = DetachedCriteria.For<AccessRoleUser>("aru")
                    .Add(Restrictions.Eq("User.Id", adminUserId))
                    .Add(Restrictions.EqProperty("AccessRole.Id", "arm.AccessRole.Id"))
                    .SetProjection(Projections.Constant(1));

                var arCriteria = DetachedCriteria.For<AccessRole>("ar")
                    .Add(Restrictions.EqProperty("Id", "aru.AccessRole.Id"))
                    .SetProjection(Projections.Constant(1));

                aruCriteria.Add(Subqueries.Exists(arCriteria));
                armCriteria.Add(Subqueries.Exists(aruCriteria));
                criteria.Add(Subqueries.Exists(armCriteria));
            }

            criteria.SetProjection(
                 Projections.ProjectionList()
                .Add(Projections.Property<RevenueHead>(m => m.Name), "Name")
                .Add(Projections.Property<RevenueHead>(m => m.Code), "Code")
                .Add(Projections.Property<RevenueHead>(m => m.Id), "Id")
                ).SetResultTransformer(Transformers.AliasToBean<RevenueHeadDropDownListViewModel>());

            return criteria.AddOrder(Order.Asc("Name"))
                .Future<RevenueHeadDropDownListViewModel>();
        }

        /// <summary>
        /// Get the list of revenue heads
        /// </summary>
        /// <returns>List<RevenueHeadDropDownListViewModel></returns>
        public List<RevenueHeadDropDownListViewModel> GetAllRevenueHeads()
        {
            return _transactionManager.GetSession().Query<RevenueHead>()
                            .Select(rev => new RevenueHeadDropDownListViewModel()
                            {
                                Id = rev.Id,
                                Name = rev.Name
                            }).ToList();
        }


        /// <summary>
        /// Get the list of MDAs for revenue heads that are billable
        /// </summary>
        /// <returns>IEnumerable{MDARevenueHeadsVM}</returns>
        public IEnumerable<MDARevenueHeadsVM> GetBillableRevenueHeadGroupByMDA()
        {
            return _transactionManager.GetSession().CreateCriteria<RevenueHead>(nameof(RevenueHead))
                .CreateAlias(nameof(RevenueHead) + "." + nameof(RevenueHead.BillingModel), nameof(RevenueHead.BillingModel))
                .CreateAlias(nameof(RevenueHead) + "." + nameof(RevenueHead.Mda), nameof(RevenueHead.Mda))
                .Add(Restrictions.IsNotNull(nameof(RevenueHead.BillingModel) + ".Id"))
                .SetProjection(Projections.ProjectionList()
                    .Add(Projections.Property<RevenueHead>(x => (x.Id)), nameof(MDARevenueHeadsVM.RevenueHeadID))
                    .Add(Projections.Property<RevenueHead>(x => (x.Mda.Id)), nameof(MDARevenueHeadsVM.MDAId))
                    .Add(Projections.Group<RevenueHead>(t => t.Mda.Id))
                    .Add(Projections.Group<RevenueHead>(t => t.Id))
            ).SetResultTransformer(Transformers.AliasToBean<MDARevenueHeadsVM>())
            .Future<MDARevenueHeadsVM>();
        }


        /// <summary>
        /// Get the billable revenue heads that belong to this MDA
        /// </summary>
        /// <param name="mDAId"></param>
        /// <returns>IEnumerable{int}</returns>
        public IEnumerable<int> GetBillableRevenueHeadsIDsForMDA(int mDAId)
        {
            return _transactionManager.GetSession().CreateCriteria<RevenueHead>(nameof(RevenueHead))
                .CreateAlias(nameof(RevenueHead) + "." + nameof(RevenueHead.BillingModel), nameof(RevenueHead.BillingModel))
                .CreateAlias(nameof(RevenueHead) + "." + nameof(RevenueHead.Mda), nameof(RevenueHead.Mda))
                .Add(Restrictions.IsNotNull(nameof(RevenueHead.BillingModel) + ".Id"))
                .Add(Restrictions.Eq(nameof(RevenueHead) + "." + nameof(RevenueHead.Mda) + ".Id", mDAId))
                .SetProjection(Projections.ProjectionList()
                    .Add(Projections.Property<RevenueHead>(x => (x.Id)))
            ).Future<int>();
        }


        /// <summary>
        /// Get all the MDAs that have a billable revenue head attached to it
        /// </summary>
        /// <returns>IEnumerable{MDAVM}</returns>
        public IEnumerable<MDAVM> GetMDAsForBillableRevenueHeads()
        {
            return _transactionManager.GetSession().CreateCriteria<RevenueHead>(nameof(RevenueHead))
                .CreateAlias(nameof(RevenueHead) + "." + nameof(RevenueHead.BillingModel), nameof(RevenueHead.BillingModel))
                .CreateAlias(nameof(RevenueHead) + "." + nameof(RevenueHead.Mda), nameof(RevenueHead.Mda))
                .Add(Restrictions.IsNotNull(nameof(RevenueHead.BillingModel) + ".Id"))
                .SetProjection(Projections.ProjectionList()
                    .Add(Projections.Property<RevenueHead>(x => (x.Mda.Name)), nameof(MDAVM.Name))
                    .Add(Projections.Property<RevenueHead>(x => (x.Mda.Id)), nameof(MDAVM.Id))
                    .Add(Projections.Property<RevenueHead>(x => (x.Mda.Code)), nameof(MDAVM.Code))
                    .Add(Projections.Group<RevenueHead>(t => t.Mda.Id))
                    .Add(Projections.Group<RevenueHead>(t => t.Mda.Name))
                    .Add(Projections.Group<RevenueHead>(t => t.Mda.Code))
            ).SetResultTransformer(Transformers.AliasToBean<MDAVM>())
            .Future<MDAVM>();
        }


        /// <summary>
        /// Get the billable revenue heads that belong to this MDA
        /// </summary>
        /// <param name="mdaId"></param>
        /// <returns>IEnumerable{int}</returns>
        public IEnumerable<RevenueHeadVM> GetBillableRevenueHeadsForMDA(int mdaId)
        {
            return _transactionManager.GetSession().CreateCriteria<RevenueHead>(nameof(RevenueHead))
                .CreateAlias(nameof(RevenueHead) + "." + nameof(RevenueHead.BillingModel), nameof(RevenueHead.BillingModel))
                .CreateAlias(nameof(RevenueHead) + "." + nameof(RevenueHead.Mda), nameof(RevenueHead.Mda))
                .Add(Restrictions.IsNotNull(nameof(RevenueHead.BillingModel) + ".Id"))
                .Add(Restrictions.Eq(nameof(RevenueHead) + "." + nameof(RevenueHead.Mda) + ".Id", mdaId))
                .SetProjection(Projections.ProjectionList()
                  .Add(Projections.Property<RevenueHead>(x => (x.Name)), nameof(RevenueHeadVM.Name))
                    .Add(Projections.Property<RevenueHead>(x => (x.Id)), nameof(RevenueHeadVM.Id))
                    .Add(Projections.Property<RevenueHead>(x => (x.Code)), nameof(RevenueHeadVM.Code))
                    .Add(Projections.Property<RevenueHead>(x => (x.Mda.Id)), nameof(RevenueHeadVM.MdaId))
            ).SetResultTransformer(Transformers.AliasToBean<RevenueHeadVM>())
            .Future<RevenueHeadVM>();
        }

        /// <summary>
        /// Get the list of revenue heads for the MDA with the specified id that the admin user has access to.
        /// </summary>
        /// <param name="mdaId"></param>
        /// <param name="adminUserId"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns></returns>
        public IEnumerable<RevenueHeadLite> GetRevenueHeadsPerMdaOnAccessList(int mdaId, int adminUserId, bool applyAccessRestrictions)
        {
            var session = _orchardServices.TransactionManager.GetSession();
            var criteria = session.CreateCriteria<RevenueHead>("RH");

            criteria.Add(Restrictions.Eq("Mda.Id", mdaId));

            if (applyAccessRestrictions)
            {
                var armCriteria = DetachedCriteria.For<AccessRoleMDARevenueHead>("arm")
                .Add(Restrictions.Disjunction()
                .Add(Restrictions
                    .And(Restrictions.EqProperty("MDA.Id", "RH.Mda.Id"), Restrictions.IsNull("RevenueHead.Id")))
                .Add(Restrictions.EqProperty("RevenueHead.Id", "RH.Id")))
                .SetProjection(Projections.Constant(1));

                var aruCriteria = DetachedCriteria.For<AccessRoleUser>("aru")
                    .Add(Restrictions.Eq("User.Id", adminUserId))
                    .Add(Restrictions.EqProperty("AccessRole.Id", "arm.AccessRole.Id"))
                    .SetProjection(Projections.Constant(1));

                var arCriteria = DetachedCriteria.For<AccessRole>("ar")
                    .Add(Restrictions.EqProperty("Id", "aru.AccessRole.Id"))
                    .SetProjection(Projections.Constant(1));

                aruCriteria.Add(Subqueries.Exists(arCriteria));
                armCriteria.Add(Subqueries.Exists(aruCriteria));
                criteria.Add(Subqueries.Exists(armCriteria));
            }

            criteria.SetProjection(
                 Projections.ProjectionList()
                .Add(Projections.Property<RevenueHead>(m => m.Name), "Name")
                .Add(Projections.Property<RevenueHead>(m => m.Id), "Id")
                .Add(Projections.Property<RevenueHead>(m => m.Mda.Id), "MDAId")
                .Add(Projections.Property<RevenueHead>(m => m.Code), "Code")
                ).SetResultTransformer(Transformers.AliasToBean<RevenueHeadLite>());

            return criteria.AddOrder(Order.Asc("Name"))
                .Future<RevenueHeadLite>();
        }


        /// <summary>
        /// Check if revenue head Id exists with for the specified mda Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mdaId"></param>
        /// <returns>bool</returns>
        public bool CheckIfRevenueHeadAndExistsWithMDA(int id, int mdaId)
        {
            return _transactionManager.GetSession().Query<RevenueHead>().Count(rh => rh.Id == id && rh.Mda.Id == mdaId) == 1;
        }

    }
}