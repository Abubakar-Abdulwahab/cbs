$(document).ready(function () {
    var token = $("input[name=__RequestVerificationToken]").val();
    processAssessmentFile();
    var PageSize = 0;
    var Page = 0;
    var payeesArray = new Array();

    function processAssessmentFile() {
            var url = '/Collection/ProcessDirectAssessmentPayeeFile';
            var requestData = { "batchToken": batchToken, "__RequestVerificationToken": token };
            $.post(url, requestData, function (data) {
                //var data = response.ResponseObject;
                if (!data.Error) {
                    //get amount and fee element
                    $("#fee").html(data.ResponseObject.Amount);
                    $("#amount").html("&#x20A6;" + data.ResponseObject.Amount);
                    $("#loader").hide();
                    //build 
                    Page = 1;
                    PageSize = data.ResponseObject.PageSize;
                    buildTable(data.ResponseObject.Payees, false);
                    handleReport(data.ResponseObject.PayeeExcelReport);
                    console.log(data);
                } else {
                   // $("#proceedbtn").attr("disabled", false);
                }
            });
    }


    $('[name="cursor"]').click(function (event) {
        event.preventDefault();
        var cursor = event.target.id;
        $("#tbody1").empty();
        $('.payeeloader').show();

        if (cursor == "moveright") {
            if (Page < PageSize)
            {
                Page += 1;
                //do ajax
                if (payeesArray[Page] == undefined) {
                    var url = '/Collection/GetNextPage';
                    var requestData = { "batchToken": batchToken, "page": Page, "__RequestVerificationToken": token };
                    $.post(url, requestData, function (data) {
                        if (!data.Error) {
                            buildTable(data.ResponseObject, false);
                            $('.payeeloader').hide();
                        } else {
                            // $("#proceedbtn").attr("disabled", false);
                        }
                    });
                } else {
                    console.log("already exists");
                    buildTable(payeesArray[Page], true);
                    console.log(" Page " + Page);
                }
                
            } else {
                //disable button
            }
        } else if (cursor == "moveleft") {
            $('.payeeloader').hide();
            Page -= 1;
            buildTable(payeesArray[Page], true);
        }
        manageCursors();
    });


    function buildTable(payees, fromArr)
    {
        $('.payeeloader').hide();
        var tbody = $("#tbody1");
        $('[data-toggle="tooltip"]').tooltip();
        //console.log(payees);
        if (payees.length > 1) {
            $("#pageSize").html(PageSize);
            $("#page").html(Page);
            
            if (!fromArr) {
                payeesArray[Page] = payees;
            }

            for (var val in payees) {
                
                var tr = $('<tr />').appendTo(tbody);
                var td1 = $('<td>' + payees[val].TIN + '</td>').appendTo(tr);

                var td2 = $('<td>' + payees[val].GrossAnnual + '</td>').appendTo(tr);

                var td3 = $('<td>' + payees[val].Exemptions + '</td>').appendTo(tr);

                var taxstring = 0;
                if (payees[val].TaxableIncome != null) { taxstring = payees[val].TaxableIncome; }
                var td4 = $('<td>' + taxstring + '</td>').appendTo(tr);

                if (payees[val].HasError)
                { var td5 = $('<td>' + '<span class="badge badge-pill badge-danger" data-toggle="tooltip" data-placement="top" title="' + payees[val].ErrorMessage + '" style="background-color: #dc3545"><b>!</b></span>' + '</td>').appendTo(tr); td5.tooltip(); }
                else { var td5 = $('<td>' + "" + '</td>').appendTo(tr); }
            }
            manageCursors();
        }
    }


    function handleReport(report)
    {
        $("#processedbadge").html(report.AmountProcessedDisplayValue + " Processed").show().prop('title', report.AmountProcessed + " processed records");

        if (report.AmountOfInvalidRecords != "0") {
            $("#invalidbadge").html(report.AmountOfInvalidRecordsDisplayValue + " invalid").show().prop('title', report.AmountOfInvalidRecords + " invalid records");
        }

        if (report.AmountOfValidRecords != "0") {
            $("#validbadge").html(report.AmountOfValidRecordsDisplayValue + " valid").show().prop('title', report.AmountOfValidRecords + " valid records");
        }
    }

    function manageCursors() {

        if (Page < PageSize && PageSize > 1) {
            $("#moveright").removeClass("chevy");
            $("#moveright").addClass("chevyEnable");
        } else {
            $("#moveright").removeClass("chevyEnable");
            $("#moveright").addClass("chevy");
        }

        if (Page > 1) {
            $("#moveleft").removeClass("chevy");
            $("#moveleft").addClass("chevyEnable");
        } else {
            $("#moveleft").removeClass("chevyEnable");
            $("#moveleft").addClass("chevy");
        }
    }
    
});