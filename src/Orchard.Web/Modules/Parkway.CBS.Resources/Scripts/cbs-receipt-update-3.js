$(document).ready(function () {
    var token = $("input[name=__RequestVerificationToken]").val();
    Page = 1;
    PageSize = parseInt($("#pageSize").text(), 10);
    manageCursors();
    var receiptsArray = new Array();
    receiptsArray[Page] = receiptsPage1;

    $('[name="cursor"]').click(function (event) {
        event.preventDefault();
        var cursor = event.target.id;
        $("#tbody").empty();
        $('.receiptloader').show();

        if (cursor == "moveright") {
            if (Page < PageSize) {
                Page += 1;
                //do ajax
                if (receiptsArray[Page] == undefined) {
                    var url = '/c/x/paye/get-next-receipt-page';
                    var requestData = { "token" : $("#token").val(), "page": Page, "__RequestVerificationToken": token };
                    $.post(url, requestData, function (data) {
                        if (!data.Error) {
                            buildTableFromProcess(data.ResponseObject.ReceiptItems, false);
                            $('.receiptloader').hide();
                        } else {
                            $('.receiptloader').hide();
                        }
                    }).fail(function () {
                        
                    }).always(function () {
                        
                    });

                } else {
                    buildTableFromProcess(receiptsArray[Page], true);
                }
            }
        } else if (cursor == "moveleft") {
            $('.receiptloader').hide();
            Page -= 1;
            if (receiptsArray[Page] != undefined) {
                buildTableFromProcess(receiptsArray[Page], true);
            } else {
                Page += 1;
            }
        }
        manageCursors();
    });


    function buildTableFromProcess(receipts, fromArr) {
        $('[data-toggle="tooltip"]').tooltip();

        $('.receiptloader').hide();

        var tbody = $("#tbody");
        if (receipts.length >= 1) {
            //set page number
            $("#page").html(Page);

            //check if this object is a new object, if it is new add to the existing payee array
            if (!fromArr) {
                receiptsArray[Page] = receipts;
            }

            for (var val in receipts) {
                var tr = $('<tr />').appendTo(tbody);
                var td0 = $('<td>' + receipts[val].PayerName + '</td>').appendTo(tr);
                var td1 = $('<td>₦' + receipts[val].AnnualEarnings.toLocaleString() + '</td>').appendTo(tr);

                var td2 = $('<td>₦' + receipts[val].Exemptions.toLocaleString() + '</td>').appendTo(tr);

                var td3 = $('<td>₦' + receipts[val].IncomeTaxValue.toLocaleString() + '</td>').appendTo(tr);

                var td4 = $('<td>' + receipts[val].Month + '</td>').appendTo(tr);

                var td5 = $('<td>' + receipts[val].Year + '</td>').appendTo(tr);

                var td6 = $('<td>' + receipts[val].PayerId + '</td>').appendTo(tr);

                var td7 = $('<td>' + '<a href="' + '/c/paye/batchitem-receipt-details/'+ receipts[val].ReceiptNumber + '" name = "viewReceipt">'+ receipts[val].ReceiptNumber + '</a>' + '</td>').appendTo(tr);
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