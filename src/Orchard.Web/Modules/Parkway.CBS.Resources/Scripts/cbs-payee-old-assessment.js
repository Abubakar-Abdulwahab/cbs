$(document).ready(function () {
    var token = $("input[name=__RequestVerificationToken]").val();
    $('[data-toggle="tooltip"]').tooltip();
    var percentage = "";
    var stop = false;
    var PageSize = 0;
    var Page = 0;
    var payeesArray = new Array();
    var canProgress = false;

    if (doWork) {
        processAssessmentFile();
        //getFileProcessPercentage();
    } else {
        Page = 1;
        //console.log($("#pageSize").text());
        PageSize = parseInt($("#pageSize").text(), 10);
        canProgress = true;
        manageCursors();
    }


    $("#confirmForm").on('submit', function (e) {
        if (!canProgress) { e.preventDefault(); }
        $("#proceedbtn").prop("disabled", true);
    });


    function processAssessmentFile() {
        var url = 'process-directAssessment-payee-file';
        var requestData = { "batchToken": batchToken, "__RequestVerificationToken": token };

        $.post(url, requestData, function (data) {
            if (!data.Error) {
                $("#totalamt").html("&#x20A6;" + data.ResponseObject.Amount);
                Page = 1;
                PageSize = data.ResponseObject.PageSize;
                $("#paginator").show();
                $("#pageSize").html(PageSize);
                $("#page").html(Page);
                handleReport(data.ResponseObject.PayeeExcelReport);
                buildTableFromProcess(data.ResponseObject.Payees, false);
                canProgress = true;
                $("#proceedbtn").attr("disabled", false);
            } else {
                $("#proceedbtn").attr("disabled", true);
                $("#level").html("Error reading the file:" + data.ResponseObject).css("color", "red");
            }
        }).fail(function () {
            $("#proceedbtn").attr("disabled", true);
            $("#level").html("Error reading the file: Please try again later or contact Admin").css("color", "red");
        }).always(function () {
            $('.payeeloader').hide();
        });
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

    ////function manageCursors() {
    ////    //console.log(PageSize);
    ////    if (Page < PageSize && PageSize > 1) {
    ////        $("#moveright").prop("disabled", false);
    ////    } else {
    ////        $("#moveright").prop("disabled", true);
    ////    }

    ////    if (Page > 1) {
    ////        $("#moveleft").prop("disabled", false);
    ////    } else {
    ////        $("#moveleft").prop("disabled", true);

    ////    }
    ////}

    function handleReport(report) {
        $('[data-toggle="tooltip"]').tooltip();
        $("#totalent").html(report.AmountProcessedDisplayValue);
        $("#totalenttp").data('bs.tooltip').config.title = report.AmountProcessed + " entries processed";

        $("#totalval").html(report.AmountOfValidRecordsDisplayValue);
        $("#totalvaltp").data('bs.tooltip').config.title = report.AmountOfValidRecords + " valid entries";

        //$("#totalvaltp").data('bs.tooltip').options.title = report.AmountOfValidRecords + " valid entries";

        $("#totalinv").html(report.AmountOfInvalidRecordsDisplayValue);
        $("#totalinvtp").data('bs.tooltip').config.title = report.AmountOfInvalidRecords + " invalid entries";
    }


    function getFileProcessPercentage() {
        var url = 'get-percentage';
        var requestData = { "batchToken": batchToken, "__RequestVerificationToken": token };
        $.post(url, requestData, function (data) {
            //console.log(data);
            if (!data.Error) {
                percentage = data.ResponseObject;
                $("#level").html(percentage + "%");
                if (data.ResponseObject == "100") {
                    stop = true;
                    canProgress = true;
                }
            }
        }).always(function () {
            if (percentage != "100") {
                setTimeout(getFileProcessPercentage, 30000);
            } else {
                stop = true;
                requestTableData();
            }
        });
    }


    function requestTableData() {
        if (percentage != "100" || stop != true) { return false;}
        //do ajax
        var url = 'get-tabledata';
        var requestData = { "batchToken": batchToken, "__RequestVerificationToken": token };
        $.post(url, requestData, function (data) {
            if (!data.Error) {
                $("#totalamt").html("&#x20A6;" + data.ResponseObject.R.Amount);
                Page = 1;
                //console.log(data.ResponseObject);
                PageSize = data.ResponseObject.R.PageSize;
                $("#paginator").show();
                $("#pageSize").html(PageSize);
                $("#page").html(Page);
                $("#proceedbtn").attr("disabled", false);
                handleReport(data.ResponseObject.R.PayeeExcelReport);
                buildTableFromProcess(data.ResponseObject.L, false)
            } else {
                // $("#proceedbtn").attr("disabled", false);
            }
        });        
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
                if (payeesArray[Page] == undefined) {
                    var url = 'get-next-page';
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
                    buildTableFromProcess(payeesArray[Page], true);
                }
            }
        } else if (cursor == "moveleft") {
            $('.payeeloader').hide();
            Page -= 1;
            if (payeesArray[Page] != undefined) {
                buildTableFromProcess(payeesArray[Page], true);
            } else {
                Page += 1;
            }
        }
        manageCursors();
    });


    function buildTableFromProcess(payees, fromArr) {
        $('[data-toggle="tooltip"]').tooltip();
        
        $('.payeeloader').hide();
        $("#level").html("100%");
        
        $("#level").html("");
        
        var tbody = $("#tbody");
       
        if (payees.length >= 1) {
            //set page number
            $("#page").html(Page);

            //check if this object is a new object, if it is new add to the existing payee array
            if (!fromArr) {
                payeesArray[Page] = payees;
            }

            for (var val in payees) {

                var tr = $('<tr />').appendTo(tbody);
                var td0 = $('<td>' + payees[val].PayeeName + '</td>').appendTo(tr);
                var td1 = $('<td>' + payees[val].TIN + '</td>').appendTo(tr);

                var td2 = $('<td>₦' + payees[val].GrossAnnual + '</td>').appendTo(tr);

                var td3 = $('<td>₦' + payees[val].Exemptions + '</td>').appendTo(tr);

                var td4 = $('<td>' + payees[val].Month + " / " + payees[val].Year + '</td>').appendTo(tr);

                var taxstring = 0;
                if (payees[val].TaxableIncome != null) { taxstring = payees[val].TaxableIncome; }
                var td5 = $('<td>₦' + taxstring + '</td>').appendTo(tr);

                if (payees[val].HasError)
                {
                    //<td><span class="status-false" title="@item.ErrorMessage" style="white-space: pre-wrap;" data-toggle="tooltip" data-placement="top" data-html="true">i</span></td>
                    var td6 = $('<td>' + '<span class="status-false" data-toggle="tooltip" data-html = "true" data-placement="top" title="' + payees[val].ErrorMessage + '" >i</span>' + '</td>').appendTo(tr);
                }
                else
                {
                    var td6 = $('<td><span class="status-true" data-toggle="tooltip" data-placement="top">&#10003;</span></td>').appendTo(tr);
                }
            }
            $('[data-toggle="tooltip"]').tooltip();
            //manage cursor, manages the pagination
            manageCursors();
        }
    }   
});