$(document).ready(function () {
    var token = $("input[name=__RequestVerificationToken]").val();
    var validatedIdentificationNumber = false;
    var identificationTypesMap = new Map();
    var taxPayerCategory = 0;
    var categoryPermissions = undefined;
    var categoryJsonSetting = undefined;
    var page = 1;
    hideBackButton(true);
    makeUserDetailsSectionRequired(false);
    enableGenderField(false);
    enableContactEntityInfo(false);

    if (formErrorNumber > 0) {
        taxPayerCategory = categoryId;
        categoryJsonSetting = categoryJsonSettings.find(function (val) { return val.Id == taxPayerCategory }).Settings;
        categoryPermissions = categoryPermissionsList.find(function (val) { return val.TaxCategoryId == taxPayerCategory });
        if (identificationTypesList != null) { identificationTypesMap.set(taxPayerCategory, identificationTypesList); }
        validatedIdentificationNumber = (hasIntegration != null) ? hasIntegration : false;
        if (!validatedIdentificationNumber) {
            $("#upload").prop("required", true);
        }
        switchPages(formErrorNumber);
    } else { switchPages(1); }


    $("[name=TaxPayerType]").click(function (event) {
        taxPayerCategory = $(this).attr("id");
        clearAllFormFields();
        $("#idTypeErrorText").empty();
        if (identificationTypesMap.has(taxPayerCategory)) {
            populateIdentificationTypes(identificationTypesMap.get(taxPayerCategory));
        } else { getIdentificationTypes(taxPayerCategory); }
        categoryJsonSetting = categoryJsonSettings.find(function (val) { return val.Id == taxPayerCategory }).Settings;
        categoryPermissions = categoryPermissionsList.find(function (val) { return val.TaxCategoryId == taxPayerCategory });
    });

    $("#idType").change(function () {
        clearAllUserDetailsSectionFormFields();
        validatedIdentificationNumber = false;
        $("#idTypeErrorText").empty();
        let identificationTypeId = $("#idType").val();
       
        if (!checkIfSelectedIdentificationTypeHasIntegration(taxPayerCategory, identificationTypeId)) {
            $("#upload").prop("required", true);
            $("#userIdentificationUploadSection").slideDown("fast", "linear", function () { changeUploadSectionToDefault(); });
        } else {
            $("#upload").prop("required", false);
            $("#userIdentificationUploadSection").slideUp("fast", "linear", function () { changeUploadSectionToDefault(); });
        }
    });
    $("#idNumber").on("input", function () { clearAllUserDetailsSectionFormFields(); validatedIdentificationNumber = false; });

    $("#mainForm").submit(function (e) {
        e.preventDefault();
        $("#idNumberErrorText").empty();
        $("#idTypeErrorText").empty();
        switch (page) {
            case 1:
                if ($("#idNumber").val().trim().length == 0) { $("#idNumberErrorText").html("Identification number length is not valid"); return; }
                $("#idNumber").prop("disabled", true);
                $("#idNumberLoader").show();
                let idNumber = $("#idNumber").val();
                let idType = $("#idType").val();
                validateIdentificationNumber(idNumber, idType);
                break;
            case 2:
                $("#mainForm").off("submit");
                $("#mainForm").submit();
                break;
            default:
                break;
        }
    });

    $("#backBtn").click(function () {
        switch (page) {
            case 1:
                break;
            case 2:
                    $("#userDetailsSection").fadeOut("fast", "linear", function () {
                        makeUserDetailsSectionRequired(false);
                        enableGenderField(false);
                        enableContactEntityInfo(false);
                        makeFilledFieldsReadyonly(false);
                        $("#userCategorySection").fadeIn("fast", "linear", function () { });
                        page -= 1;
                        $("#createAccountBtn").html("Proceed");
                        hideBackButton(true);
                    });
                break;
            default:
                break;
        }
    });

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

    $("#idType").change(function (e) {
        var idType = parseInt(e.currentTarget.value);
        let hint = identificationTypesMap.get(taxPayerCategory).find(function (val) { return val.Id == idType; }).Hint;
        $("#uploadHint").html(hint);
        $("#hintLabel").html(hint);
    });

    function makeUserDetailsSectionRequired(required) {
        $(".user-details-required").attr("required", required);
        disableRequiredFields(!required);
    }

    function clearAllFormFields() {
        $(".form-control, .custom-select").each(function () {
            this.value = "";
        });
    }

    function clearAllUserDetailsSectionFormFields() {
        $("#userDetailsSection .form-control,#userDetailsSection .custom-select").each(function () {
            this.value = "";
        });
        if (formErrorNumber == 0) { $(".validation-msg").empty(); }
    }

    function disableNavButtons(disable) {
        $("#backBtn").prop("disabled", disable);
        $("#createAccountBtn").prop("disabled", disable);
    }

    function populateIdentificationTypes(idTypes) {
        $("#idType").empty();
        $("#idType").append("<option selected disabled value = ''>Select an Identification Type</option>");
        idTypes.forEach(function (val) {
            $("#idType").append("<option value = '" + val.Id + "'>" + val.Name + "</option>");
        });
    }

    function disableUserDetailsSection(disabled) {
        $("#userDetailsSection .form-control,#userDetailsSection .custom-select").prop("disabled", disabled);
    }

    function disableRequiredFields(disable) {
        $(".user-details-required").attr("disabled", disable);
    }

    function makeFilledFieldsReadyonly(readOnly) {
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

    function switchPages(pageNumber) {
        switch (pageNumber) {
            case 1:
                makeUserDetailsSectionRequired(false);
                page = 1;
                $("#createAccountBtn").html("Proceed");
                hideBackButton(true);
                break;
            case 2:
                performCategorySpecificFieldActions();
                makeUserDetailsSectionRequired(true);
                if (validatedIdentificationNumber) { makeFilledFieldsReadyonly(true); }
                page = 2;
                $("#createAccountBtn").html("Create Account");
                hideBackButton(false);
                $("#mainForm").off("submit");
                break;
            default:
                break;
        }
    }

    function validateIdentificationNumber(idNumber, idType) {

        if (!validatedIdentificationNumber) {
            if (idType == null) {
                $("#idTypeErrorText").html("Identification type not specified.");
                $("#idNumber").prop("disabled", false);
                $("#idNumberLoader").hide();
                return;
            }

            $("#createAccountBtn").prop("disabled", true);
            let url = "/p/x/validate-identification-number";
            let data = { "__RequestVerificationToken": token, "idNumber": idNumber, "idType": idType };

            $.post(url, data, function (response) {
                if (!response.Error) {
                    if (response.ResponseObject.IsActive) {
                        if (!response.ResponseObject.HasError) {
                            validatedIdentificationNumber = true;
                            $("#userCategorySection").fadeOut("fast", "linear", function () {
                                performCategorySpecificFieldActions();
                                makeUserDetailsSectionRequired(true);
                                $("#userDetailsSection").fadeIn("fast", "linear", function () {
                                    $(document).scrollTop(0);
                                    $("#createAccountBtn").html("Create Account");
                                    var validationResponseObj = response.ResponseObject;
                                    $("#fullname").val(validationResponseObj.TaxPayerName);
                                    if (validationResponseObj.PhoneNumber != null && validationResponseObj.PhoneNumber.length > 0) { $("#phoneNumber").val(validationResponseObj.PhoneNumber); }
                                    if (validationResponseObj.RCNumber != null && validationResponseObj.RCNumber.length > 0) { $("#rcnumber").val(validationResponseObj.RCNumber); }
                                    if (validationResponseObj.EmailAddress != null && validationResponseObj.EmailAddress.length > 0) {
                                        $("#email").val(validationResponseObj.EmailAddress);
                                        $("#email").attr("readonly", true);
                                    }
                                    makeFilledFieldsReadyonly(true);
                                });
                                page += 1;
                                hideBackButton(false);
                            });
                        } else { $("#idNumberErrorText").html(response.ResponseObject.ErrorMessage); }
                    } else {
                        $("#userCategorySection").fadeOut("fast", "linear", function () {
                            performCategorySpecificFieldActions();
                            makeUserDetailsSectionRequired(true);
                            $("#userDetailsSection").fadeIn("fast", "linear", function () {
                                $(document).scrollTop(0);
                                $("#createAccountBtn").html("Create Account");
                            });
                            page += 1;
                            hideBackButton(false);
                        });
                    }
                } else {
                    let errorMsg = response.ResponseObject.split(":");
                    if (errorMsg.length > 1 && errorMsg[0] == "1") {
                        $("#idTypeErrorText").html(errorMsg[1]);
                    } else { $("#idNumberErrorText").html(errorMsg[0]); }
                }

            }).always(function () {
                $("#idNumberLoader").hide();
                $("#idNumber").prop("disabled", false);
                $("#createAccountBtn").prop("disabled", false);
            });
        } else {
            makeFilledFieldsReadyonly(true);
            $("#idNumberLoader").hide();
            $("#idNumber").prop("disabled", false);
            $("#userCategorySection").fadeOut("fast", "linear", function () {
                performCategorySpecificFieldActions();
                makeUserDetailsSectionRequired(true);
                $("#userDetailsSection").fadeIn("fast", "linear", function () {
                    $(document).scrollTop(0);
                    $("#createAccountBtn").html("Create Account");
                });
                page += 1;
                hideBackButton(false);
            });
        }
       
    }

    function getIdentificationTypes(category) {

        $("#idType").prop("disabled", true);
        $("#createAccountBtn").prop("disabled", true);
        $("#idTypeLoader").show();
        let url = "/p/x/get-identification-types";
        let data = { "__RequestVerificationToken": token, "categoryId": category };

        $.post(url, data, function (response) {
            if (!response.Error) {
                if (response.ResponseObject.length < 1) {
                    $("#idType").empty();
                    $("#idTypeErrorText").html("No identification types configured.");
                    $("#userCategorySection input").prop("disabled", false);
                } else {
                    identificationTypesMap.set(category, response.ResponseObject);
                    populateIdentificationTypes(response.ResponseObject);
                }
            } else {
                $("#idTypeErrorText").html(response.ResponseObject);
            }
        }).always(function () {
            $("#idType").prop("disabled", false);
            $("#createAccountBtn").prop("disabled", false);
            $("#idTypeLoader").hide();
        });
    }

    function changeUploadSectionToDefault() {
        $("#fileName").html("");
        $("#uploadInfo").html("Add an identification file here. (max size 2mb)");
        $("#uploadImg").show();
        $("#fileUploadbtn").prop("disabled", true);
        $("#upload").val("");
    }

    function checkIfSelectedIdentificationTypeHasIntegration(catId, identificationTypeId) {
        if (identificationTypesMap.has(catId)) {
            return identificationTypesMap.get(catId).find(function (val) { return val.Id == identificationTypeId }).HasIntegration;
        }
        return false;
    }

    function hideBackButton(hide) {
        if (hide) {
            $("#backBtn").prop("disabled", false);
            $("#backBtn").hide();
            $("#createAccountBtn").parent()[0].classList.remove("col-md-6");
            $("#createAccountBtn").parent()[0].classList.add("col-md-12");
        } else {
            $("#createAccountBtn").parent()[0].classList.remove("col-md-12");
            $("#createAccountBtn").parent()[0].classList.add("col-md-6");
            $("#backBtn").show();
        }
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
            $("#genderFieldContainer select").prop("disabled", false);
            $("#genderFieldContainer select").attr("required", true);
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