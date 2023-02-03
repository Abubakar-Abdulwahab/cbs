$(document).ready(function () {
    $("#closePassportUploadRequirementsModalBtn").click(function () {
        $(".passport-upload-requirements-modal-container").hide();
    });

    $("#passportRequirementsModalToggle").click(function () {
        $(".passport-upload-requirements-modal-container").show();
    });
});