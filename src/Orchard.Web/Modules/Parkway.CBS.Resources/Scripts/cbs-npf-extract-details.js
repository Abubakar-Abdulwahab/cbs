var addBtn = document.getElementById("addOfficerBtn");
var numberOfOfficers = document.getElementById("numberOfOfficers");
var officerList = [];
var officerListIndex = 1;
var totalOfficerPicked = 0;
var edit = false;
var originalSelectionArray = [];
var additionsArray = [];
var removalsArray = [];

addBtn.addEventListener("click", getOfficer, false);


if (proposedEscortOfficers != null) {
    edit = true;
    proposedEscortOfficers.forEach(function (value) {
        originalSelectionArray.push(value.OfficerIdentificationNumber);
        let proposedOfficer = {};
        proposedOfficer.rank = value.OfficerRankCode;
        proposedOfficer.commandName = value.OfficerCommandName;
        proposedOfficer.name = value.OfficerName;
        proposedOfficer.id = value.OfficerIdentificationNumber;
        proposedOfficer.officerLogId = value.PoliceOfficerLogId.toString();
        proposedOfficer.rankId = value.OfficerRankId.toString();
        proposedOfficer.commandId = value.OfficerCommandId;
        proposedOfficer.ippis = value.OfficerIPPISNumber;
        proposedOfficer.accountNumber = value.OfficerAccountNumber;
        officerList.push(proposedOfficer);
        officerListIndex += 1;
    });
}


function getOfficer() {
    let personnel = null;
    let serviceNumber = $("#serviceNumber").val();
    if (serviceNumber.length == 0) { alert("You need to specify the service number of the officer you want to add"); return; }
    if (checkIfOfficerAlreadyAdded(serviceNumber) > -1) { alert('Officer has already been added'); return; }

    $("#profileloader").show();
    $("#addOfficerBtn").prop("disabled", true);
    $("#serviceNumber").prop("disabled", true);
    $("#serviceNumberError").html("");
    let url = "/Admin/Police/get-personnel-with-service-number";
    let requestData = { "serviceNumber": serviceNumber, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };

    $.post(url, requestData, function (data) {
        if (!data.Error) {
            $("#profileloader").hide();
            personnel = JSON.parse(data.ResponseObject);
            totalOfficerPicked += 1;
            //if (totalOfficerPicked <= Number(numberOfOfficers.value)) {
            let requestedOfficer = {};
            requestedOfficer.rank = personnel.RankCode;
            requestedOfficer.rankId = personnel.RankId;
            requestedOfficer.id = personnel.IdNumber;
            requestedOfficer.officerLogId = personnel.PoliceOfficerLogId;
            requestedOfficer.name = personnel.Name;
            requestedOfficer.commandName = personnel.CommandName;
            requestedOfficer.commandId = personnel.CommandId;
            requestedOfficer.ippis = personnel.IppisNumber;
            requestedOfficer.accountNumber = personnel.AccountNumber;
            //requestedOfficer.qty = officerQty.value;
            addOfficer(requestedOfficer);
            //} else {
            //    totalOfficerPicked -= 1;
            //    alert("You have exceeded number of required officers");
            //}
        } else {
            $("#profileloader").hide();
            $("#addOfficerBtn").prop("disabled", false);
            $("#serviceNumber").prop("disabled", false);
            $("#serviceNumberError").html(data.ResponseObject);
        }
    }).always(function () {
        $("#profileloader").hide();
        $("#addOfficerBtn").prop("disabled", false);
        $("#serviceNumber").prop("disabled", false);
    }).fail(function () {
        $("#profileloader").hide();
        $("#serviceNumberError").html("Sorry something went wrong while processing your request. Please try again later or contact admin");
    });
}

function addOfficer(officer) {
    addToAdditions(officer);
    officerList.push(officer);
    destroyTable();
    buildTable();
    officerListIndex += 1;
    collateRemovedOfficers();
    collateAddedOfficers();
}

function buildTable() {
    let officerTable = document.getElementById("officerTable");
    officerList.forEach(function (value, index) {
        let row = officerTable.insertRow(-1);
        let cell1 = row.insertCell(0).innerHTML = '' + value.rank + ' <input type="hidden" name="OfficersSelection[' + index + '].OfficerRankId" value="' + value.rankId + '" /><input type="hidden" name="OfficersSelection[' + index + '].OfficerRankName" value="' + value.rank + '" />';
        let cell2 = row.insertCell(1).innerHTML = '' + value.name + ' <input type="hidden" name="OfficersSelection[' + index + '].PoliceOfficerLogId" value="' + value.officerLogId + '" /><input type="hidden" name="OfficersSelection[' + index + '].OfficerName" value="' + value.name + '" />';
        let cell3 = row.insertCell(2).innerHTML = '' + value.commandName + '<input type="hidden" name="OfficersSelection[' + index + '].OfficerCommandId" value="' + value.commandId + '" />';
        let cell4 = row.insertCell(3).innerHTML = '' + value.id + '<input type="hidden" name="OfficersSelection[' + index + '].ServiceNumber" value=' + value.id + ' />';
        let cell5 = row.insertCell(4).innerHTML = '' + value.ippis + '<input type="hidden" name="OfficersSelection[' + index + '].IPPIS" value=' + value.ippis + ' />';
        let cell6 = row.insertCell(5).innerHTML = '' + value.accountNumber + '<input type="hidden" name="OfficersSelection[' + index + '].AccountNumber" value=' + value.accountNumber + ' />';
        let cell7 = row.insertCell(6).innerHTML = "<span class='delete-officer-row' Title='Remove Item' onClick='removeOfficer(" + index + ")'>Remove</span>";
    });
    clearInputs();
}

function removeOfficer(index) {
    let officerTable = document.getElementById("officerTable");
    officerTable.deleteRow(index + 1);
    var removedItem = officerList.splice(index, 1);
    addToRemovals(removedItem[0]);
    officerListIndex -= 1;
    totalOfficerPicked -= 1;
    destroyTable();
    buildTable();
    collateRemovedOfficers();
    collateAddedOfficers();
}

function destroyTable() {
    let officerTable = document.getElementById("officerTable");
    for (let i = 1; i < officerListIndex; ++i) {
        officerTable.deleteRow(-1);
    }
}

function clearInputs() {
    $("#serviceNumber").val("");
}

function checkIfOfficerAlreadyAdded(officerId) {
    return officerList.findIndex((x) => { return x.id.toLowerCase() === officerId.toLowerCase(); });
}

function addToAdditions(officer) {
    if (originalSelectionArray.findIndex(function (val) { return val.toLowerCase() == officer.id.toLowerCase() }) == -1) {
        additionsArray.push(officer);
    }

    if (getIndexFromArray(removalsArray, officer) > -1) {
        removalsArray.splice(getIndexFromArray(removalsArray, officer), 1);
    }
}

function addToRemovals(officer) {
    if (originalSelectionArray.findIndex(function (val) { return val.toLowerCase() == officer.id.toLowerCase() }) > -1) {
        removalsArray.push(officer);
    }

    if (getIndexFromArray(additionsArray, officer) > -1) {
        additionsArray.splice(getIndexFromArray(additionsArray, officer), 1);
    }
}


function getIndexFromArray(arr, officer) {
    return arr.findIndex(function (val) { return val.id.toLowerCase() == officer.id.toLowerCase() });
}


function collateAddedOfficers() {
    $(".addedOfficers").remove();
    additionsArray.forEach(function (val, index) {
        $("#officerTable").append('<input type="hidden" class="addedOfficers" name="AddedOfficersSelection[' + index + '].ServiceNumber" value=' + val.id + ' /><input type="hidden" name="OfficersSelection[' + index + '].OfficerName" value="' + val.name + '" /><input type="hidden" name="OfficersSelection[' + index + '].OfficerRankName" value="' + val.rank + '" />');
        $("#officerTable").append('<input type="hidden" class="addedOfficers" name="AddedOfficersSelection[' + index + '].PoliceOfficerLogId" value=' + val.officerLogId + ' />');
    });
}


function collateRemovedOfficers() {
    $(".removedOfficers").remove();
    removalsArray.forEach(function (val, index) {
        $("#officerTable").append('<input type="hidden" class="removedOfficers" name="RemovedOfficersSelection[' + index + '].ServiceNumber" value=' + val.id + ' /><input type="hidden" name="RemovedOfficersSelection[' + index + '].OfficerName" value="' + val.name + '" /><input type="hidden" name="RemovedOfficersSelection[' + index + '].OfficerRankName" value="' + val.rank + '" />');
        $("#officerTable").append('<input type="hidden" class="removedOfficers" name="RemovedOfficersSelection[' + index + '].PoliceOfficerLogId" value=' + val.officerLogId + ' />');
    });
}

