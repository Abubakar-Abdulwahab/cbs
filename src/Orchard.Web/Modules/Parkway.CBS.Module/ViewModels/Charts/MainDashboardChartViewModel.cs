using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels.Charts
{
    public class MainDashboardChartViewModel
    {
        public BarChartViewModel BarChart { get; set; }
        public List<DoughNutChartViewModel> DoughNutCharts { get; set; }
        public ExpectationLineChart LineChart { get; set; }
        public List<PieChartsViewModel> PieChart { get; set; }
    }    
}