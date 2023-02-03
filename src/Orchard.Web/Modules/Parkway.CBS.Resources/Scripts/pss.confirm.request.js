$(document).ready(function () {
    $("#genInvBtn").prop("disabled", true);
    $("#terms").change(function () {
        if (this.checked && $("#disclaimer").is(':checked')) {
            $("#genInvBtn").prop("disabled", false);
        } else {
            $("#genInvBtn").prop("disabled", true);
        }
    });

    $("#disclaimer").change(function () {
        if (this.checked && $("#terms").is(':checked')) {
            $("#genInvBtn").prop("disabled", false);
        } else {
            $("#genInvBtn").prop("disabled", true);
        }
    });

});