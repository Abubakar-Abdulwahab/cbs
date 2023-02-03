$(document).ready(function () {
    $("input:file").change(function () {
        if ($("#upload")[0].files[0] == undefined) { changeUploadSectionToDefault(); }
        var fileName = $(this).val();
        if (fileName.length > 0) {
            $("#uploadlbl").css({ paddingRight: "0px" });
            $("#uploadlbl").removeClass('uploadlbl');
            //truncate filename
            var n = fileName.lastIndexOf('\\');
            if (n < 0) { n = fileName.lastIndexOf('/'); }
            var str = fileName.substring(n + 1, fileName.length);
            $("#fileName").html(str);
            $("#uploadInfo").html("Change branch officer file here.");
            $("#uploadImg").hide();
            $("#fileUploadbtn").prop("disabled", false);
        }
    });

    function changeUploadSectionToDefault() {
        $("#fileName").html("");
        $("#uploadInfo").html("Upload branch officer file here.");
        $("#uploadImg").show();
        $("#fileUploadbtn").prop("disabled", true);
        $("#upload").val("");
    }
});