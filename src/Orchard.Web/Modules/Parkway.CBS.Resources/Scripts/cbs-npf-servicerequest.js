$(document).ready(function () {
    $("#indexForm").on('submit', function (e) {
        var inputValue = $('#serviceData').val();
        var serviceIdentifier = $("#p-services option[value='" + inputValue + "']").attr('data-value');
        $('#serviceIdentifier').val(serviceIdentifier);
    });
});