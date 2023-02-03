$(document).ready(function () {
    var token = $("input[name=__RequestVerificationToken]").val();
    $('[data-toggle="tooltip"]').tooltip();
    var percentage = "";
    var stop = false;
    var PageSize = 0;
    var Page = 0;
    var cellSitesArray = new Array();
    var canProgress = false;
    //first leg validation for reading the file contents

    if (doWork) {
        //processAssessmentFile();
        //getFileProcessPercentage();
    } else {
        Page = 1;
        //console.log($("#pageSize").text());
        PageSize = parseInt($("#pageSize").text(), 10);
        canProgress = true;
        getFirstPageData();
        manageCursors();
    }


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
        cellSitesArray[Page] = cellSitesFirstData;
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
                if (cellSitesArray[Page] == undefined) {
                    var url = 'x/get-next-cellsites-list-page/' + operatorId + "/" + Page;
                    //var requestData = { "operatorId": operatorId, "page": Page, "__RequestVerificationToken": token };
                    $.get(url, function (data) {
                        if (!data.Error) {

                            buildTableFromProcess(data.ResponseObject.CellSites, false);
                            $('.payeeloader').hide();
                        } else {
                            $("#proceedbtn").attr("disabled", false);
                            $("#level").html("Cannot fetch more data. Please try again leter.");
                            $('.payeeloader').hide();
                        }
                    });
                } else {
                    buildTableFromProcess(cellSitesArray[Page], true);
                }
            }
        } else if (cursor == "moveleft") {
            $('.payeeloader').hide();
            Page -= 1;
            if (cellSitesArray[Page] != undefined) {
                buildTableFromProcess(cellSitesArray[Page], true);
            } else {
                Page += 1;
            }
        }
        manageCursors();
    });


    function buildTableFromProcess(cellSites, fromArr) {
        $('[data-toggle="tooltip"]').tooltip();
        $('.payeeloader').hide();
        $("#level").html("100%");

        $("#level").html("");

        var tbody = $("#tbody");
        if (cellSites.length >= 1) {
            //set page number
            $("#page").html(Page);
            //check if this object is a new object, if it is new add to the existing payee array
            if (!fromArr) {
                cellSitesArray[Page] = cellSites;
            }

            for (var val in cellSites) {
                var tr = $('<tr />').appendTo(tbody);
                var td0 = $('<td>' + cellSites[val].OperatorSiteId + '</td>').appendTo(tr);

                var td1 = $('<td>' + cellSites[val].YearOfDeployment + '</td>').appendTo(tr);

                var td2 = $('<td>' + cellSites[val].HeightOfTower + '</td>').appendTo(tr);

                var td3 = $('<td>' + cellSites[val].MastType + '</td>').appendTo(tr);

                var td4 = $('<td>(' + cellSites[val].Lat + ',' + cellSites[val].Long + ')</td>').appendTo(tr);

                var td5 = $('<td>' + cellSites[val].SiteAddress + '</td>').appendTo(tr);

                var td6 = $('<td>' + cellSites[val].Region + '</td>').appendTo(tr);

                var td7 = $('<td>' + cellSites[val].State + '</td>').appendTo(tr);

                var td8 = $('<td>' + cellSites[val].LGA + '</td>').appendTo(tr);
            }
            $('[data-toggle="tooltip"]').tooltip();
            //manage cursor, manages the pagination
            manageCursors();
        }
    }
});