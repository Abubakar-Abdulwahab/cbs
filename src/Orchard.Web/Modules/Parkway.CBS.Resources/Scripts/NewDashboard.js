$(document).ready(function(){
   $('#pickyDate').datepicker({
		format:"dd/mm/yyyy"
	});
	$('#pickyDate2').datepicker({
		format:"dd/mm/yyyy"
	});
    $("a.pdf").click(function (e) {
            //e.preventDefault();
            //debugger;
            var data = [], height = 0, doc;
            /*doc = new jsPDF('p', 'pt', 'a4', true);
            doc.setFont("times", "normal");
            doc.setFontSize(20);
            data = [];
            data = doc.tableToJson(invoices);
            height = doc.drawTable(data, {
                xstart: 10,
                ystart: 10,
                tablestart: 60,
                marginleft: 10,
                xOffset: 10,
                yOffset: 5
            });*/
            doc = new jsPDF('p', 'pt');
            var data = doc.autoTableHtmlToJson(document.getElementById("invoices"));
            var opts = {
                margin: 5, pageContent: function (data) {
                    doc.text("Borrow History");
                }
            };
            doc.autoTable(data.columns, data.rows, opts);
            doc.save("Invoices.pdf");
            return false; 
    });


        //3rd Chart SetUp
        //$('.pie-chart2').highcharts({
        //    chart: {
        //        plotBackgroundColor: null,
        //        plotBorderWidth: null,
        //        plotShadow: false
        //    },
        //    credits:{enabled:false},
        //    title: {
        //        text: 'Monthly Payment Per MDA'
        //    },
        //    tooltip: {
        //        pointFormat: '{series.name}: <b>{point.percentage}%</b>',
        //        percentageDecimals: 1
        //    },
        //    plotOptions: {
        //        pie: {
        //            allowPointSelect: true,
        //            cursor: 'pointer',
        //            dataLabels: {
        //                enabled: false,
        //            }
        //        }
        //    },
        //    series: [{
        //        type: 'pie',
        //        name: 'Browser share',
        //        data: [
        //            ['Firefox',   45.0],
        //            ['IE',       26.8],
        //            {
        //                name: 'Chrome',
        //                y: 12.8,
        //                sliced: true,
        //                selected: true
        //            },
        //            ['Safari',    8.5],
        //            ['Opera',     6.2],
        //            ['Others',   0.7]
        //        ]
        //    }]
        //});
        //  //2nd Chart SetUp
        //$('.pie-chart').highcharts({
        //    chart: {
        //        plotBackgroundColor: null,
        //        plotBorderWidth: null,
        //        plotShadow: false
        //    },
        //    credits:{enabled:false},
        //    title: {
        //        text: 'Monthly Payment Per MDA'
        //    },
        //    tooltip: {
        //        pointFormat: '{series.name}: <b>{point.percentage}%</b>',
        //        percentageDecimals: 1
        //    },
        //    plotOptions: {
        //        pie: {
        //            allowPointSelect: true,
        //            cursor: 'pointer',
        //            dataLabels: {
        //                enabled: false,
        //            }
        //        }
        //    },
        //    series: [{
        //        type: 'pie',
        //        name: 'Browser share',
        //        data: [
        //            ['Firefox',   45.0],
        //            ['IE',       26.8],
        //            {
        //                name: 'Chrome',
        //                y: 12.8,
        //                sliced: true,
        //                selected: true
        //            },
        //            ['Safari',    8.5],
        //            ['Opera',     6.2],
        //            ['Others',   0.7]
        //        ]
        //    }]
        //});

        //HighCharts Pie Chart SetUp
    //    Highcharts.chart('pie-chart', {
    //    chart: {
    //        plotBackgroundColor: null,
    //        plotBorderWidth: null,
    //        plotShadow: false,
    //        type: 'pie'
    //    },
    //    credits:{enabled:false},
    //    title: {
    //        text: 'Monthly Payment Per MDA'
    //    },
    //    tooltip: {
    //        pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
    //    },
    //    plotOptions: {
    //        pie: {
    //            allowPointSelect: true,
    //            cursor: 'pointer',
    //            dataLabels: {
    //                enabled: false,
    //                format: '<b>{point.name}</b>: {point.percentage:.1f} %',
    //                style: {
    //                    color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
    //                }
    //            }
    //        }
    //    },
    //    series: [{
    //        name: 'MDA\'s',
    //        colorByPoint: true,
    //        data: [{
    //            name: 'Microsoft Internet Explorer',
    //            y: 56.33
    //        }, {
    //            name: 'Chrome',
    //            y: 24.03,
    //            sliced: true,
    //            selected: true
    //        }, {
    //            name: 'Firefox',
    //            y: 10.38
    //        }, {
    //            name: 'Safari',
    //            y: 4.77
    //        }, {
    //            name: 'Opera',
    //            y: 0.91
    //        }, {
    //            name: 'Proprietary or Undetectable',
    //            y: 0.2
    //        }]
    //    }],
    //});
    //Highcharts.chart('container', {
    //chart: {
    //    type: 'column'
    //},
    //credits:{enabled:false},
    //title: {
    //    text: 'Monthly Revenue Expectations and Actual Receipts'
    //},
    //subtitle: {
    //    // text: 'Source: WorldClimate.com'
    //},
    //xAxis: {
    //    categories: [
    //        'Jan',
    //        'Feb',
    //        'Mar',
    //        'Apr',
    //        'May',
    //        'Jun',
    //        'Jul',
    //        'Aug',
    //        'Sep',
    //        'Oct',
    //        'Nov',
    //        'Dec'
    //    ],
    //    crosshair: true
    //},
    //yAxis: {
    //    min: 0,
    //    // useHTML: true,
    //    title: {
    //        text: 'Amount'
    //    }
    //},
    //tooltip: {
    //    headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
    //    pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
    //        '<td style="padding:0"><b> &#8358;{point.y:.1f} </b></td></tr>',
    //    footerFormat: '</table>',
    //    shared: true,
    //    useHTML: true
    //},
    //plotOptions: {
    //    column: {
    //        pointPadding: 0.2,
    //        borderWidth: 0
    //    }
    //},
    //    series: [{
    //        name: 'Revenue Expectations',
    //        data: [20000, 50000, 90000, 100000, 200000, 50000, 80000, 20000, 30000, 60000, 40000, 35000]

    //    }, {
    //        name: 'Actual Receipts',
    //        data: [10000, 40000, 30000, 50000, 120000, 40000, 30000, 15000, 10000, 20000, 5000, 12000]

    //    }]
    //});
    ////Line Charts Set Up
    //Highcharts.chart('linechart', {
    //    chart: {
    //    type: 'line'
    //    },
    //     credits:{enabled:false},
    //   title: {
    //        text: 'Solar Employment Growth by Sector, 2010-2016'
    //    },

    //   subtitle: {
    //        text: 'Source:Parkway Projects'
    //    },

    //   yAxis: {
    //        title: {
    //            text: 'Number of Employees'
    //        }
    //    },
    //    legend: {
    //        layout: 'vertical',
    //        align: 'right',
    //        verticalAlign: 'middle'
    //    },

    //   plotOptions: {
    //        series: {
    //            pointStart: 2010
    //        }
    //    },

    //   series: [{
    //        name: 'Installation',
    //        data: [43934, 52503, 57177, 69658, 97031, 119931, 137133, 154175]
    //    }, {
    //        name: 'Manufacturing',
    //        data: [24916, 24064, 29742, 29851, 32490, 30282, 38121, 40434]
    //    }, {
    //        name: 'Sales & Distribution',
    //        data: [11744, 17722, 16005, 19771, 20185, 24377, 32147, 39387]
    //    }, {
    //        name: 'Project Development',
    //        data: [null, null, 7988, 12169, 15112, 22452, 34400, 34227]
    //    }, {
    //        name: 'Other',
    //        data: [12908, 5948, 8105, 11248, 8989, 11816, 18274, 18111]
    //    }]
    //});



    $('.from').datepicker({
        autoclose:true,
        minViewMode:1,
        format:'mm/yyyy'
    }).on('changeDate',function(selected){
        startDate = new Date(selected.date.valueOf());
        startDate.setDate(startDate.getDate(new Date(selected.date.valueOf())));
    });
    $('.to').datepicker({
        autoclose:true,
        minViewMode:1,
        format:'mm/yyyy'
    }).on('changeDate',function(selected){
        startDate = new Date(selected.date.valueOf());
        startDate.setDate(startDate.getDate(new Date(selected.date.valueOf())));
    });
});