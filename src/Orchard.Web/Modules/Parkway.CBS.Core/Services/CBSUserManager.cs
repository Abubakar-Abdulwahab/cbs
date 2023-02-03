using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System.Linq;
using Parkway.CBS.Core.HelperModels;
using NHibernate.Linq;
using System;
using Orchard.Logging;

namespace Parkway.CBS.Core.Services
{
    public class CBSUserManager : BaseManager<CBSUser>, ICBSUserManager<CBSUser>
    {
        private readonly IRepository<CBSUser> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        //private readonly IPAYEBatchRecordStagingManager<PAYEBatchRecordStaging> _gh;


        public CBSUserManager(IRepository<CBSUser> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices/*, IPAYEBatchRecordStagingManager<PAYEBatchRecordStaging> gh*/) : base(repository, user, orchardServices)
        {
            _transactionManager = orchardServices.TransactionManager;
            _repository = repository;
            //_gh = gh;
        }


        //public void F()
        //{
        //    _gh.InvoiceConfirmedMovePAYE(0,0);
        //}

        /// <summary>
        /// Get the user details for the userPartRecordId
        /// </summary>
        /// <param name="userPartRecordId"></param>
        /// <returns>UserDetailsModel | null</returns>
        public UserDetailsModel GetUserDetails(int userPartRecordId)
        {
            try
            {
                return _transactionManager.GetSession().Query<CBSUser>()
                       .Where(u => u.UserPartRecord == (new UserPartRecord { Id = userPartRecordId })).Take(1)
                       .Select(u => new UserDetailsModel
                       {
                           CBSUser = u,
                           Category = u.TaxEntity.TaxEntityCategory,
                           Entity = u.TaxEntity,
                           Name = u.Name,
                           TaxPayerProfileVM = new TaxEntityViewModel
                           {
                               Recipient = u.TaxEntity.Recipient,
                               Id = u.TaxEntity.Id,
                               Address = u.TaxEntity.Address,
                               TaxPayerIdentificationNumber = u.TaxEntity.TaxPayerIdentificationNumber,
                               Email = u.TaxEntity.Email,
                               RCNumber = u.TaxEntity.RCNumber,
                               PhoneNumber = u.TaxEntity.PhoneNumber,
                               SelectedStateLGA = u.TaxEntity.StateLGA.Id,
                               SelectedLGAName = u.TaxEntity.StateLGA.Name,
                               SelectedStateName = u.TaxEntity.StateLGA.State.Name,
                               ContactPersonName = u.TaxEntity.ContactPersonName,
                               ContactPersonEmail = u.TaxEntity.ContactPersonEmail,
                               ContactPersonPhoneNumber = u.TaxEntity.ContactPersonPhoneNumber,
                               PayerId = u.TaxEntity.PayerId,
                               IdType = u.TaxEntity.IdentificationType,
                               IdNumber = u.TaxEntity.IdentificationNumber,
                               Gender = (int)u.TaxEntity.Gender,
                           },
                           CategoryVM = new TaxEntityCategoryVM { Id = u.TaxEntity.TaxEntityCategory.Id, JSONSettings = u.TaxEntity.TaxEntityCategory.JSONSettings, Name = u.TaxEntity.TaxEntityCategory.Name },
                           CBSUserVM = new CBSUserVM { Name = u.Name, Id = u.Id, Verified = u.Verified, IsAdministrator = u.IsAdministrator, Email = u.Email, PhoneNumber = u.PhoneNumber, TaxEntity = new TaxEntityViewModel { Id = u.TaxEntity.Id } }
                       })
                       .ToList().FirstOrDefault();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error getting CBUser user part Id " + userPartRecordId);
            }
            return null;
        }


        /// <summary>
        /// Get CBS user and tax entity details
        /// </summary>
        /// <param name="profileId"></param>
        /// <returns>UserDetailsModel</returns>
        public UserDetailsModel GetCBSUserAndTaxEntity(long profileId)
        {
            return _transactionManager.GetSession().Query<CBSUser>()
               .Where(u => u.Id == profileId).Take(1)
               .Select(u => new UserDetailsModel { CBSUser = u, Category = u.TaxEntity.TaxEntityCategory, Entity = u.TaxEntity, Name = u.Name })
               .ToList().FirstOrDefault();
        }


        /// <summary>
        /// Get user details for account verification
        /// </summary>
        /// <param name="userPartRecordId"></param>
        /// <returns>UserDetailsModel</returns>
        public UserDetailsModel GetUserDetailsForAccountVerification(int userPartRecordId)
        {
            return _transactionManager.GetSession().Query<CBSUser>()
                .Where(u => u.UserPartRecord == (new UserPartRecord { Id = userPartRecordId }))
                .Select(u => new UserDetailsModel
                {
                    TaxPayerProfileVM = new TaxEntityViewModel
                    {
                        Recipient = u.TaxEntity.Recipient,
                        Id = u.TaxEntity.Id,
                        Email = u.TaxEntity.Email,
                        PhoneNumber = u.TaxEntity.PhoneNumber,
                    },
                    CBSUserVM = new CBSUserVM { Id = u.Id, Verified = u.Verified, Name = u.Name, Email = u.Email, PhoneNumber = u.PhoneNumber, TaxEntity = new TaxEntityViewModel { Id = u.TaxEntity.Id } }
                })
                .ToList().FirstOrDefault();
        }


        /// <summary>
        /// Get the user details for the cbs User Id
        /// </summary>
        /// <param name="userPartRecordId"></param>
        /// <returns>UserDetailsModel | null</returns>
        public UserDetailsModel GetUserDetailsForCBSUserId(long cbsUserId)
        {
            return _transactionManager.GetSession().Query<CBSUser>()
                .Where(u => u.Id == cbsUserId)
                .Select(u => new UserDetailsModel
                {
                    TaxPayerProfileVM = new TaxEntityViewModel
                    {
                        Recipient = u.TaxEntity.Recipient,
                        Id = u.TaxEntity.Id,
                        Email = u.TaxEntity.Email,
                        PhoneNumber = u.TaxEntity.PhoneNumber,
                        Address = u.TaxEntity.Address
                    },
                    CBSUserVM = new CBSUserVM { Id = u.Id, Verified = u.Verified, Email = u.Email, PhoneNumber = u.PhoneNumber }
                })
                .ToList().FirstOrDefault();
        }


        /// <summary>
        /// Get the register user model for this email
        /// <para>This method return only part properties</para>
        /// </summary>
        /// <param name="email"></param>
        /// <returns>RegisterUserResponse</returns>
        public RegisterUserResponse GetRegisterUserResponse(string email)
        {
            return _transactionManager.GetSession().Query<CBSUser>()
                .Where(u => u.TaxEntity.Email == email)
                .Select(u => new RegisterUserResponse
                {
                    CBSUserId = u.Id,
                    TaxEntityVM = new TaxEntityViewModel
                    {
                        Id = u.TaxEntity.Id,
                        Email = u.TaxEntity.Email,
                        Recipient = u.TaxEntity.Recipient,
                        PhoneNumber = u.TaxEntity.PhoneNumber,
                    }
                }).ToList().SingleOrDefault();
        }


        /// <summary>
        /// Get the register user model for this cbs user email
        /// <para>This method return only part properties</para>
        /// </summary>
        /// <param name="email"></param>
        /// <returns>RegisterUserResponse</returns>
        public RegisterUserResponse GetRegisterCBSUserResponse(string email)
        {
            return _transactionManager.GetSession().Query<CBSUser>()
                .Where(u => u.Email == email)
                .Select(u => new RegisterUserResponse
                {
                    CBSUserId = u.Id,
                    TaxEntityVM = new TaxEntityViewModel
                    {
                        Id = u.TaxEntity.Id,
                        Email = u.TaxEntity.Email,
                        Recipient = u.TaxEntity.Recipient,
                        PhoneNumber = u.TaxEntity.PhoneNumber,
                    },
                    CBSUser = new CBSUserVM
                    {
                        Id= u.Id,
                        Email = u.Email,
                        Name = u.Name,
                        PhoneNumber = u.PhoneNumber,
                        TaxEntity = new TaxEntityViewModel { Id = u.TaxEntity.Id }
                    }
                }).ToList().SingleOrDefault();
        }


        /// <summary>
        /// Get the register user model for this cbs user phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public RegisterUserResponse GetRegisterCBSUserResponseWithPhoneNumber(string phoneNumber)
        {
            return _transactionManager.GetSession().Query<CBSUser>()
                .Where(u => u.PhoneNumber == phoneNumber)
                .Select(u => new RegisterUserResponse
                {
                    CBSUser = new CBSUserVM
                    {
                        Id = u.Id,
                        Email = u.Email,
                        Name = u.Name,
                        PhoneNumber = u.PhoneNumber,
                        TaxEntity = new TaxEntityViewModel { Id = u.TaxEntity.Id }
                    }
                }).SingleOrDefault();
        }


        /// <summary>
        /// Updates verified state for cbs user with specified id
        /// </summary>
        /// <param name="cbsUserId"></param>
        /// <param name="isVerified"></param>
        public void UpdateCBSUserVerifiedState(long cbsUserId, bool isVerified)
        {
            try
            {
                string tableName = "Parkway_CBS_Core_" + typeof(CBSUser).Name;

                var queryText = $"UPDATE {tableName} SET {nameof(CBSUser.Verified)} = :verified, {nameof(CBSUser.UpdatedAtUtc)} = :updateDate WHERE {nameof(CBSUser.Id)} = :cbsUserId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("verified", isVerified);
                query.SetParameter("cbsUserId", cbsUserId);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception updating verified state for cbs user with id {0}, Exception message {1}", cbsUserId, exception.Message));
                RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// Get cbs user with specified id
        /// </summary>
        /// <param name="cbsUserId"></param>
        /// <returns></returns>
        public CBSUserVM GetCBSUserWithId(long cbsUserId)
        {
            try
            {
                return _transactionManager.GetSession().Query<CBSUser>().Where(x => x.Id == cbsUserId).Select(x => new CBSUserVM { Id = x.Id, Name = x.Name, PhoneNumber = x.PhoneNumber, Email = x.Email }).SingleOrDefault();
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets tax entity id for cbs user with specified user part record id
        /// </summary>
        /// <param name="userPartRecordId"></param>
        /// <returns></returns>
        public long GetTaxEntityIdForAdminCBSUserWithUserPartRecord(int userPartRecordId)
        {
            try
            {
                return _transactionManager.GetSession().Query<CBSUser>().Where(x => x.UserPartRecord.Id == userPartRecordId && x.IsAdministrator).Select(x => x.TaxEntity.Id).SingleOrDefault();
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Toggles is active value for cbs user with specified user part record id
        /// </summary>
        /// <param name="userPartRecordId"></param>
        /// <param name="isActive"></param>
        public void ToggleIsActiveForCBSUserWithUserId(int userPartRecordId, bool isActive)
        {
            try
            {
                string tableName = "Parkway_CBS_Core_" + typeof(CBSUser).Name;

                var queryText = $"UPDATE {tableName} SET {nameof(CBSUser.IsActive)} = :isActive, {nameof(CBSUser.UpdatedAtUtc)} = :updateDate WHERE {nameof(CBSUser.UserPartRecord)}_Id = :userPartRecordId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("isActive", isActive);
                query.SetParameter("userPartRecordId", userPartRecordId);

                query.ExecuteUpdate();
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets admin cbs user with specified payer id
        /// </summary>
        /// <param name="payerId"></param>
        /// <returns></returns>
        public CBSUserVM GetAdminCBSUserWithPayerId(string payerId)
        {
            try
            {
                return _transactionManager.GetSession().Query<CBSUser>().Where(x => x.TaxEntity.PayerId == payerId && x.IsAdministrator).Select(x => new CBSUserVM { Id = x.Id, TaxEntity = new TaxEntityViewModel { Id = x.TaxEntity.Id } }).SingleOrDefault();
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets admin cbs user with specified tax entity id
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        public CBSUserVM GetAdminCBSUserWithTaxEntityId(long taxEntityId)
        {
            try
            {
                return _transactionManager.GetSession().Query<CBSUser>().Where(x => x.TaxEntity.Id == taxEntityId && x.IsAdministrator).Select(x => new CBSUserVM { Id = x.Id, TaxEntity = new TaxEntityViewModel { Id = x.TaxEntity.Id, CategoryId = x.TaxEntity.TaxEntityCategory.Id } }).SingleOrDefault();
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets user part record id for admin cbs user with specified tax entity id
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        public int GetUserPartRecordIdForAdminCBSUserWithTaxEntityId(long taxEntityId)
        {
            try
            {
                return _transactionManager.GetSession().Query<CBSUser>().Where(x => x.TaxEntity.Id == taxEntityId && x.IsAdministrator).Select(x => x.UserPartRecord.Id).SingleOrDefault();
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

    }
}