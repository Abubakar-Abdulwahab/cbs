var exemptionCount = 0;
var exemptionTypesDropDown;
function removeRow(id) {
    $(`#exemptionTypeRow-${id}`).remove()
}

function removeSelectedExemptionTypes(id, optionObj) {

    exemptionTypesDropDown = exemptionTypesDropDown.filter(x => {
        return x.Id != id;
    });
    $(`.exemptionOption option[value="${id}"]`).not($(optionObj)).remove();
}

$(document).ready(function () {
    var rowNumber = 0;
    exemptionTypesDropDown = exemptionTypes;
    $('[data-toggle="tooltip"]').tooltip();

    $("#lineForm").on('submit', function (e) {
        e.preventDefault();
        var form1 = document.forms['lineForm'];
        if (!form1.checkValidity()) {
            return false;
        } else {
            insertRow();
            $('#lineForm')[0].reset();

            // Reset Added input fields for exemption type
            $('.exemptionTypeRow').remove();
            exemptionCount = 0;
            exemptionTypesDropDown = exemptionTypes;
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
            $("#uploadInfo").html("Change employee schedule file here.");
            $("#uploadImg").hide();
            $("#fileUploadbtn").prop("disabled", false);
        }
    });


    // On click event handler for adding exemption types
    $("#addExemption").on('click', function () {
        if (exemptionCount < exemptionTypes.length) {
            exemptionCount++;

            let divContainer = document.createElement('div');
            $(divContainer).css("margin-top", "10px").addClass('col-md-6').append(`<label for="" class="title">Exemptions <small style="color:red; font-weight:bolder">*</small></label>`);

            let divRowcontainer = document.createElement('div');
            $(divRowcontainer).addClass('row').css("position", "relative").addClass(`exemptionTypeRow`).attr("Id", "exemptionTypeRow-" + exemptionCount + "");
            $(divRowcontainer).append(`<span class="remove" onClick="removeRow(${exemptionCount})" id="${exemptionCount}" style="position: absolute; font-weight:bold; color: red; background:none; top:20px; right: 0; z-index: 3; cursor:pointer">X</span>`); // TODO fix alignment for the close button

            var select = $('<select>').prop('id', `exe-${exemptionCount}`)
                .prop('class', 'custom-select mb-6 exemptionOption').on('change', function () {
                    let id = $(this).val();
                    exemptionTypesDropDown = exemptionTypesDropDown.filter(x => {
                        return x.Id != id;
                    });
                    $(`.exemptionOption option[value="${id}"]`).not(':selected').remove();
                });;

            select.append('<option selected disabled value="">Select Exemption</option>');
            for (const val of exemptionTypesDropDown) {
                select.append(`<option value="${val.Id}"> ${val.Name} </option>`);
            }
            $(divContainer).append(select);

            $(divContainer).appendTo(divRowcontainer);
            $(`<div style="margin-top: 10px;" class="col-md-6"><label for="" class="title">Amount <small style="color:red; font-weight:bolder">*</small></label>
                        <input type="number" class="form-control mb-6" id="exeAmount-${exemptionCount}" placeholder="Enter" min="1" step="0.01" required></div>`)
                .appendTo(divRowcontainer);
            $(divRowcontainer).insertBefore($("#exemBtnDiv")) //main row
        }
    });

    function insertRow() {
        var tbody = $("#tbody");
        let totalAmount = 0;

        var tr = $('<tr />').appendTo(tbody);
        //DirectAssessmentPayeeLines[0].PayerId
        var td2 = $('<td>' + $("#PayerId").val() + '</td>').appendTo(tr);
        $('<input hidden name="DirectAssessmentPayeeLines[' + rowNumber + '].PayerId" value="' + $("#PayerId").val() + '"/>').appendTo(td2);

        //DirectAssessmentPayeeLines[0].GrossAnnualEarning
        var grossVal = $("#GrossAnnualEarning").val();
        var td3 = $('<td>' + "₦" + new Intl.NumberFormat().format(grossVal) + '</td>').appendTo(tr);
        $('<input hidden name="DirectAssessmentPayeeLines[' + rowNumber + '].GrossAnnualEarning" value="' + $("#GrossAnnualEarning").val() + '"/>').appendTo(td3);

        //Updates the UI with Total Amount
        for (var i = 0; i <= exemptionCount; i++) {
            if (!isNaN(parseInt($(`#exeAmount-${i}`).val())) ) {
                totalAmount += parseInt($(`#exeAmount-${i}`).val());
            }
        }
        var td4 = $('<td>' + '<span id="amounttp-' + rowNumber + '" data-toggle="tooltip" data-html="true" data-placement="top">' + "₦" + new Intl.NumberFormat().format(`${totalAmount}`) + '</span>'+ '</td>').appendTo(tr);

        var exemptionLoopCount = 0;
        var amountTpInfo = "";
        for (var i = 0; i <= exemptionCount; i++) {
            if (!isNaN(parseInt($(`#exeAmount-${i}`).val()))) {

                var exemName = $(`#exe-${i} :selected`).text();
                var exemAmout = "₦" + new Intl.NumberFormat().format($(`#exeAmount-${i}`).val());
                amountTpInfo += `${exemName} : ${exemAmout} \n`

                $('<input hidden name="DirectAssessmentPayeeLines[' + rowNumber + '].PAYEExemptionTypes[' + exemptionLoopCount + '].Amount" value="' + $(`#exeAmount-${i}`).val() + '"/>').appendTo(td4);
                $('<input hidden name="DirectAssessmentPayeeLines[' + rowNumber + '].PAYEExemptionTypes[' + exemptionLoopCount + '].Id" value="' + $(`#exe-${i} :selected`).val() + '"/>').appendTo(td4);
                $('<input hidden name="DirectAssessmentPayeeLines[' + rowNumber + '].PAYEExemptionTypes[' + exemptionLoopCount + '].Name" value="' + exemName + '"/>').appendTo(td4);
                exemptionLoopCount++;
            }
        }
        $(`#amounttp-${rowNumber}`).attr('title', amountTpInfo);

        var td5 = $('<td>' + $("#Month :selected").text() + '</td>').appendTo(tr);
        $('<input hidden name="DirectAssessmentPayeeLines[' + rowNumber + '].Month" value="' + $("#Month :selected").val() + '"/>').appendTo(td5);

        var td6 = $('<td>' + $("#Year :selected").val() + '</td>').appendTo(tr);
        $('<input hidden name="DirectAssessmentPayeeLines[' + rowNumber + '].Year" value="' + $("#Year :selected").val() + '"/>').appendTo(td6);
        rowNumber++;
        $("#dataFormSubmitBtn").prop("disabled", false);
    }

});