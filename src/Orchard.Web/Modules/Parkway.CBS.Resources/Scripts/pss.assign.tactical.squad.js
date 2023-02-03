var addBtn = document.getElementById("addSquadBtn");
var tacticalSquad = document.getElementById("squadDropdown");
var officerQty = document.getElementById("officerQty");
var squadList = [];
var squadListIndex = 1;
var totalSquadsPicked = 0;
var edit = false;
var originalSelectionArray = [];
var additionsArray = [];
var removalsArray = [];

addBtn.addEventListener("click", getTacticalSquad, false);

if (assignedSquads != null) {
    edit = true;
    assignedSquads.forEach(function (value) {
        originalSelectionArray.push(value);
        let assignedTacticalSquad = {};
        assignedTacticalSquad.id = value.Command.Id.toString();
        assignedTacticalSquad.name = value.Command.Name;
        assignedTacticalSquad.officerQty = value.NumberOfOfficers.toString();
        assignedTacticalSquad.viewFormationsRoute = "/Admin/Police/Request/View/Allocated/Formations/" + value.Id + "/" + value.EscortSquadAllocationGroupId+"";
        squadList.push(assignedTacticalSquad);
        squadListIndex += 1;
    });
}

function getTacticalSquad() {
    if (tacticalSquad.options[tacticalSquad.selectedIndex] == undefined || tacticalSquad.options[tacticalSquad.selectedIndex].value == "") { alert("You need to select a tactical squad"); return; }
    if (officerQty.value == undefined || officerQty.value == "" || parseInt(officerQty.value) <= 0 || Number.isNaN(parseInt(officerQty.value))) { alert("You need to specify the number of officers"); return; }
    totalSquadsPicked += 1;
    let requestedTacticalSquad = {};
    requestedTacticalSquad.id = tacticalSquad.options[tacticalSquad.selectedIndex].value;
    requestedTacticalSquad.name = tacticalSquad.options[tacticalSquad.selectedIndex].text;
    requestedTacticalSquad.officerQty = officerQty.value;
    requestedTacticalSquad.viewFormationsRoute = "/Admin/Police/Request/View/Allocated/Formations/0/0";
    addTacticalSquad(requestedTacticalSquad);
}

function addTacticalSquad(squad) {
    if (checkIfTacticalSquadAlreadyAdded(squad.id) > -1) { alert('Tactical squad has already been added'); totalSquadsPicked -= 1; return; }
    addToAdditions(squad);
    squadList.push(squad);
    destroyTable();
    buildTable();
    squadListIndex += 1;
    //empty the input text
    collateAddedTacticalSquads();
    collateRemovedTacticalSquads();
}

function buildTable() {
    let squadTable = document.getElementById("squadTable");
    squadList.forEach(function (value, index) {
        let viewFormationsRoute = buildViewFormationsRoute(value);
        let row = squadTable.insertRow(-1);
        let cell1 = row.insertCell(0).innerHTML = '' + value.name + '<input type="hidden" name="TacticalSquadsSelection[' + index + '].TacticalSquadId" value=' + value.id + ' /><input type="hidden" name="TacticalSquadsSelection[' + index + '].SquadName" value="' + value.name + '" />';
        let cell2 = row.insertCell(1).innerHTML = '' + value.officerQty + '<input type="hidden" name="TacticalSquadsSelection[' + index + '].NumberofOfficers" value=' + value.officerQty + ' />';
        let cell3 = row.insertCell(2).innerHTML = (viewFormationsRoute != null) ? '<a href=' + viewFormationsRoute + ' target="_blank">View Formations</a>' : '';
        let cell4 = row.insertCell(3).innerHTML = "<span class='delete-squad-row' Title='Remove Item' onClick='removeSquad(" + index + ")'>Remove</span>";
    });
    clearInputs();
}

function removeSquad(index) {
    let squadTable = document.getElementById("squadTable");
    squadTable.deleteRow(index + 1);
    var removedItem = squadList.splice(index, 1);
    addToRemovals(removedItem[0]);
    squadListIndex -= 1;
    totalSquadsPicked -= 1;
    destroyTable();
    buildTable();
    collateRemovedTacticalSquads();
    collateAddedTacticalSquads();
}

function destroyTable() {
    let squadTable = document.getElementById("squadTable");
    for (let i = 1; i < squadListIndex; ++i) {
        squadTable.deleteRow(-1);
    }
}

function clearInputs() {
    document.getElementById("squadDropdown").options[0].selected = true;
    document.getElementById("officerQty").value = "";
}

function checkIfTacticalSquadAlreadyAdded(squadId) {
    return squadList.findIndex((x) => { return x.id === squadId; });
}

function addToAdditions(squad) {
    if (originalSelectionArray.findIndex(function (val) { return val.Command.Id.toString() == squad.id }) == -1) {
        additionsArray.push(squad);
    }

    if (getIndexFromArray(removalsArray, squad) > -1) {
        removalsArray.splice(getIndexFromArray(removalsArray, squad), 1);
    }
}

function addToRemovals(squad) {
    if (originalSelectionArray.findIndex(function (val) { return val.Command.Id.toString() == squad.id }) > -1) {
        removalsArray.push(squad);
    }

    if (getIndexFromArray(additionsArray, squad) > -1) {
        additionsArray.splice(getIndexFromArray(additionsArray, squad), 1);
    }
}


function getIndexFromArray(arr, squad) {
    return arr.findIndex(function (val) { return val.id == squad.id });
}


function buildViewFormationsRoute(squad) {
    let originalSelection = originalSelectionArray.find(function (value) { return value.Command.Id.toString() == squad.id });
    return (originalSelection != undefined) ? "/Admin/Police/Request/View/Allocated/Formations/" + originalSelection.Id + "/" + originalSelection.EscortSquadAllocationGroupId : null;
}

function collateAddedTacticalSquads() {
    $(".addedTacticalSquads").remove();
    additionsArray.forEach(function (val, index) {
        $("#squadTable").append('<input type="hidden" class="addedTacticalSquads" name="AddedTacticalSquads[' + index + '].TacticalSquadId" value=' + val.id + ' /><input type="hidden" class="addedTacticalSquads" name="AddedTacticalSquads[' + index + '].SquadName" value="' + val.name + '" />');
        $("#squadTable").append('<input type="hidden" class="addedTacticalSquads" name="AddedTacticalSquads[' + index + '].NumberofOfficers" value=' + val.officerQty + ' />');
    });
}


function collateRemovedTacticalSquads() {
    $(".removedTacticalSquads").remove();
    removalsArray.forEach(function (val, index) {
        $("#squadTable").append('<input type="hidden" class="removedTacticalSquads" name="RemovedTacticalSquads[' + index + '].TacticalSquadId" value=' + val.id + ' /><input type="hidden" class="removedTacticalSquads" name="RemovedTacticalSquads[' + index + '].SquadName" value="' + val.name + '" />');
        $("#squadTable").append('<input type="hidden" class="removedTacticalSquads" name="RemovedTacticalSquads[' + index + '].NumberofOfficers" value=' + val.officerQty + ' />');
    });
}

