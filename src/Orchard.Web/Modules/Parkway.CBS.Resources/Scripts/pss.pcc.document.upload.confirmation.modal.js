$(document).ready(function () {

    $("#closeConfirmModalBtn").click(function () {
        $(".document-upload-confirmation-modal-container").hide();
    });

    $("#imageUploadCondition").change(function (event) {
        if (this.checked) { $("#proceedConfirmModalBtn").prop("disabled", false); }
        else { $("#proceedConfirmModalBtn").prop("disabled", true); }
    });

    $("form").submit(function (event) {
        event.preventDefault();
        $(".document-upload-confirmation-modal-container").show();
        $("#imageUploaded").attr("src", $("#passportThumbnail").attr("src"));
    });

    $("#proceedConfirmModalBtn").click(function () {
        $("form").off("submit");
        $("form").submit();
        $("#proceedConfirmModalBtn").prop("disabled", true);
    });
});