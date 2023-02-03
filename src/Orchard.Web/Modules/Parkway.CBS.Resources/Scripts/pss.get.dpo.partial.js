var addBtn = document.getElementById("addOfficerBtn");
var numberOfOfficers = document.getElementById("numberOfOfficers");
var officerList = [];
var officerListIndex = 1;
var totalOfficerPicked = 0;

addBtn.addEventListener("click", getOfficer, false);

function getOfficer() {
    let personnel = null;
    let serviceNumber = $("#serviceNumber").val();
    if (totalOfficerPicked >= 1) { alert("You have already added a DPO"); return; }
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
            addOfficer(requestedOfficer);
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
    officerList.push(officer);
    destroyTable();
    buildTable();
    officerListIndex += 1;
}

function buildTable() {
    let dpoOfficerTable = document.getElementById("dpoOfficerTable");
    officerList.forEach(function (value, index) {
        let row = dpoOfficerTable.insertRow(-1);
        let cell1 = row.insertCell(0).innerHTML = '' + value.rank + ' <input type="hidden" name="SelectedDPO[' + index + '].OfficerRankId" value="' + value.rankId + '" /><input type="hidden" name="SelectedDPO[' + index + '].OfficerRankName" value="' + value.rank + '" />';
        let cell2 = row.insertCell(1).innerHTML = '' + value.name + ' <input type="hidden" name="SelectedDPO[' + index + '].PoliceOfficerLogId" value="' + value.officerLogId + '" /><input type="hidden" name="SelectedDPO[' + index + '].OfficerName" value="' + value.name + '" />';
        let cell3 = row.insertCell(2).innerHTML = '' + value.commandName + '<input type="hidden" name="SelectedDPO[' + index + '].OfficerCommandId" value="' + value.commandId + '" />';
        let cell4 = row.insertCell(3).innerHTML = '' + value.id + '<input type="hidden" name="SelectedDPO[' + index + '].ServiceNumber" value=' + value.id + ' />';
        let cell5 = row.insertCell(4).innerHTML = '' + value.ippis + '<input type="hidden" name="SelectedDPO[' + index + '].IPPIS" value=' + value.ippis + ' />';
        let cell6 = row.insertCell(5).innerHTML = '' + value.accountNumber + '<input type="hidden" name="SelectedDPO[' + index + '].AccountNumber" value=' + value.accountNumber + ' />';
        let cell7 = row.insertCell(6).innerHTML = "<span class='delete-officer-row' Title='Remove Item' onClick='removeOfficer(" + index + ")'>Remove</span>";
    });
    clearInputs();
}

function removeOfficer(index) {
    let dpoOfficerTable = document.getElementById("dpoOfficerTable");
    dpoOfficerTable.deleteRow(index + 1);
    officerList.splice(index, 1);
    officerListIndex -= 1;
    totalOfficerPicked -= 1;
    destroyTable();
    buildTable();
}

function destroyTable() {
    let dpoOfficerTable = document.getElementById("dpoOfficerTable");
    for (let i = 1; i < officerListIndex; ++i) {
        dpoOfficerTable.deleteRow(-1);
    }
}

function clearInputs() {
    $("#serviceNumber").val("");
}

function checkIfOfficerAlreadyAdded(officerId) {
    return officerList.findIndex((x) => { return x.id.toLowerCase() === officerId.toLowerCase(); });
}
