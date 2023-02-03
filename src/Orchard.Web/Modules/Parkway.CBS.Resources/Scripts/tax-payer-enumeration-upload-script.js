$(document).ready(function () {
    var rowNumber = 0;

    $("#lineForm").on('submit', function (e) {
        e.preventDefault();
        var form1 = document.forms['lineForm'];
        console.log(form1.checkValidity());
        if (!form1.checkValidity()) {
            return false;
        } else {
            insertRow();
            $('#lineForm')[0].reset();
        }
    });

    $("#dataForm").on('submit', function (e) {
        $("#dataFormSubmitBtn").prop("disabled", true);
    });

    $("#fileForm").on('submit', function (e) {
        $("#fileUploadbtn").prop("disabled", true);
    });

    $("#fileForm").on('submit', function (e) {
        $("#fileUploadbtn").prop("disabled", true);
    });


    $("input:file").change(function () {

        var fileName = $(this).val();
        if (fileName.length > 0) {
            $("#uploadlbl").css({ paddingRight: "0px" });
            $("#uploadlbl").removeClass('uploadlbl');
            //truncate filename
            var n = fileName.lastIndexOf('\\');
            if (n < 0) { n = fileName.lastIndexOf('/'); }
            var str = fileName.substring(n + 1, fileName.length);
            $("#fileName").html(str);
            $("#uploadInfo").html("Change enumeration schedule file here.");
            $("#uploadImg").hide();
            $("#fileUploadbtn").prop("disabled", false);
        }
    });

    function insertRow() {
        var tbody = $("#tbody");
        var tr = $('<tr />').appendTo(tbody);
        //TaxPayerEnumerationLines[0].Name
        var td2 = $('<td>' + $("#name").val() + '</td>').appendTo(tr);
        $('<input hidden name="TaxPayerEnumerationLineItems[' + rowNumber + '].Name" value="' + $("#name").val() + '"/>').appendTo(td2);

        //TaxPayerEnumerationLines[0].PhoneNumber
        var td3 = $('<td>' + $("#phoneNumber").val() + '</td>').appendTo(tr);
        $('<input hidden name="TaxPayerEnumerationLineItems[' + rowNumber + '].PhoneNumber" value="' + $("#phoneNumber").val() + '"/>').appendTo(td3);

        //TaxPayerEnumerationLines[0].Email
        var td4 = $('<td>' + $("#email").val() + '</td>').appendTo(tr);
        $('<input hidden name="TaxPayerEnumerationLineItems[' + rowNumber + '].Email" value="' + $("#email").val() + '"/>').appendTo(td4);

        //TaxPayerEnumerationLines[0].TIN
        var td5 = $('<td>' + $("#tin").val() + '</td>').appendTo(tr);
        $('<input hidden name="TaxPayerEnumerationLineItems[' + rowNumber + '].TIN" value="' + $("#tin").val() + '"/>').appendTo(td5);

        //TaxPayerEnumerationLines[0].LGA
        var td6 = $('<td>' + $("#lga :selected").val() + '</td>').appendTo(tr);
        $('<input hidden name="TaxPayerEnumerationLineItems[' + rowNumber + '].LGA" value="' + $("#lga :selected").val() + '"/>').appendTo(td6);

        //TaxPayerEnumerationLines[0].Address
        var td7 = $('<td>' + $("#address").val() + '</td>').appendTo(tr);
        $('<input hidden name="TaxPayerEnumerationLineItems[' + rowNumber + '].Address" value="' + $("#address").val() + '"/>').appendTo(td7);
        rowNumber++;
        $("#dataFormSubmitBtn").prop("disabled", false);
    }

});