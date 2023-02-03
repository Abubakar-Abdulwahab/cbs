var selectedRequests = [];
var selectedRequestListIndex = 1;
var isAccountValidated = false;
var accountBalance = 0;
var token = null;

if (selectedRequest != null && selectedRequest.length > 0) {
    $(selectedRequest).each(function (index, value) {
        addPostbackDataToTable(value);
    });
}

$("#initiatePaymentRequestForm").submit(function (e) {
    e.preventDefault();
    $("#verificationCodeContainer").css("display", "flex");
    getToken();
});

$("#paymentApprovalVerificationSubmitBtn").click(function () {
    if ($("#codeTextBox").val().length === 6 && token != null) {
        validateVerificationCode(token, $("#codeTextBox").val());
    }
});

$("#resendCode").click(function () {
    if (token != null) { resendVerificationCode(token); }
});


function addPostbackDataToTable(postbackUserData) {
    selectedRequests.push(postbackUserData);
    destroyTable();
    buildTable();
    selectedRequestListIndex += 1;
}

function validateAccountNumber(acctNo, bankId) {
    $('#accountNameError').empty();
    $("#acctNo").prop("disabled", true);
    $("#bank").prop("disabled", true);
    $("#accountNameLoader").show();

    var url = `/Admin/Police/Expenditure/Wallet/validate-account-number`;
    var requestData = { "accountNumber": acctNo, "bankId": bankId, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
    $.post(url, requestData, function (data) {
        if (!data.Error) {
            $('#acctName').val(data.ResponseObject);
            isAccountValidated = true;
            $("#retryBtnDiv").removeClass('input-group');
            $(".input-group-btn").css('display', 'none');
        }
        else {
            $('#accountNameError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >' + data.ResponseObject + '</span > ');
            $("#retryBtnDiv").addClass('input-group');
            $(".input-group-btn").css('display', '');
        }

    }).fail(function () {
        $('#accountNameError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000">An error occurred try refreshing the page.</span > ');
    }).always(function () {
        $("#accountNameLoader").hide();
        $("#acctNo").prop("disabled", false);
        $("#bank").prop("disabled", false);
    });
}

function getAccountBalance(walletId) {
    $('#accountBalanceError').empty();
    $("#accountWallet").prop("disabled", true);
    $("#accountBalanceLoader").show();

    var url = `/Admin/Police/Expenditure/Wallet/account-balance`;
    var requestData = { "walletId": walletId, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
    $.post(url, requestData, function (data) {
        if (!data.Error) {

            let balance = parseFloat(data.ResponseObject);
            $('#walletBalance').val(parseFloat(balance.toFixed(2)).toLocaleString('en-US'));
            accountBalance = balance;
        }
        else {
            $('#accountBalanceError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >' + data.ResponseObject + '</span > ');
        }

    }).fail(function () {
        $('#accountNameError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000">An error occurred try refreshing the page.</span > ');
    }).always(function () {
        $("#accountBalanceLoader").hide();
        $("#accountWallet").prop("disabled", false);
    });
}

$("#retryValidateBtn").click(function (e) {
    e.preventDefault();
    canValidateAccountNumber();
})

$("#acctNo").change(function () {

    canValidateAccountNumber();
})

$("#bank").change(function () {

    canValidateAccountNumber();
})

$("#accountWallet").change(function () {

    accountBalance = 0;
    getAccountBalance($("#accountWallet").val());
})

function canValidateAccountNumber() {

    isAccountValidated = false;
    $('#acctName').val('');

    let accountNo = $("#acctNo").val();
    let bank = $("#bank").val();

    if (accountNo != undefined && bank != undefined && accountNo.length == 10 && bank != null && bank != 0) {
        //Validate
        validateAccountNumber(accountNo, bank);
    }
}

$("#addRequest").click(function () {

    //Validate all inputs
    let sourceAccountId = $("#accountWallet").val();
    let sourceAccount = $("#accountWallet :selected").text();
    let beneficiaryName = $("#beneficiaryName").val();
    let expenditureHeadId = $("#expenditureHead").val();
    let expenditureHead = $("#expenditureHead :selected").text();
    let bank = $("#bank :selected").text();
    let bankId = $("#bank").val();
    let amount = $("#amount").val();
    let accountNumber = $("#acctNo").val();
    let accountName = $("#acctName").val();
    let balance = parseFloat(amount.replace(/,/g, ''));

    if (balance <= 0) {
        alert('Amount can not be less than or equal to zero'); return;
    }

    if (accountBalance - balance < 0) {
        alert('Total amount exceeds current balance'); return;
    }

    if (sourceAccountId == 0 || sourceAccountId == null) {
        alert('Please select a source account'); return;
    }

    if (beneficiaryName == "") {
        alert('Please enter a Beneficiary Name'); return;
    }

    if (!isAccountValidated) {
        alert('Please enter an account number/bank'); return;
    }

    if (accountNumber.length != 10) {
        alert('Please enter a valid account number'); return;
    }

    if (bankId == 0 || bankId == null) {
        alert('Please select a bank'); return;
    }

    if (expenditureHeadId == 0 || expenditureHeadId == null) {
        alert('Please select an Expenditure Head'); return;
    }

    if (amount == "") {
        alert('Please enter an amount'); return;
    }

    let requestObj = {};
    requestObj.SelectedBankId = bankId;
    requestObj.Bank = bank;
    requestObj.SelectedWalletId = sourceAccountId;
    requestObj.SelectedWallet = sourceAccount;
    requestObj.SelectedExpenditureHeadId = expenditureHeadId;
    requestObj.SelectedExpenditureHead = expenditureHead;
    requestObj.SelectedExpenditureHead = expenditureHead;
    requestObj.BeneficiaryName = beneficiaryName;
    requestObj.AccountNumber = accountNumber;
    requestObj.AccountName = accountName;
    requestObj.Amount = amount;

    selectedRequests.push(requestObj);

    accountBalance -= balance;
    $('#walletBalance').val(parseFloat(accountBalance.toFixed(2)).toLocaleString('en-US'));

    destroyTable();
    buildTable();
    selectedRequestListIndex += 1;
    clearInputs();
    $("#accountWallet").prop("disabled", true);
});

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
        let cell1 = row.insertCell(0).innerHTML = '' + value.BeneficiaryName + '<input type="hidden"  name="WalletPaymentRequests[' + index + '].SelectedBankId" value=' + value.SelectedBankId + ' /><input type="hidden" name="WalletPaymentRequests[' + index + '].Bank" value="' + value.Bank + '" /><input type="hidden" name="WalletPaymentRequests[' + index + '].SelectedWalletId" value="' + value.SelectedWalletId + '" /><input type="hidden" name="WalletPaymentRequests[' + index + '].SelectedWallet" value="' + value.SelectedWallet + '" /><input type="hidden" name="WalletPaymentRequests[' + index + '].SelectedExpenditureHeadId" value="' + value.SelectedExpenditureHeadId + '" /><input type="hidden" name="WalletPaymentRequests[' + index + '].SelectedExpenditureHead" value="' + value.SelectedExpenditureHead + '" /><input type="hidden" name="WalletPaymentRequests[' + index + '].BeneficiaryName" value="' + value.BeneficiaryName + '" /><input type="hidden" name="WalletPaymentRequests[' + index + '].AccountNumber" value="' + value.AccountNumber + '" /><input type="hidden" name="WalletPaymentRequests[' + index + '].AccountName" value="' + value.AccountName + '" /><input type="hidden" name="WalletPaymentRequests[' + index + '].Amount" value="' + value.Amount + '"/>';
        let cell2 = row.insertCell(1).innerHTML = '' + value.AccountNumber;
        let cell3 = row.insertCell(2).innerHTML = '' + value.Bank;
        let cell4 = row.insertCell(3).innerHTML = '' + value.AccountName;
        let cell5 = row.insertCell(4).innerHTML = '' + value.SelectedExpenditureHead;
        let cell6 = row.insertCell(5).innerHTML = '' + parseFloat(parseFloat(value.Amount).toFixed(2)).toLocaleString('en-US');
        let cell7 = row.insertCell(6).innerHTML = "<span class='delete-user-row' Title='Remove Item' onClick='removeRequest(" + index + ")'>Remove</span>";
    });
    clearInputs();
}

function removeRequest(index) {
    let requestsTable = document.getElementById("requestsTable");

    let requestTableRows = $('table#requestsTable').find('tbody').find('tr');
    let amountValue = $(requestTableRows[index + 1]).find('td:eq(5)').html();
    accountBalance += parseFloat(amountValue.replace(/,/g, ''));
    $('#walletBalance').val(parseFloat(accountBalance.toFixed(2)).toLocaleString('en-US'));

    requestsTable.deleteRow(index + 1);
    selectedRequests.splice(index, 1);
    selectedRequestListIndex -= 1;
    destroyTable();
    buildTable();
    if (index == 0) {
        $("#accountWallet").prop("disabled", false);
    }
}

function clearInputs() {
    $('#accountNameError').empty();
    $('#beneficiaryName').val('');
    $('#acctName').val('');
    $('#acctNo').val('');
    $('#bank').prop('selectedIndex', 0);
    $('#expenditureHead').prop('selectedIndex', 0);
    $('#amount').val('');
    isAccountValidated = false;
    $("#retryValidateBtn").remove();

}


function getToken() {
    $("#paymentApprovalVerificationError").html("");
    $("#paymentApprovalVerificationLoader").show();
    $("#paymentApprovalVerificationSubmitBtn").prop("disabled", true);
    $("#resendCode").attr("disabled", true);
    let url = "/Admin/Police/AccountWalletPaymentApprovalAJAX/Get-Token";
    let data = { "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() }
    $.post(url, data, function (response) {
        if (!response.Error) {
            token = response.ResponseObject;
        } else {
            $("#paymentApprovalVerificationError").html(response.ResponseObject);
        }
    }).always(function () {
        $("#paymentApprovalVerificationLoader").hide();
        $("#paymentApprovalVerificationSubmitBtn").prop("disabled", false);
        $("#resendCode").attr("disabled", false);
    }).fail(function () {
        $("#paymentApprovalVerificationError").html("An error occurred try refreshing the page.");
    });
}


function validateVerificationCode(token, code) {
    $("#paymentApprovalVerificationError").html("");
    $("#paymentApprovalVerificationLoader").show();
    $("#paymentApprovalVerificationSubmitBtn").prop("disabled", true);
    $("#resendCode").attr("disabled", true);
    let url = "/Admin/Police/AccountWalletPaymentApprovalAJAX/Validate-Verification-Code";
    let data = { token: token, code: code, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() }
    $.post(url, data, function (response) {
        if (!response.Error) {
            $("#initiatePaymentRequestForm").off("submit");
            $("#initiatePaymentRequestForm").submit();
        } else {
            $("#paymentApprovalVerificationError").html(response.ResponseObject);
        }
    }).always(function () {
        $("#paymentApprovalVerificationLoader").hide();
        $("#paymentApprovalVerificationSubmitBtn").prop("disabled", false);
        $("#resendCode").attr("disabled", false);
    }).fail(function () {
        $("#paymentApprovalVerificationError").html("An error occurred try refreshing the page.");
    });
}


function resendVerificationCode(token) {
    $("#paymentApprovalVerificationError").html("");
    $("#paymentApprovalVerificationLoader").show();
    $("#paymentApprovalVerificationSubmitBtn").prop("disabled", true);
    $("#resendCode").attr("disabled", true);
    let url = "/Admin/Police/AccountWalletPaymentApprovalAJAX/Resend-Verification-Code";
    let data = { token: token, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() }
    $.post(url, data, function (response) {
        if (!response.Error) {
            alert(response.ResponseObject);
        } else {
            $("#paymentApprovalVerificationError").html(response.ResponseObject);
        }
    }).always(function () {
        $("#paymentApprovalVerificationLoader").hide();
        $("#paymentApprovalVerificationSubmitBtn").prop("disabled", false);
        $("#resendCode").attr("disabled", false);
    }).fail(function () {
        $("#paymentApprovalVerificationError").html("An error occurred try refreshing the page.");
    });
}
