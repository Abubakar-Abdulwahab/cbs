$(document).ready(function () {
    var token = $("input[name=__RequestVerificationToken]").val();
    Page = 1;
    PageSize = parseInt($("#pageSize").text(), 10);
    manageCursors();
    var batchRecordsArray = new Array();
    batchRecordsArray[Page] = batchRecordsPage1;

    $('[name="cursor"]').click(function (event) {
        event.preventDefault();
        var cursor = event.target.id;
        $("#tbody").empty();
        $('.scheduleListLoader').show();

        if (cursor == "moveright") {
            if (Page < PageSize) {
                Page += 1;
                //do ajax
                if (batchRecordsArray[Page] == undefined) {
                    var url = '/c/x/paye/get-next-schedules-page';
                    var requestData = { "token": $("#token").val(), "page": Page, "__RequestVerificationToken": token };
                    $.post(url, requestData, function (data) {
                        if (!data.Error) {
                            buildTableFromProcess(data.ResponseObject.BatchRecords, false);
                            $('.scheduleListLoader').hide();
                        } else {
                            $('.scheduleListLoader').hide();
                        }
                    }).fail(function () {

                    }).always(function () {

                    });

                } else {
                    buildTableFromProcess(batchRecordsArray[Page], true);
                }
            }
        } else if (cursor == "moveleft") {
            $('.scheduleListLoader').hide();
            Page -= 1;
            if (batchRecordsArray[Page] != undefined) {
                buildTableFromProcess(batchRecordsArray[Page], true);
            } else {
                Page += 1;
            }
        }
        manageCursors();
    });


    function buildTableFromProcess(batchRecords, fromArr) {
        $('[data-toggle="tooltip"]').tooltip();

        $('.scheduleListLoader').hide();

        var tbody = $("#tbody");
        if (batchRecords.length >= 1) {
            //set page number
            $("#page").html(Page);

            //check if this object is a new object, if it is new add to the existing payee array
            if (!fromArr) {
                batchRecordsArray[Page] = batchRecords;
            }

            for (var val in batchRecords) {
                var tr = $('<tr />').appendTo(tbody);
                var td0 = $('<td>' + batchRecords[val].BatchRef + '</td>').appendTo(tr);

                var td1 = (batchRecords[val].PaymentCompleted) ? $('<td> Completed </td>').appendTo(tr) : $('<td> Not Completed </td>').appendTo(tr);

                var td2 = $('<td>' + '<a style="color: #2F4CB0" href="' + '/c/paye/schedules/utilized-receipts/' + batchRecords[val].BatchRef + '"> View </a>' + '</td>').appendTo(tr);
            }
            $('[data-toggle="tooltip"]').tooltip();
            //manage cursor, manages the pagination
            manageCursors();
        }
    }


    function manageCursors() {
        if (Page < PageSize && PageSize > 1) {
            $("#moveright").removeClass("disablelink");
            $("#moveright").prop("href", "#");
        } else {
            $("#moveright").addClass("disablelink");
            $("#moveright").prop("href", "");
        }

        if (Page > 1) {
            $("#moveleft").removeClass("disablelink");
            $("#moveleft").prop("href", "#");
        } else {
            $("#moveleft").addClass("disablelink");
            $("#moveleft").prop("href", "");
        }
    }
});