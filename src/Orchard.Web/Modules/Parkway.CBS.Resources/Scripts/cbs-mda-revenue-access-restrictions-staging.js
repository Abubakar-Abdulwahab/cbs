$(document).ready(function () {
    var additionsMap = new Map();
    var removalsMap = new Map();

    if (isEdit) {
        stagingOnMdaChange();
        $("#assignProviderFormSubmit").click(function () {
            $("#ReferenceHelpText").hide();
            $("#MDARevenueHeadAccessRestrictionsReference").val("");
            if (additionsMap.size == 0 && removalsMap.size == 0) { return; }
            $("#confirmModalBody").hide();
            $("#updateStagingloaderr").show();
            $("#submitConfirmModalBtn").attr("disabled", true);
            $("#cancelConfirmModalBtn").attr("disabled", true);
            updateStagingData();
        });
    }

    function stagingOnMdaChange() {
        $(".mdas").change(function (e) {
            if (e.currentTarget.checked) {
                stagingOnDefaultRhChange();
                $(".rh").change(function (e) {
                    stagingOnRhChange(e);// register event handler on newly populated Revenue Heads.
                });
                if (removalsMap.has(e.currentTarget.value.toString())) {
                    removalsMap.delete(e.currentTarget.value.toString());
                } // if in removals map we delete from removals map.
                if (originalMap[e.currentTarget.value] != undefined) { // if its in the originals map it was previously selected so we don't need to add to additions map.
                    return;
                }
                if (additionsMap.has(e.currentTarget.value.toString())) { return; }// if added already to additions map we don't want to add again.
                additionsMap.set(e.currentTarget.value.toString(), [0]); // adding to additions map after it has gone through all the conditions.

            } else {
                if (additionsMap.has(e.currentTarget.value.toString())) { additionsMap.delete(e.currentTarget.value.toString()); }// if in additions map we remove before adding to removals map.
                if (originalMap[e.currentTarget.value] == undefined) { // if it wasn't previously selected(in the originals map) then we do not need to add to removals map.
                    return;
                }
                if (removalsMap.has(e.currentTarget.value.toString())) { return; } //if already in removals map then we do not add again.
                removalsMap.set(e.currentTarget.value.toString(), originalMap[e.currentTarget.value]); // add to removals map after going through all the conditions.
            }
        });
    }

    //this is used to handle AJAX request that occurred from
    //external scripts attached to this. 
    $(document).ajaxComplete(function (event, xhr, settings) {
        const url = "/Admin/ExternalPaymentProvider/RevenueHeadsPerMda";
        if (settings.url == url) {
            stagingOnDefaultRhChange();
            $(".rh").change(function (e) {
                stagingOnRhChange(e);
            });
        }
    });

    function stagingOnRhChange(e) {
        if (e.currentTarget.checked) {
            let mdaId = e.currentTarget.parentElement.parentElement.classList[0].split("-")[2].toString();
            if (removalsMap.has(mdaId) && removalsMap.get(mdaId).find(function (val) { return val == e.currentTarget.value.toString() }) != undefined) {
                removeRhFromMap(removalsMap, mdaId, e.currentTarget.value);
            }
            if (originalMap[mdaId] != undefined && originalMap[mdaId].find(function (val) { return val == e.currentTarget.value.toString(); }) != undefined) {
                return;
            }

            if (!additionsMap.has(mdaId)) {
                additionsMap.set(mdaId, [e.currentTarget.value.toString()]);
            }
            else if (additionsMap.has(mdaId) && (additionsMap.get(mdaId).find(function (val) { return val == e.currentTarget.value.toString(); }) == undefined)) {
                additionsMap.get(mdaId).push(e.currentTarget.value.toString());
            }
            else {
            }
        } else {
            let mdaId = e.currentTarget.parentElement.parentElement.classList[0].split("-")[2].toString();
            if (additionsMap.has(mdaId) && additionsMap.get(mdaId).find(function (val) { return val == e.currentTarget.value; }) != undefined) {
                removeRhFromMap(additionsMap, mdaId, e.currentTarget.value);
            }
            if (originalMap[mdaId] != undefined && originalMap[mdaId].find(function (val) { return val == e.currentTarget.value; }) == undefined) {
                //this does not need to be added to the removals map because it wasn't previously selected.
                return;
            }
            if (!removalsMap.has(mdaId)) {
                removalsMap.set(mdaId, [e.currentTarget.value.toString()]);
            }
            else if (removalsMap.has(mdaId) && (removalsMap.get(mdaId).find(function (val) { return val == e.currentTarget.value.toString(); }) == undefined)) {
                removalsMap.get(mdaId).push(e.currentTarget.value.toString());
            } else {
            }
        }
    }

    function stagingOnDefaultRhChange() {
        $(".default-rh").off("change");
        $(".default-rh").change(function (e) {
            let mdaId = e.currentTarget.parentElement.parentElement.classList[0].split("-")[3].toString();
            if (e.currentTarget.checked) {
                if (additionsMap.has(mdaId)) { additionsMap.delete(mdaId); }
                if (removalsMap.has(mdaId)) {
                    removalsMap.delete(mdaId);
                }
                if (originalMap[mdaId] != undefined && originalMap[mdaId].find(function (val) { return val == e.currentTarget.value.toString(); }) != undefined) {
                    return;
                }
                if (originalMap[mdaId] != undefined) {
                    removalsMap.set(mdaId, originalMap[mdaId].map((x) => x));
                }
                if (!additionsMap.has(mdaId)) {
                    additionsMap.set(mdaId, [e.currentTarget.value.toString()]);
                }
                else if (additionsMap.has(mdaId) && (additionsMap.get(mdaId).find(function (val) { return val == e.currentTarget.value.toString(); }) == undefined)) {
                    additionsMap.delete(mdaId);
                    additionsMap.set(mdaId, [e.currentTarget.value.toString()]);
                }
            } else {
                if (additionsMap.has(mdaId)) {
                    additionsMap.delete(mdaId);
                }
                if (originalMap[mdaId] != undefined) {
                    if (!removalsMap.has(mdaId)) {
                        removalsMap.set(mdaId, [e.currentTarget.value.toString()]);
                    }
                    else if (removalsMap.has(mdaId) && (removalsMap.get(mdaId).find(function (val) { return val == e.currentTarget.value.toString(); }) == undefined)) {
                        removalsMap.delete(mdaId);
                        removalsMap.set(mdaId, [e.currentTarget.value.toString()]);
                    }
                    return;
                }
            }
        });
    }


    function removeRhFromMap(theMap, mdaId, rhId) {
        let index = theMap.get(mdaId).findIndex(function (val) { return val == rhId.toString() });
        theMap.get(mdaId).splice(index, 1);
    }

    function mapToJson(mapObj) {
        let m = {};
        mapObj.forEach(function (rh, mda) {
            m[mda] = rh;
        });
        return JSON.stringify(m);
    }

    function updateStagingData() {
        let url = "/Admin/ExternalPaymentProvider/X/Update-Restrictions";
        let removalsMapObj = mapToJson(removalsMap);
        let additionsMapObj = mapToJson(additionsMap);
        let data = { "additions": additionsMapObj, "removals": removalsMapObj, "providerId": selectedProviderId, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };

        $.post(url, data, function (response) {
            if (!response.Error) {
                $("#MDARevenueHeadAccessRestrictionsReference").val(response.ResponseObject);
                $("#confirmModalBody").show();
                $("#updateStagingloaderr").hide();
                $("#submitConfirmModalBtn").attr("disabled", false);
                $("#cancelConfirmModalBtn").attr("disabled", false);
            } else {
                $("#ReferenceHelpText").show();
                $("#ReferenceHelpText").html(response.ResponseObject.Text);
                $("#updateStagingloaderr").hide();
            }
        });
    }
});