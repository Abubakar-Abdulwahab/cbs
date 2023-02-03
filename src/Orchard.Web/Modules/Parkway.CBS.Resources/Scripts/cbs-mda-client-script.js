$(document).ready(function () {
    var mdaIindex = 0;
    var rvIndex = 0;

    $('#loadingbay').hide();

    var token = $("input[name=__RequestVerificationToken]").val();

    $("#mda_0").on("change", { name: 0 }, getRVControl);

    $("#revenueHead_0").on("change", { name: 0 }, getControl);

    function getControl(event) {
        var controlId = event.data.name;
        if ($("#revenueHead_" + controlId + " :selected").val() != "") {
            var val = $("#revenueHead_" + controlId + " :selected").val();
            var url = 'Collection/RevenueHeads';
            cleanUpDisplay(controlId);
            var requestData = { "sId": val, "__RequestVerificationToken": token };
            $.post(url, requestData, function (data) {
                if (data.length > 0) {
                    var scrollableForm = $('#maindiv');

                    var formGroup = $('<div class="form-group"></div>').appendTo(scrollableForm);
                    var selectopt = $('<select id=revenueHead_' + ++rvIndex + ' name=[' + rvIndex + '].Value class="revenueselect form-control" width= "inherit" />').appendTo(formGroup);

                    $('<option />', { value: "", text: "Select a Subrevenue Head", Selected: true }).appendTo(selectopt);
                    for (var val in data) {
                        $('<option />', { value: data[val].Value, text: data[val].Text }).appendTo(selectopt);
                    }
                    selectopt.appendTo(formGroup);
                    selectopt.on("change", { name: rvIndex }, getControl);
                    selectopt.on("remove", "#revenueHead_" + rvIndex);
                    $("#proceedbtn").attr("disabled", "disabled"); //disable button
                } else {
                    $("#proceedbtn").attr("disabled", false);
                }
            });
        } else {
            $("#proceedbtn").attr("disabled", "disabled");
            cleanUpDisplay(controlId);
        }
    }

    $('input[type=radio][name=TaxPayerType]').change(function (event) {
        console.log(event.target.dataset.value);
        if (event.target.dataset.value == true) {
            //alert("true");
            //show login page
        }
    });


    function getRVControl(event) {
        var controlId = event.data.name;
        if ($("#mda_0 :selected").val() != "") {
            var val = $("#mda_0 :selected").val();
            var url = 'Collection/MDARevenueHeads';
            var taxPayerType = $('input[name=TaxPayerType]').val();

            cleanMDAUpDisplay(controlId);
            var requestData = { "mdaSlug": val, "taxPayerType": taxPayerType, "__RequestVerificationToken": token };
            $.post(url, requestData, function (data) {
                if (data.length > 0) {
                    var scrollableForm = $('.scrollable>');
                    var formGroup = $('<div class="form-group"></div>');
                    var selectopt = $('<select id=revenueHead_0 name=[0].Value class="revenueselect form-control" width= "inherit" class=""form-control />');
                    $('<option />', { value: "", text: "Select a Revenue Head", Selected: true }).appendTo(selectopt);
                    for (var val in data) {
                        $('<option />', { value: data[val].Value, text: data[val].Text }).appendTo(selectopt);
                    }
                    selectopt.appendTo(formGroup);
                    selectopt.on("change", { name: 0 }, getControl);
                    selectopt.on("remove", "#revenueHead_0");
                    formGroup.insertAfter('#mda_0');

                } else {
                    $("#proceedbtn").attr("disabled", false);
                    cleanMDAUpDisplay(controlId)
                }
            });
        } else {
            $("#proceedbtn").attr("disabled", "disabled");
            cleanMDAUpDisplay(controlId)
        }
    }

    function cleanUpDisplay(controlId) {
        if (controlId < rvIndex) {
            for (var i = rvIndex; i > controlId; i--) {
                $("#revenueHead_" + i).remove();
            }
            rvIndex = controlId;
        }
    }

    function cleanMDAUpDisplay(controlId) {
        $("#revenueHead_0").remove();
    }

    $("#indexUserLoginForm").on('submit', function (e) {
        //data list value
        $("#babyloader").show();
        $("#signinbtn").prop("disabled", true);
        var url = "Collection/P/UserLogin";
        var requestData = { "email": $("#signinemail").val(), "password": $("#password").val() };

        $.post(url, requestData, function (data) {
            console.log(data);
            if (!data.Error) {
                location.reload();
            } else {
                var errmsg = data.ResponseObject;
                $("#loginErrorPartial").fadeIn("fast");
                $("#loginErrorPartialMsg").html(errmsg);
                $("#babyloader").hide();
                $("#signinbtn").prop("disabled", false);
            }
        });
        e.preventDefault();

    });

    $("#revenueHeadData").change(function () {
        $("#dropdownproceedbtn").prop("disabled", false);
    });

    $("#dropdownproceedbtn").click(function (e) {
        e.preventDefault();
        if ($('#revenueHeadData').val().length == 0) {
            //$("#dropdownproceedbtn").prop("disabled", true);
            return false;
        }
        else {
            var inputValue = $('#revenueHeadData').val();
            var list = $('#revenue-heads option');
            var matchFound = false;
            list.each(function (index, value) {
                if ($(value).val() == inputValue) {
                    matchFound = true;
                    return;
                }
            });

            if (matchFound) {
                $("#categorySection").hide();
                $("#revName").html(inputValue);
                checkIfLoginIsRequired($('input[name=TaxPayerType]:checked'), inputValue, $("#revenue-heads option[value='" + inputValue + "']").attr('data-value'));
                //toThaLeft(inputValue, $("#revenue-heads option[value='" + inputValue + "']").attr('data-value'));
            }
            //if (matchFound && $('#tinOrRin1').val().length > 0) {
            //    //document.indexForm.submit();
            //    $('#indexForm').submit();
            //}
        }

    });

    $("#indexForm").on('submit', function (e) {
        //data list value
        var inputVal = $('#revenueHeadData').val();
        inputVal = jQuery.trim(inputVal);
        var identifier = $('#revenueHeadIdentifier');
        var optionVal = $("#revenue-heads option[value='" + inputVal + "']").attr('data-value');
        identifier.val(optionVal);
        return true;
    });


    $("#proceedbtn").click(function (e) {
        e.preventDefault();
        //get revenue head name
        if ($('#revenueHead_' + rvIndex + ' :selected').text().length <= 0
            || $('#revenueHead_' + rvIndex).val().length <= 0
            || $('#revenueHead_' + rvIndex + ' :selected').text() == "Select a Revenue Head") { return false; }
        var val = $("#mda_0 :selected").text();

        var parts = val.split(' | ');
        var mdaName = parts[0];
        $('#revenueHeadIdentifier').val("");
        //check if the category requires login
        //var taxPayerType = $('input[name=TaxPayerType]:checked').val();
        //console.log($('input[name=TaxPayerType]:checked'));
        checkIfLoginIsRequired($('input[name=TaxPayerType]:checked'), $('#revenueHead_' + rvIndex + ' :selected').text() + " (mda " + mdaName + ")", $('#revenueHead_' + rvIndex + ' :selected').val());

        //toThaLeft($('#revenueHead_' + rvIndex + ' :selected').text() + " (mda " + mdaName + ")", $('#revenueHead_' + rvIndex + ' :selected').val());
        //$('#indexForm').get(0).submit();
    });


    //$('#revenueHeadData').keypress(function (e) {

    //    if (e.keyCode == 13) {
    //        var inputValue = $('#revenueHeadData').val();
    //        var list = $('#revenue-heads option');
    //        var matchFound = false;
    //        list.each(function (index, value) {
    //            if ($(value).val() == inputValue) {
    //                matchFound = true;
    //                return;
    //            }
    //        });

    //        if (matchFound)
    //        {
    //            //console.log($('input[name=TaxPayerType]:checked'));
    //            checkIfLoginIsRequired($('input[name=TaxPayerType]:checked'), inputValue, $("#revenue-heads option[value='" + inputValue + "']").attr('data-value'));
    //            //toThaLeft(inputValue, $("#revenue-heads option[value='" + inputValue + "']").attr('data-value'));
    //        }
    //        //if (matchFound && $('#tinOrRin1').val().length > 0) {
    //        //    //document.indexForm.submit();
    //        //    $('#indexForm').submit();
    //        //}
    //    }
    //});

    function checkIfLoginIsRequired(taxPayerTypeObj, revenueHeadSelected, revenueHeadValue) {
        var requiresLogin = taxPayerTypeObj["0"].dataset.value;
        if (requiresLogin == "True" && !loggedIn) {
            disableCategory();
            showLoginPage();
        } else {
            toThaLeft(revenueHeadSelected, revenueHeadValue);
        }
    }

    $("#signlink").click(function (e) {
        $("#parentOverlay").show();
        e.preventDefault();
    });

    $("#closeOverlay").click(function (e) {
        $("#parentOverlay").hide();
    });

    function disableCategory() {
        $('#categoryOverlay').css('display', "block");
    }

    function showLoginPage() {
        $("#infoBridge").html("You need to sigin for this tax category&nbsp;");
        $("#parentOverlay").show();
        $("#scrollable2").show();
        //window.open("/Collection/P/UserLogin","windowName", "width=200,height=200,scrollbars=no");
        $("#scrollable").animate({ "left": "90000px" }, "slow");
        $("#scrollable2").animate({ "left": "0px" }, "slow");
    }


    $("#loginbtn").click(function (e) {
        $("#babyloader").show();
        $("#email").prop("required", true);
        $("#password").prop("required", true);
        //HTMLFormElement.reportValidity();
        //var r = this.checkValidity();
        //console.log(r);
        this.checkValidity;
        //$.post(url, requestData, function (data) {
        //    //console.log(data.IsDirectAssessment);
        //    if (!data.HasErrors) {
        //        //console.log(data);
        //        var formDiv = $('#formCtrls');
        //        var counter = 0;
        //        if (data.IsDirectAssessment) {
        //            $('#directAssessmentFileUpload').show();
        //            $('#coyname').prop('required', true);
        //            $('#coyemail').prop('required', true);
        //            $('#coyaddress').prop('required', true);
        //            $('#coyphone').prop('required', true);
        //            $('#rcnumber').prop('required', true);
        //        } else {
        //            $('#directAssessmentFileUpload').hide();
        //            $('#coyname').prop('required', false);
        //            $('#coyemail').prop('required', false);
        //            $('#coyaddress').prop('required', false);
        //            $('#coyphone').prop('required', false);
        //            $('#rcnumber').prop('required', false);
        //        }
        //        for (var val in data.Forms) {
        //            //text area
        //            //console.log(data.Forms[0]);
        //            if (data.Forms[counter].FormControlTypeString == "TextArea") {
        //                $('<p>' + data.Forms[counter].FormLabel + '</p>').appendTo(formDiv);
        //                $('<textarea type="text" id="[' + counter + '].FormValue" name="[' + counter + '].FormValue" class="form-control" placeholder="Enter your ' + data.Forms[counter].FormLabel + '" required rows="3" />').appendTo(formDiv);
        //                $('<input type="hidden" id="[' + counter + '].FormIdentifier" name="[' + counter + '].FormIdentifier" value="' + data.Forms[counter].FormIdentifier + '"/>').appendTo(formDiv);
        //                $('<input type="hidden" id="[' + counter + '].FormLabel" name="[' + counter + '].FormLabel" value="' + data.Forms[counter].FormLabel + '"/>').appendTo(formDiv);
        //            }
        //            if (data.Forms[counter].FormControlTypeString == "TextBox") {
        //                $('<p>' + data.Forms[counter].FormLabel + '</p>').appendTo(formDiv);
        //                $('<input type="text" id="[' + counter + '].FormValue" name="[' + counter + '].FormValue" class="form-control" placeholder="Enter your ' + data.Forms[counter].FormLabel + '" required />').appendTo(formDiv);
        //                $('<input type="hidden" id="[' + counter + '].FormIdentifier" name="[' + counter + '].FormIdentifier" value="' + data.Forms[counter].FormIdentifier + '"/>').appendTo(formDiv);
        //                $('<input type="hidden" id="[' + counter + '].FormLabel" name="[' + counter + '].FormLabel" value="' + data.Forms[counter].FormLabel + '"/>').appendTo(formDiv);
        //            }
        //            counter++;
        //        }
        //        $('#formCtrls').show();

        //    } else {
        //        //do nothing for now
        //    }
        //    $('#loadingbay').hide();
        //    $("#paymentbtn").attr("disabled", false);
        //});

        //$('#indexForm').get(0).submit();
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
            $("#uploadlbl").html(str);
        }
        //else {
        //    $("#uploadlbl").css({ paddingRight: "0px" });
        //}

        //$("#uploadlbl").removeClass('uploadlbl');
        //$("#uploadlbl").html(fileName);
    });

    function toThaLeft(revenueHeadName, revenueHeadValue) {
        $("#scrollable").hide();
        $("#scrollable").animate({ "left": "90000px" }, "slow");
        $("#scrollable1").animate({ "left": "0px" }, "slow");
        $("#revName").val(revenueHeadName);
        $('#revName').prop('title', revenueHeadName);
        //
        var url = 'Collection/GetForms';
        var taxPayerType = $('input[name=TaxPayerType]:checked').val();
        var requestData = { "srevenueHeadId": revenueHeadValue, "taxCategory": taxPayerType, "__RequestVerificationToken": token };
        $("#loadingbay").show();
        $.post(url, requestData, function (data) {
            //console.log(data.IsDirectAssessment);
            if (!data.HasErrors) {
                //console.log(data);
                var formDiv = $('#formCtrls');
                var counter = 0;
                if (data.IsDirectAssessment) {
                    $('#directAssessmentFileUpload').show();
                    $('#coyname').prop('required', true);
                    $('#coyemail').prop('required', true);
                    $('#coyaddress').prop('required', true);
                    $('#coyphone').prop('required', true);
                    $('#rcnumber').prop('required', true);
                } else {
                    $('#directAssessmentFileUpload').hide();
                    $('#coyname').prop('required', false);
                    $('#coyemail').prop('required', false);
                    $('#coyaddress').prop('required', false);
                    $('#coyphone').prop('required', false);
                    $('#rcnumber').prop('required', false);
                }
                for (var val in data.Forms) {
                    //text area
                    //console.log(data.Forms[0]);
                    if (data.Forms[counter].FormControlTypeString == "TextArea") {
                        $('<p>' + data.Forms[counter].FormLabel + '</p>').appendTo(formDiv);
                        $('<textarea type="text" id="[' + counter + '].FormValue" name="[' + counter + '].FormValue" class="form-control" placeholder="Enter your ' + data.Forms[counter].FormLabel + '" required rows="3" />').appendTo(formDiv);
                        $('<input type="hidden" id="[' + counter + '].FormIdentifier" name="[' + counter + '].FormIdentifier" value="' + data.Forms[counter].FormIdentifier + '"/>').appendTo(formDiv);
                        $('<input type="hidden" id="[' + counter + '].FormLabel" name="[' + counter + '].FormLabel" value="' + data.Forms[counter].FormLabel + '"/>').appendTo(formDiv);
                    }
                    if (data.Forms[counter].FormControlTypeString == "TextBox") {
                        $('<p>' + data.Forms[counter].FormLabel + '</p>').appendTo(formDiv);
                        $('<input type="text" id="[' + counter + '].FormValue" name="[' + counter + '].FormValue" class="form-control" placeholder="Enter your ' + data.Forms[counter].FormLabel + '" required />').appendTo(formDiv);
                        $('<input type="hidden" id="[' + counter + '].FormIdentifier" name="[' + counter + '].FormIdentifier" value="' + data.Forms[counter].FormIdentifier + '"/>').appendTo(formDiv);
                        $('<input type="hidden" id="[' + counter + '].FormLabel" name="[' + counter + '].FormLabel" value="' + data.Forms[counter].FormLabel + '"/>').appendTo(formDiv);
                    }
                    counter++;
                }
                $('#formCtrls').show();

            } else {
                //do nothing for now
            }
            $('#loadingbay').hide();
            $("#paymentbtn").attr("disabled", false);
        });
    }


    //$("input").keyup(function () {
    //    var dataList = $("#revenue-heads");
    //    var inputObj = $("#input-data");
    //    console.log(inputObj.val());
    //    var customerType = $('input[name=CustomerType]').val();

    //    var queryValue = $("#input-data").val();
    //    var url = 'Collection/Billables';

    //    var requestData = { "queryText": queryValue, "customerType": customerType, "__RequestVerificationToken": token };
    //    $.post(url, requestData, function (data) {
    //        if (data.length > 0) {
    //            for (var val in data) {
    //                $('<option />', { value: data[val].RevenueHeadId, text: data[val].RevenueHeadName }).appendTo(dataList);
    //            }
    //        } else {

    //        }
    //    });
    //});
});