namespace Parkway.CBS.Module.ViewModels.Charts
{
    public class BarChartViewModel
    {
        public string[][] Labels { get; set; }
        public string[] BackGroundColors { get; set; }
        public string[] BorderColors { get; set; }
        public decimal[] ExpectedAmountData { get; set; }
        public decimal[] PaidAmountData { get; set; }
        public decimal[] PendingAmountData { get; set; }
    }
}