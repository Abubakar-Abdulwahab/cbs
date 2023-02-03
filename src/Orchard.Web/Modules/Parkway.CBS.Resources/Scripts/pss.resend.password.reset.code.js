$(document).ready(function () {
    $('#resendCode').click(function (e) {
        $("#resendCode").prop("disabled", true);
        var url = 'x/resend-password-reset-verification-code';
        var requestData = { "token": token , "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
        $.post(url, requestData, function (data) {
            if (!data.Error) {
                $('#infMsg').html(data.ResponseObject);
                $('#infoDiv').show();
            } else {
                if (data.ResponseObject.Refresh !== undefined) {
                    $('#infMsg').html(data.ResponseObject.Message);
                    $('#infoDiv').show();
                    clearInterval(2);
                    window.location.reload();
                } else {
                    $('#errorMsg').html(data.ResponseObject);
                    $('#errDiv').show();
                }
            }
        }).fail(function () {

        }).always(function () {
            $("#resendCode").prop("disabled", false);
        });
    });

});