$(document).ready(function () {
    $("#formSubmit").on('submit', function (e) {
        var inputValue = $('#revenueHeadData').val();
        var revenueHeadId = $("#revenue-heads option[value='" + inputValue + "']").attr('data-value');
        $('#revenueHeadId').val(revenueHeadId);
    });
});