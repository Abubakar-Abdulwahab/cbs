$(document).ready(function () {
    var token = $("input[name=__RequestVerificationToken]").val();
    var Page = 1;
    var subUsersMap = new Map();
    var userPartClassPattern = /user-part-/d//;
    getFirstPageData();
    manageCursors();


    $("#addSubUserBtn").click(function () {
        $("#registerSubUserModal").show();
    });

    $("#closeRegisterSubUserModal").click(function () {
        $("#registerSubUserModal").hide();
    });

    $("#createSubUserModal").on('submit', function (e) {
        var inputValue = $('#branchData').val();
        var branchValueIdentifier = $('#branches option[value="' + inputValue + '"]').attr('data-value');
        $('#branchIdentifier').val(branchValueIdentifier);
        return;
    });

    $(".deactivate-sub-user-stat").click(function (e) {
        deactivateSubUser(this.classList);
    });

    $(".activate-sub-user-stat").click(function (e) {
        activateSubUser(this.classList);
    });

    function deactivateSubUser(classList) {
        let userPartId = 0;
        $(classList).each(function (index, element) {
            if (userPartClassPattern.test(element)) {
                let lastClassArr = element.split("-");
                userPartId = lastClassArr[lastClassArr.length - 1];
            }
        });
        toggleUserStatus(userPartId, false);
    }

    function activateSubUser(classList) {
        let userPartId = 0;
        $(classList).each(function (index, element) {
            if (userPartClassPattern.test(element)) {
                let lastClassArr = element.split("-");
                userPartId = lastClassArr[lastClassArr.length - 1];
            }
        });

        toggleUserStatus(userPartId, true);
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

    function getFirstPageData() {
        Page = 1;
        subUsersMap.set(Page, subUsersPage1);
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
                if (!subUsersMap.has(Page)) {
                    fetchSubUsers();
                } else {
                    buildTableFromProcess(subUsersMap.get(Page), true);
                }
            }
        } else if (cursor == "moveleft") {
            Page -= 1;
            if (subUsersMap.has(Page)) {
                buildTableFromProcess(subUsersMap.get(Page), true);
            } else {
                fetchSubUsers();
            }
        }
        manageCursors();
    });

    function fetchSubUsers() {
        var reqTokenParsed = $("#token").val();
        var url = '/p/sub-users/move-right';
        var requestData = { "page": Page, "__RequestVerificationToken": token, "token": reqTokenParsed };
        $.get(url, requestData, function (data) {
            if (!data.Error) {
                if (data.ResponseObject.SubUsers.length === 0) {
                    $("#infoFlashMsg").show();
                    $("#infoMsg").html("No more records found");
                    $('.payeeloader').hide();
                    $(window).scrollTop(0);
                    return;
                }
                buildTableFromProcess(data.ResponseObject.SubUsers, false);
                $('.payeeloader').hide();
            } else {
                $("#errorFlash").show();
                $("#errorMsg").html(data.ResponseObject);
                $('.payeeloader').hide();
                $(window).scrollTop(0);
            }
        }).fail(function () {
            $("#errorFlash").show();
            $("#errorMsg").html("Sorry something went wrong while processing your request. Please try again later or contact admin.");
            $('.payeeloader').hide();
            $(window).scrollTop(0);
        });
    }

    function buildTableFromProcess(subUsersList, fromArr) {
        $('.payeeloader').hide();
        $("#errorMsg").html("");
        $("#errorFlash").hide();
        $("#errorMsg").html("");
        $("#infoFlashMsg").hide();

        var tbody = $("#tbody");
        if (subUsersList.length >= 1) {
            //set page number
            $("#page").html(Page);
            //check if this object is a new object, if it is new add to the existing payee array
            if (!fromArr) {
                subUsersMap.set(Page,subUsersList);
            }

            for (var val in subUsersList) {
                var tr = $('<tr />').appendTo(tbody);

                var td1 = $('<td>' + subUsersList[val].Name + '</td>').appendTo(tr);

                var td2 = $('<td>' + subUsersList[val].PhoneNumber + '</td>').appendTo(tr);

                var td3 = $('<td>' + subUsersList[val].Email + '</td>').appendTo(tr);

                var td4 = $('<td>' + subUsersList[val].BranchName + '</td>').appendTo(tr);

                var td5 = $('<td>' + subUsersList[val].Address + '</td>').appendTo(tr);

                var td6 = $('<td>' + (subUsersList[val].Verified ? '<div class="status-approved">Verified</div>' : '<div class="status-inactive">Unverified</div>') + '</td>').appendTo(tr);

                var td7 = $('<td>' + ((subUsersList[val].IsAdministrator) ? "" : (subUsersList[val].IsActive) ? `<div style="position:relative"><button type="button" class="btn btn-link px-0 deactivate-sub-user-stat user-part-${subUsersList[val].UserPartRecordId}">Deactivate</button><span id="toggleUserStatLoader${subUsersList[val].UserPartRecordId}" class="profileloader" style="z-index:10;display:none;left:25px;top:0"></span></div>` : `<div style="position:relative"><button type="button" class="btn btn-link px-0 activate-sub-user-stat user-part-${subUsersList[val].UserPartRecordId}">Activate</button><span id="toggleUserStatLoader${subUsersList[val].UserPartRecordId}" class="profileloader" style="z-index:10;display:none;left:25px;top:0"></span></div>`) + '</td>').appendTo(tr);
            }
            //manage cursor, manages the pagination
            manageCursors();

            $(".deactivate-sub-user-stat").click(function (e) {
                deactivateSubUser(this.classList);
            });

            $(".activate-sub-user-stat").click(function (e) {
                activateSubUser(this.classList);
            });
        }
    }


    function toggleUserStatus(userId, isActive) {
        let userPartClass = ".user-part-" + userId;
        $("button" + userPartClass).prop("disabled", true);
        $("#toggleUserStatLoader"+userId+"").show();
        let url = "/p/sub-users/toggle-status";
        let data = { "__RequestVerificationToken": token, "subUserUserId": userId, "isActive": isActive }
        $.post(url, data, function (response) {
            if (!data.Error) {
                if ($("button" + userPartClass).hasClass("deactivate-sub-user-stat")) {
                    $("button" + userPartClass).removeClass("deactivate-sub-user-stat");
                    $("button" + userPartClass).addClass("activate-sub-user-stat");
                    $("button" + userPartClass).html("Activate");
                    $("button" + userPartClass).off("click");
                    $("button" + userPartClass).click(function (e) { activateSubUser(this.classList); });
                } else {
                    $("button" + userPartClass).removeClass("activate-sub-user-stat");
                    $("button" + userPartClass).addClass("deactivate-sub-user-stat");
                    $("button" + userPartClass).html("Deactivate");
                    $("button" + userPartClass).off("click");
                    $("button" + userPartClass).click(function (e) { deactivateSubUser(this.classList); });
                }
                subUsersMap.delete(Page);
                $("#infoFlashMsg").show();
                $("#infoMsg").html(response.ResponseObject);
            } else {
                $("#errorFlash").show();
                $("#errorMsg").html(response.ResponseObject);
            }
        }).always(function () {
            $("button" + userPartClass).prop("disabled", false);
            $("#toggleUserStatLoader" + userId + "").hide();
        }).fail(function () {
            $("#errorFlash").show();
            $("#errorMsg").html("Sorry something went wrong while processing your request. Please try again later or contact admin.");
        });
    }
});