var departmentsMap = new Map();

if ($("#commandCategory").val() !== "0" && $("#commands").val() === "") {
    $("#commands").empty();
    $('#commandSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000">No command was found for the selected formation level.</span>');
}

$("#commandCategory").change(function (e) {
    debugger;
    if (e.currentTarget.value == "0") { return; }
    getDepartments(e.currentTarget.value);
});


function buildDropDown(dropdownContainerId, dropdownData, defaultElementLabel) {
    $(dropdownContainerId).empty();
    let options = "<option selected disabled value=''>Select a " + defaultElementLabel + "</option>"
    $(dropdownData).each(function (index, val) {
        options += '<option value="' + val.Id + '">' + val.Name + '</option>';
    });

    $(dropdownContainerId).append(options);
}

function getDepartments(commandCategoryId) {

    $("#commandCategory").prop("disabled", true);
    $("#commandLoader").show();
    $('#commandSearchError').empty();

    if (!departmentsMap.has(commandCategoryId.toString())) {
        //do ajax call
        var url = '/Admin/Police/get-commands-by-commandcategory';
        var requestData = { "commandCategoryId": commandCategoryId, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
        $.post(url, requestData, function (data) {
            if (!data.Error) {
                if (data.ResponseObject.length == 0) {
                    $('#commandSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >No command was found in the selected formation level.</span > ');
                    return;
                }
                departmentsMap.set(commandCategoryId.toString(), data.ResponseObject);
              
                buildDropDown("#commands", departmentsMap.get(commandCategoryId.toString()), "Command");
            } else {
                $('#commandSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >' + data.ResponseObject + '</span > ');
            }

        }).fail(function () {
            $('#commandSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000">An error occurred try refreshing the page.</span > ');
        }).always(function () {
            $("#commandLoader").hide();
            $("#commandCategory").prop("disabled", false);

        });

    } else {
        $("#commandLoader").hide();
        buildDropDown("#commands", departmentsMap.get(commandCategoryId.toString()), "Command");
    }
}
