$(document).ready(function () {
    $("#resultContainer").hide();

    $("#searchForFileNumber").on('submit', function (e) {
        event.preventDefault();
        getFileNumberDetails($('#InvoiceNumber').val());
        //var inputValue = $('#commandList').val();
        //var commandIdentifier = $('#commands option[value="' + inputValue + '"]').attr('data-value');
        //$('#selectedCommand').val(commandIdentifier);
        //return true;
    });


    function getFileNumberDetails(fileNumber) {
        event.preventDefault();
        $("#resultContainer").hide();
        $("#fileNumberErrorMessage").html("");
        $("#profileloader").show();
        $("#InvoiceNumber").prop("disabled", true);
        var url = 'X/Change-Passport-Photo/Get-File-NumberChange-Passport-Photo';
        var requestData = { "fileNumber": fileNumber, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
        $.post(url, requestData, function (data) {
            if (!data.Error) {
                populateCharacterCertificateDetailsTable(data.ResponseObject);
                $("#resultContainer").show();
                $("#passPortFileNumber").val(fileNumber);
            } else {
                $("#fileNumberErrorMessage").html(data.ResponseObject);
            }
        }).fail(function () {
            $("#searchResult").html("Search query failed");
            $("#resultContainer").hide();
            emptyPopulatedCharacterCertificateDetailsTable();
            $("#fileNumberErrorMessage").html("Sorry something went wrong while processing your request. Please try again later or contact admin.");
        }).always(function () {
            $("#InvoiceNumber").prop("disabled", false);
            $("#profileloader").hide();
        });
    }


    function populateCharacterCertificateDetailsTable(data) {
        $("#name").html(data.CustomerName);
        $("#extReason").html(data.ReasonForInquiry);
        $("#dateOfBirth").html(data.DateOfBirth);
        $("#placeOfBirth").html(data.PlaceOfBirth);
        $("#destCountry").html(data.DestinationCountry);
        $("#countryOfPassport").html(data.CountryOfPassport);
        $("#passportNumber").html(data.PassportNumber);
        $("#placeOfIssuance").html(data.PlaceOfIssuance);
        $("#dateOfIssuance").html(data.DateOfIssuanceString);
        if (data.PreviouslyConvicted) { $("#prevConvicted").html("YES"); } else { $("#prevConvicted").html("NO"); }
    }


    function emptyPopulatedCharacterCertificateDetailsTable() {
        $("#name").html("");
        $("#extReason").html("");
        $("#dateOfBirth").html("");
        $("#placeOfBirth").html("");
        $("#destCountry").html("");
        $("#countryOfPassport").html("");
        $("#passportNumber").html("");
        $("#placeOfIssuance").html("");
        $("#dateOfIssuance").html("");
        $("#prevConvicted").html("");
    }

   
    //function clearCommandList() {
    //    $("#commandList").val('');
    //    $("#commands").empty();
    //}

    //function buildCommandDropDown(id) {
    //    var options = "";
    //    $(commandMap.get(id)).each(function () {
    //        options += '<option data-value="' + this.Id + '" value="' + this.Name + '" ></option>';
    //    });
    //    $("#commands").append(options);
    //}

    //$("#extractForm").on('submit', function (e) {
    //    //data list value
    //    var inputValue = $('#commandList').val();
    //    var commandIdentifier = $('#commands option[value="' + inputValue + '"]').attr('data-value');
    //    $('#selectedCommand').val(commandIdentifier);
    //    return true;
    //});


    //function getCommandList(lgaId) {
    //    event.preventDefault();
    //    $('#searchError').empty();
    //    //do ajax call
    //    var url = 'x/get-area-divisional-commands-in-lga';
    //    var requestData = { "lgaId": lgaId, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
    //    $.post(url, requestData, function (data) {
    //        if (!data.Error) {
    //            commandMap.set(lgaId, data.ResponseObject);
    //            buildCommandDropDown(lgaId);
    //        } else {
    //            $('#searchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >' + data.ResponseObject + '</span > ');
    //        }

    //    }).fail(function () {
    //        $('#searchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000">An error occurred try refreshing the page.</span > ');
    //    }).always(function () {
    //    });
    //}

    //function getCommandListByState(stateId) {
    //    event.preventDefault();
    //    $('#searchError').empty();

    //    //do ajax call
    //    var url = 'x/get-commands-in-state';
    //    var requestData = { "stateId": stateId, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
    //    $.post(url, requestData, function (data) {
    //        if (!data.Error) {
    //            commandMap.set(stateId, data.ResponseObject);
    //            buildCommandDropDown(stateId);
    //        } else {
    //            $('#searchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >' + data.ResponseObject + '</span > ');
    //        }

    //    }).fail(function () {
    //        $('#searchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000">An error occurred try refreshing the page.</span > ');
    //    }).always(function () {
    //    });
    //}


});