var addBtn = document.getElementById("addFormationdBtn");
var lgaDropdown = document.getElementById("formationLGA");
var formations = document.getElementById("commands");
var officerQty = document.getElementById("officerQty");
var formationsList = [];
var formationsListIndex = 1;
var totalFormationsPicked = 0;
var edit = false;
var originalSelectionArray = [];
var additionsArray = [];
var removalsArray = [];
const onTableRebuild = new Event("formationsTableRebuild");

addBtn.addEventListener("click", getFormation, false);

if (formationsAllocated != null) {
    edit = true;
    formationsAllocated.forEach(function (value) {
        originalSelectionArray.push(value);
        let allocatedFormation = {};
        allocatedFormation.id = value.FormationId.toString();
        allocatedFormation.name = value.FormationName;
        allocatedFormation.lgaId = value.LGAId;
        allocatedFormation.lga = value.LGAName;
        allocatedFormation.officerQty = value.NumberOfOfficers.toString();
        allocatedFormation.officersProvidedQty = value.NumberOfOfficersAssignedByCommander.toString();
        formationsList.push(allocatedFormation);
        formationsListIndex += 1;
    });
}

function getFormation() {
    if (lgaDropdown.options[lgaDropdown.selectedIndex] == undefined || lgaDropdown.options[lgaDropdown.selectedIndex].value == "" || lgaDropdown.options[lgaDropdown.selectedIndex].value == "0") { alert("You need to select an LGA"); return; }
    if (formations.options[formations.selectedIndex] == undefined || formations.options[formations.selectedIndex].value == "" || formations.options[formations.selectedIndex].value == "0") { alert("You need to select a formation"); return; }
    if (officerQty.value == undefined || officerQty.value == "" || parseInt(officerQty.value) <= 0 || Number.isNaN(parseInt(officerQty.value))) { alert("You need to specify the number of officers"); return; }
    totalFormationsPicked += 1;
    let requestedFormation = {};
    requestedFormation.id = formations.options[formations.selectedIndex].value;
    requestedFormation.name = formations.options[formations.selectedIndex].text;
    requestedFormation.lgaId = lgaDropdown.options[lgaDropdown.selectedIndex].value;
    requestedFormation.lga = lgaDropdown.options[lgaDropdown.selectedIndex].text;
    requestedFormation.officerQty = officerQty.value;
    requestedFormation.officersProvidedQty = "0";
    addFormation(requestedFormation);
}

function addFormation(formation) {
    if (checkIfFormationAlreadyAdded(formation.id) > -1) { alert('Formation has already been added'); totalFormationsPicked -= 1; return; }
    addToAdditions(formation);
    formationsList.push(formation);
    destroyTable();
    buildTable();
    formationsListIndex += 1;
    //empty the input text
    collateAddedFormations();
    collateRemovedFormations();
}

function buildTable() {
    let formationsTable = document.getElementById("formationsTable");
    formationsList.forEach(function (value, index) {
        let originalSelection = getNumberOfAllocatedOfficers(value.id);
        let numberOfAllocatedOffiers = (originalSelection != undefined) ? originalSelection.NumberOfOfficersAssignedByCommander : value.officersProvidedQty;
        let row = formationsTable.insertRow(-1);
        let cell1 = row.insertCell(0).innerHTML = '' + value.lga + '<input type="hidden" value=' + value.lgaId + ' />';
        let cell2 = row.insertCell(1).innerHTML = '' + value.name + '<input type="hidden" name="FormationsSelection[' + index + '].FormationId" value=' + value.id + ' /><input type="hidden" name="FormationsSelection[' + index + '].FormationName" value="' + value.name + '" />';
        let cell3 = row.insertCell(2).innerHTML = '' + value.officerQty + '<input type="hidden" name="FormationsSelection[' + index + '].NumberofOfficers" value=' + value.officerQty + ' />';
        let cell4 = row.insertCell(3).innerHTML = numberOfAllocatedOffiers;
        let cell5 = row.insertCell(4).innerHTML = (numberOfAllocatedOffiers > 0) ? '<button type="button" class="view-officers-toggle esc-' + req + '-' + value.id + '">View Officers</button>' : '';
        let cell6 = row.insertCell(5).innerHTML = "<span class='delete-formation-row' Title='Remove Item' onClick='removeFormation(" + index + ")'>X</span>";
    });
    clearInputs();
    document.dispatchEvent(onTableRebuild);
}

function removeFormation(index) {
    let formationsTable = document.getElementById("formationsTable");
    formationsTable.deleteRow(index + 1);
    var removedItem = formationsList.splice(index, 1);
    addToRemovals(removedItem[0]);
    formationsListIndex -= 1;
    totalFormationsPicked -= 1;
    destroyTable();
    buildTable();
    collateAddedFormations();
    collateRemovedFormations();
}

function destroyTable() {
    let formationsTable = document.getElementById("formationsTable");
    for (let i = 1; i < formationsListIndex; ++i) {
        formationsTable.deleteRow(-1);
    }
}

function clearInputs() {
    document.getElementById("formationLGA").options[0].selected = true;
    $("#commands").empty();
    $("#commands").append("<option value='0'>Select a formation</option>");
    document.getElementById("officerQty").value = "";
}

function checkIfFormationAlreadyAdded(formationId) {
    return formationsList.findIndex((x) => { return x.id === formationId; });
}

function addToAdditions(formation) {
    if (originalSelectionArray.findIndex(function (val) { return val.FormationId == formation.id }) == -1) {
        additionsArray.push(formation);
    }

    if (getIndexFromArray(removalsArray, formation) > -1) {
        removalsArray.splice(getIndexFromArray(removalsArray, formation), 1);
    }
}

function addToRemovals(formation) {
    if (originalSelectionArray.findIndex(function (val) { return val.FormationId == formation.id }) > -1) {
        removalsArray.push(formation);
    }

    if (getIndexFromArray(additionsArray, formation) > -1) {
        additionsArray.splice(getIndexFromArray(additionsArray, formation), 1);
    }
}


function getIndexFromArray(arr, formation) {
    return arr.findIndex(function (val) { return val.id == formation.id });
}

function getNumberOfAllocatedOfficers(formationId) {
    return originalSelectionArray.find(function (val) { return val.FormationId == formationId });
}

function collateAddedFormations() {
    $(".addedFormations").remove();
    additionsArray.forEach(function (val, index) {
        $("#formationsTable").append('<input type="hidden" class="addedFormations" name="AddedFormations[' + index + '].FormationId" value=' + val.id + ' /><input type="hidden" class="addedFormations" name="AddedFormations[' + index + '].FormationName" value="' + val.name + '" />');
        $("#formationsTable").append('<input type="hidden" class="addedFormations" name="AddedFormations[' + index + '].NumberofOfficers" value=' + val.officerQty + ' />');
    });
}


function collateRemovedFormations() {
    $(".removedFormations").remove();
    removalsArray.forEach(function (val, index) {
        $("#formationsTable").append('<input type="hidden" class="removedFormations" name="RemovedFormations[' + index + '].FormationId" value=' + val.id + ' /><input type="hidden" class="removedFormations" name="RemovedFormations[' + index + '].FormationName" value="' + val.name + '" />');
        $("#formationsTable").append('<input type="hidden" class="removedFormations" name="RemovedFormations[' + index + '].NumberofOfficers" value=' + val.officerQty + ' />');
    });
}

