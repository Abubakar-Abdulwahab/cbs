$(document).ready(function () {
    $('a[name="viewInvoice"]').click(function (e) {
        e.preventDefault();
        window.open(this.href, "cbsinvoice", "width=800,height=800,scrollbars=yes")
    });


    $("#closeErrorFlash").click(function (e) {
        $('#errorMsg').html("");
        $('#errorFlash').hide();
    });
});