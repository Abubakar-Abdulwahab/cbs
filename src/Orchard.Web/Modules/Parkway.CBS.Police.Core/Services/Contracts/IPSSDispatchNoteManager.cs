using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSDispatchNoteManager<PSSDispatchNote> : IDependency, IBaseManager<PSSDispatchNote>
    {
        /// <summary>
        /// Gets dispatch note with specified file ref number
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        DispatchNoteVM GetDispatchNote(string fileRefNumber);
    }
}
