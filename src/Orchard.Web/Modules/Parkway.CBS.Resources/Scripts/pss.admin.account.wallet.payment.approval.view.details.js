$(document).ready(function () {
    var token = null;
    var isApprove = false;

    $("#approvalBtn").click(function () {
        isApprove = true;
        $("#verificationCodeContainer").css("display", "flex");
        getToken();
    });

    $("#declineBtn").click(function () {
        isApprove = false;
        $("#verificationCodeContainer").css("display", "flex");
        getToken();
    });

    $("#paymentApprovalVerificationSubmitBtn").click(function () {
        if ($("#codeTextBox").val().length === 6 && token != null) {
            validateVerificationCode(token, $("#codeTextBox").val(), isApprove);
        }
    });

    $("#resendCode").click(function () {
        if (token != null) { resendVerificationCode(token); }
    });


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


    function validateVerificationCode(token, code, isApprove) {
        $("#paymentApprovalVerificationError").html("");
        $("#paymentApprovalVerificationLoader").show();
        $("#paymentApprovalVerificationSubmitBtn").prop("disabled", true);
        $("#resendCode").attr("disabled", true);
        let url = "/Admin/Police/AccountWalletPaymentApprovalAJAX/Validate-Verification-Code";
        let data = { token: token, code: code, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() }
        $.post(url, data, function (response) {
            if (!response.Error) {
                if (isApprove) {
                    window.location.href = $("#accountWalletPaymentApprovalURL").val();
                } else { window.location.href = $("#accountWalletPaymentDeclineURL").val(); }
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
            } else{
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
});