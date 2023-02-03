$(document).ready(function () {
    $(".view-officers-toggle").click(function () {
        $("#allocatedOfficersModal").show();
        $("body").css("overflow", "hidden");
        let viewOfficersBtnClassListLength = this.classList.length;
        let viewOfficersParams = this.classList[viewOfficersBtnClassListLength - 1].split("-");
        viewOfficers(viewOfficersParams);
    });

    $("#closeViewOfficersModal").click(function () {
        $("#allocatedOfficersModal").hide();
        $("#allocatedOfficersTableBody").empty();
        $("body").css("overflow", "auto");
    });

    document.addEventListener("formationsTableRebuild", function () {
        $(".view-officers-toggle").click(function () {
            $("#allocatedOfficersModal").show();
            $("body").css("overflow", "hidden");
            let viewOfficersBtnClassListLength = this.classList.length;
            let viewOfficersParams = this.classList[viewOfficersBtnClassListLength - 1].split("-");
            viewOfficers(viewOfficersParams);
        });
    }, false);

    function viewOfficers(viewOfficersParams) {
        $("#searchError").html("");
        $("#allocatedOfficersLoader").show();
        let url = "/Admin/Police/Request/view-allocated-officers";
        let data = { "requestId": viewOfficersParams[1], "commandId": viewOfficersParams[2], "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };

        $.post(url, data, function (response) {
            if (!response.Error) {
                buildAllocatedOfficersTable(response.ResponseObject);
            } else {
                $("#searchError").html(response.ResponseObject);
            }
        }).always(function () {
            $("#allocatedOfficersLoader").hide();
        }).fail(function () {
            $("#allocatedOfficersLoader").hide();
            $("#searchError").html(response.ResponseObject);
        });
    }


    function buildAllocatedOfficersTable(allocatedOfficers) {
        $("#allocatedOfficersTableBody").empty();
        if (allocatedOfficers.length == 0) {
            $("#allocatedOfficersTableBody").append("<tr>");
            $("#allocatedOfficersTableBody").append("<td colspan='5'>Officers have not yet been selected by squadron leader</td>");
            $("#allocatedOfficersTableBody").append("</tr>");
            return;
        }
        allocatedOfficers.forEach(function (value) {
            $("#allocatedOfficersTableBody").append("<tr>");
            $("#allocatedOfficersTableBody").append("<td>" + value.OfficerName + "</td><td>" + value.OfficerRankName + "</td><td>" + value.OfficerCommandName + "</td><td>" + value.OfficerIdentificationNumber + "</td><td>" + value.DateCreated +"</td>");
            $("#allocatedOfficersTableBody").append("</tr>");
        });
    }
});