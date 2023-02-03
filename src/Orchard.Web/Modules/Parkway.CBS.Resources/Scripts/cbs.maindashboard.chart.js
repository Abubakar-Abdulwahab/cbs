$(document).ready(function () {

    buildPieChart();

    function buildPieChart() {
        for (var i = 0; i < pies.length; i++) {
            var container = document.querySelector('.chart-container');

            var div = document.createElement('div');
            div.classList.add('col-md-4');

            var canvas = document.createElement('canvas');
            canvas.classList.add('pie-chart');
            div.appendChild(canvas);
            container.appendChild(div);

            var ctx = canvas.getContext('2d');

            mouldDoughnut(ctx, pies[i]);
        }
    }

    buildLineChart();

    function buildLineChart() {
        if (!lineChart) { return false;}
        if (lineChart.Labels.length > 0) {
            var container = document.querySelector('.chart-container');

            var div = document.createElement('div');
            div.classList.add('chart-container');

            var canvas = document.createElement('canvas');
            canvas.classList.add('bar-chart');
            div.appendChild(canvas);
            container.appendChild(div);

            var ctx = canvas.getContext('2d');

            var chart = new Chart(ctx, {
                // The type of chart we want to create
                type: 'line',

                // The data for our dataset
                data: {
                    labels: lineChart.Labels,

                    datasets: [{
                        label: "Expectation Trend",
                        fill: false,
                        backgroundColor: 'rgb(255, 99, 132)',
                        borderColor: 'rgb(255, 99, 132)',
                        data: lineChart.ExpectedAmountData,
                    }]
                },

                // Configuration options go here
                options: {
                    //showLines: false, // disable for all datasets
                    elements: {
                        line: {
                            tension: 0, // disables bezier curves
                        }
                    },
                    scales: {
                        xAxes: [{
                            gridLines: {
                                drawBorder: true,
                                drawOnChartArea: false,
                            },
                            scaleLabel: {
                                display: true,
                                labelString: 'Months'
                            }
                        }],
                        yAxes: [{
                            gridLines: {
                                drawBorder: true,
                                drawOnChartArea: false,
                            },
                            ticks: {
                                beginAtZero: false,
                                callback: function (decimalValue, index, labels) {
                                    //reference: https://stackoverflow.com/questions/25880767/chart-js-number-format
                                    decimalValue += '';
                                    x = decimalValue.split('.');
                                    x1 = x[0];
                                    x2 = x.length > 1 ? '.' + x[1] : '';
                                    var rgx = /(\d+)(\d{3})/;
                                    while (rgx.test(x1)) {
                                        x1 = x1.replace(rgx, '$1' + ',' + '$2');
                                    }
                                    return x1 + x2;
                                }
                            },
                            scaleLabel: {
                                display: true,
                                labelString: 'Amount \u20A6'
                            }
                        }]
                    }
                }
            });
        }
    }
    //var ctxe = document.getElementById('myChart').getContext('2d');



    buildBarChart();
    function buildBarChart() {
        if (!barChart || barChart.Labels != null) {
            var container = document.querySelector('.chart-container');

            var div = document.createElement('div');
            div.classList.add('chart-container');

            var canvas = document.createElement('canvas');
            canvas.classList.add('bar-chart');
            div.appendChild(canvas);
            container.appendChild(div);

            var ctx = canvas.getContext('2d');

            var myChart = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: barChart.Labels,
                    datasets: [{
                        label: 'Expected Amount',
                        data: barChart.ExpectedAmountData,
                        backgroundColor: 'rgba(255,99,132,1)',
                        borderColor: 'rgba(0,0,0,0)',
                        borderWidth: 1
                    }, {
                        label: 'Amount Paid',
                        data: barChart.PaidAmountData,
                        backgroundColor: 'rgba(54, 162, 235, 1)',
                        borderColor: 'rgba(0,0,0,0)',
                        borderWidth: 1
                    },
                    {
                        label: 'Amount Pending',
                        data: barChart.PendingAmountData,
                        backgroundColor: 'rgba(255, 206, 86, 1)',
                        borderColor: 'rgba(0, 0, 0, 0)',
                        borderWidth: 1
                    }]
                },
                options: {
                    tooltips: {
                        cornerRadius: 0,
                    },
                    scales: {
                        xAxes: [{
                            gridLines: {
                                drawBorder: true,
                                drawOnChartArea: false,
                            },
                            scaleLabel: {
                                display: true,
                                labelString: 'Months'
                            }
                        }],
                        yAxes: [{
                            gridLines: {
                                drawBorder: true,
                                drawOnChartArea: false,
                            },
                            ticks: {
                                beginAtZero: true,
                                callback: function (decimalValue, index, labels) {
                                    //reference: https://stackoverflow.com/questions/25880767/chart-js-number-format
                                    decimalValue += '';
                                    x = decimalValue.split('.');
                                    x1 = x[0];
                                    x2 = x.length > 1 ? '.' + x[1] : '';
                                    var rgx = /(\d+)(\d{3})/;
                                    while (rgx.test(x1)) {
                                        x1 = x1.replace(rgx, '$1' + ',' + '$2');
                                    }
                                    return x1 + x2;
                                }
                            },
                            scaleLabel: {
                                display: true,
                                labelString: 'Amount \u20A6'
                            }
                        }]
                    }
                }
            });
        }
    }


    //DOUGH-NUTS
    bakeDoughnuts();
    function bakeDoughnuts() {
        //bake them
        for (var i = 0; i < dougCharts.length; i++) {
            var container = document.querySelector('.chart-container');

            var div = document.createElement('div');
            div.classList.add('col-md-4');

            var canvas = document.createElement('canvas');
            canvas.classList.add('chart-doughnut');
            div.appendChild(canvas);
            container.appendChild(div);

            var ctx = canvas.getContext('2d');

            mouldDoughnut(ctx, dougCharts[i]);
        }
    }

    function mouldDoughnut(dough, fillings) {

        console.log(fillings);

        var doNut = new Chart(dough, {
            type: 'doughnut',
            data: {
                fillOpacity: 0.1,
                datasets: [{
                    data: fillings.Data,
                    backgroundColor: fillings.BackGroundColors,
                    label: 'Dataset 1'
                }],
                labels: fillings.Labels
            },
            options: {
                tooltips: {
                    cornerRadius: 0,
                },
                responsive: true,
                legend: {
                    position: 'bottom',
                },
                title: {
                    display: true,
                    text: fillings.Description
                },
                animation: {
                    animateScale: true,
                    animateRotate: true
                }
            }
        });
    }
});