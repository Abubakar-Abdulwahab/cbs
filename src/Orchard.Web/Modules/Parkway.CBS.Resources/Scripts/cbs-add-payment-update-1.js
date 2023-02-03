$(document).ready(function () {


    $("#addPaymentForm").on('submit', function (event) {
        event.preventDefault();
        //get payment channel
        if ($("#PaymentChannel").val() != null && $("#PaymentChannel").val().length > 0) {
            $("#mpaymentMethod").html($("#PaymentChannel :selected").text());
        }
        //get amount paid
        //new Intl.NumberFormat().format($("#AmountPaid").val()
        $("#mamount").html(new Intl.NumberFormat().format($("#AmountPaid").val()));
        //get payment date
        $("#mpaymentdate").html($("#datepicker5").val());
        //get payment ref
        $("#mref").html($("#PaymentReference").val());
        $("#confirmPayment").modal('show');
    });


    $("#confirmed").click(function () {
        $('#addPaymentForm').get(0).submit();
    });

    $('a[name="viewInvoice"]').click(function (e) {
        e.preventDefault();
        window.open(this.href, "cbsinvoice", "width=800,height=800,scrollbars=yes");
    });
});