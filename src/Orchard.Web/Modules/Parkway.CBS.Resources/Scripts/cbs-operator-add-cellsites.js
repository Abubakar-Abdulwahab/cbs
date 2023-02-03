$(document).ready(function () {
    $("#searchOperator").on('submit', function (e) {
        //data list value
        var inputValue = $('#operatorData').val();
        var payerId = $('#operators option[value="' + inputValue + '"]').attr('data-value');
        $('#payerId').val(payerId);
        $('#taxCategory').val($('input[name=TaxPayerType]:checked').val());
    });
});