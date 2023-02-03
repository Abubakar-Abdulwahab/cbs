namespace Parkway.CBS.Core.HelperModels
{
    public class AssessmentModel
    {
        public decimal Amount { get; set; }
        public bool IsRecurring { get; set; }
        public bool IsPrepaid { get; set; }
        /// <summary>
        /// set this value to true if the billing type is direct assessment with file upload only
        /// </summary>
        public bool IsDirectAssessment { get; set; }
    }    
}