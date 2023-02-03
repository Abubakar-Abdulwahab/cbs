$(document).ready(function () {

    $('.taxPayer').click(function (event) {
        event.defaultPrevented;
        var id = $(this).attr("id");
        $("#taxPayerId").val(id);
        $('#confirmForm').get(0).submit();
    });

});