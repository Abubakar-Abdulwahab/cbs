$(document).ready(function () {
    $('#accounttr').hide();
    $('#UseTSA').click(function () {
        if ($('#UseTSA').is(':checked')) {
            $("#bank").attr("disabled", "disabled");
            $("#MDA_BankDetails_BankAccountNumber").attr("disabled", "disabled");
            $('#accounttr').hide();
        } else {
            $("#bank").removeAttr("disabled");
            $("#MDA_BankDetails_BankAccountNumber").removeAttr("disabled");
        }
    });

    $("#bank").on("change", function () {
        if ($("#bank").val() != "") {
            if ($("#MDA_BankDetails_BankAccountNumber").val().trim().length > 0) {
                $("#accountName").val("Parkway Projects Limited Account");
                $("#accounttr").show();
            } else {
                $("#accountName").val("");
                $('#accounttr').hide();
            }
        } else {
        }
    });

    $('#MDA_BankDetails_BankAccountNumber').on('change', function () {
        if ($("#MDA_BankDetails_BankAccountNumber").val().trim().length <= 0) {
            $('#accounttr').hide();
        }
    });
});