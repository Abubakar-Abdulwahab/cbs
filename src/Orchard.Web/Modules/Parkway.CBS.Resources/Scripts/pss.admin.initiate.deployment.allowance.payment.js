$(document).ready(function () {
    var nowTemp = new Date();
    var now = new Date(nowTemp.getFullYear(), nowTemp.getMonth(), nowTemp.getDate(), 0, 0, 0, 0);

    var startDate = $('#startDate').datepicker({
        onRender: function (date) {
            return date.valueOf() >= now.valueOf() ? 'disabled' : '';
        }
    }).on('changeDate', function (ev) {
        var newDate = new Date(ev.date)
        newDate.setDate(newDate.getDate());
        endDate.setValue(newDate);
        startDate.hide();
        $('#endDate')[0].focus();
    }).data('datepicker');

    var endDate = $('#endDate').datepicker({
        onRender: function (date) {
            return date.valueOf() < startDate.date.valueOf() ? 'disabled' : '';
        }
    }).on('changeDate', function (ev) {
        endDate.hide();

        const diffInMs = ev.date.valueOf() - startDate.date.valueOf();
        const diffInDays = diffInMs / (1000 * 60 * 60 * 24) + 1;

        $('#noOfDayLbl').val(diffInDays);

    }).data('datepicker');

    $('.pickyNoFutureDate').datepicker({
        format: "dd/mm/yyyy",

        onRender: function (date) {
            var nowTemp = new Date();
            var endNow = new Date(nowTemp.getFullYear(), nowTemp.getMonth(), nowTemp.getDate(), 0, 0, 0, 0);
            return date.valueOf() > endNow.valueOf() ? 'disabled' : '';
        }
    });
});

var selectedRequests = [];
var selectedRequestListIndex = 1;
var fetchedAccountNumbers = new Map();
var validatedOfficerAccountNames = new Map();
var validatedOfficerAccountName = "";

if (selectedRequestsPostback != undefined && selectedRequestsPostback != null && $("#accountWalletId").val() != undefined && $("#accountWalletId").val() != null && $("#accountWalletId").val() > 0) {
    selectedRequests = selectedRequestsPostback.map(function (value) {
        selectedRequestListIndex += 1;
        let requestObj = {};
        requestObj.accountNumber = value.AccountNumber;
        requestObj.accountName = value.AccountName;
        requestObj.dayTypeId = value.DayTypeId;
        requestObj.dayTypeName = value.DayTypeName;
        requestObj.commandTypeId = value.CommandTypeId;
        requestObj.commandTypeName = value.CommandTypeName;
        requestObj.startDate = value.StartDate;
        requestObj.endDate = value.EndDate;
        requestObj.bankName = value.BankName;
        requestObj.bankCode = value.BankCode;
        requestObj.numberOfDays = value.Duration;
        requestObj.amount = value.AmountString;
        return requestObj;
    });
    buildTable();
}

$("#accountNumberSearchBtn").click(function () {
    if ($("#officerAccountNumber").val() === "" || $("#officerAccountNumber").val() === undefined) {
        alert("Account number is required");
        return;
    }

    if ($("#officerAccountNumber").val().length !== 10) {
        alert("Account number must be 10 digits");
        return;
    }

    if ($("#officerBank").val() === "" || $("#officerBank").val() === undefined || $("#officerBank").val() === null) {
        alert("Select a bank");
        return;
    }

    if (fetchedAccountNumbers.has($("#officerAccountNumber").val() + "-" + $("#officerBank").val())) {
        $("#accountNameError").html("");
        $("#officerAccountName").val(fetchedAccountNumbers.get($("#officerAccountNumber").val() + "-" + $("#officerBank").val()));
    } else { getPersonnelAccountName($("#officerAccountNumber").val(), $("#officerBank").val()); }

});

$("#invoiceNumber").on("input", function () {
    $("#invoiceNumb").val(this.value);
});

$("#officerAccountNumber").on("input", function () {
    $("#officerAccountName").val("");
});

$("#officerBank").change(function () {
    $("#officerAccountName").val("");
});

$("#addRequest").click(function () {

    //Validate all inputs
    if ($("#accountWallet").val() === 0 || $("#accountWallet").val() === null || $("#accountWallet").val() === undefined) {
        alert('Please select a source account'); return;
    }

    if ($("#officerAccountNumber").val() === "" || $("#officerAccountNumber").val() === undefined || $("#officerAccountNumber").val() === null) {
        alert('Please validate the account number'); return;
    }

    if ($("#officerBank").val() === "" || $("#officerBank").val() === undefined || $("#officerBank").val() === null) {
        alert('Please select a bank'); return;
    }

    if ($("#officerAccountName").val() === "" || $("#officerAccountName").val() === undefined || $("#officerAccountName").val() === null) {
        alert('Please validate the account number'); return;
    }

    if ($("#invoiceNumber").val() === "" || $("#invoiceNumber").val() === undefined || $("#invoiceNumber").val() === null) {
        alert('Please enter an invoice number'); return;
    }

    if ($("#dayType").val() === "" || $("#dayType").val() === undefined || $("#dayType").val() === null) {
        alert('Please select the type of day'); return;
    }

    if ($("#commandType").val() === "" || $("#commandType").val() === undefined || $("#commandType").val() === null) {
        alert('Please select a unit'); return;
    }

    if (checkIfCommandTypeAndDayTypeComboExists($("#commandType").val(), $("#dayType").val()) > -1) {
        alert('Personnel with the same unit and day type has already been allocated'); return;
    }

    if ($("#startDate").val() === "" || $("#startDate").val() === undefined) {
        alert('Please enter the start date'); return;
    }

    if ($("#endDate").val() === "" || $("#endDate").val() === undefined) {
        alert('Please enter the end date'); return;
    }

    computePersonnelAmount($("#startDate").val(), $("#endDate").val(), $("#commandType").val(), $("#dayType").val(), $("#invoiceNumber").val(), $("#accountWallet").val());
});


$("#initiateDeploymentAllowanceReqForm").submit(function (e) {
    e.preventDefault();
    if ($("#invoiceNumber").val() === "" || $("#invoiceNumber").val() === undefined) {
        alert("Invoice Number is required");
        return;
    }

    $("#accountWalletId").val($("#accountWallet").val());
    $("#accountWalletName").val($("#accountWallet :selected").html());

    $("#initiateDeploymentAllowanceReqForm").off("submit");
    $("#initiateDeploymentAllowanceReqForm").submit();
    $("#initiatePaymentBtn").prop("disabled", true);
});

function addCurrentRequestItemToTable(amount) {
    let requestObj = {};
    requestObj.accountNumber = $("#officerAccountNumber").val();
    requestObj.accountName = $("#officerAccountName").val();
    requestObj.dayTypeId = $("#dayType").val();
    requestObj.dayTypeName = $("#dayType :selected").html();
    requestObj.commandTypeId = $("#commandType").val();
    requestObj.commandTypeName = $("#commandType :selected").html();
    requestObj.startDate = $("#startDate").val();
    requestObj.endDate = $("#endDate").val();
    requestObj.bankName = $("#officerBank :selected").html();
    requestObj.bankCode = $("#officerBank").val();
    requestObj.numberOfDays = getNumberOfDays();
    requestObj.amount = amount;
    selectedRequests.push(requestObj);

    destroyTable();
    buildTable();
    selectedRequestListIndex += 1;
    clearInputs();
    $("#accountWallet").prop("disabled", true);
    $("#invoiceNumber").prop("disabled", true);
}

function destroyTable() {
    let requestsTable = document.getElementById("requestsTable");
    for (let i = 1; i < selectedRequestListIndex; ++i) {
        requestsTable.deleteRow(-1);
    }
}

function buildTable() {
    let requestsTable = document.getElementById("requestsTable");
    selectedRequests.forEach(function (value, index) {
        let row = requestsTable.insertRow(-1);
        let cell1 = row.insertCell(0).innerHTML = '' + value.accountName + '<input type="hidden"  name="DeploymentAllowancePaymentRequests[' + index + '].AccountName" value="' + value.accountName + '" />';
        let cell2 = row.insertCell(1).innerHTML = '' + value.commandTypeName + '<input type="hidden"  name="DeploymentAllowancePaymentRequests[' + index + '].CommandTypeName" value="' + value.commandTypeName + '" /><input type="hidden"  name="DeploymentAllowancePaymentRequests[' + index + '].CommandTypeId" value="' + value.commandTypeId + '" />';
        let cell3 = row.insertCell(2).innerHTML = '' + value.bankName + '<input type="hidden"  name="DeploymentAllowancePaymentRequests[' + index + '].BankName" value="' + value.bankName + '" /><input type="hidden"  name="DeploymentAllowancePaymentRequests[' + index + '].BankCode" value="' + value.bankCode + '" />';
        let cell4 = row.insertCell(3).innerHTML = '' + value.accountNumber + '<input type="hidden"  name="DeploymentAllowancePaymentRequests[' + index + '].AccountNumber" value="' + value.accountNumber + '" />';
        let cell5 = row.insertCell(4).innerHTML = '' + value.startDate + '<input type="hidden"  name="DeploymentAllowancePaymentRequests[' + index + '].StartDate" value="' + value.startDate + '" />';
        let cell6 = row.insertCell(5).innerHTML = '' + value.endDate + '<input type="hidden"  name="DeploymentAllowancePaymentRequests[' + index + '].EndDate" value="' + value.endDate + '" />';
        let cell7 = row.insertCell(6).innerHTML = '' + value.dayTypeName + '<input type="hidden"  name="DeploymentAllowancePaymentRequests[' + index + '].DayTypeName" value="' + value.dayTypeName + '" /><input type="hidden"  name="DeploymentAllowancePaymentRequests[' + index + '].DayTypeId" value="' + value.dayTypeId + '" />';
        let cell8 = row.insertCell(7).innerHTML = '' + value.numberOfDays + '<input type="hidden"  name="DeploymentAllowancePaymentRequests[' + index + '].Duration" value="' + value.numberOfDays + '" />';
        let cell9 = row.insertCell(8).innerHTML = '' + value.amount + '<input type="hidden"  name="DeploymentAllowancePaymentRequests[' + index + '].AmountString" value="' + value.amount + '" />';
        let cell10 = row.insertCell(9).innerHTML = "<span class='delete-user-row' Title='Remove Item' onClick='removeRequest(" + index + ")'>Remove</span>";
    });
    clearInputs();
}

function removeRequest(index) {
    let requestsTable = document.getElementById("requestsTable");

    let requestTableRows = $('table#requestsTable').find('tbody').find('tr');

    requestsTable.deleteRow(index + 1);
    selectedRequests.splice(index, 1);
    selectedRequestListIndex -= 1;
    destroyTable();
    buildTable();
    if (selectedRequestListIndex === 1) {
        $("#accountWallet").prop("disabled", false);
        $("#invoiceNumber").prop("disabled", false);
    }
}

function getPersonnelAccountName(accountNumber, bankCode) {
    $("#accountNameError").html("");
    $("#accountNameLoader").show();
    $("#addRequest").prop("disabled", true);
    $("#officerBank").prop("disabled", true);
    $("#officerAccountNumber").prop("disabled", true);
    $("#accountNumberSearchBtn").prop("disabled", true);
    let url = "/Admin/Police/Expenditure/Deployment/Allowance/Payment/get-personnel-account-name";
    let data = { "accountNumber": accountNumber, "bankCode": bankCode, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };

    $.post(url, data, function (response) {
        if (!response.Error) {
            $("#officerAccountName").val(response.ResponseObject);
            fetchedAccountNumbers.set(accountNumber + "-" + bankCode, response.ResponseObject);
        } else {
            $("#accountNameError").html(response.ResponseObject);
        }
    }).always(function () {
        $("#accountNameLoader").hide();
        $("#addRequest").prop("disabled", false);
        $("#officerBank").prop("disabled", false);
        $("#officerAccountNumber").prop("disabled", false);
        $("#accountNumberSearchBtn").prop("disabled", false);
    }).fail(function () {
        $("#accountNameLoader").hide();
        $("#addRequest").prop("disabled", false);
        $("#officerBank").prop("disabled", false);
        $("#officerAccountNumber").prop("disabled", false);
        $("#accountNumberSearchBtn").prop("disabled", false);
        $("#accountNameError").html("Sorry something went wrong while processing your request. Please try again later or contact admin");
    });
}

function computePersonnelAmount(startDate, endDate, commandTypeId, dayTypeId, invoiceNumber, sourceAccountId) {
    let url = "/Admin/Police/Expenditure/Deployment/Allowance/Payment/compute-personnel-amount";
    let data = { "startDate": startDate, "endDate": endDate, "commandTypeId": commandTypeId, "dayTypeId": dayTypeId, "invoiceNumber": invoiceNumber, "sourceAccountId": sourceAccountId, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };

    $("#initiatePaymentBtn").prop("disabled", true);
    $("#accountNumberSearchBtn").prop("disabled", true);
    $("#addRequest").prop("disabled", true);
    $("#addRequestLoader").show();
    $("#paymentRequestItemAmountError").html("");
    $.post(url, data, function (response) {
        if (!response.Error) {
            addCurrentRequestItemToTable(response.ResponseObject);
        } else {
            $("#paymentRequestItemAmountError").html(response.ResponseObject);
        }
    }).always(function () {
        $("#initiatePaymentBtn").prop("disabled", false);
        $("#accountNumberSearchBtn").prop("disabled", false);
        $("#addRequest").prop("disabled", false);
        $("#addRequestLoader").hide();
    }).fail(function () {
        $("#initiatePaymentBtn").prop("disabled", false);
        $("#accountNumberSearchBtn").prop("disabled", false);
        $("#addRequest").prop("disabled", false);
        $("#addRequestLoader").hide();
        $("#paymentRequestItemAmountError").html("Sorry something went wrong while processing your request. Please try again later or contact admin");
    });
}

function checkIfPersonnelAlreadyAdded(accountNumber, bankCode) {
    return selectedRequests.findIndex(function (value) { return value.accountNumber === accountNumber && value.bankCode === bankCode });
}

function checkIfCommandTypeAndDayTypeComboExists(commandTypeId, dayTypeId) {
    return selectedRequests.findIndex(function (value) { return value.commandTypeId === commandTypeId && value.dayTypeId === dayTypeId });
}

function clearInputs() {
    $('#officerName').val('');
    $("#officerBank").val('');
    $("#officerAccountNumber").val('');
    $("#officerAccountName").val('');
    $("#startDate").val('');
    $("#endDate").val('');
    $("#commandType").prop('selectedIndex', 0);
    $("#dayType").prop('selectedIndex', 0);
    $("#noOfDayLbl").val("");
}

function computeNumberOfDays(startDateString, endDateString) {
    let startDateElements = startDateString.split("/");
    let endDateElements = endDateString.split("/");
    let startDate = new Date(startDateElements[2], startDateElements[1], startDateElements[0]);
    let endDate = new Date(endDateElements[2], endDateElements[1], endDateElements[0]);
    if (startDate > Date()) { alert("Start date cannot be a date in the future"); return; }
    if (endDate > Date()) { alert("End date cannot be a date in the future"); return; }

    return ((endDate - startDate) / (1000 * 60 * 60 * 24)) + 1;
}

function getNumberOfDays() {
    return $("#noOfDayLbl").val();
}