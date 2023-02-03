$(document).ready(function () {
    var ajaxContext = null;
    var ajaxContextTwo = null;
    var revHeadCollection = new Map();
    var mdaCollection = new Map();
    var selectedRhAndMdaCollection = new Map();

    $(".mdas").attr("disabled", false);
    $("#assignPermissionsFormSubmit").attr("disabled", false);
    $("#assignPermissionsFormCancel").attr("disabled", false);

    if (selectedMdas != null && selectedMdas.length > 0) { getRevenueHeadsPerMda(selectedMdas); }

    $("#assignPermissionsForm").submit(function (e) {
        buildConstrainedRhAndMdaCollection();
    });

    $("#assignPermissionsFormSubmit").click(function () {
        showConfirmModal();
    });

    onChangeMda();

    $("#permissionList").change(function () {
        $("#permissionList").prop("disabled", true);
        $("#mdaHelpText").hide();
        let selectedAccessType = event.currentTarget.value;
        //if (selectedMdaId == 0) { return; }
        removeAllRevHeads();
        removeMdas();
        if (mdaCollection.has(selectedAccessType)) {
            //build the list
            BuildMdaDropdown(mdaCollection.get(selectedAccessType));
            $("#permissionList").prop("disabled", false);
        } else {
            //make a request
            getMdasForAccessType(selectedAccessType);
            
        }
    });




    function getRevenueHeadsPerMda(filteredMdaList) {
        $("#profileloader").show();
        const url = "/Admin/RevenueHeadPermissions/RevenueHeadsPerMda";
        const data = { "mdaIds": JSON.stringify(filteredMdaList), "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
        if (ajaxContext != null) { ajaxContext.abort(); }
        ajaxContext = $.post(url, data, function (response) {
            if (!response.Error) {
                if (response.ResponseObject.length == 0) { $("#profileloader").hide(); return; }
                for (let i = 0; i < response.ResponseObject.length; ++i) {
                    addRevHead(response.ResponseObject[i], response.ResponseObject[i][0].MDAId);
                    buildRevHeadDropDown(response.ResponseObject[i], response.ResponseObject[i][0].MDAId, "#rhList", true);
                }
            } else {
                $("#helpText").html(response.ResponseObject);
                $("#helpText").show();
                $("#profileloader").hide();
            }
        });
    }

    function getMdasForAccessType(accessType) {
        $("#mdaprofileloader").show();
        const url = "/Admin/RevenueHeadPermissions/MdasForAccessType";
        const data = { "accessType": accessType, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
        if (ajaxContextTwo != null) { ajaxContextTwo.abort(); }
        ajaxContextTwo = $.post(url, data, function (response) {
            if (!response.Error) {
                if (response.ResponseObject.length == 0) {
                    $("#mdaHelpText").html("No MDAs found for selected Access Type");
                    $("#mdaHelpText").show();
                    $("#profileloader").hide();
                    return;
                }
                addMDA(accessType, response.ResponseObject);
                BuildMdaDropdown(response.ResponseObject);
                //buildRevHeadDropDown(response.ResponseObject[i], response.ResponseObject[i][0].MDAId, "#rhList", true);
            } else {
                $("#mdaHelpText").html(response.ResponseObject);
                $("#mdaHelpText").show();
                $("#mdaprofileloader").hide();
            }
        }).always(function () {
            $("#permissionList").prop("disabled", false);
            $("#mdaprofileloader").hide();
        });
    }

    function addRevHead(revHeads, mdaId) {
        revHeadCollection.set(mdaId, revHeads);
    }

    function addMDA(accessType, mdas) {
        mdaCollection.set(accessType, mdas);
    }


    function buildRevHeadDropDown(revHeads, mdaId, selector, checkAllRh = false) {
        let allChecked = (SelectedRhAndMdas[mdaId] != null && SelectedRhAndMdas[mdaId][0] == 0) ? true : false;
        let listItems = "<li class = 'rev-head-" + mdaId + "' ><div class='divider'>" + $(".mdas[value='" + mdaId + "']")[0].parentElement.innerText + "</div></li>";
        if (allChecked) {
            listItems += "<li class = 'default-rev-head-" + mdaId + "'><label><input type='checkbox' value=" + 0 + " name = 'SelectedRevenueHeads' id='mda" + mdaId + "' checked />  All</label></li>";
        } else {
            listItems += "<li class = 'default-rev-head-" + mdaId + "'><label><input type='checkbox' value=" + 0 + " name = 'SelectedRevenueHeads' id='mda" + mdaId + "' />  All</label></li>";
        }
        for (let r = 0; r < revHeads.length; ++r) {
            if (SelectedRhAndMdas[mdaId] != null && SelectedRhAndMdas[mdaId].length > 0) {
                if (SelectedRhAndMdas[mdaId].find(function (val) { return (val == parseInt(revHeads[r].Id)) || (val == 0); }) != null) {
                    listItems += "<li class = 'rev-head-" + mdaId + "'><label><input type='checkbox' value=" + parseInt(revHeads[r].Id) + " name = 'SelectedRevenueHeads' class='rh' checked/> " + revHeads[r].Name + "</label></li>";
                } else {
                    listItems += "<li class = 'rev-head-" + mdaId + "'><label><input type='checkbox' value=" + parseInt(revHeads[r].Id) + " name = 'SelectedRevenueHeads' class='rh' /> " + revHeads[r].Name + "</label></li>";
                }
            } else {
                listItems += "<li class = 'rev-head-" + mdaId + "'><label><input type='checkbox' value=" + parseInt(revHeads[r].Id) + " name = 'SelectedRevenueHeads' class='rh' /> " + revHeads[r].Name + "</label></li>";
            }
        }
        $(selector).prepend(listItems);
        if (allChecked) { checkAll(".rev-head-" + mdaId + ""); }
        scrollToElement("#rhList", "#rhList > li:nth-child(1)");
        $("#profileloader").hide();
        $("#mda" + mdaId + "").click(function () {
            if (this.checked) {
                checkAll(".rev-head-" + mdaId + "");
            } else { unCheckAll(".rev-head-" + mdaId + ""); }
        });
        $(".rh").off("change");
        $(".rh").change(function () { if (event.currentTarget.checked) { $(this).attr("checked", true); } });
        if (checkAllRh && !allChecked && SelectedRhAndMdas[mdaId] == null) { $("#mda" + mdaId + "").attr("checked", true); checkAll(".rev-head-" + mdaId + ""); }
        if (SelectedRhAndMdas[mdaId] != null) { delete (SelectedRhAndMdas[mdaId]); }
    }

    function BuildMdaDropdown(mdas) {
        let listItems = "";
        for (let i = 0; i < mdas.length; ++i) {
            listItems += "<li><label><input type='checkbox' class='mdas' id='mdaItem" + mdas[i].Id + "' value=" + parseInt(mdas[i].Id) + " name = 'SelectedMdas' /> " + mdas[i].Name + "</label></li>";
        }

        $("#mdaList").append(listItems);
        onChangeMda();
    }


    function filterMdaIds(mdaList, removeFetchedMdas) {
        let filteredMdaList = [];
        for (let i = 0; i < mdaList.length; ++i) {
            if (removeFetchedMdas && !revHeadCollection.has(parseInt(mdaList[i].value))) {
                filteredMdaList.push(parseInt(mdaList[i].value));
            }
        }
        return filteredMdaList;
    }


    function filterRhIds(rhList) {
        let filteredRhList = [];
        for (let i = 0; i < rhList.length; ++i) {
            filteredRhList.push(parseInt(rhList[i].value));
        }
        return filteredRhList;
    }


    function buildConstrainedRhAndMdaCollection() {
        selectedRhAndMdaCollection = new Map();
        let rhIds = filterRhIds($(".rh:checked"));
        revHeadCollection.forEach(function (rh, mda) {
            let rhArr = [];
            for (let n = 0; n < rhIds.length; ++n) {
                if (rh.find(function (x) { return x.Id == rhIds[n]; }) != null) {
                    rhArr.push(rhIds[n]);
                }
            }
            if (rhArr.length >= rh.length && $(".default-rev-head-" + mda + "").find("input[type='checkbox']").attr("checked")) {
                rhArr = [0];
            }
            if (rhArr.length > 0) { selectedRhAndMdaCollection.set(mda, rhArr); }
        });
        $("#selectedRhAndMdas").val(mapToJson(selectedRhAndMdaCollection));
    }


    function removeRevHeads(mdaId, selector) {
        $(selector).remove(".default-rev-head-" + mdaId + "");
        $(selector).remove(".rev-head-" + mdaId + "");
    }

    function removeAllRevHeads() {
        $("#rhList").empty();
    }

    function removeMdas() {
        $("#mdaList").empty();
    }

    function mapToJson(mapObj) {
        let m = {};
        mapObj.forEach(function (rh, mda) {
            m[mda] = rh;
        });
        return JSON.stringify(m);
    }

    function showConfirmModal() {
        $("#confirmModal .items").remove();
        $("#confirmProfileloader").show();
        let mdas = $(".mdas:checked");
        for (let i = 0; i < mdas.length; ++i) {
            let currentValue = mdas[i].value;
            let mdaRhs = $(".rev-head-" + currentValue + "").find("input[type=checkbox]:checked");
            if (mdaRhs.length < 1) { $("#mdaItem" + currentValue).attr("checked", false); continue; }
            let ntable = "<table class='items'><thead><th>" + mdas[i].parentElement.innerText + "</th></thead>";
            ntable += "<tbody>";

            for (let d = 0; d < mdaRhs.length; ++d) {
                let row = "<tr><td>" + mdaRhs[d].parentElement.innerText + "</td></tr>";
                ntable += row;
            }
            ntable += "</tbody>";
            ntable += "</table>";
            $("#confirmModalContent").append(ntable);
        }
        $("#submitConfirmModalBtn").attr("disabled", false);
        $("#cancelConfirmModalBtn").attr("disabled", false);
        $("#confirmProfileloader").hide();
    }

    function checkAll(mdaIdClass) {
        $(mdaIdClass).find("input[type='checkbox']").prop("checked", true);
        $(mdaIdClass).find("input[type='checkbox']").prop("disabled", true);
    }

    function unCheckAll(mdaIdClass) {
        $(mdaIdClass).find("input[type='checkbox']").prop("checked", false);
        $(mdaIdClass).find("input[type='checkbox']").prop("disabled", false);
    }

    function scrollToElement(containerSelector, elementSelector) {
        $(containerSelector).animate({
            scrollTop: $(elementSelector).offset().top - $(containerSelector).offset().top + $(containerSelector).scrollTop()
        }, 600);
    }

    function onChangeMda() {
        $(".mdas").change(function () {
            $("#helpText").hide();
            let selectedMdaId = parseInt(event.currentTarget.value);
            let checked = event.currentTarget.checked;
            if (selectedMdaId == 0) { return; }
            if (checked) {
                $(this).attr("checked", true);
                if (revHeadCollection.has(selectedMdaId)) {
                    //build the list
                    buildRevHeadDropDown(revHeadCollection.get(selectedMdaId), selectedMdaId, "#rhList", true);
                } else {
                    //make a request
                    getRevenueHeadsPerMda(filterMdaIds($(".mdas:checked"), true));
                }
            } else {
                removeRevHeads(selectedMdaId, "#rhList li");
            }

        });
    }
});