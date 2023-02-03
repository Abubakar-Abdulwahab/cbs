$(document).ready(function () {
    var ajaxReq = null;
    var ajaxReqTwo = null;
    var payerIdValidated = false;
    var invoiceNumberValidated = false;
    var whollyExemptedUploadName = null;
    var schoolCertUploadName = null;
    var acctStatementUploadName = null;
    var formPage = 1;
    $("#backBtn").prop("disabled", true);
    makeApplicantInfoFieldsRequired(true);
    makeLLinfoRequired(false);
    makeExemptionInfoRequired(false);
    makeAcctStatementSectionRequired(false);
    if ($("#applicantTIN").val() == "") {
        disableFields();
    } else { payerIdValidated = true; invoiceNumberValidated = true; }
    if (document.getElementById("rented").checked) {
        $(".landLord").prop("disabled", false);
    } else {
        $(".landLord").prop("disabled", true);
    }

    $("#applicantTIN").on("input", function (e) {
        if (e.currentTarget.value.length < 7) {
            makeFieldInvalid("#applicantTIN", "#payerIdInvalid");
            $("#applicantTIN").removeClass("validated");
            $("#payerIdInvalid").hide();
            $("#payerIdValid").hide();
            disableFields();
            payerIdValidated = false;
            return;
        }
        validatePayerId(e.currentTarget.value);
    });

    $("#invoiceNumber").on("input", function (e) {
        if (e.currentTarget.value.length < 10) {
            makeFieldInvalid("#invoiceNumber", "#invoiceNumberInvalid");
            $("#invoiceNumber").removeClass("validated");
            $("#invoiceNumberInvalid").hide();
            invoiceNumberValidated = false;
            return;
        }
        validateDevelopmentLevyInvoice(e.currentTarget.value);
    });

    $("#whollyExemptedForms input:file").change(function () {

        let fileName = $(this).val();
        if (fileName.length > 0) {
            //truncate filename
            let n = fileName.lastIndexOf('\\');
            if (n < 0) { n = fileName.lastIndexOf('/'); }
            whollyExemptedUploadName = fileName.substring(n + 1, fileName.length);
            $("#whollyExemptedFileName").html(whollyExemptedUploadName);
            $("#whollyExemptedForms #uploadInfo").html("Change exemption certificate file here.");
            $("#whollyExemptedForms #uploadInfo").css("color", "#ccc");
            $("#whollyExemptedForms #uploadImg").hide();
        }
    });



    $("#tccform").submit(function (e) {
        e.preventDefault();
        if (formPage === 1) {
            if (!payerIdValidated) { $("#applicantTIN").focus(); return; }
            $("#applicantInfo").fadeOut("fast", "linear", function () {
                $("#backBtn").prop("disabled", false);
                $(window).scrollTop(0);
                $("#llInfo").fadeIn("fast", "linear", function () {
                    if ($("#rented").prop("checked")) { makeRentedFieldsRequired(true); }
                    else { makeRentedFieldsRequired(false); }
                    formPage = 2;
                    makeApplicantInfoFieldsRequired(false);
                    makeLLinfoRequired(true);
                    makeExemptionInfoRequired(false);
                    makeAcctStatementSectionRequired(false);
                });
            })
        } else if (formPage === 2) {
            $("#llInfo").fadeOut("fast", "linear", function () {
                $(window).scrollTop(0);
                $("#exemptionInfo").fadeIn("fast", "linear", function () {
                    formPage = 3;
                    makeApplicantInfoFieldsRequired(false);
                    makeLLinfoRequired(false);
                    makeExemptionInfoRequired(true);
                    makeAcctStatementSectionRequired(false);
                });
            })
        } else if (formPage === 3) {
            if (!invoiceNumberValidated) { $("#invoiceNumber").focus(); return; }
            //check if exemption file has been uploaded
            if ($("#exemptionOptions").val() == "2") { if (whollyExemptedUploadName == null) { return; } }
            else if ($("#exemptionOptions").val() == "4") { if (schoolCertUploadName == null) { return; } }
            $("#exemptionInfo").fadeOut("fast", "linear", function () {
                $(window).scrollTop(0);
                //$("#acctStatementSection").fadeIn("fast", "linear", function () {
                //    formPage = 4;
                //    makeApplicantInfoFieldsRequired(false);
                //    makeLLinfoRequired(false);
                //    makeExemptionInfoRequired(false);
                //});
                $("#submitBtn").html("Submit");
                if (document.getElementById("certify").checked) {
                    $("#submitBtn").prop("disabled", false);
                } else {
                    $("#submitBtn").prop("disabled", true);
                }
                $(window).scrollTop(0);
                populateSummarySection();
                $("#confirmationSection").fadeIn("fast", "linear", function () {
                    formPage = 5;
                    makeApplicantInfoFieldsRequired(false);
                    makeLLinfoRequired(false);
                    makeExemptionInfoRequired(false);
                    makeAcctStatementSectionRequired(false);
                    makeconfirmSectionRequired(true);
                });
            })
        //} else if (formPage === 4) {
        //    if (acctStatementUploadName == null) { return; }
        //    $("#acctStatementSection").fadeOut("fast", "linear", function () {
        //        $("#submitBtn").html("Submit");
        //        if (document.getElementById("certify").checked) {
        //            $("#submitBtn").prop("disabled", false);
        //        } else {
        //            $("#submitBtn").prop("disabled", true);
        //        }
        //        $(window).scrollTop(0);
        //        populateSummarySection();
        //        $("#confirmationSection").fadeIn("fast", "linear", function () {
        //            formPage = 5;
        //            makeApplicantInfoFieldsRequired(false);
        //            makeLLinfoRequired(false);
        //            makeExemptionInfoRequired(false);
        //            makeAcctStatementSectionRequired(false);
        //            makeconfirmSectionRequired(true);
        //        });
        //    })
        } else if (formPage === 5) {
            $("#tccform").off("submit");
            $("#tccform").submit();
        }
    });

    $("#certify").click(function (e) {
        if (formPage === 5) {
            if (this.checked) {
                $("#submitBtn").prop("disabled", false);
            } else {
                $("#submitBtn").prop("disabled", true);
            }
        }
    });


    $("#backBtn").click(function (e) {
        e.preventDefault();
        if (formPage === 3) {
            $("#exemptionInfo").fadeOut("fast", "linear", function () {
                $("#submitBtn").prop("disabled", false);
                $(window).scrollTop(0);
                $("#llInfo").fadeIn("fast", "linear", function () { /*$(".llInfo").css("display","flex");*/
                    formPage = 2;
                    makeApplicantInfoFieldsRequired(false);
                    makeLLinfoRequired(true);
                    makeExemptionInfoRequired(false);
                    makeAcctStatementSectionRequired(false);
                });
            })
        } else if (formPage === 2) {
            $("#llInfo").fadeOut("fast", "linear", function () {
                $("#submitBtn").prop("disabled", false);
                $("#backBtn").prop("disabled", true);
                $(window).scrollTop(0);
                $("#applicantInfo").fadeIn("fast", "linear", function () {
                    formPage = 1;
                    makeApplicantInfoFieldsRequired(true);
                    makeLLinfoRequired(false);
                    makeExemptionInfoRequired(false);
                    makeAcctStatementSectionRequired(false);
                });
            })
        //} else if (formPage === 4) {
        //    $("#acctStatementSection").fadeOut("fast", "linear", function () {
        //        $(window).scrollTop(0);
        //        $("#exemptionInfo").fadeIn("fast", "linear", function () {
        //            formPage = 3;
        //            makeApplicantInfoFieldsRequired(false);
        //            makeLLinfoRequired(false);
        //            makeExemptionInfoRequired(true);
        //            makeAcctStatementSectionRequired(false);
        //        });
        //    })
        } else if (formPage === 5) {
            $("#confirmationSection").fadeOut("fast", "linear", function () {
                $("#submitBtn").prop("disabled", false);
                $("#submitBtn").html("Next");
                $(window).scrollTop(0);
                //$("#acctStatementSection").fadeIn("fast", "linear", function () {
                //    formPage = 4;
                //    makeApplicantInfoFieldsRequired(false);
                //    makeLLinfoRequired(false);
                //    makeExemptionInfoRequired(false);
                //    //makeAcctStatementSectionRequired(true);
                //    makeconfirmSectionRequired(false);
                //});
                $("#exemptionInfo").fadeIn("fast", "linear", function () {
                    formPage = 3;
                    makeApplicantInfoFieldsRequired(false);
                    makeLLinfoRequired(false);
                    makeExemptionInfoRequired(true);
                    makeAcctStatementSectionRequired(false);
                    makeconfirmSectionRequired(false);
                });
            })
        }
    });

    $("#rented").change(function (e) {
        if (this.checked) {
            $(".landLord").prop("disabled", false);
        }
    });

    $("#notRented").change(function (e) {
        if (this.checked) {
            $(".landLord").prop("disabled", true);
            $("#landlordName").val("");
            $("#landlordAddress").val("");
        }
    });

    $("#acctStatementSection input:file").change(function () {
        let fileName = $(this).val();
        if (fileName.length > 0) {
            //truncate filename
            let n = fileName.lastIndexOf('\\');
            if (n < 0) { n = fileName.lastIndexOf('/'); }
            acctStatementUploadName = fileName.substring(n + 1, fileName.length);
            $("#uploadAcctStatementFileName").html(acctStatementUploadName);
            $("#acctStatementSection #uploadInfo").html("Change account statement file here.");
            $("#acctStatementSection #uploadInfo").css("color", "#ccc");
            $("#acctStatementSection #uploadImg").hide();
        }
    });

    $("#whollyExemptedCheckBox").change(function (e) {
        if (this.checked) {
            $("#studentForms").slideUp("fast", "linear");
            $("#unemployedHouseWifeForms").slideUp("fast", "linear");
            $("#whollyExemptedForms").slideDown("fast", "linear");
            $("#whollyExemptedForms").css("display", "flex");
            $("#whollyExemptedForms input:file").change(function () {

                let fileName = $(this).val();
                if (fileName.length > 0) {
                    //truncate filename
                    let n = fileName.lastIndexOf('\\');
                    if (n < 0) { n = fileName.lastIndexOf('/'); }
                    whollyExemptedUploadName = fileName.substring(n + 1, fileName.length);
                    $("#whollyExemptedFileName").html(whollyExemptedUploadName);
                    $("#whollyExemptedForms #uploadInfo").html("Change exemption certificate file here.");
                    $("#whollyExemptedForms #uploadInfo").css("color", "#ccc");
                    $("#whollyExemptedForms #uploadImg").hide();
                }
            });
        } else {
            $("#whollyExemptedForms input:file").off("change");
            $("#whollyExemptedForms").slideUp("fast", "linear");
        }
    });

    $("#unemployedHouseWifeCheckBox").change(function (e) {
        if (this.checked) {
            $("#studentForms").slideUp("fast", "linear");
            $("#whollyExemptedForms").slideUp("fast", "linear");
            $("#unemployedHouseWifeForms").slideDown("fast", "linear");
            $("#unemployedHouseWifeForms").css("display", "flex");
        } else {
            $("#unemployedHouseWifeForms").slideUp("fast", "linear");
        }
    });

    $("#studentCheckBox").change(function (e) {
        if (this.checked) {
            $("#unemployedHouseWifeForms").slideUp("fast", "linear", function () { $("#unemployedHouseWifeForms input:file").off("change"); });
            $("#whollyExemptedForms").slideUp("fast", "linear", function () { $("#whollyExemptedForms input:file").off("change"); });
            $("#studentForms").slideDown("fast", "linear");
            $("#studentForms").css("display", "flex");
            $("#studentForms input:file").change(function () {

                let fileName = $(this).val();
                if (fileName.length > 0) {
                    //truncate filename
                    let n = fileName.lastIndexOf('\\');
                    if (n < 0) { n = fileName.lastIndexOf('/'); }
                    schoolCertUploadName = fileName.substring(n + 1, fileName.length);
                    $("#uploadSchoolCertFileName").html(schoolCertUploadName);
                    $("#studentForms #uploadInfo").html("Change student first leaving certificate file here.");
                    $("#studentForms #uploadInfo").css("color", "#ccc");
                    $("#studentForms #uploadImg").hide();
                }
            });
        } else {
            $("#studentForms input:file").off("change");
            $("#studentForms").slideUp("fast", "linear");
        }
    });


    $("#exemptionOptions").change(function (e) {
        if(this.value === "1"){
            $("#studentForms").slideUp("fast", "linear", function () { $("#studentForms input:file").off("change"); });
            $("#unemployedHouseWifeForms").slideUp("fast", "linear", function () { $("#unemployedHouseWifeForms input:file").off("change"); });
            $("#whollyExemptedForms").slideUp("fast", "linear", function () { $("#whollyExemptedForms input:file").off("change"); });
            schoolCertUploadName = null;
            whollyExemptedUploadName = null;
            }
        else if (this.value === "2") {
            $("#studentForms").slideUp("fast", "linear", function () { $("#studentForms input:file").off("change"); });
            $("#unemployedHouseWifeForms").slideUp("fast", "linear", function () { $("#unemployedHouseWifeForms input:file").off("change"); });
            $("#whollyExemptedForms").slideDown("fast", "linear");
            $("#whollyExemptedForms").css("display", "flex");
            $("#whollyExemptedForms input:file").change(function () {
                schoolCertUploadName = null;
                let fileName = $(this).val();
                if (fileName.length > 0) {
                    //truncate filename
                    let n = fileName.lastIndexOf('\\');
                    if (n < 0) { n = fileName.lastIndexOf('/'); }
                    whollyExemptedUploadName = fileName.substring(n + 1, fileName.length);
                    $("#whollyExemptedFileName").html(whollyExemptedUploadName);
                    $("#whollyExemptedForms #uploadInfo").html("Change exemption certificate file here.");
                    $("#whollyExemptedForms #uploadInfo").css("color", "#ccc");
                    $("#whollyExemptedForms #uploadImg").hide();
                }
            });
        } else if (this.value === "3") {
            $("#studentForms").slideUp("fast", "linear", function () { $("#studentForms input:file").off("change"); });
            $("#whollyExemptedForms").slideUp("fast", "linear", function () { $("#whollyExemptedForms input:file").off("change"); });
            $("#unemployedHouseWifeForms").slideDown("fast", "linear");
            $("#unemployedHouseWifeForms").css("display", "flex");
            schoolCertUploadName = null;
            whollyExemptedUploadName = null;
        } else if (this.value === "4") {
            $("#unemployedHouseWifeForms").slideUp("fast", "linear", function () { });
            $("#whollyExemptedForms").slideUp("fast", "linear");
            $("#studentForms").slideDown("fast", "linear");
            $("#studentForms").css("display", "flex");
            $("#studentForms input:file").change(function () {
                whollyExemptedUploadName = null;
                let fileName = $(this).val();
                if (fileName.length > 0) {
                    //truncate filename
                    let n = fileName.lastIndexOf('\\');
                    if (n < 0) { n = fileName.lastIndexOf('/'); }
                    schoolCertUploadName = fileName.substring(n + 1, fileName.length);
                    $("#uploadSchoolCertFileName").html(schoolCertUploadName);
                    $("#studentForms #uploadInfo").html("Change student first leaving certificate file here.");
                    $("#studentForms #uploadInfo").css("color", "#ccc");
                    $("#studentForms #uploadImg").hide();
                }
            });
        }

    });

function makeApplicantInfoFieldsRequired(val){
    $("#name").attr("required",val);
    $("#applicantPhoneNumber").attr("required",val);
    $("#applicantTIN").attr("required",val);
    $("#address").attr("required",val);
    $("#applicantBusinessAddress").attr("required",val);
}  

function makeLLinfoRequired(val){
    $("#reason").attr("required",val);
}

function makeRentedFieldsRequired(val){
    $("#landlordName").attr("required",val);
    $("#landlordAddress").attr("required",val);
}

function makeExemptionInfoRequired(val){
    $("#invoiceNumber").attr("required",val);
}  

function makeAcctStatementSectionRequired(val){
    $("#certify").attr("required",val);
}

function makeconfirmSectionRequired(val){
    $("#certify").attr("required",val);
}

    function validatePayerId(payerId) {
        if (payerId == undefined) { return; }
        if (ajaxReq != null) { ajaxReq.abort(); }
        $("#payerIdErrorText").hide();
        $("#applicantTIN").removeClass("validated");
        $("#applicantTIN").removeClass("not-validated");
        $("#payerIdValid").hide();
        $("#payerIdInvalid").hide();
        $("#payerIdLoader").show();
        $("#name").attr("disabled",true);
        $("#applicantPhoneNumber").attr("disabled", true);
        $("#address").attr("disabled", true);
        let url = "/c/x/validate-payerid";
        let data = { "payerId": payerId, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
        ajaxReq = $.post(url, data, function (response) {
            if (!response.Error) {
                $("#applicantTIN").addClass("validated");
                $("#payerIdValid").show();
                $("#name").val(response.ResponseObject.Name);
                $("#applicantPhoneNumber").val(response.ResponseObject.PhoneNumber);
                $("#address").html(response.ResponseObject.Address);
                enableFields();
                payerIdValidated = true;
            } else {
                makeFieldInvalid("#applicantTIN", "#payerIdInvalid");
                disableFields();
                payerIdValidated = false;
                $("#payerIdErrorText").html(response.ResponseObject);
                $("#payerIdErrorText").show();
            }
        }).always(function () { $("#payerIdLoader").hide(); });
    }

    function validateDevelopmentLevyInvoice(invoiceNumber) {
        if (invoiceNumber == undefined) { return; }
        if (ajaxReqTwo != null) { ajaxReqTwo.abort(); }
        $("#invoiceNumberErrorText").hide();
        $("#invoiceNumber").removeClass("validated");
        $("#invoiceNumber").removeClass("not-validated");
        $("#invoiceNumberValid").hide();
        $("#invoiceNumberInvalid").hide();
        $("#invoiceNumberLoader").show();
        let url = "/c/x/validate-development-levy-invoice";
        let data = { "invoiceNumber": invoiceNumber, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
        ajaxReqTwo = $.post(url, data, function (response) {
            if (!response.Error) {
                $("#invoiceNumber").addClass("validated");
                $("#invoiceNumberValid").show();
                invoiceNumberValidated = true;
            } else {
                makeFieldInvalid("#invoiceNumber", "#invoiceNumberInvalid");
                invoiceNumberValidated = false;
                $("#invoiceNumberErrorText").html(response.ResponseObject);
                $("#invoiceNumberErrorText").show();
            }
        }).always(function () { $("#invoiceNumberLoader").hide(); });
    }

    function makeFieldInvalid(fieldId, fieldInvalidIconId) {
        $(fieldId).addClass("not-validated");
        $(fieldInvalidIconId).show();
    }

    function disableFields() {
        $("#name").val("");
        $("#applicantPhoneNumber").val("");
        $("#address").html("");
        $("#name").attr("disabled", true);
        $("#applicantPhoneNumber").attr("disabled", true);
        $("#applicantJob").attr("disabled", true);
        $("#applicationYear").attr("disabled", true);
        $("#address").attr("disabled", true);
        $("#applicantBusinessAddress").attr("disabled", true);
        $("#rented").attr("disabled", true);
        $("#notRented").attr("disabled", true);
    }

    function enableFields() {
        $("#name").attr("disabled", false);
        $("#applicantPhoneNumber").attr("disabled", false);
        $("#applicantJob").attr("disabled", false);
        $("#applicationYear").attr("disabled", false);
        $("#address").attr("disabled", false);
        $("#applicantBusinessAddress").attr("disabled", false);
        $("#rented").attr("disabled", false);
        $("#notRented").attr("disabled", false);
    }

    function populateSummarySection() {
        $("#applicantNameAns").html($("#name").val());
        $("#applicantNumberAns").html($("#applicantPhoneNumber").val());
        $("#applicantTinAns").html($("#applicantTIN").val());
        $("#applicantJobAns").html($("#applicantJob").val());
        $("#applicantHomeAddressAns").html($("#address").val());
        $("#applicantOfficeAddressAns").html($("#applicantBusinessAddress").val());
        $("#reasonAns").html($("#reason").val());
        $("#exemptionTypeAns").html($("#exemptionOptions").find(":selected").html());
        $("#invoiceNumberAns").html($("#invoiceNumber").val());
        $("#applicationYearAns").html($("#applicationYear").val());
        if ($("#exemptionOptions").val() == "1") {
            $("#exemptionTypeTil").addClass("til-mid");
            $("#invoiceNumberTil").addClass("til-mid");
            $("#husbandNameAns").parent().hide();
            $("#husbandAddressAns").parent().hide();
            $("#institutionNameAns").parent().hide();
            $("#idCardNumberAns").parent().hide();
            $("#exemptionInfoSummary").show();
        } else if ($("#exemptionOptions").val() == "2") {
            $("#exemptionTypeTil").addClass("til-mid");
            $("#invoiceNumberTil").addClass("til-mid");
            $("#husbandNameAns").parent().hide();
            $("#husbandAddressAns").parent().hide();
            $("#institutionNameAns").parent().hide();
            $("#idCardNumberAns").parent().hide();
            $("#exemptionInfoSummary").show();
        } else if ($("#exemptionOptions").val() == "3") {
            $("#exemptionTypeTil").removeClass("til-mid");
            $("#invoiceNumberTil").removeClass("til-mid");
            $("#exemptionInfoSummary").show();
            $("#husbandNameAns").html($("#husbandName").val());
            $("#husbandNameAns").parent().show();
            $("#husbandAddressAns").html($("#husbandAddress").val());
            $("#husbandAddressAns").parent().show();
            $("#institutionNameAns").parent().hide();
            $("#idCardNumberAns").parent().hide();
        } else if ($("#exemptionOptions").val() == "4") {
            $("#exemptionTypeTil").removeClass("til-mid");
            $("#invoiceNumberTil").removeClass("til-mid");
            $("#exemptionInfoSummary").show();
            $("#husbandNameAns").parent().hide();
            $("#husbandAddressAns").parent().hide();
            $("#institutionNameAns").html($("#nameOfInstitute").val());
            $("#institutionNameAns").parent().show();
            $("#idCardNumberAns").html($("#idCardNumber").val());
            $("#idCardNumberAns").parent().show();
        }

        if ($("#rented").prop("checked")) {
            $("#appartmentTypeAns").html("Rented");
            $("#landlordNameAns").html($("#landlordName").val());
            //$("#landlordNameAns").parent().show();
            $("#landlordBusinessAddressAns").html($("#landlordAddress").val());
            //$("#landlordBusinessAddressAns").parent().show();
        }
        if ($("#notRented").prop("checked")) {
            $("#appartmentTypeAns").html("Not Rented");
            $("#landlordNameAns").html("");
            $("#landlordBusinessAddressAns").html("");
            //$("#landlordNameAns").parent().hide();
            //$("#landlordBusinessAddressAns").parent().hide();
        }
        populateAttachmentSummary();
    }

    function populateAttachmentSummary() {
        if (acctStatementUploadName != null) { $("#uploadAcctStatementSummaryFileName").html(acctStatementUploadName); $("#uploadAcctStatementSummary").show(); }
        else { $("#uploadAcctStatementSummary").hide(); }
        if (schoolCertUploadName != null) { $("#uploadSchoolCertSummaryFileName").html(schoolCertUploadName); $("#uploadSchoolCertSummary").show(); }
        else { $("#uploadSchoolCertSummary").hide(); }
        if (whollyExemptedUploadName != null) { $("#whollyExemptedSummaryFileName").html(whollyExemptedUploadName); $("#uploadExemptionCertSummary").show(); }
        else { $("#uploadExemptionCertSummary").hide(); }
    }

});