$(document).ready(function () {
    var completionStatus = false;
    var stop = false;
    if (batchStatus != batchValidatedStatus && batchStatus != batchCompletedStatus) {
        getBatchUploadCompletionStatus();
    }


    function getBatchUploadCompletionStatus() {
        let url = "/Admin/Police/BranchSubUsers/Upload/Check-Upload-Validation-Completion-Status";
        let data = { "batchToken": batchToken, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };

        $.post(url, data, function (response) {
            if (!response.Error) {
                completionStatus = response.ResponseObject.Completed;
                if (completionStatus) { location.reload(); }
            } else {
                $("#errorMessage").html("<span class='field-validation-error tiny-caption' style = 'color:#ff0000' >" + response.ResponseObject + "</span >");
                $("#errorMessage").show();
                stop = true;
                $("#uploadStatusLoader").hide();
            }
        }).always(function () {
            if (completionStatus != true && stop == false) {
                setTimeout(getBatchUploadCompletionStatus, 3000);
            }
        }).fail(function () {
            $("#errorMessage").html("<span class='field-validation-error tiny-caption' style='color:#ff0000'>An error occurred try refreshing the page.</span >");
            $("#errorMessage").show();
            $("#uploadStatusLoader").hide();
        });
    }
});