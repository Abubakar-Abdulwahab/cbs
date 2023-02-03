namespace Parkway.CBS.Core.FormControlsComposition.Implementations
{
    public abstract class BaseFormControlsComposition
    {
        protected Orchard.Data.ITransactionManager _transactionManager;


        public virtual void SetTransactionManagerForDBQueries(Orchard.Data.ITransactionManager transactionManager)
        { _transactionManager = transactionManager; }
    }
}