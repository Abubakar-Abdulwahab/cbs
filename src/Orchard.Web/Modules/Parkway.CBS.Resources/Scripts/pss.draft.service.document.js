$(document).ready(function () {
    var token = $("input[name=__RequestVerificationToken]").val();

    $("#draftDocumentEmbedElement").on("load", function () {
        $("#confirmPreviewDocumentBtn").show();
        $("#infoMessage").show();
        $("#initLoader").hide();
    });

    $("#confirmPreviewDocumentBtn").on("click", function () {
        confirmAdminHasViewedPreview();
    });

    function confirmAdminHasViewedPreview() {
        $(".page-submit-btns button").prop("disabled", true);
        $("#errorMessage").hide();
        $("#confirmPreviewDocumentBtn").prop("disabled", true);
        $("#payerIdLoader").show();

        let url = "/Admin/Police/Request/Approval/View/Draft/Document/Confirm/" + fileRefNumber;
        let data = { "__RequestVerificationToken": token};

        $.post(url, data, function (response) {
            if (!response.Error) {
                $("#confirmPreviewDocumentBtn").prop("disabled", true);
                $("#confirmPreviewDocumentBtn").html(response.ResponseObject);
                $("#infoMessage").hide();
                $(".page-submit-btns button").prop("disabled", false);
            } else {
                //show error text
                $("#confirmPreviewDocumentBtn").prop("disabled", false);
                $("#errorMessage").html(response.ResponseObject);
                $("#errorMessage").show();
            }
        }).always(function () {
            $("#payerIdLoader").hide();
            $(".page-submit-btns button").prop("disabled", false);
        }).fail(function () {
            $("#errorMessage").html(response.ResponseObject);
            $("#errorMessage").show();
            $("#confirmPreviewDocumentBtn").prop("disabled", false);
        });
    }
});