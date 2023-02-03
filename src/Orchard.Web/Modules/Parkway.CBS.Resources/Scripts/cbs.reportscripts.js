$(document).ready(function () {
    $('#pickyDate').datepicker({
        format: "dd/mm/yyyy"
    });

    $('#pickyDate2').datepicker({
        format: "dd/mm/yyyy"
    });

    $("#mda_selected").on("change", function () {
        
        if (($("#mda_selected :selected").val() != "") && ($("#mda_selected :selected").val() != "0")) {

            var url = 'X/InvoiceReportRevenueHeadsPerMDA';
            var verToken = $("input[name=__RequestVerificationToken]").val();
            var requestData = { "sId": $("#mda_selected :selected").val(), "__RequestVerificationToken": verToken, "objectToken": Token.defaultValue };
            $("#errorFlash").hide();
            $("#textloader").show();
           
            $.post(url, requestData, function (data) {
                if (!data.Error) {
                    var options = '<option value="0">All Revenue Heads</option>';
                    if (data.ResponseObject.length > 0) {
                        for (i = 0; i < data.ResponseObject.length; i++) {
                            options += '<option value="' + data.ResponseObject[i].Id + '">' + data.ResponseObject[i].Name + '</option>';
                        }
                    }
                    $("#revenuehead_selected").empty().append(options);
                    $("#textloader").hide();

                } else {
                    $("#errorFlash").show();
                    $("#textloader").hide();
                    $("#errorFlash").text(data.ResponseObject);
                    $("#revenuehead_selected").empty().append('<option value="" disabled selected>No Revenue Heads Found</option>');
                    $("#revenuehead_selected").prop('required', true);
                }
            });
        }
        else if ($("#mda_selected :selected").val() == "0") {
            $("#revenuehead_selected").empty().append('<option value="" disabled selected></option>');
            $("#revenuehead_selected").removeAttr('required');
        }
    });


    $('a[name="viewInvoice"]').click(function (e) {
        e.preventDefault();
        window.open(this.href, "cbsinvoice", "width=800,height=800,scrollbars=yes");
    });

    $('a[name="viewReceipt"]').click(function (e) {
        e.preventDefault();
        window.open(this.href, "CBS Receipt", "width=800,height=800,scrollbars=yes");
    });

    $('a[name="viewCertificate"]').click(function (e) {
        e.preventDefault();
        window.open(this.href, "TCC Certificate", "width=800,height=800,scrollbars=yes");
    });

});