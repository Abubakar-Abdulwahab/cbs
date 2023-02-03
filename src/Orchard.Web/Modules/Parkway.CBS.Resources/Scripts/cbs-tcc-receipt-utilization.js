$(document).ready(function () {
    var ajaxReq = null;
    var ajaxReqTwo = null;
    var ajaxReqThree = null;
    var selectedReceipts = new Map();
    const nairaHtmlEntity = "&#x20A6";
    var currentReceipt = null;



    $("#receiptModalCloseBtn").click(function () {
        if (ajaxReq != null) { ajaxReq.abort(); }
        if (ajaxReqTwo != null) { ajaxReqTwo.abort(); }
        hideErrorText();
        $("#receiptDetailsMessageText").hide();
        $("#receiptDetailsErrorText").hide();
        $("#receiptNumberLoader").hide();
        $("#receiptDetailsLoader").hide();
        disableReceiptNumberFields(false);
        $("#applyReceiptBtn").prop("disabled", true);
        $("#receiptDetailsSection").hide();
        disableFormSubmit(false);
    });

    $("#generateInvoiceBtn").click(function () {
        getOutstandingAmount(batchRef);
    });

    $("#generateInvoiceModalModalCloseBtn").click(function () {
        if (ajaxReqThree != null) { ajaxReqThree.abort(); }
    });

    $("#receiptModalToggle").click(function () {
        disableFormSubmit(true);
        $("#receiptNumber").val("");
    });

    $("#receiptNumberSubmitBtn").click(function (e) {
        hideErrorText();
        $("#receiptDetailsMessageText").hide();
        if ($("#receiptNumber").val().length >= 12) {
            $("#receiptNumber").val($("#receiptNumber").val().toUpperCase());
            let receiptNumber = $("#receiptNumber").val();
            disableReceiptNumberFields(true);
            clearReceiptDetails();
            $("#receiptNumberLoader").show();
            if (checkIfReceiptHasBeenAdded(receiptNumber)) {
                $("#receiptNumberLoader").hide();
                disableReceiptNumberFields(false);
                displayErrorText("Receipt has already been applied to this schedule.");
                $("#receiptDetailsSection").hide();
            } else {
                fetchReceipt(receiptNumber);
            }
        } else {
            displayErrorText("Receipt number must be more than 12 characters.");
        }
    });

    $("#applyReceiptBtn").click(function () {
        applyReceipt();
    });


    function addReceiptToTable(receiptObj) {
        let receiptRow = "<tr><td>PKWY-0000002</td><td>Not Utilized</td><td>&#8358;10,000,000.00</td><td>&#8358;0.00</td><td>&#8358;10,000,000.00</td></tr>";
        $("#receiptTbody").append(receiptRow);
    }

    function clearReceiptTable() {
        $("#receiptTbody").empty();
        let emptyItem = "<tr><td colspan = '5' >No receipts applied</td></tr>";
        $("#receiptTbody").append(emptyItem);
        $("#receiptProceedBtn").hide();
    }

    function buildReceiptTable() {
        $("#receiptTbody").empty();
        let tableRow = "";
        selectedReceipts.forEach(function () {
            tableRow += "<tr><td>PKWY-0000002</td><td>Not Utilized</td><td>&#8358;10,000,000.00</td><td>&#8358;0.00</td><td>&#8358;10,000,000.00</td></tr>";
        });
        $("#receiptTbody").append(tableRow);
    }

    function fetchReceipt(receiptNumber) {

        let url = "/c/x/get-paye-receipt";
        let data = { "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val(), "receiptNumber": receiptNumber };

        ajaxReq = $.post(url, data, function (response) {
            if (!response.Error) {
                $("#receiptNumberLoader").hide();
                populateReceiptDetails(response.ResponseObject);
                currentReceipt = response.ResponseObject;
                //console.log(response.ResponseObject);
                if ($("#receiptDetailsSection").css("display") == "none") {
                    $("#receiptDetailsSection").show();
                }
                //toggleApplyButton(response.ResponseObject.Status);
                if (parseFloat(response.ResponseObject.AvailableAmount) <= 0) {
                    $("#applyReceiptBtn").prop("disabled", true);
                    $("#receiptDetailsMessageText").html("Receipt has been fully utilized.");
                    $("#receiptDetailsMessageText").show();
                } else { $("#applyReceiptBtn").prop("disabled", false); }
                //$("#receiptNumberModal").modal("hide");
                //selectedReceipts.set(response.ResponseObject.Id, response.ResponseObject);
                //addReceiptToTable(response.ResponseObject);
            } else {
                $("#receiptNumberLoader").hide();
                ReceiptDetailsDoesNotExist();
                $("#receiptDetailsSection").hide();
                $("#receiptNumberErrorText").html(response.ResponseObject);
                $("#receiptNumberErrorText").css("display", "block");
            }
        }).always(function () { $("#receiptNumberLoader").hide(); disableReceiptNumberFields(false); });
    }

    function disableReceiptNumberFields(val) {
        $("#receiptNumber").prop("disabled", val);
        $("#receiptNumberSubmitBtn").prop("disabled", val);
    }

    function displayErrorText(val) {
        $("#receiptNumberErrorText").html(val);
        $("#receiptNumberErrorText").css("display", "block");
    }

    function hideErrorText() {
        $("#receiptNumberErrorText").html("");
        $("#receiptNumberErrorText").hide();
    }

    function populateReceiptDetails(receipt) {
        $("#totalAmt").removeClass("lazyColor");
        $("#totalAmt").html("" + nairaHtmlEntity + " " + receipt.TotalAmount+"");
        $("#availAmt").removeClass("lazyColor");
        $("#availAmt").html("" + nairaHtmlEntity + " " + receipt.AvailableAmount+"");
        $("#utilizedAmt").removeClass("lazyColor");
        $("#utilizedAmt").html("" + nairaHtmlEntity + " " + receipt.UtilizedAmount+"");
        $("#receiptNumberHeader").removeClass("lazyColor");
        $("#receiptNumberHeader").html(receipt.ReceiptNumber);
    }

    function clearReceiptDetails() {
        $("#totalAmt").html("");
        $("#totalAmt").addClass("lazyColor");
        $("#availAmt").html("");
        $("#availAmt").addClass("lazyColor");
        $("#utilizedAmt").html("");
        $("#utilizedAmt").addClass("lazyColor");
        $("#receiptNumberHeader").html("");
        $("#receiptNumberHeader").addClass("lazyColor");
    }

    function ReceiptDetailsDoesNotExist() {
        $("#totalAmt").removeClass("lazyColor");
        $("#totalAmt").html("...");
        $("#availAmt").removeClass("lazyColor");
        $("#availAmt").html("...");
        $("#utilizedAmt").removeClass("lazyColor");
        $("#utilizedAmt").html("...");
        $("#receiptNumberHeader").removeClass("lazyColor");
        $("#receiptNumberHeader").html("...");
        $("#applyReceiptBtn").prop("disabled", true);
    }

    function applyReceipt() {
        $("#receiptDetailsErrorText").html("");
        $("#receiptDetailsErrorText").hide();
        disableReceiptNumberFields(true);
        $("#applyReceiptBtn").prop("disabled",true);
        if (currentReceipt != null && batchRef.length > 0) {
            $("#receiptDetailsLoader").show();
            let url = "/c/x/apply-paye-receipt";
            let data = { "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val(), "receiptNumber": currentReceipt.ReceiptNumber, "batchRef": batchRef };

            ajaxReqTwo = $.post(url, data, function (response) {
                if (!response.Error) {
                    location.reload();
                } else {
                    $("#receiptDetailsErrorText").html(response.ResponseObject);
                    $("#receiptDetailsErrorText").show();
                    $("#applyReceiptBtn").prop("disabled", false);
                }
            }).always(function () {
                $("#receiptDetailsLoader").hide();
                disableReceiptNumberFields(false);
            });
        }
    }

    function getOutstandingAmount(batchRef) {
        if (batchRef.length == 0) { return; }
        $("#generateInvoiceSubmitBtn").prop("disabled", true);
        $("generateInvoiceErrorText").html("");
        $("generateInvoiceErrorText").hide();
        $("#generateInvoiceRemark").html("");
        $("#generateInvoiceRemark").addClass("lazyColor");
        let url = "/c/x/get-outstanding-amount";
        let data = { "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val(), "batchRef": batchRef };
        ajaxReqThree = $.post(url, data, function (response) {
            if (!response.Error) {
                if (response.ResponseObject.Completed) { location.reload(); }
                $("#generateInvoiceRemark").html("BY CLICKING PROCEED YOU WILL BE GENERATING AN INVOICE FOR THE OUTSTANDING AMOUNT ON THE SCHEDULE (" + nairaHtmlEntity + " " + response.ResponseObject.Amount + ")");
                $("#generateInvoiceSubmitBtn").prop("disabled", false);
            } else {
                $("generateInvoiceErrorText").html(response.ResponseObject);
                $("generateInvoiceErrorText").show();
            }
        }).always(function () { $("#generateInvoiceRemark").removeClass("lazyColor"); });
    }

    function addReceipt(receipt) {
        selectedReceipts.set(receipt.Id, receipt);
        addReceiptToTable(receipt);
    }

    function checkIfReceiptHasBeenAdded(receiptNumber) {
        selectedReceipts.forEach((val, key) => { if (val.ReceiptNumber == receiptNumber) { return true; } });
        return false;
    }

    function disableFormSubmit(val) {
        if (val) { $("#receiptProceedBtn").prop("disabled", val); }
        else { $("#receiptProceedBtn").prop("disabled", val); }
    }

    function toggleApplyButton(val) {
        switch (val) {
            case statusNone:
                $("#applyReceiptBtn").prop("disabled", false);
                break;
            case statusUnutilized:
                $("#applyReceiptBtn").prop("disabled", false);
                break;
            case statusPartlyUtilized:
                $("#applyReceiptBtn").prop("disabled", false);
                break;
            case statusFullyUtilized:
                $("#applyReceiptBtn").prop("disabled", true);
                break;
             default:
                $("#applyReceiptBtn").prop("disabled", false);
        }
    }

    function formatAmount(x) {
        return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
});