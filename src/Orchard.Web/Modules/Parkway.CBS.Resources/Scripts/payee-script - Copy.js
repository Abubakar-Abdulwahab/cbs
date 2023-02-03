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

    //$("#nav-home-tab").click(function (e) {
    //    $("#nav-upload-tab").removeClass("active");
    //    $("#nav-home-tab").addClass("active");
    //});

    //$("#nav-upload-tab").click(function (e) {
    //    $("#nav-upload-tab").addClass("active");
    //    $("#nav-home-tab").removeClass("active");
    //});


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
            $("#uploadInfo").html("Change employee schedule file here.");
            $("#uploadImg").hide();
            $("#fileUploadbtn").prop("disabled", false);
        }        
    });

    function insertRow() {
        var tbody = $("#tbody");
        var tr = $('<tr />').appendTo(tbody);
        //DirectAssessmentPayeeLines[0].TaxPayerName
        var td1 = $('<td>' + $("#TaxPayerName").val() + '</td>').appendTo(tr);
        var tdInput1 = $('<input hidden name="DirectAssessmentPayeeLines[' + rowNumber + '].TaxPayerName" value="' + $("#TaxPayerName").val() + '"/>').appendTo(td1);
        //DirectAssessmentPayeeLines[0].PayerId
        var td2 = $('<td>' + $("#PayerId").val() + '</td>').appendTo(tr);
        var tdInput2 = $('<input hidden name="DirectAssessmentPayeeLines[' + rowNumber + '].PayerId" value="' + $("#PayerId").val() + '"/>').appendTo(td2);

        //DirectAssessmentPayeeLines[0].GrossAnnualEarning
       var grossVal = $("#GrossAnnualEarning").val();
       var td3 = $('<td>' + "₦" + new Intl.NumberFormat().format(grossVal) + '</td>').appendTo(tr);
       var tdInput3 = $('<input hidden name="DirectAssessmentPayeeLines[' + rowNumber + '].GrossAnnualEarning" value="' + $("#GrossAnnualEarning").val() + '"/>').appendTo(td3);

        //DirectAssessmentPayeeLines[0].Exemptions
       var td4 = $('<td>' + "₦" + new Intl.NumberFormat().format($("#Exemptions").val()) + '</td>').appendTo(tr);
       var tdInput4 = $('<input hidden name="DirectAssessmentPayeeLines[' + rowNumber + '].Exemptions" value="' + $("#Exemptions").val() + '"/>').appendTo(td4);

        //DirectAssessmentPayeeLines[0].Month
        //DirectAssessmentPayeeLines[0].Year
        //$('#revenueHead_' + rvIndex + ' :selected').text()
       var td5 = $('<td>' + $("#Month :selected").val() + " " + $("#Year :selected").val() + '</td>').appendTo(tr);
       var tdInput5 = $('<input hidden name="DirectAssessmentPayeeLines[' + rowNumber + '].Year" value="' + $("#Year :selected").val() + '"/>').appendTo(td5);
       var tdInput51 = $('<input hidden name="DirectAssessmentPayeeLines[' + rowNumber + '].Month" value="' + $("#Month :selected").val() + '"/>').appendTo(td5);

        //DirectAssessmentPayeeLines[0].LGA
       var td6 = $('<td>' + $("#LGA :selected").text() + " " + $("#Email").val() + '</td>').appendTo(tr);
       var tdInput6 = $('<input hidden name="DirectAssessmentPayeeLines[' + rowNumber + '].LGA" value="' + $("#LGA :selected").val() + '"/>').appendTo(td6);
       var tdInput61 = $('<input hidden name="DirectAssessmentPayeeLines[' + rowNumber + '].Email" value="' + $("#Email").val() + '"/>').appendTo(td6);

        //DirectAssessmentPayeeLines[0].PhoneNumber
        var td7 = $('<td>' + $("#PhoneNumber").val() + '</td>').appendTo(tr);
        var tdInput7 = $('<input hidden name="DirectAssessmentPayeeLines[' + rowNumber + '].PhoneNumber" value="' + $("#PhoneNumber").val() + '"/>').appendTo(td7);

        //DirectAssessmentPayeeLines[0].Email
        var td8 = $('<td>' + $("#Address").val() + '</td>').appendTo(tr);
        var tdInput7 = $('<input hidden name="DirectAssessmentPayeeLines[' + rowNumber + '].Address" value="' + $("#Address").val() + '"/>').appendTo(td8);

        //var td9 = $('<td><div class="dropdown"><img src="/media/images/menu.png" alt="" class="dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><div class="dropdown-menu" aria-labelledby="dropdownMenuButton"><a class="dropdown-item" href="#">Delete</a></div></div></td>').appendTo(tr);
        //clear form fields
        rowNumber++;
        $("#dataFormSubmitBtn").prop("disabled", false);
    }
});