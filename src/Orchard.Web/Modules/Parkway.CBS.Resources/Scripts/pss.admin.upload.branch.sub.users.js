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
            $("#uploadInfo").html("Change branch sub users file here.");
            $("#uploadImg").hide();
            $("#fileUploadbtn").prop("disabled", false);
        }
    });


    $("#uploadBranchSubUsersForm").submit(function (e) {
        e.preventDefault();
        if ($("#upload")[0].files[0] == undefined) {
            alert("You need to upload a branch sub users file to proceed");
        } else {
            $("#uploadBranchSubUsersForm").off("submit");
            $("#uploadBranchSubUsersForm").submit();
        }
    });

    function changeUploadSectionToDefault() {
        $("#fileName").html("");
        $("#uploadInfo").html("Upload branch sub users file here.");
        $("#uploadImg").show();
        $("#fileUploadbtn").prop("disabled", true);
        $("#upload").val("");
    }
});