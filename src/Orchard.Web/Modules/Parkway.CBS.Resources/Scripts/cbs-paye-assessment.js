$(document).ready(function () {
    var token = $("input[name=__RequestVerificationToken]").val();
    $('[data-toggle="tooltip"]').tooltip();
    var percentage = "";
    var stop = false;
    var PageSize = 0;
    var Page = 0;
    var payesArray = new Array();
    var canProgress = false;

    getFileProcessPercentage();

    $("#confirmForm").on('submit', function (e) {
        if (!canProgress) { e.preventDefault(); }
        $("#proceedbtn").prop("disabled", true);
    });


    //function processAssessment() {
    //    var url = 'x/process-paye';
    //    var requestData = { "batchToken": batchToken, "__RequestVerificationToken": token };

    //    $.post(url, requestData, function (data) {
    //        if (data.Error) {
    //            $("#proceedbtn").attr("disabled", true);
    //            $("#level").html(data.ResponseObject).css("color", "red");
    //        }
    //    }).fail(function () {
    //        $("#proceedbtn").attr("disabled", true);
    //        $("#level").html("Error processing request. Please try again later or contact Admin").css("color", "red");
    //    }).always(function () {
    //        $('.payeeloader').hide();
    //    });
    //}

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


    function getFileProcessPercentage() {
        var url = '/c/x/paye-get-process-percentage';
        var requestData = { "batchToken": batchToken, "__RequestVerificationToken": token };
        $.post(url, requestData, function (data) {
            if (!data.Error) {
                percentage = data.ResponseObject;
                $("#level").html(percentage + "%");
                if (data.ResponseObject === 100) {
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
            if (percentage !== 100) {
                setTimeout(getFileProcessPercentage, 3000);
            } else {
                stop = true;
                requestTableData();
            }
        });
    }


    function requestTableData() {
        if (percentage !== 100 || stop !== true) { return false; }
        //do ajax
        var url = '/c/x/get-paye-report-data';
        var requestData = { "batchToken": batchToken, "__RequestVerificationToken": token };
        $.post(url, requestData, function (data) {
            if (!data.Error) {
                $("#totalamt").html("&#x20A6;" + data.ResponseObject.Amount);
                Page = 1;
                PageSize = data.ResponseObject.PageSize;
                $("#paginator").show();
                $("#pageSize").html(PageSize);
                $("#page").html(Page);
                if (data.ResponseObject.PayeeExcelReport.TotalAmountToBePaid > 0) {
                    $("#proceedbtn").attr("disabled", false);
                }
                handleReport(data.ResponseObject.PayeeExcelReport);
                buildTableFromProcess(data.ResponseObject.Payees, false);
            } else {
                $("#proceedbtn").attr("disabled", true);
                $("#level").html("Error processing your request : Please try again later or contact Admin").css("color", "red");
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
                if (payesArray[Page] === undefined) {
                    var url = '/c/x/get-paye-next-page';
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
                    buildTableFromProcess(payesArray[Page], true);
                }
            }
        } else if (cursor === "moveleft") {
            $('.payeeloader').hide();
            Page -= 1;
            if (payesArray[Page] !== undefined) {
                buildTableFromProcess(payesArray[Page], true);
            } else {
                Page += 1;
            }
        }
        manageCursors();
    });


    function buildTableFromProcess(payes, fromArr) {
        $('[data-toggle="tooltip"]').tooltip();

        $('.payeeloader').hide();
        $("#level").html("100%");

        $("#level").html("");

        var tbody = $("#tbody");

        if (payes.length >= 1) {
            //set page number
            $("#page").html(Page);

            //check if this object is a new object, if it is new add to the existing payee array
            if (!fromArr) {
                payesArray[Page] = payes;
            }

            for (var val in payes) {

                var tr = $('<tr />').appendTo(tbody);
                $('<td>' + payes[val].PayeeName + '</td>').appendTo(tr);
                $('<td>' + payes[val].PayerId + '</td>').appendTo(tr);

                $('<td>₦' + payes[val].GrossAnnual + '</td>').appendTo(tr);

                $('<td>₦' + payes[val].Exemptions + '</td>').appendTo(tr);

                $('<td>' + payes[val].Month + " | " + payes[val].Year + '</td>').appendTo(tr);

                var taxstring = 0;
                if (payes[val].TaxableIncome !== null) { taxstring = payes[val].TaxableIncome; }
                $('<td>₦' + taxstring + '</td>').appendTo(tr);

                if (payes[val].HasError) {
                    $('<td>' + '<span class="status-false" data-toggle="tooltip" data-html = "true" data-placement="top" title="' + payes[val].ErrorMessage + '" >i</span>' + '</td>').appendTo(tr);
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