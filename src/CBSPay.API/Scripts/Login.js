$(document).ready(function () {


    $("#btnSearch").click(function () {

        if ($("#InvoiceNumber").val() == "") {
            alert("Invoice Number is required");
        }
        else {

            var token = $("input[name=__RequestVerificationToken]").val();
            var invoiceNumber = $("#InvoiceNumber").val();
            var url = "/Admin/Payment/SearchInvoicePaymentDetail";
            console.log(token);



            $.ajax({
                type: "POST",
                url: "/Admin/Payment/SearchInvoicePaymentDetail",
                data: { InvoiceNumber: invoiceNumber, __RequestVerificationToken: token },
                success: function (result) {
                    $("#InvoiceDetails").html(result);
                }
            });

        }
    })



})