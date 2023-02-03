using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Client.Web.Controllers.Handlers.Contracts
{
    public interface ICommonHandler : IDependency
    {
        /// <summary>
        /// <summary>
        /// Get the details of the logged in user
        /// <para>Would return an initialized object at all time, so check if entity is null to validate if there is a user for this request</para>
        /// </summary>
        /// <returns>UserDetailsModel</returns>
        /// </summary>
        /// <returns></returns>
        UserDetailsModel GetLoggedInUserDetails();

        /// <summary>
        /// Get header object
        /// </summary>
        /// <returns></returns>
        HeaderObj GetHeaderObj();

        /// <summary>
        /// Fill header obj
        /// <para>will always return an instance of the return object</para>
        /// </summary>
        /// <param name="userDetails"></param>
        /// <returns>HeaderObj</returns>
        HeaderObj HeaderFiller(UserDetailsModel userDetails);


    }
}
