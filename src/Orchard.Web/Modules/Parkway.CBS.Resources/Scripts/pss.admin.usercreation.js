var departmentsMap = new Map();
var sectionsMap = new Map();
var subSectionsMap = new Map();
var flowDefinitionMap = new Map();
var flowDefinitionLevelsMap = new Map();
var selectedCommandsList = [];
var selectedCommandsListIndex = 1;
var checkForSelectedDefinitions = false;
var checkForSelectedDefinitionLevels = false;

if (selectedUserType > 0) {
    if (selectedUserType == approver) {
        $("#flowDefinitionContainer").show();
        $("#flowDefinitionLevelContainer").show();
        $("#commandAccessTypeContainer").show();
        checkForSelectedDefinitions = true;
        checkForSelectedDefinitionLevels = true;
    } else if (selectedUserType == viewer) {
        $("#flowDefinitionContainer").hide();
        $("#flowDefinitionLevelContainer").hide();
    }

    if (selectedCommands != null && selectedCommands.length > 0) {
        $(selectedCommands).each(function (index, value) {
            addPostbackDataToTable(value);
        });
    }
    $(selectedServiceTypes).each(function (index, val) {
        registerOnChangeEventOnCheckboxList("#flowDefinitionCheckboxList", val);
    });
}

if ($("#commandCategory").val() !== "0" && $("#commands").val() === "") {
    $("#commands").empty();
    $('#searchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000">No command was found for the selected formation level.</span>');
}

$('#displayInput').change(function () {
    if (!this.checked) {
        $('#Fullname').removeAttr("readonly");
        $('#Username').removeAttr("readonly");
       // $('#PhoneNumber').removeAttr("readonly");
    }
    else {
        $('#Fullname').prop('readonly', true);
        $('#Username').prop('readonly', true);
       // $('#PhoneNumber').prop('readonly', true);
    }
});

$("#commandCategory").change(function (e) {
    if (e.currentTarget.value == "0") { return; }
    getDepartments(e.currentTarget.value);
});

$("#departmentLevel").change(function (e) {
    if (e.currentTarget.value == "0") { return; }
    getSections(getCommandCode(departmentsMap, e.currentTarget.value));
});

$("#sectionLevel").change(function (e) {
    if (e.currentTarget.value == "0") { return; }
    getSubSections(getCommandCode(sectionsMap, e.currentTarget.value));
});

$("#officerCommandCategory").change(function (e) {
    if (e.currentTarget.value == "0") { return; }
    getOfficerDepartments(e.currentTarget.value);
});

$("#officerDepartmentLevel").change(function (e) {
    if (e.currentTarget.value == "0") { return; }
    if (departmentsMap.size == 0) {
        getOfficerDepartmentsMap($("#officerCommandCategory").val(), e.currentTarget.value);
    }
    else {
        getOfficerSections(getCommandCode(departmentsMap, e.currentTarget.value));
    }
});

$("#officerSectionLevel").change(function (e) {

    if (e.currentTarget.value == "0") { return; }
    let commandCode = $(this).find(':selected').data('code');

    if (commandCode != undefined) {
        $('#officerSubSectionSearchError').empty();
        $("#officerSubSectionLevel").empty();
        $("#officerSubSectionLevel").append("<option selected disabled value=''>Select a Sub Section</option>");
        getOfficerSubSections(commandCode);
    }
    else {
        getOfficerSubSections(getCommandCode(sectionsMap, e.currentTarget.value));
    }
});

$("#addCommand").click(function () {
    addCurrentlySelectedLowestLevelCommand();
    emptyFormationDropdowns();
});

$("#searchBtn").click(function () {
    getOfficerDetails($("#apNumber").val());
});

$("#adminUserTypeId").change(function (e) {
    if (e.currentTarget.value.toString() == "" || e.currentTarget.value.toString() == undefined) { return; }
    $("#flowDefinitionLevelSearchError").html("");
    checkForSelectedDefinitions = false;
    checkForSelectedDefinitionLevels = false;
    if (e.currentTarget.value.toString() == approver) {
        $("#flowDefinitionContainer").show();
        $("#flowDefinitionLevelContainer").show();
        $("#commandAccessTypeContainer").show();
        checkForSelectedDefinitionLevels = true;
        checkForSelectedDefinitions = true;
    } else if (e.currentTarget.value.toString() == viewer) {
        $("#flowDefinitionContainer").hide();
        $("#flowDefinitionLevelContainer").hide();
        $("#commandAccessTypeContainer").hide();
        emptyApproverDropdowns();
        clearAllCommandsWithAccessType(approver);
    }
});


$("#serviceCheckboxList input[type=checkbox]").change(function (e) {
    $("#serviceTypeSearchError").html("");
    if (e.currentTarget.value == "") { return; }
    checkForSelectedDefinitionLevels = false;
    checkForSelectedDefinitions = false;
    $("#flowDefinitionSearchError").html("");
    $("#flowDefinitionLevelSearchError").html("");
    if (e.currentTarget.checked) {
        getFlowDefinition(e.currentTarget.value, this.nextElementSibling.innerText);
    } else {
        emptyDefinitionLevels($(".definition-" + e.currentTarget.value), e.currentTarget.value)
        $(".definition-" + e.currentTarget.value).remove();
        if ($("#flowDefinitionCheckboxList input[type=checkbox]").length == 0) {
            $("#flowDefinitionCheckboxList").append("<li class='multi-select-dropdown-default-option'>Select a service to have it's flow definitions listed here</li>");
        }
    }
});


$("form").submit(function (e) {
    e.preventDefault();
    if ($(".multi-select-command-option").length == 0) {
        alert("You need to select at least one command");
        return;
    }
    if ($("#serviceCheckboxList .multi-select-checkbox-options label input[type=checkbox]:checked").length == 0) {
        $("#serviceTypeSearchError").html("You need to select at least one service");
        $("html, body").animate({ scrollTop: $('#serviceTypeContainer').offset().top }, 500);
        return;
    }
    if (checkForSelectedDefinitionLevels || checkForSelectedDefinitions) {
        checkIfFlowDefinitionsHaveBeenSelected();
    } else { $("form").off("submit"); $("form").submit(); }
});

function buildDropDown(dropdownContainerId, dropdownData, defaultElementLabel) {
    $(dropdownContainerId).empty();
    let options = "<option selected disabled value=''>Select a " + defaultElementLabel + "</option>"
    $(dropdownData).each(function (index, val) {
        options += '<option value="' + val.Id + '">' + val.Name + '</option>';
    });

    $(dropdownContainerId).append(options);
}

function prependToCheckboxList(containerId, data, parentId, titleHeader) {
    if ($(containerId + " .multi-select-dropdown-default-option").length > 0) { $(containerId).empty(); }
    let listItems = '<li class="multi-select-dropdown-divider-option ' + parentId + '">' + titleHeader + '</li>';
    $(data).each(function (index, val) {
        listItems += '<li class="multi-select-checkbox-options ' + parentId + '"><label><input type="checkbox" value="' + val.Id + '" name="SelectedFlowDefinitions[]" /> <span>' + val.Name + '</span></label></li>';
    });
    $(containerId).prepend(listItems);
}

function registerOnChangeEventOnCheckboxList(containerId, serviceTypeId) {
    $(containerId + " .multi-select-checkbox-options.definition-" + serviceTypeId + " input[type=checkbox]").change(function (e) {
        if (e.currentTarget.value == "") { return; }
        if (e.currentTarget.checked) {
            getApprovalFlowDefinitionLevels(e.currentTarget.value, this.nextElementSibling.innerText, serviceTypeId);
        } else {
            $(".parent-" + serviceTypeId + "-definition-level-" + e.currentTarget.value).remove();
            if ($("#flowDefinitionLevelCheckboxList input[type=checkbox]").length == 0) {
                $("#flowDefinitionLevelCheckboxList").append("<li class='multi-select-dropdown-default-option'>Select a flow definition to have it's approval levels listed here</li>");
            }
        }
    });
}

function emptyDefinitionLevels(definitionCollection, serviceTypeId) {
    $(definitionCollection).each(function (index, val) {
        if (val.children[0] != undefined) {
            $(".parent-" + serviceTypeId + "-definition-level-" + val.children[0].children[0].value).remove();
        }
    });
    if ($("#flowDefinitionLevelCheckboxList input[type=checkbox]").length == 0) {
        $("#flowDefinitionLevelCheckboxList").empty();
        $("#flowDefinitionLevelCheckboxList").append("<li class='multi-select-dropdown-default-option'>Select a flow definition to have it's approval levels listed here</li>");
    }
}

function prependToCheckboxListWithTitle(containerId, data, parentId, titleHeader) {
    if ($(containerId + " .multi-select-dropdown-default-option").is(":visible")) { $(containerId).empty(); }
    let listItems = '<li class="multi-select-dropdown-divider-option '+ parentId +'">'+ titleHeader +'</li>';
    $(data).each(function (index, val) {
        listItems += '<li class="multi-select-checkbox-options '+ parentId +' "><label><input type="checkbox" value="' + val.Id + '" name="SelectedFlowDefinitionLevels[]" /> <span>' + val.PositionName + '</span></label></li>';
    });

    $(containerId).prepend(listItems);
}

function buildFlowDefinitionLevelCheckboxList(containerId, data) {
    $(containerId).empty();
    let options = "";
    $(data).each(function (index, val) {
        options += '<li class="multi-select-checkbox-options"><label><input type="checkbox" value="' + val.Id + '" name="SelectedFlowDefinitionLevels[]" /> <span>' + val.PositionName + '</span></label></li>';
    });
    $(containerId).append(options);
}

function getOfficerDetails(apNumber) {
    $("#apNumber").prop("disabled", true);
    $("#apNumberLoader").show();
    $('#apNumberSearchError').empty();
    $('#officerDepartmentLevel').empty();
    $('#officerSectionLevel').empty();
    $('#officerSubSectionLevel').empty();
    var url = `/Admin/Police/get-officer-with-service-number`;
    var requestData = { "serviceNumber": apNumber, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
    $.get(url, requestData, function (data) {
        if (!data.Error) {
            
            $('#Fullname').val(data.ResponseObject.Name);
            $('#Username').val(data.ResponseObject.IdNumber);
            $('#PhoneNumber').val(data.ResponseObject.PhoneNumber);

            getOfficerDepartments(data.ResponseObject.CommandCategoryId, data.ResponseObject.CommandId);
            $("#officerCommandCategory").val(data.ResponseObject.CommandCategoryId);

            if (data.ResponseObject.SubCommandId !== 0) {
                getOfficerSections(data.ResponseObject.CommandCode, data.ResponseObject.SubCommandId);
                if (data.ResponseObject.SubSubCommandId !== 0) {
                    getOfficerSubSections(data.ResponseObject.SubCommandCode, data.ResponseObject.SubSubCommandId);
                }
            }

        } else {
            $('#apNumberSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >' + data.ResponseObject + '</span > ');
        }

    }).fail(function () {
        $('#apNumberSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000">An error occurred try refreshing the page.</span > ');
    }).always(function () {
        $("#apNumberLoader").hide();
        $("#apNumber").prop("disabled", false);
        disableAllCommandDropdowns(false);
    });
}


function getOfficerDepartmentsMap(commandCategoryId, sectionId) {
    disableAllCommandDropdowns(true);
    $("#officerDepartmentLoader").show();
    $('#officerDepartmentSearchError').empty();
    if (!departmentsMap.has(commandCategoryId.toString())) {
        //do ajax call
        var url = '/Admin/Police/get-commands-by-commandcategory';
        var requestData = { "commandCategoryId": commandCategoryId, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
        $.post(url, requestData, function (data) {
            if (!data.Error) {
                departmentsMap.set(commandCategoryId.toString(), data.ResponseObject);
                getOfficerSections(getCommandCode(departmentsMap, sectionId));
                $("#officerSectionLevel").empty();
                $("#officerSectionLevel").append("<option selected disabled value=''>Select a Section</option>");
                $("#officerSubSectionLevel").empty();
                $("#officerSubSectionLevel").append("<option selected disabled value=''>Select a Sub Section</option>");
            } else {
                $('#officerDepartmentSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >' + data.ResponseObject + '</span > ');
            }

        }).fail(function () {
            $('#officerDepartmentSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000">An error occurred try refreshing the page.</span > ');
        }).always(function () {
            $("#officerDepartmentLoader").hide();
            disableAllCommandDropdowns(false);
        });

    } else {
        disableAllCommandDropdowns(false);
        $("#officerDepartmentLoader").hide();
        $("#officerSectionLevel").empty();
        $("#officerSectionLevel").append("<option selected disabled value=''>Select a Section</option>");
        $("#officerSubSectionLevel").empty();
        $("#officerSubSectionLevel").append("<option selected disabled value=''>Select a Sub Section</option>");
        buildDropDown("#officerDepartmentLevel", departmentsMap.get(commandCategoryId.toString()), "Department");
    }
}

function getOfficerDepartments(commandCategoryId, selectedOption = 0) {
    disableAllCommandDropdowns(true);
    $("#officerDepartmentLoader").show();
    $('#officerDepartmentSearchError').empty();
    if (!departmentsMap.has(commandCategoryId.toString())) {
        //do ajax call
        var url = '/Admin/Police/get-commands-by-commandcategory';
        var requestData = { "commandCategoryId": commandCategoryId, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
        $.post(url, requestData, function (data) {
            if (!data.Error) {
                if (data.ResponseObject.length == 0) { $('#officerDepartmentSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >No command was found in the selected formation level.</span > '); return; }
                departmentsMap.set(commandCategoryId.toString(), data.ResponseObject);
              
                buildDropDown("#officerDepartmentLevel", departmentsMap.get(commandCategoryId.toString()), "Department");
                if (selectedOption == 0) {
                    $("#officerSectionLevel").empty();
                    $("#officerSectionLevel").append("<option selected disabled value=''>Select a Section</option>");
                    $("#officerSubSectionLevel").empty();
                    $("#officerSubSectionLevel").append("<option selected disabled value=''>Select a Sub Section</option>");
                }
                else {
                    $("#officerDepartmentLevel").val(selectedOption);
                }
            } else {
                $('#officerDepartmentSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >' + data.ResponseObject + '</span > ');
            }

        }).fail(function () {
            $('#officerDepartmentSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000">An error occurred try refreshing the page.</span > ');
        }).always(function () {
            $("#officerDepartmentLoader").hide();
            disableAllCommandDropdowns(false);
        });

    } else {
        disableAllCommandDropdowns(false);
        $("#officerDepartmentLoader").hide();
        buildDropDown("#officerDepartmentLevel", departmentsMap.get(commandCategoryId.toString()), "Department");
        if (selectedOption == 0) {
            $("#officerSectionLevel").empty();
            $("#officerSectionLevel").append("<option selected disabled value=''>Select a Section</option>");
            $("#officerSubSectionLevel").empty();
            $("#officerSubSectionLevel").append("<option selected disabled value=''>Select a Sub Section</option>");
        }
        else {
            $("#officerDepartmentLevel").val(selectedOption);
        }
    }
}


function getOfficerSections(departmentCode, selectedOption = 0) {
    disableAllCommandDropdowns(true);
    $("#officerSectionLoader").show();
    $('#officerSectionSearchError').empty();
    if (!sectionsMap.has(departmentCode.toString())) {
        //do ajax call
        var url = '/Admin/Police/get-commands-by-parent-code-formation-level';
        var requestData = { "parentCode": departmentCode, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
        $.post(url, requestData, function (data) {
            if (!data.Error) {
                if (data.ResponseObject.length == 0) { $('#officerSectionSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >No commands found.</span > '); return; }
                sectionsMap.set(departmentCode.toString(), data.ResponseObject);
                
                buildDropDown("#officerSectionLevel", sectionsMap.get(departmentCode.toString()), "Section");
                if (selectedOption == 0) {
                    $("#officerSubSectionLevel").empty();
                    $("#officerSubSectionLevel").append("<option selected disabled value=''>Select a Sub Section</option>");
                }
                else {
                    $("#officerSectionLevel").val(selectedOption)

                }
            } else {
                $('#officerSectionSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >' + data.ResponseObject + '</span > ');
            }

        }).fail(function () {
            $('#officerSectionSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000">An error occurred try refreshing the page.</span > ');
        }).always(function () {
            $("#officerSectionLoader").hide();
            disableAllCommandDropdowns(false);
        });

    } else {
        disableAllCommandDropdowns(false);
        $("#officerSectionLoader").hide();
        buildDropDown("#officerSectionLevel", sectionsMap.get(departmentCode.toString()), "Section");
        if (selectedOption == 0) {
            $("#officerSubSectionLevel").empty();
            $("#officerSubSectionLevel").append("<option selected disabled value=''>Select a Sub Section</option>");
        }
        else {
            $("#officerSectionLevel").val(selectedOption)

        }
    }
}


function getOfficerSubSections(sectionCode, selectedOption = 0) {
    disableAllCommandDropdowns(true);
    $("#officerSubSectionLoader").show();
    $('#officerSubSectionSearchError').empty();
    if (!subSectionsMap.has(sectionCode.toString())) {
        //do ajax call
        var url = '/Admin/Police/get-commands-by-parent-code-formation-level';
        var requestData = { "parentCode": sectionCode, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
        $.post(url, requestData, function (data) {
            if (!data.Error) {
                if (data.ResponseObject.length == 0) { $('#officerSubSectionSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >No commands found.</span > '); return; }
                subSectionsMap.set(sectionCode.toString(), data.ResponseObject);
                buildDropDown("#officerSubSectionLevel", subSectionsMap.get(sectionCode.toString()), "Sub Section");
                if (selectedOption != 0) {
                    $("#officerSubSectionLevel").val(selectedOption);
                }
            } else {
                $('#officerSubSectionSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >' + data.ResponseObject + '</span > ');
            }

        }).fail(function () {
            $('#officerSubSectionSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000">An error occurred try refreshing the page.</span > ');
        }).always(function () {
            $("#officerSubSectionLoader").hide();
            disableAllCommandDropdowns(false);
        });

    } else {
        disableAllCommandDropdowns(false);
        $("#officerSubSectionLoader").hide();
        buildDropDown("#officerSubSectionLevel", subSectionsMap.get(sectionCode.toString()), "Sub Section");
        if (selectedOption != 0) {
            $("#officerSubSectionLevel").val(selectedOption);
        }
    }
}

function getDepartments(commandCategoryId) {
    disableAllCommandDropdowns(true);
    $("#departmentLoader").show();
    $('#departmentSearchError').empty();
    if (!departmentsMap.has(commandCategoryId.toString())) {
        //do ajax call
        var url = '/Admin/Police/get-commands-by-commandcategory';
        var requestData = { "commandCategoryId": commandCategoryId, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
        $.post(url, requestData, function (data) {
            if (!data.Error) {
                if (data.ResponseObject.length == 0) {
                    $('#departmentSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >No command was found in the selected formation level.</span > ');
                    $("#departmentLevel").empty()
                    $("#subSectionLevel").empty();
                    $("#sectionLevel").empty();
                    $("#sectionLevel").append("<option selected disabled value=''>Select a Section</option>");
                    $("#subSectionLevel").append("<option selected disabled value=''>Select a Sub Section</option>");
                    $("#departmentLevel").append("<option selected disabled value=''>Select a Department</option>");
                    return;
                }
                departmentsMap.set(commandCategoryId.toString(), data.ResponseObject);
                $("#sectionLevel").empty();
                $("#sectionLevel").append("<option selected disabled value=''>Select a Section</option>");
                $("#subSectionLevel").empty();
                $("#subSectionLevel").append("<option selected disabled value=''>Select a Sub Section</option>");
                buildDropDown("#departmentLevel", departmentsMap.get(commandCategoryId.toString()), "Department");
            } else {
                $('#departmentSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >' + data.ResponseObject + '</span > ');
            }

        }).fail(function () {
            $('#departmentSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000">An error occurred try refreshing the page.</span > ');
        }).always(function () {
            $("#departmentLoader").hide();
            disableAllCommandDropdowns(false);
        });

    } else {
        disableAllCommandDropdowns(false);
        $("#departmentLoader").hide();
        $("#sectionLevel").empty();
        $("#sectionLevel").append("<option selected disabled value=''>Select a Section</option>");
        $("#subSectionLevel").empty();
        $("#subSectionLevel").append("<option selected disabled value=''>Select a Sub Section</option>");
        buildDropDown("#departmentLevel", departmentsMap.get(commandCategoryId.toString()), "Department");
    }
}


function getSections(departmentCode) {
    disableAllCommandDropdowns(true);
    $("#sectionLoader").show();
    $('#sectionSearchError').empty();
    if (!sectionsMap.has(departmentCode.toString())) {
        //do ajax call
        var url = '/Admin/Police/get-commands-by-parent-code-formation-level';
        var requestData = { "parentCode": departmentCode, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
        $.post(url, requestData, function (data) {
            if (!data.Error) {
                if (data.ResponseObject.length == 0) {
                    $('#sectionSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >No commands found</span > ');
                    $("#subSectionLevel").empty();
                    $("#sectionLevel").empty();
                    $("#sectionLevel").append("<option selected disabled value=''>Select a Section</option>");
                    $("#subSectionLevel").append("<option selected disabled value=''>Select a Sub Section</option>");
                    return;
                }
                sectionsMap.set(departmentCode.toString(), data.ResponseObject);
                $("#subSectionLevel").empty();
                $("#subSectionLevel").append("<option selected disabled value=''>Select a Sub Section</option>");
                buildDropDown("#sectionLevel", sectionsMap.get(departmentCode.toString()), "Section");
            } else {
                $('#sectionSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >' + data.ResponseObject + '</span > ');
                emptyFormationDropdowns();
            }

        }).fail(function () {
            $('#sectionSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000">An error occurred try refreshing the page.</span > ');
        }).always(function () {
            $("#sectionLoader").hide();
            disableAllCommandDropdowns(false);
        });

    } else {
        disableAllCommandDropdowns(false);
        $("#sectionLoader").hide();
        $("#subSectionLevel").empty();
        $("#subSectionLevel").append("<option selected disabled value=''>Select a Sub Section</option>");
        buildDropDown("#sectionLevel", sectionsMap.get(departmentCode.toString()), "Section");
    }
}


function getSubSections(sectionCode) {
    disableAllCommandDropdowns(true);
    $("#subSectionLoader").show();
    $('#subSectionSearchError').empty();
    if (!subSectionsMap.has(sectionCode.toString())) {
        //do ajax call
        var url = '/Admin/Police/get-commands-by-parent-code-formation-level';
        var requestData = { "parentCode": sectionCode, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
        $.post(url, requestData, function (data) {
            if (!data.Error) {
                if (data.ResponseObject.length == 0) {
                    $("#subSectionLevel").empty();
                    $("#subSectionLevel").append("<option selected disabled value=''>Select a Sub Section</option>");
                    $('#subSectionSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >No commands found</span > ');
                    return;
                }
                subSectionsMap.set(sectionCode.toString(), data.ResponseObject);
                buildDropDown("#subSectionLevel", subSectionsMap.get(sectionCode.toString()), "Sub Section");
            } else {
                $('#subSectionSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >' + data.ResponseObject + '</span > ');
                emptyFormationDropdowns();
            }

        }).fail(function () {
            $('#subSectionSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000">An error occurred try refreshing the page.</span > ');
        }).always(function () {
            $("#subSectionLoader").hide();
            disableAllCommandDropdowns(false);
        });

    } else {
        disableAllCommandDropdowns(false);
        $("#subSectionLoader").hide();
        buildDropDown("#subSectionLevel", subSectionsMap.get(sectionCode.toString()), "Sub Section");
    }
}


function getFlowDefinition(serviceType, titleHeader) {
    disableAllServiceTypeDropdowns(true);
    $("#flowDefinitionLoader").show();
    $('#flowDefinitionSearchError').empty();
    if (!flowDefinitionMap.has(serviceType.toString())) {
        //do ajax call
        var url = '/Admin/Police/get-flow-definitions-for-service';
        var requestData = { "serviceTypeId": serviceType, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
        $.post(url, requestData, function (data) {
            if (!data.Error) {
                flowDefinitionMap.set(serviceType.toString(), data.ResponseObject);
                prependToCheckboxList("#flowDefinitionCheckboxList", flowDefinitionMap.get(serviceType.toString()), "definition-" + serviceType, titleHeader);
                registerOnChangeEventOnCheckboxList("#flowDefinitionCheckboxList", serviceType);
            } else {
                $('#flowDefinitionSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >' + data.ResponseObject + '</span > ');
            }

        }).fail(function () {
            $('#flowDefinitionSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000">An error occurred try refreshing the page.</span > ');
        }).always(function () {
            $("#flowDefinitionLoader").hide();
            disableAllServiceTypeDropdowns(false);
        });

    } else {
        disableAllServiceTypeDropdowns(false);
        $("#flowDefinitionLoader").hide();
        prependToCheckboxList("#flowDefinitionCheckboxList", flowDefinitionMap.get(serviceType.toString()), "definition-" + serviceType, titleHeader);
        registerOnChangeEventOnCheckboxList("#flowDefinitionCheckboxList", serviceType);
    }
}

function getApprovalFlowDefinitionLevels(definitionId, titleHeader, serviceTypeId) {
    disableAllServiceTypeDropdowns(true);
    $("#flowDefinitionLevelLoader").show();
    $('#flowDefinitionLevelSearchError').empty();
    if (!flowDefinitionLevelsMap.has(definitionId.toString())) {
        //do ajax call
        var url = '/Admin/Police/get-approval-flow-definition-levels-for-definition';
        var requestData = { "definitionId": definitionId, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
        $.post(url, requestData, function (data) {
            if (!data.Error) {
                flowDefinitionLevelsMap.set(definitionId.toString(), data.ResponseObject);
                prependToCheckboxListWithTitle("#flowDefinitionLevelCheckboxList", flowDefinitionLevelsMap.get(definitionId.toString()), "parent-" + serviceTypeId + "-definition-level-" + definitionId, titleHeader);
            } else {
                $('#flowDefinitionLevelSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >' + data.ResponseObject + '</span > ');
            }

        }).fail(function () {
            $('#flowDefinitionLevelSearchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000">An error occurred try refreshing the page.</span > ');
        }).always(function () {
            $("#flowDefinitionLevelLoader").hide();
            disableAllServiceTypeDropdowns(false);
        });

    } else {
        disableAllServiceTypeDropdowns(false);
        $("#flowDefinitionLevelLoader").hide();
        prependToCheckboxListWithTitle("#flowDefinitionLevelCheckboxList", flowDefinitionLevelsMap.get(definitionId.toString()), "parent-" + serviceTypeId + "-definition-level-" + definitionId, titleHeader);
    }
    checkForSelectedDefinitionLevels = true;
}


function disableAllCommandDropdowns(enable) {
    $("#commandCategory").prop("disabled", enable);
    $("#departmentLevel").prop("disabled", enable);
    $("#sectionLevel").prop("disabled", enable);
    $("#subSectionLevel").prop("disabled", enable);
}


function disableAllServiceTypeDropdowns(enable) {
    $("#serviceCheckboxList input[type=checkbox]").prop("disabled", enable);
    $("#flowDefinitionCheckboxList input[type=checkbox]").prop("disabled", enable);
}


function getCommandCode(mapName, commandId) {
    let code = "";
    mapName.forEach(function (val) {
        $(val).each(function (index, value) {
            if (value.Id.toString() == commandId.toString()) {
                code = value.Code;
            }
        });
    });
    return code;
}

function getCommand(mapName, commandId) {
    let command = undefined;
    mapName.forEach(function (val) {
        $(val).each(function (index, value) {
            if (value.Id.toString() == commandId.toString()) {
                command = value;
            }
        });
    });
    return command;
}


function getCurrentlySelectedLowestLevelCommand() {
    if ($("#adminUserTypeId").val() == null || $("#adminUserTypeId").val().toString() == "") { alert("you need to select the admin access type"); return; }
    if ($("#commandCategory").val() == null || $("#commandCategory").val().toString() == "") { alert("you need to select a formation"); return; }
    if ($("#departmentLevel").val() == null || $("#departmentLevel").val().toString() == "") { alert("you need to select a department"); return; }
    if (($("#adminUserTypeId").val() == approver && $("#commandAccessType").val() == null) || ($("#adminUserTypeId").val() == approver && $("#commandAccessType").val().toString() == "")) { alert("you need to select the access type for the command"); return; }

    if ($("#subSectionLevel").val() != null && $("#subSectionLevel").val().toString() != "") {
        return getCommand(subSectionsMap, $("#subSectionLevel").val().toString());
    }

    if ($("#sectionLevel").val() != null && $("#sectionLevel").val().toString() != "") {
        return getCommand(sectionsMap, $("#sectionLevel").val().toString());
    }

    if ($("#departmentLevel").val() != null && $("#departmentLevel").val().toString() != "") {
        return getCommand(departmentsMap, $("#departmentLevel").val().toString());
    }
    return null;
}


function addCurrentlySelectedLowestLevelCommand() {
    let command = getCurrentlySelectedLowestLevelCommand();
    if (command == undefined) { return; }
    if (checkIfCommandAlreadyAdded(command.Id) > -1) { alert('Command has already been added'); return; }
    command.AccessType = $("#commandAccessType").val();
    selectedCommandsList.push(command);
    destroyTable();
    buildTable();
    selectedCommandsListIndex += 1;
}

function addPostbackDataToTable(postbackCommandData) {
    selectedCommandsList.push(postbackCommandData);
    destroyTable();
    buildTable();
    selectedCommandsListIndex += 1;
}

function destroyTable() {
    let commandsTable = document.getElementById("commandsTable");
    for (let i = 1; i < selectedCommandsListIndex; ++i) {
        commandsTable.deleteRow(-1);
    }
}


function buildTable() {
    let commandsTable = document.getElementById("commandsTable");
    selectedCommandsList.forEach(function (value, index) {
        let segmentName = getSegmentName(value.Code);
        let row = commandsTable.insertRow(-1);
        let cell1 = row.insertCell(0).innerHTML = '' + value.Name + '<input type="hidden" class="multi-select-command-option" name="SelectedCommands[' + index + '].Id" value="' + value.Id + '" /><input type="hidden" name="SelectedCommands[' + index + '].Name" value="' + value.Name + '" /><input type="hidden" name="SelectedCommands[' + index + '].Code" value="' + value.Code + '" />';
        let cell2 = row.insertCell(1).innerHTML = '' + (segmentName != null) ? segmentName : '';
        let cell3 = row.insertCell(2).innerHTML = '' + getAccessType(value.AccessType, index);
        let cell4 = row.insertCell(3).innerHTML = checkAllSectionsAndSubSectionsMarkup(index, value);
        let cell5 = row.insertCell(4).innerHTML = "<span class='delete-commad-row' Title='Remove Item' onClick='removeCommand(" + index + ")'>Remove</span>";
    });
    clearInputs();
}

function removeCommand(index) {
    let commandsTable = document.getElementById("commandsTable");
    commandsTable.deleteRow(index + 1);
    selectedCommandsList.splice(index, 1);
    selectedCommandsListIndex -= 1;
    destroyTable();
    buildTable();
}

function clearInputs() {
    document.getElementById("commandCategory").options[0].selected = true;
    document.getElementById("departmentLevel").options[0].selected = true;
    document.getElementById("sectionLevel").options[0].selected = true;
    document.getElementById("subSectionLevel").options[0].selected = true;
}

function checkIfCommandAlreadyAdded(commandId) {
    return selectedCommandsList.findIndex((x) => { return x.Id === commandId; });
}

function emptyApproverDropdowns() {
    $("#flowDefinitionSearchError").html("");
    $("#flowDefinitionLevelSearchError").html("");
    $("#flowDefinitionCheckboxList input[type=checkbox]").prop("checked", false);
    $("#flowDefinitionLevelCheckboxList").empty();
    $("#flowDefinitionLevelCheckboxList").append("<li class='multi-select-dropdown-default-option'>Select a flow definition to have it's approval levels listed here</li>");
}

function getSegmentName(code) {
    let segmentSplit = code.toString().split("-");
    if (segmentSplit.length == 1) { return null; }
    if (segmentSplit.length == 2) { return "Department"; }
    if (segmentSplit.length == 3) { return "Section"; }
    if (segmentSplit.length == 4) { return "Sub Section"; }
}


function checkIfFlowDeinfitionLevelsHaveBeenSelected() {
    let checkedOptions = $("#flowDefinitionLevelCheckboxList .multi-select-checkbox-options label input[type=checkbox]:checked").length;
    if (checkedOptions == 0) {
        $("#flowDefinitionLevelSearchError").html("You need to select at least one flow definition level");
        $("html, body").animate({ scrollTop: $('#flowDefinitionLevelContainer').offset().top }, 500);
        return;
    }
    $("form").off("submit");
    $("form").submit();
}

function checkIfFlowDefinitionsHaveBeenSelected() {
    let checkedOptions = $("#flowDefinitionCheckboxList .multi-select-checkbox-options label input[type=checkbox]:checked").length;
    if (checkedOptions == 0) {
        $("#flowDefinitionSearchError").html("You need to select at least one flow definition");
        $("html, body").animate({ scrollTop: $('#flowDefinitionContainer').offset().top }, 500);
        return;
    }
    checkIfFlowDeinfitionLevelsHaveBeenSelected();
}

function emptyFormationDropdowns() {
    $("#commandCategory").prop('selectedIndex', 0);
    $("#departmentLevel").empty();
    $("#departmentLevel").append("<option selected disabled value=''>Select a Department</option>");
    $("#subSectionLevel").empty();
    $("#subSectionLevel").append("<option selected disabled value=''>Select a Sub Section</option>");
    $("#sectionLevel").empty();
    $("#sectionLevel").append("<option selected disabled value=''>Select a Section</option>");
    $("#commandAccessType").prop('selectedIndex', 0);
}


function checkAllSectionsAndSubSectionsMarkup(index, command) {
    let commandCodeArr = command.Code.split("-");
    if (commandCodeArr.length == 3) { return '<label class="inline-check-box">All Sub-Sections <input type="checkbox" value="true" name="SelectedCommands[' + index + '].SelectAllSubSections" ' + ((command.SelectAllSubSections) ? 'checked=true' : '') + ' onChange ="cacheAllSubSectionState(' + index + ',this)"  /><label>'; }
    if (commandCodeArr.length == 2) { return '<label class="inline-check-box">All Sections <input type="checkbox" value="true" name="SelectedCommands[' + index + '].SelectAllSections" ' + ((command.SelectAllSections) ? 'checked=true' : '') + ' onChange ="cacheAllSectionState(' + index + ',this)" /></label><br/><label class="inline-check-box">All Sub-Sections <input type="checkbox" value="true" name="SelectedCommands[' + index + '].SelectAllSubSections" ' + ((command.SelectAllSubSections) ? 'checked=true' : '') + ' onChange ="cacheAllSubSectionState(' + index + ',this)" /></label>'; }
    return "";
}


function cacheAllSectionState(index, state) {
    selectedCommandsList[index].SelectAllSections = $(state).prop("checked")
}


function cacheAllSubSectionState(index, state) {
    selectedCommandsList[index].SelectAllSubSections = $(state).prop("checked")
}

function getAccessType(accessTypeId, index) {
    return (accessTypeId == approver) ? "Approver<input type='hidden' value='" + accessTypeId + "' name='SelectedCommands[" + index + "].AccessType' />" : "Viewer<input type='hidden' value='" + accessTypeId + "' name='SelectedCommands[" + index + "].AccessType' />";
}

function clearAllCommandsWithAccessType(accessType) {
    if (selectedCommandsList.findIndex(function (val) { return parseInt(val.AccessType) == parseInt(accessType) }) == -1) { return; }
    let tempArrayWithoutAccessType = [];
    selectedCommandsList.map(function (val, index) {
        if (parseInt(val.AccessType) != parseInt(accessType)) {
            tempArrayWithoutAccessType.push(val);
        }
    });

    selectedCommandsList = tempArrayWithoutAccessType;
    destroyTable();
    buildTable();
}
