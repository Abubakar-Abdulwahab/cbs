$(document).ready(function () {
    // console.log('Hello!');
    // console.log(datepicker);
    $('.pickyDate').datepicker({
        format: "dd/mm/yyyy"
    });

    $('.pickyNoFutureDate').datepicker({
        format: "dd/mm/yyyy",

        onRender: function (date) {
            var nowTemp = new Date();
            var endNow = new Date(nowTemp.getFullYear(), nowTemp.getMonth(), nowTemp.getDate(), 0, 0, 0, 0);
            return date.valueOf() > endNow.valueOf() ? 'disabled' : '';
        }
    });


    $('.pickyNoFutureDatePast14Years').datepicker({
        format: "dd/mm/yyyy",
        viewMode: 'years',
        onRender: function (date) {
            var nowTemp = new Date();
            var endNow = new Date(nowTemp.getFullYear() - 14, nowTemp.getMonth(), nowTemp.getDate(), 0, 0, 0, 0);
            return date.valueOf() > endNow.valueOf() ? 'disabled' : '';
        }
    });

    var nowTemp = new Date();
    var now = new Date(nowTemp.getFullYear(), nowTemp.getMonth(), nowTemp.getDate(), 0, 0, 0, 0);

    var startDate = $('#startDate').datepicker({
        onRender: function (date) {
            return date.valueOf() <= now.valueOf() ? 'disabled' : '';
        }
    }).on('changeDate', function (ev) {
        if (ev.date.valueOf() > endDate.date.valueOf()) {
            var newDate = new Date(ev.date)
            newDate.setDate(newDate.getDate());
            endDate.setValue(newDate);
        }
        startDate.hide();
        $('#endDate')[0].focus();
    }).data('datepicker');

    var endDate = $('#endDate').datepicker({
        onRender: function (date) {
            return date.valueOf() < startDate.date.valueOf() ? 'disabled' : '';
        }
    }).on('changeDate', function (ev) {
        endDate.hide();

        const diffInMs = ev.date.valueOf() - startDate.date.valueOf();
        const diffInDays = diffInMs / (1000 * 60 * 60 * 24) + 1;

        $('#noOfDayLbl').text(`Number of Day(s) : ${diffInDays.toString()}`);

    }).data('datepicker');

    $('#pickyDate2').datepicker({
        format: "dd/mm/yyyy"
    });
});