var selectedUsernames = [];
var selectedUsernameListIndex = 1;
var originalSelectionArray = [];
var additionsArray = [];
var removalsArray = [];

$("#searchBtn").click(function () {

    if ($("#adminUserUsername").val() == "") {
        alert('Please enter a username');
        return;
    }

    getAdminUserDetails($("#adminUserUsername").val());
});

if (selectedUsers != null && selectedUsers.length > 0) {
    $(selectedUsers).each(function (index, value) {
        addPostbackDataToTable(value);
    });
}

function addPostbackDataToTable(postbackUserData) {
    originalSelectionArray.push(postbackUserData);
    selectedUsernames.push(postbackUserData);
    destroyTable();
    buildTable();
    selectedUsernameListIndex += 1;
}

function getAdminUserDetails(adminUsername) {

    $("#adminUserUsername").prop("disabled", true);
    $("#adminUserUsernameLoader").show();
    clearInputs();

    var url = `/Admin/Police/Expenditure/Wallet/get-admin-user-detail`;
    var requestData = { "adminUsername": adminUsername, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
    $.post(url, requestData, function (data) {
        if (!data.Error) {
            $('#fullName').val(data.ResponseObject.Fullname);
            $('#userName').val(data.ResponseObject.Username);
            $('#phoneNumber').val(data.ResponseObject.PhoneNumber);
            $('#email').val(data.ResponseObject.Email);
            $('#officerFormation').val(data.ResponseObject.OfficerFormation);
            $('#officerDepartment').val(data.ResponseObject.OfficerDepartment);
            $('#officerSubSection').val(data.ResponseObject.OfficerSubSection);
            $('#officerSection').val(data.ResponseObject.OfficerSection);
        }
        else {
            $('#adminUserUsernameError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >' + data.ResponseObject + '</span > ');
        }

    }).fail(function () {
        $('#adminUserUsernameError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000">An error occurred try refreshing the page.</span > ');
    }).always(function () {
        $("#adminUserUsernameLoader").hide();
        $("#adminUserUsername").prop("disabled", false);
    });
}

$("#addUser").click(function () {

    if ($("#userName").val() == "") {
        alert('Please search/enter a username'); return;
    }
    let username = $("#userName").val();
    if (selectedUsernames.findIndex(function (value, index) { return value.Username == username }) == -1) {

        let userObj = {};
        userObj.Username = username;

        if ($("#workflowLevel").val() == null) {
            alert('Please select a role');
            return;
        }
        userObj.SelectedFlowDefinitionLevelId = $("#workflowLevel").val();
        userObj.FlowDefintionLevelName = $('#workflowLevel :selected').text();

        let commandName;
        if ($("#officerSubSection").val() != "") {
            commandName = $("#officerSubSection").val();
        }
        else if ($("#officerSection").val() != "") {
            commandName = $("#officerSection").val();
        }
        else if ($("#officerDepartment").val() != "") {
            commandName = $("#officerDepartment").val();
        }
        else {
            commandName = $("#officerFormation").val();
        }

        userObj.CommandName = commandName;
        selectedUsernames.push(userObj);
        addToAdditions(userObj);
        destroyTable();
        buildTable();
        selectedUsernameListIndex += 1;
        clearInputs();
        collateAddedUsers();
        collateRemovedUsers();
    }
    else
    {
        alert('User already selected for a role');
        return;
    }
});

$("#submitBtn").click(function (event) {
    event.preventDefault();
    if (selectedUsernames == null || selectedUsernames.length <= 0) {
        alert('Please enter a user'); return;
    }
    else {
        $(this).closest('form').submit();
    }
});

function destroyTable() {
    let walletsTable = document.getElementById("walletsTable");
    for (let i = 1; i < selectedUsernameListIndex; ++i) {
        walletsTable.deleteRow(-1);
    }
}


function buildTable() {
    let walletsTable = document.getElementById("walletsTable");
    selectedUsernames.forEach(function (value, index) {
        let row = walletsTable.insertRow(-1);
        let cell1 = row.insertCell(0).innerHTML = '' + value.Username + '<input type="hidden"  name="WalletUsers[' + index + '].Username" value=' + value.Username + ' /><input type="hidden" name="WalletUsers[' + index + '].SelectedFlowDefinitionLevelId" value="' + value.SelectedFlowDefinitionLevelId + '" /><input type="hidden" name="WalletUsers[' + index + '].FlowDefintionLevelName" value="' + value.FlowDefintionLevelName + '" /><input type="hidden" name="WalletUsers[' + index + '].CommandName" value="' + value.CommandName + '" />';
        let cell2 = row.insertCell(1).innerHTML = '' + value.CommandName;
        let cell3 = row.insertCell(2).innerHTML = '' + value.FlowDefintionLevelName
        let cell4 = row.insertCell(3).innerHTML = "<span class='delete-user-row' Title='Remove Item' onClick='removeUser(" + index + ")'>Remove User</span>";
    });
    clearInputs();
}

function removeUser(index) {
    let walletsTable = document.getElementById("walletsTable");
    walletsTable.deleteRow(index + 1);
    var removedItem = selectedUsernames.splice(index, 1);
    addToRemovals(removedItem[0]);
    selectedUsernameListIndex -= 1;
    destroyTable();
    buildTable();
    collateRemovedUsers();
    collateAddedUsers();
}

function clearInputs() {
    $('#workflowLevel').prop('selectedIndex', 0);
    $('#adminUserUsernameError').empty();
    $('#fullName').val('');
    $('#adminUserUsername').val('');
    $('#userName').val('');
    $('#phoneNumber').val('');
    $('#email').val('');
    $('#officerFormation').val('');
    $('#officerDepartment').val('');
    $('#officerSubSection').val('');
    $('#officerSection').val('');
}

function addToAdditions(userObj) {
    if (originalSelectionArray.findIndex(function (val) { return val.Username == userObj.Username }) == -1) {
        additionsArray.push(userObj);
    }

    if (getIndexFromArray(removalsArray, userObj) > -1) {
        removalsArray.splice(getIndexFromArray(removalsArray, userObj), 1);
    }
}

function addToRemovals(userObj) {
    if (originalSelectionArray.findIndex(function (val) { return val.Username == userObj.Username }) > -1) {
        removalsArray.push(userObj);
    }

    if (getIndexFromArray(additionsArray, userObj) > -1) {
        additionsArray.splice(getIndexFromArray(additionsArray, userObj), 1);
    }
}


function getIndexFromArray(arr, userObj) {
    return arr.findIndex(function (val) { return val.Username == userObj.Username });
}


function collateAddedUsers() {
    $(".addedUsers").remove();
    additionsArray.forEach(function (val, index) {
        $("#walletsTable").append('<input type="hidden" class="addedUsers" name="AddedWalletUsers[' + index + '].Username" value="' + val.Username + '" /><input type="hidden" class="addedUsers" name="AddedWalletUsers[' + index + '].SelectedFlowDefinitionLevelId" value="' + val.SelectedFlowDefinitionLevelId + '" />');
        $("#walletsTable").append('<input type="hidden" class="addedUsers" name="AddedWalletUsers[' + index + '].FlowDefintionLevelName" value="' + val.FlowDefintionLevelName + '" /><input type="hidden" class="addedUsers" name="AddedWalletUsers[' + index + '].CommandName" value="' + val.CommandName + '" />');
    });
}


function collateRemovedUsers() {
    $(".removedUsers").remove();
    removalsArray.forEach(function (val, index) {
        $("#walletsTable").append('<input type="hidden" class="removedUsers" name="RemovedWalletUsers[' + index + '].Username" value="' + val.Username + '" /><input type="hidden" class="removedUsers" name="RemovedWalletUsers[' + index + '].SelectedFlowDefinitionLevelId" value="' + val.SelectedFlowDefinitionLevelId + '" />');
        $("#walletsTable").append('<input type="hidden" class="removedUsers" name="RemovedWalletUsers[' + index + '].FlowDefintionLevelName" value="' + val.FlowDefintionLevelName + '" /><input type="hidden" class="removedUsers" name="RemovedWalletUsers[' + index + '].CommandName" value="' + val.CommandName + '" />');
    });
}