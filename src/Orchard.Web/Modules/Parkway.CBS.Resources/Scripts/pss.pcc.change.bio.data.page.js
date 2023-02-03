$(document).ready(function () {
    $("#resultContainer").hide();

    $("#searchForFileNumber").on('submit', function (e) {
        event.preventDefault();
        getFileNumberDetails($('#fileNumber').val());
    });


    function getFileNumberDetails(fileNumber) {
        event.preventDefault();
        $("#resultContainer").hide();
        $("#fileNumberErrorMessage").html("");
        $("#profileloader").show();
        $("#fileNumber").prop("disabled", true);
        var url = 'X/Change-Passport-Photo/Get-File-NumberChange-Passport-Photo';
        var requestData = { "fileNumber": fileNumber, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
        $.post(url, requestData, function (data) {
            if (!data.Error) {
                populateCharacterCertificateDetailsTable(data.ResponseObject);
                $("#resultContainer").show();
                $("#bioDataFileNumber").val(fileNumber);
            } else {
                $("#fileNumberErrorMessage").html(data.ResponseObject);
            }
        }).fail(function () {
            $("#searchResult").html("Search query failed");
            $("#resultContainer").hide();
            emptyPopulatedCharacterCertificateDetailsTable();
            $("#fileNumberErrorMessage").html("Sorry something went wrong while processing your request. Please try again later or contact admin.");
        }).always(function () {
            $("#fileNumber").prop("disabled", false);
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

});