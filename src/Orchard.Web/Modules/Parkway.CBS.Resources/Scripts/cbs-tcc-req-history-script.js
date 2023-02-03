$(document).ready(function () {
    var token = $("input[name=__RequestVerificationToken]").val();
    var percentage = "";
    var stop = false;
    //var PageSize = 0;
    var Page = 0;
    var requestsArray = new Array();
    var canProgress = false;
    Page = 1;
    //PageSize = parseInt($("#pageSize").text(), 10);
    canProgress = true;
    getFirstPageData();
    manageCursors();


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
        requestsArray[Page] = requestPage1;
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
                    var reqTokenParsed = $("#token").val();
                    var url = 'x/get-next-tcc-requests';
                    var requestData = {"page": Page, "__RequestVerificationToken": token, "token" : reqTokenParsed};
                    $.get(url, requestData, function (data) {
                        if (!data.Error) {
                            if(data.ResponseObject.Requests.length === 0){
                            $("#infoFlashMsg").show();
                            $("#infoMsg").html("No more records found");  
                            $('.payeeloader').hide();
                            $(window).scrollTop(0);
                            return;
                            }
                            buildTableFromProcess(data.ResponseObject.Requests, false);
                            $('.payeeloader').hide();
                        } else {
                            $("#errorFlash").show();
                            $("#errorMsg").html(data.ResponseObject.Message);
                            $('.payeeloader').hide();
                            $(window).scrollTop(0);
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
        $('.payeeloader').hide();
        $("#errorMsg").html("");
        $("#errorFlash").hide();
        $("#errorMsg").html("");
        $("#infoFlashMsg").hide();

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

                var td1 = $('<td>' + requestList[val].ApplicationNumber + '</td>').appendTo(tr);

                var td3 = $('<td>' + requestList[val].RequestReason + '</td>').appendTo(tr);

                var td6 = $('<td>' + requestList[val].Status + '</td>').appendTo(tr);

                var td7 = $('<td>' + requestList[val].RequestDate + '</td>').appendTo(tr);
            }
            //manage cursor, manages the pagination
            manageCursors();
        }
    }
});