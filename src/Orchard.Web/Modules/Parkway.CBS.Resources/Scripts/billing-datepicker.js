$(document).ready(function(){
    var nowTemp = new Date();
    var now = new Date(nowTemp.getFullYear(),nowTemp.getMonth(),nowTemp.getDate(),0,0,0,0);
    var checkin = $("#datepicker3").datepicker({
        onRender: function (date) {
            return date.valueOf() < now.valueOf() ? 'disabled' : '';
        }
    });

    $("#datepickernextday").datepicker({
        onRender: function (date) {
            return date.valueOf() < now.valueOf() + 1 ? 'disabled' : '';
        }
    });

    $("#datepicker5").datepicker({
        onRender: function (date) {
            return date.valueOf() > now.valueOf() ? 'disabled' : '';
        }
    });

    $("#datepicker4").datepicker({
        onRender: function (date) {
            var endNow = new Date(nowTemp.getFullYear(), nowTemp.getMonth(), nowTemp.getDate() + 1, 0, 0, 0, 0);
            return date.valueOf() < endNow.valueOf() ? 'disabled' : '';
        }
    });

    $('#pickyDate').datepicker({
		format:"dd/mm/yyyy"
	});
	$('#pickyDate2').datepicker({
		format:"dd/mm/yyyy"
	});
    $('#datepicker3').datepicker({
        format:"dd/mm/yyyy"
    });
    $('#datepickernextday').datepicker({
        format: "dd/mm/yyyy"
    });
    $('#datepicker5').datepicker({
        format: "dd/mm/yyyy"
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
    });
})