$(document).ready(function () {
    var token = $("input[name=__RequestVerificationToken]").val();
    $('[data-toggle="tooltip"]').tooltip();
    var percentage = "";
    var stop = false;
    var PageSize = 0;
    var Page = 0;
    var paymentsArray = new Array();
    var canProgress = false;
    var options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
    Page = 1;
    PageSize = parseInt($("#pageSize").text(), 10);
    canProgress = true;
    getFirstPageData();
    manageCursors();


    $("#confirmForm").on('submit', function (e) {
        if (!canProgress) { e.preventDefault(); }
        $("#proceedbtn").prop("disabled", true);
    });


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


    function getFirstPageData() {
        Page = 1;
        paymentsArray[Page] = paymentPage1;
    }



    $('[name="cursor"]').click(function (event) {
        event.preventDefault();
        var cursor = event.target.id;
        $("#tbody").empty();
        $('.payeeloader').show();

        if (cursor == "moveright") {
            if (Page < PageSize) {
                Page += 1;

                //do ajax
                if (paymentsArray[Page] == undefined) {
                    var url = 'get-next-payment-list-page';
                    var requestData = { "operatorId": operatorId, "page": Page, "__RequestVerificationToken": token, "datefilter": datefilter };
                    $.get(url, requestData, function (data) {
                        if (!data.Error) {
                            buildTableFromProcess(data.ResponseObject.ReportRecords, false);
                            $('.payeeloader').hide();
                        } else {
                            $("#proceedbtn").attr("disabled", false);
                            $("#level").html("Cannot fetch more data. Please try again leter.");
                            $('.payeeloader').hide();
                        }
                    });
                } else {
                    buildTableFromProcess(paymentsArray[Page], true);
                }
            }
        } else if (cursor == "moveleft") {
            $('.payeeloader').hide();
            Page -= 1;
            if (paymentsArray[Page] != undefined) {
                buildTableFromProcess(paymentsArray[Page], true);
            } else {
                Page += 1;
            }
        }
        manageCursors();
    });


    function buildTableFromProcess(paymentList, fromArr) {
        $('[data-toggle="tooltip"]').tooltip();
        $('.payeeloader').hide();
        $("#level").html("100%");

        $("#level").html("");

        var tbody = $("#tbody");
        if (paymentList.length >= 1) {
            //set page number
            $("#page").html(Page);
            //check if this object is a new object, if it is new add to the existing payee array
            if (!fromArr) {
                paymentsArray[Page] = paymentList;
            }

            for (var val in paymentList) {
                var tr = $('<tr />').appendTo(tbody);
                var td0 = $('<td>' + paymentList[val].PaymentDateStringVal + '</td>').appendTo(tr);

                var td1 = $('<td>' + '<a href="view-receipt/' + paymentList[val].ReceiptNumber + '" name = "viewReceipt"> ' + paymentList[val].ReceiptNumber + '</a>' + '</td>').appendTo(tr);

                var td2 = $('<td>' + paymentList[val].TaxPayerName + '</td>').appendTo(tr);

                var td3 = $('<td>' + paymentList[val].TaxPayerTIN + '</td>').appendTo(tr);

                var td4 = $('<td>(' + paymentList[val].RevenueHeadName + ')</td>').appendTo(tr);

                var td5 = $('<td>' + paymentList[val].PaymentRef + '</td>').appendTo(tr);

                var td6 = $('<td>' + paymentList[val].PaymentMethod + '</td>').appendTo(tr);

                var td7 = $('<td>' + '<a href="view-invoice/' + paymentList[val].InvoiceNumber + '" name = "viewReceipt"> ' + paymentList[val].InvoiceNumber + '</a>' + '</td>').appendTo(tr);

                var td8 = $('<td>' + paymentList[val].Bank + '</td>').appendTo(tr);

                var td9 = $('<td>' + Math.floor(paymentList[val].Amount).toFixed(2) + '</td>').appendTo(tr);
            }
            $('[data-toggle="tooltip"]').tooltip();
            //manage cursor, manages the pagination
            manageCursors();
        }
    }


    $('a[name="viewInvoice"]').click(function (e) {
        e.preventDefault();
        window.open(this.href, "cbsinvoice", "width=800,height=800,scrollbars=yes");
    });

    //$('a[name="viewReceipt"]').click(function (e) {
    //    e.preventDefault();
    //    window.open(this.href, "CBS Receipt", "width=800,height=800,scrollbars=yes");
    //});

});