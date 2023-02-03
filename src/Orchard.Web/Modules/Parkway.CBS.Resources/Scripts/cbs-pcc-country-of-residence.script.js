$(document).ready(function () {

    $("#residenceCountryData").change(function () {
        $('#residenceCountryIdentifier').val($('#residenceCountries option[value="' + $('#residenceCountryData').val() + '"]').attr('data-value'));
    });
});