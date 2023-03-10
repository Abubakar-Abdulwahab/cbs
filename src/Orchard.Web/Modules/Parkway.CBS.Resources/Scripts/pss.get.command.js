$(document).ready(function () {
    var commandMap = new Map();
    $('#lga').change(function () {
        clearCommandList();
        getCommandList($('#lga').val());
    });
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

    $("#extractForm").on('submit', function (e) {
        //data list value
        var inputValue = $('#commandList').val();
        var commandIdentifier = $('#commands option[value="' + inputValue + '"]').attr('data-value');
        $('#selectedCommand').val(commandIdentifier);
        return true;
    });


    function getCommandList(lgaId) {
        event.preventDefault();
        $('#searchError').empty();
        //do ajax call
        var url = 'x/get-area-divisional-commands-in-lga';
        var requestData = { "lgaId": lgaId, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
        $.post(url, requestData, function (data) {
            if (!data.Error) {
                commandMap.set(lgaId, data.ResponseObject);
                buildCommandDropDown(lgaId);
            } else {
                $('#searchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >' + data.ResponseObject + '</span > ');
            }

        }).fail(function () {
            $('#searchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000">An error occurred try refreshing the page.</span > ');
        }).always(function () {
        });
    }

    function getCommandListByState(stateId) {
        event.preventDefault();
        $('#searchError').empty();

        //do ajax call
        var url = 'x/get-commands-in-state';
        var requestData = { "stateId": stateId, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
        $.post(url, requestData, function (data) {
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