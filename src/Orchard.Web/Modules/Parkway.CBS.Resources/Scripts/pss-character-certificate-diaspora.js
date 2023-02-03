$(document).ready(function () {

    $("#reasonForInquiry").change(function () {
        if (reasonsForInquiryMap.get(parseInt($(this).val())).ShowFreeForm) {
            $("#otherReasonsContainer").slideDown("fast", "linear", function () { });
            $("#otherReasons").prop("required", true);
        } else {
            $("#otherReasonsContainer").slideUp("fast", "linear", function () { });
            $("#otherReasons").val("");
            $("#otherReasons").prop("required", false);
        }
    });

    $("#characterCertificateForm").on("submit", function (e) {

        let countryInputValue = $('#countryData').val();
        let countryValueIdentifier = $('#countries option[value="' + countryInputValue + '"]').attr('data-value');
        $("#countryIdentifier").val(countryValueIdentifier);

        let countryOfOriginInputValue = $('#originCountryData').val();
        let countryOfOriginIdentifier = $('#originCountries option[value="' + countryOfOriginInputValue + '"]').attr('data-value');
        $('#originCountryIdentifier').val(countryOfOriginIdentifier);

        let countryOfPassportInputValue = $('#passportCountryData').val();
        let countryOfPassportIdentifier = $('#passportCountries option[value="' + countryOfPassportInputValue + '"]').attr('data-value');
        $('#passportCountryIdentifier').val(countryOfPassportIdentifier);
    });

    $("#uploadPassportPhotograph").change(function (event) {
        if (event.currentTarget.files.length == 0) { $("#passportThumbnail").attr('src', '/media/images/placeholder_image.jpg'); return; }
        $("#passportThumbnail").attr('src', URL.createObjectURL(event.currentTarget.files[0]));
    });

    $("#uploadInternationalPassport").change(function (event) {
        if (event.currentTarget.files.length == 0) {
            $("#passportDatapageThumbnail").hide();
            $("#passportDatapageThumbnail").attr('src', '/media/images/placeholder_image.jpg');
            $("#passportDatapageThumbnail").show();
            return;
        }
        $("#passportDatapageThumbnail").attr('src', URL.createObjectURL(event.currentTarget.files[0]));
    });

    $("#originCountryData").change(function () {
        if (this.value.toLowerCase() == "nigeria") {
            $("#stateOfOriginContainer").show();
            $("#stateOfOrigin").prop("required", true);
        } else {
            $("#stateOfOrigin").prop("selectedIndex", 0);
            $("#stateOfOrigin").prop("required", false);
            $("#stateOfOriginContainer").hide();
        }
    });

    function toggleRequiredPropertyForInternationalPassport(boolVal) {
        $("#passportNumber").attr("required", boolVal);
        $("#placeOfIssuance").attr("required", boolVal);
        $("#dateOfIssuance").attr("required", boolVal);
        $("#uploadInternationalPassport").attr("required", boolVal);
        $("#countryData").attr("required", boolVal);
        $("#passportCountryData").attr("required", boolVal);
        $("#countryData").prop("disabled", !boolVal);
        displayRequiredAsteriskForInternationalPassport(boolVal);
    }

    function displayRequiredAsteriskForInternationalPassport(boolVal) {
        if (boolVal) {
            $('label[for="passportNumber"] .required-sym').show();
            $('label[for="placeOfIssuance"] .required-sym').show();
            $('label[for="dateOfIssuance"] .required-sym').show();
            $('label[for="uploadInternationalPassport"] .required-sym').show();
            $('label[for="destinationCountry"] .required-sym').show();
            $('label[for="passportCountryData"] .required-sym').show();
        } else {
            $('label[for="passportNumber"] .required-sym').hide();
            $('label[for="placeOfIssuance"] .required-sym').hide();
            $('label[for="dateOfIssuance"] .required-sym').hide();
            $('label[for="uploadInternationalPassport"] .required-sym').hide();
            $('label[for="destinationCountry"] .required-sym').hide();
            $('label[for="passportCountryData"] .required-sym').hide();
        }
    }


    $("#proofOfResidence").change(function (event) {
        if (event.currentTarget.files.length === 0)
        {
            $("#proofOfResidenceThumbnail").hide();
            $("#proofOfResidenceThumbnail").attr('src', '/media/images/cloud_upload.svg');
            $("#proofOfResidenceThumbnail").show();
            return;
        }
        $("#proofOfResidenceThumbnail").attr('src', URL.createObjectURL(event.currentTarget.files[0]));
    });

    $("#fingerPrint").change(function (event) {
        if (event.currentTarget.files.length === 0) {
            $("#fingerPrintThumbnail").hide();
            $("#fingerPrintThumbnail").attr('src', '/media/images/cloud_upload.svg');
            $("#fingerPrintThumbnail").show();
            return;
        }
        $("#fingerPrintThumbnail").attr('src', URL.createObjectURL(event.currentTarget.files[0]));
    });

    $(".proof-of-residence-preview-document-embed > .embed-overlay").click(function () {
        $("#documentPreviewEmbed").attr("src", $("#proofOfResidenceThumbnail").attr("src"));
        $("#documentPreviewEmbed").show();
        $(".document-preview-modal-container").show();
    });

    $(".finger-print-preview-document-embed > .embed-overlay").click(function () {
        $("#documentPreviewEmbed").attr("src", $("#fingerPrintThumbnail").attr("src"));
        $("#documentPreviewEmbed").show();
        $(".document-preview-modal-container").show();
    });

});