$(document).ready(function () {
    var commandLGAMap = new Map();
    if ($("#state").val() !== "0" && $("#lga").val() !== "0" && $("#commands").val() === "")
    {
        $("#commands").empty();
        $('#searchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" > No command was found in the selected local government area. </span > ');
    }
    $("#state").change(function () { clearCommandList(); $("#commands").append("<option value='0'>All Commands</option>"); });
    $('#lga').change(function () {
        clearCommandList();
        if ($('#lga').val() !== "0") {
            getCommandList($('#lga').val());
        } else {
            $("#commands").append("<option value='0'>All Commands</option>");
        }
    });

    function clearCommandList() {
        $("#commands").empty();
        $('#searchError').empty();
        $("#commandList").val("");
        $("#selectedCommand").val("");
    }

    function buildCommandDropDown(lgaId) {
        var options = "<option value='0'>All Commands</option>";
        $(commandLGAMap.get(lgaId)).each(function () {
            options += '<option value="' + this.Id + '">' + this.Name + ' (' + this.Code + ')</option>';
        });
        $("#commands").append(options);
        $("#profileloader").hide();
        $("#commands").prop("disabled", false);
    }

    function getCommandList(lgaId) {
        $("#commands").prop("disabled", true);
        $("#profileloader").show();
        event.preventDefault();
        var commandList = commandLGAMap.get(lgaId);
        $('#searchError').empty();
        if (commandList === undefined) {
            //do ajax call
            var url = '/Admin/Police/get-commands-in-lga';
            var requestData = { "lgaId": lgaId, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
            $.post(url, requestData, function (data) {
                if (!data.Error) {
                    commandLGAMap.set(lgaId, data.ResponseObject);
                    buildCommandDropDown(lgaId);
                } else {
                    $('#searchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >' + data.ResponseObject + '</span > ');
                }

            }).fail(function () {
                $('#searchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000">An error occurred try refreshing the page.</span > ');
            }).always(function () {
                $("#profileloader").hide();
                $("#commands").prop("disabled", false);
            });

        } else {
            buildCommandDropDown(lgaId);
        }
    }

});