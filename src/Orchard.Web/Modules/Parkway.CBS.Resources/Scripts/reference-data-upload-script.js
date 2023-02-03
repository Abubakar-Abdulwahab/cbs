$(document).ready(function () {

    $("#fileForm").on('submit', function (e) {
        $("#fileUploadbtn").prop("disabled", true);
    });

    $("input:file").change(function () {

        var fileName = $(this).val();
        if (fileName.length > 0) {
            $("#fileUploadbtn").prop("disabled", false);
        }
    });


});