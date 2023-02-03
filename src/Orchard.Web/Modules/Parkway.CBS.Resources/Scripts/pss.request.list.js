$(document).ready(function () {
    var token = $("input[name=__RequestVerificationToken]").val();
    var PageSize = 0;
    var Page = 0;
    var requestsArray = new Array();
    Page = 1;
    PageSize = parseInt($("#pageSize").text(), 10);
    getFirstPageData();
    manageCursors();

    if (hasError) {
        $("#flash").show();
        $("#errorMsg").html(errorMessage);
    }

    $("form").submit(function () {
        var inputValue = $('#requestStatus').val();
        var requestStatusValueIdentifier = (inputValue == null || inputValue == undefined || inputValue == 0) ? reqStatus : $('#statuses option[value="' + inputValue + '"]').attr('data-value');
        $('#requestStatusIdentifier').val(requestStatusValueIdentifier);
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
        requestsArray[Page] = paymentPage1;
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
                if (requestsArray[Page] == undefined) {
                    var url = 'get-next-request-list-page';
                    var requestData = { "operatorId": operatorId, "page": Page, "__RequestVerificationToken": token, "status": reqStatus, "startDateFilter" : startDateFilter, "endDateFilter" : endDateFilter };
                    $.get(url, requestData, function (data) {
                        if (!data.Error) {
                            buildTableFromProcess(data.ResponseObject.Requests, false);
                            $('.payeeloader').hide();
                            $("#flash").hide();
                            $("#errorMsg").html("");
                        } else {
                            $('.payeeloader').hide();
                            $("#flash").show();
                            $("#errorMsg").html(data.ResponseObject.Text);
                        }
                    });
                } else {
                    buildTableFromProcess(requestsArray[Page], true);
                }
            }
        } else if (cursor == "moveleft") {
            $('.payeeloader').hide();
            Page -= 1;
            if (requestsArray[Page] != undefined) {
                buildTableFromProcess(requestsArray[Page], true);
            } else {
                Page += 1;
            }
        }
        manageCursors();
    });


    function buildTableFromProcess(requestList, fromArr) {
        //$('[data-toggle="tooltip"]').tooltip();
        $('.payeeloader').hide();
        $("#level").html("100%");

        $("#level").html("");

        var tbody = $("#tbody");
        if (requestList.length >= 1) {
            //set page number
            $("#page").html(Page);
            //check if this object is a new object, if it is new add to the existing payee array
            if (!fromArr) {
                requestsArray[Page] = requestList;
            }

            for (var val in requestList) {
                var tr = $('<tr />').appendTo(tbody);
                var td0 = $('<td>' + requestList[val].RequestDateString + '</td>').appendTo(tr);

                var td2 = $('<td>' + requestList[val].CustomerName + '</td>').appendTo(tr);

                var td3 = $('<td>' + requestList[val].FileRefNumber + '</td>').appendTo(tr);

                var td4 = $('<td>' + getRequestStatus(requestList[val].Status) + '</td>').appendTo(tr);

                var td5 = $('<td>' + requestList[val].LastActionDateString + '</td>').appendTo(tr);

                var td6 = $('<td>' + requestList[val].ServiceName + '</td>').appendTo(tr);

                var td7 = $('<td>' + '<a href="' + getDetailsLink(requestList[val].FileRefNumber) + '" name = "viewReceipt"> View Details </a>' + '</td>').appendTo(tr);

                var td8 = $('<td>' + '<a href="' + getInvoiceLink(requestList[val].FileRefNumber) + '" name = "ViewInvoices" target="_blank"> View Invoices </a>' + '</td>').appendTo(tr);

            }
            $(window).scrollTop(0);
            //manage cursor, manages the pagination
            manageCursors();
        }
    }

    $('a[name="viewInvoice"]').click(function (e) {
        e.preventDefault();
        window.open(this.href, "cbsinvoice", "width=800,height=800,scrollbars=yes");
    });

    function getRequestStatus(status) {
        switch(status){
            case pendingInvoicePayment:
              return "<div class='status-pending'>Pending</div>";
              break;
           case pendingApproval:
                return "<div class='status-pending'>Pending</div>";
              break;
           case approved:
                return "<div class='status-approved'>Approved</div>";
              break;
           case confirmed:
              return "<div class='status-approved'>Confirmed</div>";
              break;
           case rejected:
                return "<div class='status-declined'>Declined</div>";
              break;
           default:
                return "<div class='status-pending'>Pending</div>";
               break; 
        }
    }

    function getDetailsLink(fileRefNumber){
        return "request-details/" + fileRefNumber;
    }

    function getInvoiceLink(fileNumber){
        return "request-invoice/" + fileNumber;
    }

});