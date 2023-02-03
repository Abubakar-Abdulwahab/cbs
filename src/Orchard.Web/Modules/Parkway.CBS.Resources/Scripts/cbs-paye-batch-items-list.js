$(document).ready(function () {
    var token = $("input[name=__RequestVerificationToken]").val();
    Page = 1;
    PageSize = parseInt($("#pageSize").text(), 10);
    manageCursors();
    var batchItemsArray = new Array();
    batchItemsArray[Page] = batchItemsPage1;

    $('[name="cursor"]').click(function (event) {
        event.preventDefault();
        var cursor = event.target.id;
        $("#tbody").empty();
        $('.batchItemsLoader').show();

        if (cursor == "moveright") {
            if (Page < PageSize) {
                Page += 1;
                //do ajax
                if (batchItemsArray[Page] == undefined) {
                    var url = '/c/x/paye/batch-items/get-next-batch-items';
                    var requestData = { "batchRef": batchRef, "page": Page, "__RequestVerificationToken": token };
                    $.post(url, requestData, function (data) {
                        if (!data.Error) {
                            buildTableFromProcess(data.ResponseObject.BatchItems, false);
                            $('.batchItemsLoader').hide();
                        } else {
                            $('.batchItemsLoader').hide();
                        }
                    }).fail(function () {

                    }).always(function () {

                    });

                } else {
                    buildTableFromProcess(batchItemsArray[Page], true);
                }
            }
        } else if (cursor == "moveleft") {
            $('.batchItemsLoader').hide();
            Page -= 1;
            if (batchItemsArray[Page] != undefined) {
                buildTableFromProcess(batchItemsArray[Page], true);
            } else {
                Page += 1;
            }
        }
        manageCursors();
    });


    function buildTableFromProcess(batchItems, fromArr) {
        $('[data-toggle="tooltip"]').tooltip();

        $('.batchItemsLoader').hide();

        var tbody = $("#tbody");
        if (batchItems.length >= 1) {
            //set page number
            $("#page").html(Page);

            //check if this object is a new object, if it is new add to the existing payee array
            if (!fromArr) {
                batchItemsArray[Page] = batchItems;
            }

            for (var val in batchItems) {
                var tr = $('<tr />').appendTo(tbody);
                var td0 = $('<td>' + batchItems[val].PayerName + '</td>').appendTo(tr);
                var td1 = $('<td>₦' + batchItems[val].GrossAnnual.toLocaleString() + '</td>').appendTo(tr);

                var td2 = $('<td>₦' + batchItems[val].Exemptions.toLocaleString() + '</td>').appendTo(tr);

                var td3 = $('<td>₦' + batchItems[val].IncomeTaxPerMonth.toLocaleString() + '</td>').appendTo(tr);

                var td6 = $('<td>' + batchItems[val].Month + "|" + batchItems[val].Year + '</td>').appendTo(tr);

                var td7 = $('<td>' + batchItems[val].PayerId + '</td>').appendTo(tr);
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