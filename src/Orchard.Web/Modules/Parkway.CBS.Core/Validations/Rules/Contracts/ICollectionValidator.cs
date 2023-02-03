using Orchard;
using System.Web.Mvc;

namespace Parkway.CBS.Core.Validations.Rules.Contracts
{
    public interface ICollectionValidator : IDependency
    {
        string ValidationName();
        C Validate<C>(dynamic data) where C : Controller;
    }
}
