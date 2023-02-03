using System;
using System.Collections.Generic;
using System.Linq;
using Orchard;
using Orchard.Localization;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Module.ViewModels.Charts;
using System.Globalization;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Module.Controllers.Handlers
{
    public abstract class MDARevenueHeadHandler : BaseHandler
    {
        private readonly IOrchardServices _orchardServices;
        public Localizer T { get; set; }
        public readonly IAdminSettingManager<ExpertSystemSettings> _settingsRepository;

        public MDARevenueHeadHandler(IOrchardServices orchardServices, IAdminSettingManager<ExpertSystemSettings> settingsRepository) : base(orchardServices, settingsRepository)
        {
            _orchardServices = orchardServices;
            T = NullLocalizer.Instance;
            _settingsRepository = settingsRepository;
        }


        protected ExpectationLineChart BuildExpectationLineChart<M>(IList<StatsPerMonth> statsForTheYearPerQuarter) where M : MDARevenueHead
        {
            if (statsForTheYearPerQuarter == null || statsForTheYearPerQuarter.Count() <= 0) { return new ExpectationLineChart(); }
            string[][] labels = { };
            List<string[]> Labels = new List<string[]>(3);
            foreach (var stat in statsForTheYearPerQuarter)
            {
                Labels.Add(new[] { stat.DueDate.ToString("MMMM", CultureInfo.InvariantCulture), stat.DueDate.Date.Year.ToString() });
            }

            labels = Labels.ToArray();
            decimal[] expectedAmountData = statsForTheYearPerQuarter.Select(x => x.AmountExpected).ToArray();
            return new ExpectationLineChart()
            {
                BackGroundColors = "rgb(255, 99, 132)",
                BorderColors = "rgba(255,99,132,0.5)",
                ExpectedAmountData = expectedAmountData,
                Labels = labels
            };
        }

        protected BarChartViewModel BuildMultiBarChartView<M>(IList<StatsPerMonth> statsForTheYearPerQuarter) where M : MDARevenueHead
        {
            if (statsForTheYearPerQuarter == null || statsForTheYearPerQuarter.Count() <= 0) { return new BarChartViewModel(); }
            //list of quarter indexes
            int[,] quarterArray = { { 1, 1, 1, 4, 4, 4, 7, 7, 7, 9, 9, 9 } };
            int[,] quarterIndexArray = { { 0, 0, 0, 3, 3, 3, 6, 6, 6, 9, 9, 9 } };
            //get quarter to start from
            int quarterStartIndex = quarterIndexArray[0, (DateTime.Now.Month - 1)];
            int quarterIndex = quarterArray[0, (DateTime.Now.Month - 1)];
            var statsThisQuarter = statsForTheYearPerQuarter.Where(s => (s.DueDate.Month >= quarterIndex) && (s.DueDate.Month <= (quarterIndex + 3))).Select(s => s);

            string[][] labels = { };
            List<string[]> Labels = new List<string[]>(3);
            foreach (var stat in statsThisQuarter)
            {
                Labels.Add(new[] { stat.DueDate.ToString("MMMM", CultureInfo.InvariantCulture), stat.DueDate.Date.Year.ToString() });
            }

            labels = Labels.ToArray();

            string[] backGroundColors = { "rgba(255, 99, 132, 5)" };
            string[] borderColours = { "rgba(255,99,132,0.5)", "rgba(54, 162, 235, 5)", "rgba(255, 206, 86, 5)" };

            decimal[] expectedAmountData = statsThisQuarter.Select(x => x.AmountExpected).ToArray();
            decimal[] paidAmountData = statsThisQuarter.Select(x => x.AmountPaid).ToArray();
            decimal[] pendingAmountData = statsThisQuarter.Select(x => (x.AmountExpected - x.AmountPaid)).ToArray();

            return new BarChartViewModel()
            {
                Labels = labels,
                BackGroundColors = backGroundColors,
                BorderColors = borderColours,
                ExpectedAmountData = expectedAmountData,
                PaidAmountData = paidAmountData,
                PendingAmountData = pendingAmountData,
            };
        }

        protected List<DoughNutChartViewModel> BuildDoughNuts<M>(IList<StatsPerMonth> statsForTheYearPerQuarter) where M : MDARevenueHead
        {
            List<DoughNutChartViewModel> quarters = new List<DoughNutChartViewModel>(4);

            #region Q1
            DoughNutChartViewModel quarter1 = new DoughNutChartViewModel();
            string[] Q1labels = { "Amount Expected", "Amount Received", "Amount Pending" };
            string[] Q1backGroundColors = { "rgba(255,99,132, 1)", "rgba(54, 162, 235, 1)", "rgba(255, 206, 86, 1)" };
            var statsForQ1 = statsForTheYearPerQuarter.Where(x => (x.DueDate.Month >= 1) && (x.DueDate.Month <= 3));

            if (statsForQ1 == null || statsForQ1.Count() <= 0) { return quarters; }

            decimal[] Q1data = { statsForQ1.Select(x => x.AmountExpected).Sum(x => x)
                              , statsForQ1.Select(x => x.AmountPaid).Sum(x => x)
                              , statsForQ1.Select(x => (x.AmountExpected - x.AmountPaid)).Sum(x => x) };

            quarter1.Labels = Q1labels;
            quarter1.BackGroundColors = Q1backGroundColors;
            quarter1.Data = Q1data;
            quarter1.Description = "Revenue Summary for Quarter 1";

            quarters.Add(quarter1);
            #endregion

            #region Q2
            DoughNutChartViewModel quarter2 = new DoughNutChartViewModel();
            string[] Q2labels = { "Amount Expected", "Amount Received", "Amount Pending" };
            string[] Q2backGroundColors = { "rgba(255, 99, 132, 1)", "rgba(54, 162, 235, 1)", "rgba(255, 206, 86, 1)" };
            var statsForQ2 = statsForTheYearPerQuarter.Where(x => (x.DueDate.Month >= 4) && (x.DueDate.Month <= 6));

            if (statsForQ2 == null || statsForQ2.Count() <= 0) { return quarters; }

            decimal[] Q2data = { statsForQ2.Select(x => x.AmountExpected).Sum(x => x)
                              , statsForQ2.Select(x => x.AmountPaid).Sum(x => x)
                              , statsForQ2.Select(x => (x.AmountExpected - x.AmountPaid)).Sum(x => x) };

            quarter2.Labels = Q2labels;
            quarter2.BackGroundColors = Q2backGroundColors;
            quarter2.Data = Q2data;
            quarter2.Description = "Revenue Summary for Quarter 2";

            quarters.Add(quarter2);
            #endregion

            #region Q3
            DoughNutChartViewModel quarter3 = new DoughNutChartViewModel();
            string[] Q3labels = { "Amount Expected", "Amount Received", "Amount Pending" };
            string[] Q3backGroundColors = { "rgba(255, 99, 132, 1)", "rgba(54, 162, 235, 1)", "rgba(255, 206, 86, 1)" };
            var statsForQ3 = statsForTheYearPerQuarter.Where(x => (x.DueDate.Month >= 7) && (x.DueDate.Month <= 9));

            if (statsForQ3 == null || statsForQ3.Count() <= 0) { return quarters; }

            decimal[] Q3data = { statsForQ3.Select(x => x.AmountExpected).Sum(x => x)
                              , statsForQ3.Select(x => x.AmountPaid).Sum(x => x)
                              , statsForQ3.Select(x => (x.AmountExpected - x.AmountPaid)).Sum(x => x) };

            quarter3.Labels = Q3labels;
            quarter3.BackGroundColors = Q3backGroundColors;
            quarter3.Data = Q3data;
            quarter3.Description = "Revenue Summary for Quarter 3";

            quarters.Add(quarter3);
            #endregion

            #region Q4
            DoughNutChartViewModel quarter4 = new DoughNutChartViewModel();
            string[] Q4labels = { "Amount Expected", "Amount Received", "Amount Pending" };
            string[] Q4backGroundColors = { "rgba(255, 99, 132, 1)", "rgba(54, 162, 235, 1)", "rgba(255, 206, 86, 1)" };
            var statsForQ4 = statsForTheYearPerQuarter.Where(x => (x.DueDate.Month >= 10) && (x.DueDate.Month <= 12));

            if (statsForQ4 == null || statsForQ4.Count() <= 0) { return quarters; }

            decimal[] Q4data = { statsForQ4.Select(x => x.AmountExpected).Sum(x => x)
                              , statsForQ4.Select(x => x.AmountPaid).Sum(x => x)
                              , statsForQ4.Select(x => (x.AmountExpected - x.AmountPaid)).Sum(x => x) };

            quarter4.Labels = Q4labels;
            quarter4.BackGroundColors = Q4backGroundColors;
            quarter4.Data = Q4data;
            quarter4.Description = "Revenue Summary for Quarter 4";

            quarters.Add(quarter4);
            #endregion

            return quarters;
        }

        protected List<PieChartsViewModel> BuildPieCharts(IList<StatsPerMonth> statsForPieChart, MDA mda)
        {
            List<PieChartsViewModel> pies = new List<PieChartsViewModel>();

            #region expectation pie chart
            decimal[] expectationData = statsForPieChart.Select(st => st.AmountExpected).ToArray<decimal>();

            string[] xbackGroundColors = {
               "rgba(255,99,132,0.5)", "rgba(54, 162, 235, 0.5)", "rgba(255, 206, 86, 0.5)",
                "rgba(245,245,220,0.5)", "rgba(255,255,224, 0.5)", "rgba(255, 206, 86, 0.5)",
                "rgba(255,228,196,0.5)", "rgba(0,255,0, 0.5)", "rgba(255,0,255, 0.5)",
                "rgba(0,0,255,0.5)", "rgba(255,0,255, 0.5)", "rgba(128,0,0, 0.5)",
                "rgba(165,42,42,0.5)", "rgba(0,255,0, 0.5)", "rgba(255, 206, 86, 0.5)",
                "rgba(102,205,170,0.5)", "rgba(60,179,113, 0.5)", "rgba(123,104,238, 0.5)",
                "rgba(100,149,237,0.5)", "rgba(25,25,112, 0.5)", "rgba(253,245,230, 0.5)",
                "rgba(189,183,107,0.5)", "rgba(0,100,0, 0.5)", "rgba(169,169,169, 0.5)",
                "rgba(85,107,47,0.5)", "rgba(107,142,45, 0.5)", "rgba(152,251,152, 0.5)",
                "rgba(40,79,79,0.5)", "rgba(219,112,147, 0.5)", "rgba(255,250,240, 0.5)",
                };
            string[] xlabels = statsForPieChart.Select(st => st.RevenueHead.Name).ToArray<string>();

            var xsummation = expectationData.Sum();
            if (xsummation == 0) { expectationData = new decimal[] { 100 }; xlabels = new string[] { "N/A" }; xbackGroundColors = new string[] { "rgba(192,192,192,0.5)" }; }

            PieChartsViewModel expectationPieChart = new PieChartsViewModel() { BackGroundColors = xbackGroundColors, Data = expectationData, Description = "Amount Expected", Labels = xlabels };
            pies.Add(expectationPieChart);
            #endregion

            #region amount paid pie chart
            decimal[] amountPaidData = statsForPieChart.Select(st => st.AmountPaid).ToArray();
            string[] pbackGroundColors = {
                "rgba(255,99,132,0.5)", "rgba(54, 162, 235, 0.5)", "rgba(255, 206, 86, 0.5)",
                "rgba(245,245,220,0.5)", "rgba(255,255,224, 0.5)", "rgba(255, 206, 86, 0.5)",
                "rgba(255,228,196,0.5)", "rgba(0,255,0, 0.5)", "rgba(255,0,255, 0.5)",
                "rgba(0,0,255,0.5)", "rgba(255,0,255, 0.5)", "rgba(128,0,0, 0.5)",
                "rgba(165,42,42,0.5)", "rgba(0,255,0, 0.5)", "rgba(255, 206, 86, 0.5)",
                "rgba(102,205,170,0.5)", "rgba(60,179,113, 0.5)", "rgba(123,104,238, 0.5)",
                "rgba(100,149,237,0.5)", "rgba(25,25,112, 0.5)", "rgba(253,245,230, 0.5)",
                "rgba(189,183,107,0.5)", "rgba(0,100,0, 0.5)", "rgba(169,169,169, 0.5)",
                "rgba(85,107,47,0.5)", "rgba(107,142,45, 0.5)", "rgba(152,251,152, 0.5)",
                "rgba(40,79,79,0.5)", "rgba(219,112,147, 0.5)", "rgba(255,250,240, 0.5)",
                };
            string[] plabels = statsForPieChart.Select(st => st.RevenueHead.Name).ToArray<string>();
            var psummation = amountPaidData.Sum();
            if(psummation == 0) { amountPaidData = new decimal[] {100 }; plabels = new string[] { "N/A" }; pbackGroundColors = new string[] { "rgba(192,192,192,0.5)" }; }
            //decimal[] amountPaidData = { 9 };
            PieChartsViewModel amountPaidPieChart = new PieChartsViewModel() { Labels = plabels, Description = "Amount Paid", Data = amountPaidData, BackGroundColors = pbackGroundColors };
            pies.Add(amountPaidPieChart);
            #endregion

            #region amount paid pie chart
            decimal[] amountPendingData = statsForPieChart.Select(st => (st.AmountExpected - st.AmountPaid)).ToArray();
            string[] ppbackGroundColors = {
                "rgba(255,99,132,0.5)", "rgba(54, 162, 235, 0.5)", "rgba(255, 206, 86, 0.5)",
                "rgba(245,245,220,0.5)", "rgba(255,255,224, 0.5)", "rgba(255, 206, 86, 0.5)",
                "rgba(255,228,196,0.5)", "rgba(0,255,0, 0.5)", "rgba(255,0,255, 0.5)",
                "rgba(0,0,255,0.5)", "rgba(255,0,255, 0.5)", "rgba(128,0,0, 0.5)",
                "rgba(165,42,42,0.5)", "rgba(0,255,0, 0.5)", "rgba(255, 206, 86, 0.5)",
                "rgba(102,205,170,0.5)", "rgba(60,179,113, 0.5)", "rgba(123,104,238, 0.5)",
                "rgba(100,149,237,0.5)", "rgba(25,25,112, 0.5)", "rgba(253,245,230, 0.5)",
                "rgba(189,183,107,0.5)", "rgba(0,100,0, 0.5)", "rgba(169,169,169, 0.5)",
                "rgba(85,107,47,0.5)", "rgba(107,142,45, 0.5)", "rgba(152,251,152, 0.5)",
                "rgba(40,79,79,0.5)", "rgba(219,112,147, 0.5)", "rgba(255,250,240, 0.5)",
                };
            string[] pplabels = statsForPieChart.Select(st => st.RevenueHead.Name).ToArray<string>();
            var ppsummation = amountPendingData.Sum();
            if (ppsummation == 0) { amountPendingData = new decimal[] { 100 }; pplabels = new string[] { "N/A" }; ppbackGroundColors = new string[] { "rgba(192,192,192,0.5)" }; }
            PieChartsViewModel amountPendingPieChart = new PieChartsViewModel() { BackGroundColors = ppbackGroundColors, Data = amountPendingData, Description = "Amount Pending", Labels = pplabels };
            pies.Add(amountPendingPieChart);
            #endregion
            return pies;
        }

        protected List<PieChartsViewModel> BuildPieCharts(IList<StatsPerMonth> statsForPieChart)
        {
            List<PieChartsViewModel> pies = new List<PieChartsViewModel>();

            #region expectation pie chart
            decimal[] expectationData = statsForPieChart.Select(st => st.AmountExpected).ToArray<decimal>();
            string[] xbackGroundColors = {
                "rgba(255,99,132,0.5)", "rgba(54, 162, 235, 0.5)", "rgba(255, 206, 86, 0.5)",
                "rgba(245,245,220,0.5)", "rgba(255,255,224, 0.5)", "rgba(255, 206, 86, 0.5)",
                "rgba(255,228,196,0.5)", "rgba(0,255,0, 0.5)", "rgba(255,0,255, 0.5)",
                "rgba(0,0,255,0.5)", "rgba(255,0,255, 0.5)", "rgba(128,0,0, 0.5)",
                "rgba(165,42,42,0.5)", "rgba(0,255,0, 0.5)", "rgba(255, 206, 86, 0.5)",
                "rgba(102,205,170,0.5)", "rgba(60,179,113, 0.5)", "rgba(123,104,238, 0.5)",
                "rgba(100,149,237,0.5)", "rgba(25,25,112, 0.5)", "rgba(253,245,230, 0.5)",
                "rgba(189,183,107,0.5)", "rgba(0,100,0, 0.5)", "rgba(169,169,169, 0.5)",
                "rgba(85,107,47,0.5)", "rgba(107,142,45, 0.5)", "rgba(152,251,152, 0.5)",
                "rgba(40,79,79,0.5)", "rgba(219,112,147, 0.5)", "rgba(255,250,240, 0.5)",
                };
            string[] xlabels = statsForPieChart.Select(st => st.Mda.Name).ToArray<string>();

            var xsummation = expectationData.Sum();
            if (xsummation == 0) { expectationData = new decimal[] { 100 }; xlabels = new string[] { "N/A" }; xbackGroundColors = new string[] { "rgba(192,192,192,0.5)" }; }

            PieChartsViewModel expectationPieChart = new PieChartsViewModel() { BackGroundColors = xbackGroundColors, Data = expectationData, Description = "Amount Expected", Labels = xlabels };
            pies.Add(expectationPieChart);
            #endregion

            #region amount paid pie chart
            decimal[] amountPaidData = statsForPieChart.Select(st => st.AmountPaid).ToArray();

            string[] pbackGroundColors = {
                "rgba(255,99,132,0.5)", "rgba(54, 162, 235, 0.5)", "rgba(255, 206, 86, 0.5)",
                "rgba(245,245,220,0.5)", "rgba(255,255,224, 0.5)", "rgba(255, 206, 86, 0.5)",
                "rgba(255,228,196,0.5)", "rgba(0,255,0, 0.5)", "rgba(255,0,255, 0.5)",
                "rgba(0,0,255,0.5)", "rgba(255,0,255, 0.5)", "rgba(128,0,0, 0.5)",
                "rgba(165,42,42,0.5)", "rgba(0,255,0, 0.5)", "rgba(255, 206, 86, 0.5)",
                "rgba(102,205,170,0.5)", "rgba(60,179,113, 0.5)", "rgba(123,104,238, 0.5)",
                "rgba(100,149,237,0.5)", "rgba(25,25,112, 0.5)", "rgba(253,245,230, 0.5)",
                "rgba(189,183,107,0.5)", "rgba(0,100,0, 0.5)", "rgba(169,169,169, 0.5)",
                "rgba(85,107,47,0.5)", "rgba(107,142,45, 0.5)", "rgba(152,251,152, 0.5)",
                "rgba(40,79,79,0.5)", "rgba(219,112,147, 0.5)", "rgba(255,250,240, 0.5)",
                };
            string[] plabels = statsForPieChart.Select(st => st.Mda.Name).ToArray<string>();

            var psummation = amountPaidData.Sum();
            if (psummation == 0) { amountPaidData = new decimal[] { 100 }; plabels = new string[] { "N/A" }; pbackGroundColors = new string[] { "rgba(192,192,192,0.5)" }; }

            PieChartsViewModel amountPaidPieChart = new PieChartsViewModel() { Labels = plabels, Description = "Amount Paid", Data = amountPaidData, BackGroundColors = pbackGroundColors };
            pies.Add(amountPaidPieChart);
            #endregion

            #region amount paid pie chart
            decimal[] amountPendingData = statsForPieChart.Select(st => (st.AmountExpected - st.AmountPaid)).ToArray();

            string[] ppbackGroundColors = {
                "rgba(255,99,132,0.5)", "rgba(54, 162, 235, 0.5)", "rgba(255, 206, 86, 0.5)",
                "rgba(245,245,220,0.5)", "rgba(255,255,224, 0.5)", "rgba(255, 206, 86, 0.5)",
                "rgba(255,228,196,0.5)", "rgba(0,255,0, 0.5)", "rgba(255,0,255, 0.5)",
                "rgba(0,0,255,0.5)", "rgba(255,0,255, 0.5)", "rgba(128,0,0, 0.5)",
                "rgba(165,42,42,0.5)", "rgba(0,255,0, 0.5)", "rgba(255, 206, 86, 0.5)",
                "rgba(102,205,170,0.5)", "rgba(60,179,113, 0.5)", "rgba(123,104,238, 0.5)",
                "rgba(100,149,237,0.5)", "rgba(25,25,112, 0.5)", "rgba(253,245,230, 0.5)",
                "rgba(189,183,107,0.5)", "rgba(0,100,0, 0.5)", "rgba(169,169,169, 0.5)",
                "rgba(85,107,47,0.5)", "rgba(107,142,45, 0.5)", "rgba(152,251,152, 0.5)",
                "rgba(40,79,79,0.5)", "rgba(219,112,147, 0.5)", "rgba(255,250,240, 0.5)",
                };
            string[] pplabels = statsForPieChart.Select(st => st.Mda.Name).ToArray<string>();

            var ppsummation = expectationData.Sum();
            if (psummation == 0) { amountPendingData = new decimal[] { 100 }; pplabels = new string[] { "N/A" }; ppbackGroundColors = new string[] { "rgba(192,192,192,0.5)" }; }

            PieChartsViewModel amountPendingPieChart = new PieChartsViewModel() { BackGroundColors = ppbackGroundColors, Data = amountPendingData, Description = "Amount Pending", Labels = pplabels };
            pies.Add(amountPendingPieChart);
            #endregion

            return pies;
        }

    }
}