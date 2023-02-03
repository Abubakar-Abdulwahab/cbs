$(document).ready(function () {
    if (approvalMessage != null && approvalMessage != undefined && approvalMessage != "") {
        $("#aigApprovalBtn").click(function () {
            if (confirm(approvalMessage)) {
                $("#escortForm").submit();
            }
        });
    }
});