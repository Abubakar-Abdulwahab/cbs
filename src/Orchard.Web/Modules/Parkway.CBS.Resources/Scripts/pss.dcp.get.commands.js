$(document).ready(function () {
    var commandLGAMap = new Map();
    $('#formationLGA').change(function () {
        clearCommandList();
        getCommandList($('#formationLGA').val());
    });

    function clearCommandList() {
        $("#commands").empty();
        $("#commandList").val("");
        $("#selectedCommand").val("");
    }

    function buildCommandDropDown(lgaId) {
        var options = "<option value=''>Select a formation</option>";
        $(commandLGAMap.get(lgaId)).each(function () {
            options += '<option value="' + this.Id + '">' + this.Name + '</option>';
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
            var url = '/Admin/Police/get-area-divisional-commands-for-admin-in-LGA';
            var requestData = { "lgaId": lgaId, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
            $.post(url, requestData, function (data) {
                if (!data.Error) {
                    if (data.ResponseObject.length == 0) { $('#searchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >No commands found</span > '); }
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