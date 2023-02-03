namespace Parkway.CBS.ClientServices.Settlement.Contract
{
    public interface ISettlement
    {

        /// <summary>
        /// Do settlement work
        /// </summary>
        /// <param name="tenantName"></param>
        void DoSettlement(string tenantName);
             
    }
}
