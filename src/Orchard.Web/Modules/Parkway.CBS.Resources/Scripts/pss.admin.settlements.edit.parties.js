var addBtn = document.getElementById("addFeePartyBtn");
var settlementFeeParties = document.getElementById("feePartyDropdown");
var feePartyPercentage = document.getElementById("feePartyPercentage");
var feePartySplittingAdapter = document.getElementById("selectedAdapter");
var hasAdditionalSplits = document.getElementById("hasAdditionalSplits");
var selectedFeeParties = [];
var feePartyListIndex = 1;
var totalFeePartiesPicked = 0;
var edit = false;
var originalSelectionArray = [];
var additionsArray = [];
var removalsArray = [];

addBtn.addEventListener("click", getFeeParty, false);

$("#settlementEditFeePartyForm").on("submit", function (e) {
    let feePartiesAllocatedPercentage = parseFloat(toTwoDecimalPlaces(checkIfSettlementCompletelyAllocated()));
    if (feePartiesAllocatedPercentage > 0) {
        e.preventDefault();
        alert("Fee Parties have not been completely allocated, there's still " + toTwoDecimalPlaces(feePartiesAllocatedPercentage) + "% of the settlement left");
    }
});

if (settlementPartiesAllocated != null) {
    edit = true;
    settlementPartiesAllocated.forEach(function (value) {
        originalSelectionArray.push(value);
        let allocatedFeeParty = {};
        allocatedFeeParty.id = value.FeePartyId.toString();
        allocatedFeeParty.name = value.SettlementFeePartyName;
        allocatedFeeParty.percentage = toTwoDecimalPlaces(value.DeductionValue);
        allocatedFeeParty.adapterId = value.AdapterId.toString();
        allocatedFeeParty.adapter = value.AdapterName;
        allocatedFeeParty.hasAdditionalSplits = value.HasAdditionalSplits;
        selectedFeeParties.push(allocatedFeeParty);
        feePartyListIndex += 1;
    });
}

function getFeeParty() {
    if (settlementFeeParties.options[settlementFeeParties.selectedIndex] == undefined || settlementFeeParties.options[settlementFeeParties.selectedIndex].value == "") { alert("You need to select a fee party"); return; }
    if (feePartyPercentage.value == undefined || feePartyPercentage.value == "" || parseFloat(toTwoDecimalPlaces(feePartyPercentage.value)) <= 0 || Number.isNaN(parseFloat(feePartyPercentage.value))) { alert("You need to specify the fee party's percentage of the settlement"); return; }
    if (!checkIfAddedPercentageIsValid(toTwoDecimalPlaces(feePartyPercentage.value))) { alert("Percentage specified is above what's available"); return; }
    if ((hasAdditionalSplits.checked && feePartySplittingAdapter.options[feePartySplittingAdapter.selectedIndex] == undefined) || (hasAdditionalSplits.checked && feePartySplittingAdapter.options[feePartySplittingAdapter.selectedIndex].value == "")) { alert("You need to select a splitting adapter"); return; }
    totalFeePartiesPicked += 1;
    let requestedFeeParty = {};
    requestedFeeParty.id = settlementFeeParties.options[settlementFeeParties.selectedIndex].value;
    requestedFeeParty.name = settlementFeeParties.options[settlementFeeParties.selectedIndex].text;
    requestedFeeParty.percentage = toTwoDecimalPlaces(feePartyPercentage.value);
    requestedFeeParty.adapterId = (hasAdditionalSplits.checked) ? feePartySplittingAdapter.options[feePartySplittingAdapter.selectedIndex].value : "";
    requestedFeeParty.adapter = (hasAdditionalSplits.checked) ? feePartySplittingAdapter.options[feePartySplittingAdapter.selectedIndex].text : "";
    requestedFeeParty.hasAdditionalSplits = hasAdditionalSplits.checked;
    addSettlementFeeParty(requestedFeeParty);
}


function addSettlementFeeParty(feeParty) {
    if (checkIfFeePartyAlreadyAdded(feeParty.id) > -1) { alert('Fee Party has already been added'); totalFeePartiesPicked -= 1; return; }
    addToAdditions(feeParty);
    selectedFeeParties.push(feeParty);
    destroyTable();
    buildTable();
    feePartyListIndex += 1;
    //empty the input text
    collateAddedFeeParties();
    collateRemovedFeeParties();
}


function buildTable() {
    let feePartyTable = document.getElementById("feePartiesTable");
    selectedFeeParties.forEach(function (value, index) {
        let row = feePartyTable.insertRow(-1);
        let cell1 = row.insertCell(0).innerHTML = '' + value.name + '<input type="hidden" name="SelectedSettlementFeeParties[' + index + '].FeePartyId" value=' + value.id + ' /><input type="hidden" name="SelectedSettlementFeeParties[' + index + '].SettlementFeePartyName" value="' + value.name + '" />';
        let cell2 = row.insertCell(1).innerHTML = '' + value.percentage + '%<input type="hidden" name="SelectedSettlementFeeParties[' + index + '].DeductionValue" value=' + value.percentage + ' />';
        let cell3 = row.insertCell(2).innerHTML = '' + (value.adapter != null) ? value.adapter : '' + '<input type="hidden" name="SelectedSettlementFeeParties[' + index + '].AdapterId" value=' + value.adapterId + ' /><input type="hidden" name="SelectedSettlementFeeParties[' + index + '].HasAdditionalSplits" value=' + value.hasAdditionalSplits + ' />';
        let cell4 = row.insertCell(3).innerHTML = "<span class='delete-fee-party-btn' Title='Remove Item' onClick='removeFeeParty(" + index + ")'>Remove</span>";
    });
    clearInputs();
}

function removeFeeParty(index) {
    let feePartyTable = document.getElementById("feePartiesTable");
    feePartyTable.deleteRow(index + 1);
    var removedItem = selectedFeeParties.splice(index, 1);
    addToRemovals(removedItem[0]);
    feePartyListIndex -= 1;
    totalFeePartiesPicked -= 1;
    destroyTable();
    buildTable();
    collateAddedFeeParties();
    collateRemovedFeeParties();
}

function destroyTable() {
    let feePartyTable = document.getElementById("feePartiesTable");
    for (let i = 1; i < feePartyListIndex; ++i) {
        feePartyTable.deleteRow(-1);
    }
}

function clearInputs() {
    document.getElementById("feePartyDropdown").options[0].selected = true;
    document.getElementById("feePartyPercentage").value = 0;
    document.getElementById("selectedAdapter").options[0].selected = true;
    document.getElementById("hasAdditionalSplits").checked = false;
}

function checkIfFeePartyAlreadyAdded(feePartyId) {
    return selectedFeeParties.findIndex((x) => { return x.id === feePartyId; });
}

function addToAdditions(feeParty) {
    if (originalSelectionArray.findIndex(function (val) { return val.FeePartyId.toString() == feeParty.id && val.AdapterId.toString() == feeParty.adapterId && parseFloat(val.DeductionValue) == parseFloat(feeParty.percentage) }) == -1) {
        additionsArray.push(feeParty);
    }

    if (getIndexFromArray(removalsArray, feeParty) > -1) {
        removalsArray.splice(getIndexFromArray(removalsArray, feeParty), 1);
    }
}

function addToRemovals(feeParty) {
    if (originalSelectionArray.findIndex(function (val) { return val.FeePartyId.toString() == feeParty.id }) > -1) {
        removalsArray.push(feeParty);
    }

    if (getIndexFromArray(additionsArray, feeParty) > -1) {
        additionsArray.splice(getIndexFromArray(additionsArray, feeParty), 1);
    }
}


function getIndexFromArray(arr, feeParty) {
    return arr.findIndex(function (val) { return val.id == feeParty.id });
}


function checkIfSettlementCompletelyAllocated() {
    let percentageTotal = parseFloat(0);
    selectedFeeParties.forEach(function (value) {
        let percentageInFloat = parseFloat(value.percentage);
        percentageTotal += percentageInFloat;
    });
    return parseFloat(toTwoDecimalPlaces("100")) - parseFloat(toTwoDecimalPlaces(percentageTotal));
}

function checkIfAddedPercentageIsValid(percentage) {
    let availablePercentage = toTwoDecimalPlaces(checkIfSettlementCompletelyAllocated());
    if (parseFloat(percentage) <= parseFloat(availablePercentage))
    { return true; }
    return false;
}

function collateAddedFeeParties() {
    $(".addedFeeParties").remove();
    additionsArray.forEach(function (val, index) {
        $("#feePartiesTable").append('<input type="hidden" class="addedFeeParties" name="AddedSettlementFeeParties[' + index + '].FeePartyId" value=' + val.id + ' /><input type="hidden" class="addedFeeParties" name="AddedSettlementFeeParties[' + index + '].SettlementFeePartyName" value="' + val.name + '" />');
        $("#feePartiesTable").append('<input type="hidden" class="addedFeeParties" name="AddedSettlementFeeParties[' + index + '].DeductionValue" value=' + val.percentage + ' />');
        $("#feePartiesTable").append('<input type="hidden" class="addedFeeParties" name="AddedSettlementFeeParties[' + index + '].AdapterId" value=' + val.adapterId + ' /><input type="hidden" class="addedFeeParties" name="AddedSettlementFeeParties[' + index + '].HasAdditionalSplits" value=' + val.hasAdditionalSplits + ' />');
    });
}


function collateRemovedFeeParties() {
    $(".removedFeeParties").remove();
    removalsArray.forEach(function (val, index) {
        $("#feePartiesTable").append('<input type="hidden" class="removedFeeParties" name="RemovedSettlementFeeParties[' + index + '].FeePartyId" value=' + val.id + ' /><input type="hidden" class="removedFeeParties" name="RemovedSettlementFeeParties[' + index + '].SettlementFeePartyName" value="' + val.name + '" />');
        $("#feePartiesTable").append('<input type="hidden" class="removedFeeParties" name="RemovedSettlementFeeParties[' + index + '].DeductionValue" value=' + val.percentage + ' />');
        $("#feePartiesTable").append('<input type="hidden" class="removedFeeParties" name="RemovedSettlementFeeParties[' + index + '].AdapterId" value=' + val.adapterId + ' /><input type="hidden" class="removedFeeParties" name="RemovedSettlementFeeParties[' + index + '].HasAdditionalSplits" value=' + val.hasAdditionalSplits + ' />');
    });
}


function toTwoDecimalPlaces(number) { return parseFloat(number).toFixed(2); }