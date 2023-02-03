using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Parkway.CBS.Module.API.Controllers.Handlers.Contracts
{
    public interface IAPIMDAHandler : IDependency
    {
        APIResponse CreateMDA(MDAController callback, CreateMDAModel model, HttpFileCollectionWrapper files, dynamic headerParams = null);


        APIResponse EditMDA(MDAController callback, EditMDAModel model, HttpFileCollectionWrapper httpFileCollectionWrapper, dynamic headerParams = null);
    }
}
