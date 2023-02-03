$(document).ready(function () {
    let numberOfOfficersUrl = "/Admin/Police/Request/requested-number-of-officers";
    $("#squadOfficerApprovalBtn").click(function (e) {
        e.preventDefault();
        $("#squadfficersLoader").show();
        $("#squadOfficerApprovalBtn").prop("disabled", true);
        $("#searchError").html("");
        let data = { "formationAllocationId": formationAllocationId, "allocationGroupId": allocationGroupId, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };

        $.post(numberOfOfficersUrl, data, function (response) {
            if (!response.Error) {
                let numberOfAssignedOfficers = officerListIndex - 1;
                if (numberOfAssignedOfficers < response.ResponseObject) {
                    if (confirm("You haven't assigned the required number of officers, you're " + (response.ResponseObject - numberOfAssignedOfficers) + " officers short. Are you sure you want to proceed?")) {
                        $("#escortForm").submit();
                    } else {
                    }
                } else {
                    $("#escortForm").submit();
                }
            } else {
                $("#searchError").html(response.ResponseObject);
            }
        }).always(function () {
            $("#squadfficersLoader").hide();
            $("#squadOfficerApprovalBtn").prop("disabled", false);
        }).fail(function () {
            $("#squadfficersLoader").hide();
            $("#squadOfficerApprovalBtn").prop("disabled", false);
            $("#searchError").html(response.ResponseObject);
        });
    });
});