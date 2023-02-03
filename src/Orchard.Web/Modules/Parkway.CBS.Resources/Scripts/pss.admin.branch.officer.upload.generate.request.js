$(document).ready(function () {
    var escortServiceTypeMap = new Map();
    var tacticalSquadMap = new Map();
    var subLevelCommandsMap = new Map();
    var stateFormationsMap = new Map();

    $("#serviceCategory").change(function () {
        $("#escortServiceCategoryExtraFields").hide();
        makeExtraFieldsRequired(false);
        if (escortServiceTypeMap.has(parseInt($(this).val()))) {
            buildCategoryTypeDropdown(escortServiceTypeMap.get(parseInt($(this).val())))
        } else { getCategoryTypesForServiceCategory($(this).val()) }
        moveCommandDropdown();
        getStateCommands(getStateId(), getLgaId());
    });


    $("#categoryType").change(function () {
        if (escortServiceTypeMap != undefined && escortServiceTypeMap.has(parseInt($("#serviceCategory").val())) && escortServiceTypeMap.get(parseInt($("#serviceCategory").val()))[getIndexOfCategoryTypeInServiceCategoryMap(parseInt($("#serviceCategory").val()), parseInt($(this).val()))].ShowExtraFields) {
            $("#escortServiceCategoryExtraFields").show();
            makeExtraFieldsRequired(true);
            resetCommandDropdown();
        } else { $("#escortServiceCategoryExtraFields").hide(); makeExtraFieldsRequired(false); }
        moveCommandDropdown();
    });


    $("#commandType").change(function () {
        disableAndMakeNotRequired("#commandList");
        disableAndMakeNotRequired("#tacticalSquad");
        $("#commandDiv").hide();
        $("#commandList")[0].options[0].selected;
        $("#tacticalSquadDiv").hide();
        getTacticalSquad(this.value);
    });


    $("#tacticalSquad").change(function () {
        $("#commandList")[0].options[0].selected;
        moveCommandDropdownToDefault();
        getNextLevelCommands(tacticalSquadMap.get($('#commandType').val()).find(function (val) { return val.Id == this.value; }, this).Code);
    });


    $("#originState").change(function () {
        moveCommandDropdown();
        if (!$("#tacticalSquad").is(":visible")) {
            showStateLoaderAndDisable();
            getStateCommands(this.value);
        }
    });


    $("#originLga").change(function () {
        if (!$("#tacticalSquad").is(":visible")) {
            showStateLoaderAndDisable();
            getStateCommands(getStateId(), this.value);
        }
    });


    $("#state").change(function () {
        moveCommandDropdown();
        if (!$("#originState").is(":visible") && !$("#tacticalSquad").is(":visible")) {
            showStateLoaderAndDisable();
            getStateCommands(this.value);
        }
    });


    $("#lga").change(function () {
        if (!$("#originState").is(":visible") && !$("#tacticalSquad").is(":visible")) {
            showStateLoaderAndDisable();
            getStateCommands(getStateId(), this.value);
        }
    });


    function makeExtraFieldsRequired(required) {
        $("#escortServiceCategoryExtraFields .form-control, #escortServiceCategoryExtraFields .form-control").prop("required", required);
    }


    function getCategoryTypesForServiceCategory(serviceCategoryId) {
        let url = "/Admin/Police/BranchOfficer/Get-Service-Category-Types";
        let data = { "serviceCategoryId": serviceCategoryId, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
        $("#serviceCategory").prop("disabled", true);
        $("#escortServiceCategoryLoader").show();
        $.post(url, data, function (response) {
            if (!response.Error) {
                escortServiceTypeMap.set(parseInt(serviceCategoryId), response.ResponseObject);
                buildCategoryTypeDropdown(response.ResponseObject);
            } else {
                $("#categoryType").empty();
                $("#categoryType").hide();
            }
        }).always(function () {
            $("#escortServiceCategoryLoader").hide();
            $("#serviceCategory").prop("disabled", false);
        });
    }


    function getTacticalSquad(commandTypeId) {
        $("#commandTypeLoader").show();
        $("#commandType").prop("disabled", true);
        $("#commandTypeSearchError").html("");
        $("#commandSearchError").html("");
        if (!tacticalSquadMap.has(commandTypeId)) {
            let url = "/Admin/Police/BranchOfficer/Get-Escort-Tactical-Squads";
            let data = { "commandTypeId": commandTypeId, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
            $.post(url, data, function (response) {
                if (!response.Error) {
                    if (response.ResponseObject.length != 0) {
                        tacticalSquadMap.set(commandTypeId, response.ResponseObject);
                        buildTacticalSquadDropdown(tacticalSquadMap.get(commandTypeId));
                    } else { moveCommandDropdown(); getStateCommands(getStateId(), getLgaId()); }
                } else {
                    $("#commandTypeSearchError").html(response.ResponseObject);
                }
            }).always(function () {
                $("#commandTypeLoader").hide();
                $("#commandType").prop("disabled", false);
            }).fail(function () {
                $("#commandTypeLoader").hide();
                $("#commandType").prop("disabled", false);
            });
        } else {
            $("#commandTypeLoader").hide();
            $("#commandType").prop("disabled", false);
            buildTacticalSquadDropdown(tacticalSquadMap.get(commandTypeId));
        }
    }


    function buildTacticalSquadDropdown(tacticalSquads) {
        $("#tacticalSquad").empty();
        $("#tacticalSquad").append("<option value='' selected disabled>Select tactical squad</option>");
        tacticalSquads.forEach(function (squad) {
            $("#tacticalSquad").append("<option value='" + squad.Id + "'>" + squad.Name + "</option>");
        });
        $("#tacticalSquadDiv").show();
        enableAndMakeRequired("#tacticalSquad");
    }


    function getNextLevelCommands(code) {
        $("#tacticalSquadloader").show();
        $("#tacticalSquad").prop("disabled", true);
        $("#tacticalSquadSearchError").html("");
        $("#commandSearchError").html("");
        if (!subLevelCommandsMap.has(code)) {
            let url = "/Admin/Police/BranchOfficer/Get-Next-Level-Commands-With-Code";
            let data = { "code": code, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
            $.post(url, data, function (response) {
                if (!response.Error) {
                    if (response.ResponseObject.length != 0) {
                        subLevelCommandsMap.set(code, response.ResponseObject);
                        buildCommandDropdown(subLevelCommandsMap.get(code));
                    } else {
                        $("#commandList").empty();
                        enableAndMakeRequired("#commandList");
                        $("#commandList").append("<option value='' selected disabled>Select command/formation</option>");
                        $("#commandSearchError").html("No commands found for selected tactical squad");
                        $("#commandDiv").show();
                    }
                } else {
                    $("#tacticalSquadSearchError").html(response.ResponseObject);
                }
            }).always(function () {
                $("#tacticalSquadloader").hide();
                $("#tacticalSquad").prop("disabled", false);
            }).fail(function () {
                $("#tacticalSquadloader").hide();
                $("#tacticalSquad").prop("disabled", false);
            });
        } else {
            $("#tacticalSquadloader").hide();
            $("#tacticalSquad").prop("disabled", false);
            buildCommandDropdown(subLevelCommandsMap.get(code));
        }
    }


    function buildCommandDropdown(commands) {
        $("#commandList").empty();
        $("#commandList").val("");
        $("#commandList").append("<option value='' selected disabled>Select command/formation</option>");
        $("#commandList").append("<option value='0'>Any</option>");
        commands.forEach(function (command) {
            $("#commandList").append("<option value='" + command.Id + "'>" + command.Name.trim() + "</option>");
        });
        $("#commandDiv").show();
        enableAndMakeRequired("#commandList");
    }


    function enableAndMakeRequired(fieldId) {
        $(fieldId).prop("disabled", false);
        $(fieldId).prop("required", true);
    }


    function disableAndMakeNotRequired(fieldId) {
        $(fieldId).prop("required", false);
        $(fieldId).prop("disabled", true);
        $(fieldId).val("");
    }


    function buildCategoryTypeDropdown(categoryTypes) {
        $("#categoryType").empty();
        if (categoryTypes == undefined || categoryTypes == null || categoryTypes.length < 1) {
            $("#categoryType").prop("required", false);
            $("#escortServiceCategoryType").hide();
            return;
        }
        $("#categoryType").append("<option selected disabled value=''>Select a Category Type</option>");
        categoryTypes.forEach(function (val) {
            $("#categoryType").append("<option value ='" + val.Id + "'>" + val.Name + "</option>");
        });
        $("#escortServiceCategoryType").show();
        $("#categoryType").prop("required", true);
    }


    function getStateCommands(stateId, lgaId) {
        if (stateId == "" || stateId == 0 || stateId == null) { return; }
        let stateFormationMapKey = parseInt(stateId) + "-" + (Number.isNaN(parseInt(lgaId)) ? 0 : parseInt(lgaId));
        $("#commandTypeSearchError").html("");
        $("#commandSearchError").html("");
        $("#commandloader").show();
        $("#commandList").prop("disabled", true);
        if (!stateFormationsMap.has(stateFormationMapKey)) {
            let url = "/Admin/Police/BranchOfficer/Get-State-Formations";
            let data = { "stateId": stateId, "lgaId": lgaId, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
            $.post(url, data, function (response) {
                if (!response.Error) {
                    if (response.ResponseObject.length != 0) {
                        stateFormationsMap.set(stateFormationMapKey, response.ResponseObject);
                        buildCommandDropdown(stateFormationsMap.get(stateFormationMapKey));
                    } else {
                        $("#commandList").empty();
                        enableAndMakeRequired("#commandList");
                        $("#commandList").append("<option value='' selected disabled>Select command/formation</option>");
                        $("#commandSearchError").html("No commands found for selected state and LGA");
                        $("#commandDiv").show();
                    }
                } else {
                    $("#commandTypeSearchError").html(response.ResponseObject);
                }
            }).always(function () { hideStateLoaderAndEnable(); $("#commandloader").hide(); $("#commandList").prop("disabled", false); })
                .fail(function () { hideStateLoaderAndEnable(); $("#commandloader").hide(); $("#commandList").prop("disabled", false); });
        } else {
            buildCommandDropdown(stateFormationsMap.get(stateFormationMapKey));
            hideStateLoaderAndEnable();
            $("#commandloader").hide();
            $("#commandList").prop("disabled", false);
        }
    }


    function getStateId() {
        if ($("#originState").is(":visible")) {
            return $("#originState").val();
        } else {
            return $("#state").val();
        }
    }


    function getLgaId() {
        if ($("#originState").is(":visible")) {
            return $("#originLga").val();
        } else {
            return $("#lga").val();
        }
    }


    function getIndexOfCategoryTypeInServiceCategoryMap(serviceCategoryId, categoryTypeId) {
        return escortServiceTypeMap.get(serviceCategoryId).findIndex(function (val) { return val.Id == categoryTypeId });
    }

    function moveCommandDropdown() {
        if (!$("#tacticalSquad").is(":visible")) {
            if ($("#originState").is(":visible")) {
                $("#escortServiceOriginFormations").append($("#commandDiv"));
                $("#tacticalSquadFormations").empty();
                $("#escortServiceDeliveryFormations").empty();
            } else {
                $("#escortServiceDeliveryFormations").append($("#commandDiv"));
                $("#tacticalSquadFormations").empty();
                $("#escortServiceOriginFormations").empty();
            }
        }
    }

    function resetCommandDropdown() {
        $("#commandList").empty();
        $("#commandList").append("<option value='' selected disabled>Select command/formation</option>");
    }


    function moveCommandDropdownToDefault() {
        $("#commandSearchError").html("");
        $("#tacticalSquadFormations").append($("#commandDiv"));
        $("#escortServiceOriginFormations").empty();
        $("#escortServiceDeliveryFormations").empty();
    }


    function showStateLoaderAndDisable() {
        if (!$("#tacticalSquad").is(":visible")) {
            if ($("#originState").is(":visible")) {
                $("#originState").prop("disabled", true);
                $("#originLga").prop("disabled", true);
            } else {
                $("#state").prop("disabled", true);
                $("#lga").prop("disabled", true);
            }
        }
    }


    function hideStateLoaderAndEnable() {
        if (!$("#tacticalSquad").is(":visible")) {
            if ($("#originState").is(":visible")) {
                $("#originState").prop("disabled", false);
                $("#originLga").prop("disabled", false);
            } else {
                $("#state").prop("disabled", false);
                $("#lga").prop("disabled", false);
            }
        }
    }
});