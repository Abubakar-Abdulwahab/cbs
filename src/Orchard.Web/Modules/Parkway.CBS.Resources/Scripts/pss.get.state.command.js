$(document).ready(function () {
    var commandMap = new Map();

    $('#state').change(function () {
        clearCommandList();
        getCommandListByState($('#state').val());
    });

    function clearCommandList() {
        $("#commandList").val('');
        $("#commands").empty();
    }

    function buildCommandDropDown(id) {
        var options = "";
        $(commandMap.get(id)).each(function () {
            options += '<option data-value="' + this.Id + '" value="' + this.Name + '" ></option>';
        });
        $("#commands").append(options);
    }

    function getCommandListByState(stateId) {
        event.preventDefault();
        $('#searchError').empty();
        //do ajax call
        var url = 'x/get-request-service-state-commands';
        var requestData = { "stateId": stateId, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
        $.post(url, requestData, function (data) {
            console.log(data);
            if (!data.Error) {
                commandMap.set(stateId, data.ResponseObject);
                buildCommandDropDown(stateId);
            } else {
                $('#searchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >' + data.ResponseObject + '</span > ');
            }
        }).fail(function () {
            $('#searchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000">An error occurred try refreshing the page.</span > ');
        }).always(function () {
        });
    }


});