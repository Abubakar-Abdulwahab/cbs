$(document).ready(function () {
    $("#fileForm").on('submit', function (e) {
        $("#fileUploadbtn").prop("disabled", true);
    });


    $("input:file").change(function () {

        var fileName = $(this).val();
        if (fileName.length > 0) {
            $("#uploadlbl").css({ paddingRight: "0px" });
            $("#uploadlbl").removeClass('uploadlbl');
            //truncate filename
            var n = fileName.lastIndexOf('\\');
            if (n < 0) { n = fileName.lastIndexOf('/'); }
            var str = fileName.substring(n + 1, fileName.length);
            $("#fileName").html(str);
            $("#uploadInfo").html("Change employee schedule file here.");
            $("#uploadImg").hide();
            $("#fileUploadbtn").prop("disabled", false);
        }
    });
});