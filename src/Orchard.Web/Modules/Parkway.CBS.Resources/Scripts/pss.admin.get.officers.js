$(document).ready(function () {

    var commandId = undefined;
    var rankId = undefined;
    var officersMap = new Map();
    var selectedCommand = $("#commands").val();

    $("#officerRank").change(function () {
        $("#secondProfileloader").show();
        clearErrorMessage();
        //if (selectedCommand == "") {
        //    var inputValue = $('#commandList').val();
        //    commandId = $('#commands option[value="' + inputValue + '"]').attr('data-value');
        //} else { commandId = selectedCommand; }

        rankId = $("#officerRank").val();
        if (selectedCommand !== undefined && rankId !== undefined) {
            let officerId = getOfficerId(selectedCommand, rankId);
            if (officersMap.has(officerId)) {
                buildOfficerDropDown(officerId);
            } else {
                getPoliceOfficers(selectedCommand, rankId);
            }
        }
    });


    $("#commands").change(function () {
        $("#secondProfileloader").show();
        clearErrorMessage();
        //var inputValue = $('#commands').val();
        //commandId = $('#commands option[value="' + inputValue + '"]').attr('data-value');
        selectedCommand = $("#commands").val();
        

        rankId = $("#officerRank").val();
        if (selectedCommand !== undefined && rankId !== undefined) {
            let officerId = getOfficerId(selectedCommand, rankId);
            if (officersMap.has(officerId)) {
                buildOfficerDropDown(officerId);
            } else {
                getPoliceOfficers(selectedCommand, rankId);
            }
        }
    });


    function getPoliceOfficers(command, rank) {
        clearOfficerList();
        disableField(true);
        let url = "/Admin/Police/get-police-officers-of-rank-in-command";
        let args = { "commandId": command, "rankId": rank, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };

        $.post(url, args, function (response) {
            if (!response.Error) {
                let officerId = getOfficerId(commandId,rankId);
                officersMap.set(officerId, response.ResponseObject);
                buildOfficerDropDown(officerId);
            } else {
                $('#policeSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >' + response.ResponseObject + '</span > ');
                disableField(false);
            }

        }).fail(function () {
            $('#policeSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000">An error occurred try refreshing the data.</span > ');
            disableField(false);
        }).always(function () {
            $("#secondProfileloader").hide();
        });
    }

    function buildOfficerDropDown(id) {
        clearOfficerList();
        disableField(true);
        var options = "";
        $(officersMap.get(id)).each(function () {
            options += '<option  value="' + this.Id + '" >'+ this.Name +'</option>';
        });
        $("#policeOfficer").append(options);
        $("#secondProfileloader").hide();
        disableField(false);
    }

    function clearOfficerList() {
        $("#policeOfficer").empty();
    }

    function clearErrorMessage() {
        $('#policeSearchError').empty();
    }

    function getOfficerId(commandId,rankId) {
        return commandId + rankId;
    }

    function disableField(disable) {
        $("#policeOfficer").prop("disabled", disable);
    }

});