$(document).ready(function(){
	
    //HighCharts SetUp
    Highcharts.chart('pie-chart', {
    chart: {
        plotBackgroundColor: null,
        plotBorderWidth: null,
        plotShadow: false,
        type: 'pie'
    },
    title: {
        text: 'Monthly Payment Per Revenue Head'
    },
    tooltip: {
        pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
    },
    plotOptions: {
        pie: {
            allowPointSelect: true,
            cursor: 'pointer',
            dataLabels: {
                enabled: true,
                format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                style: {
                    color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                }
            }
        }
    },
    series: [{
        name: 'MDAs',
        colorByPoint: true,
        data: [{
            name: 'Religious Affairs',
            y: 56.33
        }, {
            name: 'Public Affairs',
            y: 24.03,
            sliced: true,
            selected: true
        }, {
            name: 'Agriculture',
            y: 10.38
        }, {
            name: 'Aviation',
            y: 4.77
        }, {
            name: 'Education',
            y: 0.91
        }, {
            name: 'Environment and Natural Resources',
            y: 0.2
        }]
    }]
});
})