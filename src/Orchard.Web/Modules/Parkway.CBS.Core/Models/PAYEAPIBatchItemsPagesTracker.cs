namespace Parkway.CBS.Core.Models
{
    public class PAYEAPIBatchItemsPagesTracker : CBSBaseModel
    {
        public virtual long Id { get; set; }

        public virtual PAYEAPIRequest PAYEAPIRequest { get; set; }

        /// <summary>
        /// Page number for the request
        /// </summary>
        public virtual int PageNumber { get; set; }
    }
}