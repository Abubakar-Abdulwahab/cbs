$(document).ready(function () {
    var commandLGAMap = new Map();
    $("#state").change(function () { clearCommandList(); });
    $('#lga').change(function () {
        clearCommandList();
        getCommandList($('#lga').val());
    });

    function clearCommandList() {
        $("#commands").empty();
        $("#commandList").val("");
        $("#selectedCommand").val("");
    }

    function buildCommandDropDown(lgaId) {
        var options = "<option value=''>Select a Command</option>";
        $(commandLGAMap.get(lgaId)).each(function () {
            options += '<option value="' + this.Id + '">' + this.Name + ' (' + this.Code + ')</option>';
        });
        $("#commands").append(options);
        $("#profileloader").hide();
        $("#commands").prop("disabled", false);
    }

    //$("form").on('submit', function (e) {
    //    //data list value
    //    var inputValue = $('#commandList').val();
    //    var commandIdentifier = $('#commands option[value="' + inputValue + '"]').attr('data-value');
    //    $('#selectedCommand').val(commandIdentifier);
    //    return true;
    //});


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