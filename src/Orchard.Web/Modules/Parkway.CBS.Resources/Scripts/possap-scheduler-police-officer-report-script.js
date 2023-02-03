$(document).ready(function () {
    var loading = true;
    var counter = 0;
    var requestIdentifier = "";
    var searchConstraintsExists = false;
    var gottenExternalReportData = false;
    var loadingIntervalId = 0;
    var page = 1;
    var reportRecordsMap = new Map();

    attachOverlay();
    getData();
    

    function getData() {
        showErrorText(false);
        showLoadingText(true);

        let url = "/Admin/Police/Scheduling/Police-Officer/Get-Data";
        let data = { "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val(), "searchParametersToken": searchParametersToken};

        $("#overlay").show();
        changeLoadingText("Getting data");
        showProgress();
        $.post(url, data, function (response) {
            console.log(response);
            if (!response.Error) {
                requestIdentifier = response.ResponseObject;
                checkConstraints(searchParametersToken, requestIdentifier);
            } else {
                console.log(response);
                showLoadingText(false);
                changeErrorText(response.ResponseObject);
                showErrorText(true);
            }
        }).always(function () {
            loading = false;
            showProgress();
        });
    }

    function checkConstraints(searchParams, requestIdentifier) {
        searchConstraintsExists = false;
        let url = "/Admin/Police/Scheduling/Police-Officer/Check-Constraints";
        let data = { "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val(), "searchParams": searchParams, "requestIdentifier": requestIdentifier };

        $("#overlay").show();
        changeLoadingText("Checking cache for data");
        showProgress();
        $.post(url, data, function (response) {
            console.log(response);
            if (!response.Error) {
                searchConstraintsExists = response.ResponseObject;
                if (searchConstraintsExists) { getReportData(searchParams, requestIdentifier); }
                else { getExternalReportData(searchParams, requestIdentifier); }
            } else {
                showLoadingText(false);
                changeErrorText(response.ResponseObject);
                showErrorText(true);
            }
        }).always(function () {
            loading = false;
            showProgress();
        });
    }

    function getReportData(searchParams, requestIdentifier) {
        let url = "/Admin/Police/Scheduling/Police-Officer/Get-Report-Data";
        let data = { "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val(), "searchParams": searchParams, "requestIdentifier": requestIdentifier  };

        $("#overlay").show();
        changeLoadingText("Getting report data");
        showProgress();
        $.post(url, data, function (response) {
            console.log(response);
            if (!response.Error) {
                //reportRecordsMap.Set(page, response.ResponseObject.ReportRecords);
                BuildTable(response.ResponseObject.ReportRecords);
                getPager(1, response.ResponseObject.TotalNumberOfOfficers); //get pager
                $("#overlay").hide();
            } else {
                showLoadingText(false);
                changeErrorText(response.ResponseObject);
                showErrorText(true);
            }
        }).always(function () {
            loading = false;
            showProgress();
        });
    }

    function getExternalReportData(searchParams, requestIdentifier) {
        gottenExternalReportData = false;
        let url = "/Admin/Police/Scheduling/Police-Officer/Get-External-Report-Data";
        let data = { "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val(), "searchParams": searchParams, "requestIdentifier": requestIdentifier  };

        $("#overlay").show();
        changeLoadingText("Getting report data from external source");
        showProgress();
        $.post(url, data, function (response) {
            console.log(response);
            if (!response.Error) {
                gottenExternalReportData = response.ResponseObject;
                if (gottenExternalReportData) { getReportData(); }
            } else {
                showLoadingText(false);
                changeErrorText(response.ResponseObject);
                showErrorText(true);
            }
        }).always(function () {
            loading = false;
            showProgress();
        });
    }

    function getPager(page, numberOfOfficers) {
        let url = "/Admin/Police/Scheduling/Police-Officer/Get-Pager";
        let data = { "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val(), "page": page, "totalOfficers": numberOfOfficers };

        $.post(url, data, function (response) {
            if (!response.Error) {
                $("#pagerDiv").append(response.ResponseObject);


                //handles what happens when page size numbers are clicked
                $(".page-size-options ul.selector li a").click(function (e) {
                    e.preventDefault();
                    console.log(e.currentTarget.search);
                    let clickedPageNumber = e.currentTarget.search.split("&")[1].split("=")[1];
                    let clickedPageSizeNumber = e.currentTarget.search.split("&")[0].split("=")[1];
                    $("#pagerPageNumber").val(clickedPageNumber);
                    $("#pagerPageSizeNumber").val(clickedPageSizeNumber);
                    $("#officerReportForm").submit();
                });

                //handle what happens when page numbers are clicked
                $("ul.pager li a").click(function (e) {
                    e.preventDefault();
                    let clickedPageNumber = e.currentTarget.search.split("=")[1];
                    $("#pagerPageNumber").val(clickedPageNumber);
                    $("#officerReportForm").submit();
                });


            } else {
                console.log(response);
            }
        });
    }

    function changeLoadingText(loadingText) {
        $("#loadingText").html(loadingText);
    }

    function changeErrorText(errorText) {
        $("#errorText").html(errorText);
    }

    function showLoadingText(show) {
        if (show) { $(".loadingTextField").show(); }
        else { $(".loadingTextField").hide(); }
    }

    function showErrorText(show) {
        if (show) { $(".errorTextField").show(); }
        else { $(".errorTextField").hide(); }
    }

    function showProgress() {
        if (loading) {
            loadingIntervalId = setInterval(showProgressDots, 500);
        } else {
            $("#progressDots").html("");
            clearInterval(loadingIntervalId);
        }
    }

    function showProgressDots() {
        if (counter >= 0) {
            $("#progressDots").html(""); counter = -5;
        } else {
            $("#progressDots")[0].innerHTML += "."; counter++;
        }
    }

    function BuildTable(reportRecords) {
        $("#reportRecordsTableBody").empty();
        reportRecords.forEach(function (recordObj) {
            let recordRow = "<tr><td>" + recordObj.Name + "</td><td>" + recordObj.ServiceNumber + "</td><td>" + recordObj.IPPISNumber + "</td><td>" + recordObj.RankName + "</td><td>" + recordObj.Gender + "</td><td>" + recordObj.StateName + "</td><td>" + recordObj.LGAName + "</td><td>" + recordObj.CommandName + "</td><td></td><td><a href='#'>change command</a></td></tr>";
            $("#reportRecordsTableBody").append(recordRow);
        });
    }

    function attachOverlay() {
        let overlay = "<div class='overlay' id='overlay'><div class='loadingContentContainer'><p class='loadingTextField'><span id='loadingText'>Getting data</span><span id='progressDots'></span></p><p class='errorTextField'><span id='errorText'></span></p></div></div >";
        $("#content").append(overlay);
    }
});