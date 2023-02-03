$(document).ready(function () {
    var token = $("input[name=__RequestVerificationToken]").val();
    $('[data-toggle="tooltip"]').tooltip();
    var completionStatus = "";
    var stop = false;
    var PageSize = 0;
    var Page = 0;
    var enumerationLineItemsArray = new Array();
    var canProgress = false;

    getEnumerationFileCompletionStatus();

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


    function handleReport(report) {
        $('[data-toggle="tooltip"]').tooltip();
        $("#totalent").html(report.StringNumberOfRecordsDisplayValue);
        $("#totalenttp").data('bs.tooltip').config.title = report.StringNumberOfRecords + " entries processed";

        $("#totalval").html(report.StringNumberOfValidRecordsDisplayValue);
        $("#totalvaltp").data('bs.tooltip').config.title = report.StringNumberOfValidRecords + " valid entries";

        $("#totalinv").html(report.StringNumberOfInvalidRecordsDisplayValue);
        $("#totalinvtp").data('bs.tooltip').config.title = report.StringNumberOfInvalidRecords + " invalid entries";
    }


    function getEnumerationFileCompletionStatus() {
        var url = '/c/x/tax-payer-enumeration-schedule-completion-status';
        var requestData = { "batchToken": batchToken, "__RequestVerificationToken": token };
        $.post(url, requestData, function (data) {
            if (!data.Error) {
                completionStatus = data.ResponseObject;
                //$("#level").html(completionStatus + "%");
                if (data.ResponseObject === true) {
                    stop = true;
                    canProgress = true;
                }
            } else {
                stop = true;
                $("#proceedbtn").attr("disabled", true);
                $('.payeeloader').hide();
                $("#level").html("Error processing request. Please try again later or contact Admin. " + data.ResponseObject).css("color", "red");
            }
        }).always(function () {
            if (completionStatus !== true) {
                setTimeout(getEnumerationFileCompletionStatus, 3000);
            } else {
                stop = true;
                requestTableData();
            }
        });
    }


    function requestTableData() {
        if (completionStatus !== true || stop !== true) { return false; }
        //do ajax
        var url = '/c/x/tax-payer-enumeration-schedule-report-data';
        var requestData = { "batchToken": batchToken, "__RequestVerificationToken": token };
        $.post(url, requestData, function (data) {
            if (!data.Error) {
                //$("#totalamt").html("&#x20A6;" + data.ResponseObject.Amount);
                Page = 1;
                PageSize = data.ResponseObject.PageSize;
                $("#paginator").show();
                $("#pageSize").html(PageSize);
                $("#page").html(Page);
                if (data.ResponseObject.EnumerationItemsExcelReport.NumberOfValidRecords > 0) {
                    $("#proceedbtn").attr("disabled", false);
                }
                handleReport(data.ResponseObject.EnumerationItemsExcelReport);
                buildTableFromProcess(data.ResponseObject.LineItems, false);
            } else {
                $("#proceedbtn").attr("disabled", true);
                $("#level").html("Error processing your request : Please try again later or contact Admin").css("color", "red");
                $('.payeeloader').hide();
            }
        });
    }


    $('[name="cursor"]').click(function (event) {
        event.preventDefault();
        var cursor = event.target.id;
        $("#tbody").empty();
        $('.payeeloader').show();

        if (cursor === "moveright") {
            if (Page < PageSize) {
                Page += 1;
                //do ajax
                if (enumerationLineItemsArray[Page] === undefined) {
                    var url = '/c/x/get-tax-payer-enumeration-line-items-next-page';
                    var requestData = { "batchToken": batchToken, "page": Page, "__RequestVerificationToken": token };
                    $.post(url, requestData, function (data) {
                        if (!data.Error) {
                            buildTableFromProcess(data.ResponseObject, false);
                            $('.payeeloader').hide();
                        } else {
                            $("#proceedbtn").attr("disabled", false);
                            $("#level").html("Cannot fetch more data. Please try again leter.");
                            $('.payeeloader').hide();
                        }
                    });
                } else {
                    buildTableFromProcess(enumerationLineItemsArray[Page], true);
                }
            }
        } else if (cursor === "moveleft") {
            $('.payeeloader').hide();
            Page -= 1;
            if (enumerationLineItemsArray[Page] !== undefined) {
                buildTableFromProcess(enumerationLineItemsArray[Page], true);
            } else {
                Page += 1;
            }
        }
        manageCursors();
    });


    function buildTableFromProcess(lineItems, fromArr) {
        $('[data-toggle="tooltip"]').tooltip();

        $('.payeeloader').hide();
        //$("#level").html("100%");

        $("#level").html("");

        var tbody = $("#tbody");

        if (lineItems.length >= 1) {
            //set page number
            $("#page").html(Page);

            //check if this object is a new object, if it is new add to the existing enumerationLineItems array
            if (!fromArr) {
                enumerationLineItemsArray[Page] = lineItems;
            }

            for (var val in lineItems) {

                var tr = $('<tr />').appendTo(tbody);
                $('<td>' + lineItems[val].Name + '</td>').appendTo(tr);
                $('<td>' + lineItems[val].PhoneNumber + '</td>').appendTo(tr);

                $('<td>' + lineItems[val].Email + '</td>').appendTo(tr);

                $('<td>' + lineItems[val].TIN + '</td>').appendTo(tr);

                $('<td>' + lineItems[val].LGA + '</td>').appendTo(tr);
                $('<td>' + lineItems[val].Address + '</td>').appendTo(tr);

                if (lineItems[val].HasError) {
                    $('<td>' + '<span class="status-false" data-toggle="tooltip" data-html = "true" data-placement="top" title="' + lineItems[val].ErrorMessages + '" >i</span>' + '</td>').appendTo(tr);
                }
                else {
                    $('<td><span class="status-true" data-toggle="tooltip" data-placement="top">&#10003;</span></td>').appendTo(tr);
                }
            }
            $('[data-toggle="tooltip"]').tooltip();
            //manage cursor, manages the pagination
            manageCursors();
        }
    }
});