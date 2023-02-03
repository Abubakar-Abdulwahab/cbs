using Orchard;

namespace Parkway.CBS.Police.Core.Seeds.Contracts
{
    public interface IIdentificationTypeSeeds : IDependency
    {
        /// <summary>
        /// Populate identification types.
        /// </summary>
        /// <returns></returns>
        bool PopulateIdentificationTypes();
    }
}
