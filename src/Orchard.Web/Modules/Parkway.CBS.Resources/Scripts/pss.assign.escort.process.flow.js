var selectedUsernames = [];
var selectedUsernameListIndex = 1;
var processStages = new Map();
var currentlySelectedUser = null;

$("#searchBtn").click(function () {
    if ($("#username").val() == "") {
        alert('Please enter a username');
        return;
    }

    getAdminUserDetails($("#username").val());
});

$("#processFlowCommandType").change(function (e) {
    $("#processFlowStageSearchError").empty();
    if (isNaN(e.currentTarget.value)) { alert("Selected unit is not valid"); return; }
    $("#processFlowStageLoader").show();
    $("#processFlowCommandType").prop("disabled", true);
    $("#processFlowStageDefinition").prop("disabled", true);
    if (processStages.has(e.currentTarget.value)) {
        buildProcessStages(processStages.get(e.currentTarget.value));
    } else {
        getEscortProcessStageDefinitions(e.currentTarget.value);
    }
});

if (assignedProcessStages != null && assignedProcessStages.length > 0) {
    $(assignedProcessStages).each(function (index, value) {
        addPostbackDataToTable(value);
    });
}

function addPostbackDataToTable(postbackUserData) {
    selectedUsernames.push({
        id: postbackUserData.AdminUserId,
        username: postbackUserData.AdminUsername,
        processStage: postbackUserData.LevelName,
        processStageId: postbackUserData.LevelId,
        commandTypeId: postbackUserData.CommandTypeId
    });
    destroyTable();
    buildTable();
    selectedUsernameListIndex += 1;
}

function getAdminUserDetails(adminUsername) {
    $("button").prop("disabled", true);
    $("#username").prop("disabled", true);
    $("#usernameLoader").show();
    clearInputs();

    var url = "/Admin/Police/Escort/Process/Flow/get-admin-user-details";
    var requestData = { "adminUsername": adminUsername, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
    $.post(url, requestData, function (data) {
        if (!data.Error && data.StatusCode == 200) {
            currentlySelectedUser = data.ResponseObject;
            $('#fullName').val(data.ResponseObject.Fullname);
            $('#email').val(data.ResponseObject.Email);
            $('#phoneNumber').val(data.ResponseObject.PhoneNumber);
            $('#command').val(data.ResponseObject.CommandName);
        }
        else {
            $('#usernameSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >' + data.ResponseObject + '</span > ');
        }

    }).fail(function () {
        currentlySelectedUser = null;
        $('#usernameSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000">An error occurred try refreshing the page.</span > ');
    }).always(function () {
        $("#usernameLoader").hide();
        $("#username").prop("disabled", false);
        $("button").prop("disabled", false);
    });
}

function getEscortProcessStageDefinitions(commandType) {
    $("#processFlowCommandType").prop("disabled", true);
    $("#processFlowStageDefinition").prop("disabled", true);

    let url = "/Admin/Police/Escort/Process/Flow/get-escort-process-stages";
    let requestData = { "commandTypeId": commandType, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };;
    $.post(url, requestData, function (data) {
        if (!data.Error && data.StatusCode == 200) {
            processStages.set(commandType, data.ResponseObject);
            buildProcessStages(data.ResponseObject);
        } else {
            $('#processFlowStageSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >' + data.ResponseObject + '</span > ');
        }
    }).always(function () {
        $("#processFlowStageLoader").hide();
        $("#processFlowCommandType").prop("disabled", false);
        $("#processFlowStageDefinition").prop("disabled", false);
    }).fail(function () {
        $("#processFlowStageSearchError").html('<span class="field-validation-error tiny-caption" style = "color:#ff0000">An error occurred try refreshing the page.</span >');
        $("#processFlowCommandType").prop("disabled", false);
        $("#processFlowStageDefinition").prop("disabled", false);
    });
}

$("#addProcessFlow").click(function () {

    if (currentlySelectedUser == null) {
        alert('Please search/enter a username'); return;
    }

    if ($("#processFlowCommandType").val() == undefined || $("#processFlowCommandType").val() == null || $("#processFlowCommandType").val() <= 0) {
        alert('Please select a unit'); return;
    }

    if ($("#processFlowStageDefinition").val() == undefined || $("#processFlowStageDefinition").val() == null || $("#processFlowStageDefinition").val() <= 0) {
        alert('Please select a process stage'); return;
    }

    let user = currentlySelectedUser;
    if (selectedUsernames.findIndex(function (value, index) { return value.username == user.Username }) == -1) {

        let userObj = {};
        userObj.username = user.Username;
        userObj.processStage = $("#processFlowStageDefinition :selected").text();
        userObj.processStageId = $("#processFlowStageDefinition").val();
        userObj.id = user.Id;
        userObj.commandTypeId = $("#processFlowCommandType").val();

        selectedUsernames.push(userObj);
        destroyTable();
        buildTable();
        selectedUsernameListIndex += 1;
        $("#username").val('');
        clearInputs();
    }
    else {
        alert('User already assigned to a process stage');
        return;
    }
});

$("form").submit(function (event) {
    event.preventDefault();
    if (selectedUsernames == null || selectedUsernames.length <= 0) {
        alert('Please assign a user to a process stage'); return;
    }
    else {
        $("form").off("submit");
        $(this).closest('form').submit();
    }
});

function destroyTable() {
    let processFlowTable = document.getElementById("processFlowTable");
    for (let i = 1; i < selectedUsernameListIndex; ++i) {
        processFlowTable.deleteRow(-1);
    }
}

function buildProcessStages(stages) {
    $("#processFlowStageDefinition").empty();
    $("#processFlowStageDefinition").append("<option selected disabled value=''>Select a Stage</option>");
    $(stages).each(function (index, value) {
        $("#processFlowStageDefinition").append("<option value=" + value.Id + ">" + value.Name + "</option>");
    });

    $("#processFlowStageLoader").hide();
    $("#processFlowCommandType").prop("disabled", false);
    $("#processFlowStageDefinition").prop("disabled", false);
}

function buildTable() {
    let processFlowTable = document.getElementById("processFlowTable");
    selectedUsernames.forEach(function (value, index) {
        let row = processFlowTable.insertRow(-1);
        let cell1 = row.insertCell(0).innerHTML = '' + value.username + '<input type="hidden"  name="EscortProcessFlows[' + index + '].AdminUserId" value=' + value.id + ' /><input type="hidden"  name="EscortProcessFlows[' + index + '].AdminUsername" value="' + value.username + '" />';
        let cell2 = row.insertCell(1).innerHTML = '' + value.processStage + '<input type="hidden"  name="EscortProcessFlows[' + index + '].LevelId" value=' + value.processStageId + ' /><input type="hidden"  name="EscortProcessFlows[' + index + '].CommandTypeId" value=' + value.commandTypeId + ' /><input type="hidden"  name="EscortProcessFlows[' + index + '].LevelName" value="' + value.processStage + '" />';
        let cell3 = row.insertCell(2).innerHTML = "<span class='delete-user-row' Title='Remove Item' onClick='removeUser(" + index + ")'>Remove User</span>";
    });
    clearInputs();
}

function removeUser(index) {
    let processFlowTable = document.getElementById("processFlowTable");
    processFlowTable.deleteRow(index + 1);
    selectedUsernames.splice(index, 1);
    selectedUsernameListIndex -= 1;
    destroyTable();
    buildTable();
}

function clearInputs() {
    $('#processFlowCommandType').prop('selectedIndex', 0);
    $('#processFlowStageDefinition').empty();
    $('#processFlowStageDefinition').append("<option selected disabled value=''>Select a Stage</option>");
    $('#usernameSearchError').empty();
    $('#fullName').val('');
    $('#phoneNumber').val('');
    $('#email').val('');
    $('#command').val('');
    currentlySelectedUser = null;
}