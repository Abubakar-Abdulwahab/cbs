using Orchard;

namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICoreExtractDetails : IDependency
    {
        /// <summary>
        /// Check if <paramref name="affivdavitNumber"/> does not exist with the user 
        /// with the <paramref name="taxEntityId"/>
        /// </summary>
        /// <param name="affivdavitNumber"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        bool CheckIfExistingAffidavitNumber(string affivdavitNumber, long taxEntityId);
    }
}
