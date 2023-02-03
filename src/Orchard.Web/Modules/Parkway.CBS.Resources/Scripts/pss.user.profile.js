$(document).ready(function () {
    var token = $("input[name=__RequestVerificationToken]").val();
    var validatedIdentificationNumber = false;
    var signUpFullCloseBtn = document.querySelector(".signup-modal-full .modal-close-btn");
    var categoryPermissions = undefined;
    var categoryJsonSetting = undefined;

    categoryPermissions = categoryPermissionsList.find(function (val) { return val.TaxCategoryId == taxPayerType });
    categoryJsonSetting = categoryJsonSettings.find(function (val) { return val.Id == taxPayerType }).Settings;

    performCategorySpecificFieldActions();

    if (!loggedIn) {
        $("#userProfileForm").on('submit', function (event) {
            event.preventDefault();
            validateIdentificationNumber();
        });
    }

    if ($("input[name='HasAlternativeContactInfo']") != undefined) {
        $("input[name='HasAlternativeContactInfo']").change(function (e) {
            if (e.currentTarget.value == "true") {
                $("#AltContactInfoContainer input").prop("readonly", false);
                $("#AltContactInfoContainer").slideDown("fast", "linear", function () {
                    $("#AltContactInfoContainer input").prop("required", true);
                });
            } else {
                $("#AltContactInfoContainer").slideUp("fast", "linear", function () {
                    $("#AltContactInfoContainer input").prop("required", false);
                    $("#AltContactInfoContainer input").prop("readonly", true);
                });
            }
        });
    }

    if (hasAlternativeContactInfo) {
        $("#AltContactInfoContainer input").prop("readonly", false);
        $("#AltContactInfoContainer input").prop("required", true);
    }

    $("#idType").change(function (e) {
        makeFilledFieldsReadonly(false);
        $("#idNumber").val("");
        $("#idNumberErrorText").html("");
        $("#idTypeErrorText").html("");
        let val = $("#idType").val();
        if (!identificationTypesMap.get(parseInt(val)).HasIntegration) {
            $("#upload").prop("required", true);
            $("#userIdentificationUploadDiv").slideDown("fast", "linear", function () { changeUploadSectionToDefault(); });
            let hint = identificationTypesMap.get(parseInt(val)).Hint;
            $("#uploadHint").html(hint);
        } else {
            $("#upload").prop("required", false);
            $("#userIdentificationUploadDiv").slideUp("fast", "linear", function () { changeUploadSectionToDefault(); });
        }
    });

    $("#idNumber").change(function () {
        if (validatedIdentificationNumber) { clearFilledFields(); validatedIdentificationNumber = false; }
        makeFilledFieldsReadonly(false);
        $("#idNumberErrorText").html("");
    });

    if (hasIntegration != null && !hasIntegration) {
        $("#upload").prop("required", true);
        $("#userIdentificationUploadDiv").slideDown("fast", "linear", function () { });
    } else if (hasIntegration != null && hasIntegration) { makeFilledFieldsReadonly(true); validatedIdentificationNumber = true; }

    $("input:file").change(function () {
        if ($("#upload")[0].files[0] == undefined) { changeUploadSectionToDefault(); }
        var fileName = $(this).val();
        if (fileName.length > 0) {
            $("#uploadlbl").css({ paddingRight: "0px" });
            $("#uploadlbl").removeClass('uploadlbl');
            //truncate filename
            var n = fileName.lastIndexOf('\\');
            if (n < 0) { n = fileName.lastIndexOf('/'); }
            var str = fileName.substring(n + 1, fileName.length);
            $("#fileName").html(str);
            $("#uploadInfo").html("Change identification file here.");
            $("#uploadImg").hide();
            $("#fileUploadbtn").prop("disabled", false);
        }
    });

    function changeUploadSectionToDefault() {
        $("#fileName").html("");
        $("#uploadInfo").html("Add an identification file here. (max size 2mb)");
        $("#uploadImg").show();
        $("#fileUploadbtn").prop("disabled", true);
        $("#upload").val("");
    }

    function validateIdentificationNumber() {
        validatedIdentificationNumber = false;
        $("#idNumberLoader").show();
        $("#idNumber").prop("readonly", true);
        $("#idNumberErrorText").html("");
        $("#idTypeErrorText").html("");
        $("#proceedBtn").prop("disabled", true);
        let idNumber = $("#idNumber").val();
        let idType = $("#idType").val();
        let url = "/p/x/validate-identification-number";
        let data = { "__RequestVerificationToken": token, "idNumber": idNumber, "idType": idType };

        $.post(url, data, function (response) {
            if (!response.Error) {
                if (response.ResponseObject.IsActive) {
                    if (!response.ResponseObject.HasError) {
                        var validationResponseObj = response.ResponseObject;
                        $("#fullname").val(validationResponseObj.TaxPayerName);
                        if (validationResponseObj.PhoneNumber != null && validationResponseObj.PhoneNumber.length > 0) { $("#phoneNumber").val(validationResponseObj.PhoneNumber); }
                        if (validationResponseObj.RCNumber != null && validationResponseObj.RCNumber.length > 0) { $("#rcnumber").val(validationResponseObj.RCNumber); }
                        validatedIdentificationNumber = true;
                        makeFilledFieldsReadonly(true);
                        $("#userProfileForm").off("submit");
                        $("#userProfileForm").submit();
                    } else {
                        $("#idNumberErrorText").html(response.ResponseObject.ErrorMessage);
                    }
                } else {
                    $("#userProfileForm").off("submit");
                    $("#userProfileForm").submit();
                }
            } else {
                let errorMsg = response.ResponseObject.split(":");
                if (errorMsg.length > 1 && errorMsg[0] == "1") {
                    $("#idTypeErrorText").html(errorMsg[1]);
                } else { $("#idNumberErrorText").html(errorMsg[0]); }
            }
        }).always(function () {
            $("#idNumberLoader").hide();
            $("#idNumber").prop("readonly", false);
            $("#proceedBtn").prop("disabled", false);
        });
    }

    function makeFilledFieldsReadonly(readOnly) {
        if (categoryPermissions != undefined) {
            for (let i = 0; i < categoryPermissions.TaxCategoryPermissions.length; ++i) {
                if (categoryPermissions.TaxCategoryPermissions[i].Name == "IsEmployer") {
                    $("#fullname").attr("readonly", readOnly);
                    $("#rcnumber").attr("readonly", readOnly);
                } else {
                    $("#fullname").attr("readonly", readOnly);
                    $("#phoneNumber").attr("readonly", readOnly);
                }
            }
        } else {
            $("#fullname").attr("readonly", readOnly);
            $("#phoneNumber").attr("readonly", readOnly);
        }
    }

    function clearFilledFields() {
        $("#fullname").val("");
        $("#phoneNumber").val("");
    }

    function performCategorySpecificFieldActions() {
        if (categoryJsonSetting.ValidateGenderInfo) { enableGenderField(true); }
        else { enableGenderField(false); }
        if (categoryJsonSetting.ValidateContactEntityInfo) { enableContactEntityInfo(true); }
        else { enableContactEntityInfo(false); }
        if (categoryPermissions != undefined) {
            for (let i = 0; i < categoryPermissions.TaxCategoryPermissions.length; ++i) {
                if (categoryPermissions.TaxCategoryPermissions[i].Name == "IsEmployer") {
                    $("[for=fullname]").html("Company Name<span class='required-sym'>*</span>");
                    $("#fullname").attr("placeholder", "Enter Company Name");
                    $("#fullname").removeAttr("pattern");
                    $("#nameHint").html("Please enter your company name.");
                } else {
                    $("[for=fullname]").html("Full Name<span class='required-sym'>*</span>");
                    $("#fullname").attr("placeholder", "Enter Full Name");
                    $("#fullname").attr("pattern", "([A-Za-z]{2,}\\s[A-Za-z]{2,}|[A-Za-z]{2,}\\s[A-Za-z]{2,}\\s[A-Za-z]{2,})");;
                    $("#nameHint").html("Please enter your full name. (Firstname Lastname)");
                }
            }
        } else {
            $("[for=fullname]").html("Full Name<span class='required-sym'>*</span>");
            $("#fullname").attr("placeholder", "Enter Full Name");
            $("#fullname").attr("pattern", "([A-Za-z]{2,}\\s[A-Za-z]{2,}|[A-Za-z]{2,}\\s[A-Za-z]{2,}\\s[A-Za-z]{2,})");;
            $("#nameHint").html("Please enter your full name. (Firstname Lastname)");
        }
    }

    function enableGenderField(enable) {
        if (enable) {
            $("#genderFieldContainer").show();
            if (!loggedIn) {
                $("#genderFieldContainer select").prop("disabled", false);
                $("#genderFieldContainer select").attr("required", true);
            }
        } else {
            $("#genderFieldContainer select").prop("disabled", true);
            $("#genderFieldContainer select").attr("required", false);
            $("#genderFieldContainer").hide();
        }
    }

    function enableContactEntityInfo(enable) {
        if (enable) {
            $(".contact-person-info-required").show();
            $(".contact-person-info-required input").prop("required", enable);
        } else {
            $(".contact-person-info-required input").prop("required", enable);
            $(".contact-person-info-required").hide();
        }
    }
});