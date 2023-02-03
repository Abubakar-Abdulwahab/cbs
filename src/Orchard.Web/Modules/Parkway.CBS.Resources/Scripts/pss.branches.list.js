$(document).ready(function () {
    var token = $("input[name=__RequestVerificationToken]").val();
    var Page = 1;
    var branchesArray = new Array();
    getFirstPageData();
    manageCursors();

    $("#addBranchBtn").click(function () {
        $("#branchRegisterModal").show();
    });

    $("#closeRegisterBranchModal").click(function () {
        $("#branchRegisterModal").hide();
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
        branchesArray[Page] = branchesPage1;
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
                if (branchesArray[Page] == undefined) {
                    var reqTokenParsed = $("#token").val();
                    var url = '/p/branches/move-right';
                    var requestData = { "page": Page, "__RequestVerificationToken": token, "token": reqTokenParsed };
                    $.get(url, requestData, function (data) {
                        if (!data.Error) {
                            if (data.ResponseObject.Branches.length === 0) {
                                $("#infoFlashMsg").show();
                                $("#infoMsg").html("No more records found");
                                $('.payeeloader').hide();
                                $(window).scrollTop(0);
                                return;
                            }
                            buildTableFromProcess(data.ResponseObject.Branches, false);
                            $('.payeeloader').hide();
                        } else {
                            $("#errorFlash").show();
                            $("#errorMsg").html(data.ResponseObject.Message);
                            $('.payeeloader').hide();
                            $(window).scrollTop(0);
                        }
                    });
                } else {
                    buildTableFromProcess(branchesArray[Page], true);
                }
            }
        } else if (cursor == "moveleft") {
            $('.payeeloader').hide();
            Page -= 1;
            if (branchesArray[Page] != undefined) {
                buildTableFromProcess(branchesArray[Page], true);
            } else {
                Page += 1;
            }
        }
        manageCursors();
    });

    function buildTableFromProcess(branchesList, fromArr) {
        $('.payeeloader').hide();
        $("#errorMsg").html("");
        $("#errorFlash").hide();
        $("#errorMsg").html("");
        $("#infoFlashMsg").hide();

        var tbody = $("#tbody");
        if (branchesList.length >= 1) {
            //set page number
            $("#page").html(Page);
            //check if this object is a new object, if it is new add to the existing payee array
            if (!fromArr) {
                branchesArray[Page] = branchesList;
            }

            for (var val in branchesList) {
                var tr = $('<tr />').appendTo(tbody);

                var td1 = $('<td>' + branchesList[val].CreatedAtParsed + '</td>').appendTo(tr);

                var td2 = $('<td>' + branchesList[val].Name + '</td>').appendTo(tr);

                var td3 = $('<td>' + branchesList[val].Address + '</td>').appendTo(tr);

                var td4 = $('<td>' + branchesList[val].StateName + '</td>').appendTo(tr);

                var td5 = $('<td>' + branchesList[val].LGAName + '</td>').appendTo(tr);
            }
            //manage cursor, manages the pagination
            manageCursors();
        }
    }
});