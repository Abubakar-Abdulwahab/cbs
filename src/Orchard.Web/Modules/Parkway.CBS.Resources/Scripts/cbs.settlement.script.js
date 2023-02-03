$(document).ready(function () {
    handleFixedPart(selectedFrq);
    var ajaxContext = null;
    var revHeadCollection = new Map();
    var selectedRhAndMdaCollection = new Map();
    var confirmed = false;
    var formIsValid = false;

    $(".mdas").attr("disabled", false);
    $(".default-mda").attr("disabled", false);
    if (selectedMdas != null && selectedMdas.length > 0) { 
        getRevenueHeadsPerMda(selectedMdas);
    }

    $("#paymentChannelItem0").change(function (e) {
        if (e.currentTarget.checked) {
            $(".payment-channels").prop("checked", true);
            $(".payment-channels").prop("disabled", true);
        } else {
            $(".payment-channels").prop("checked", false);
            $(".payment-channels").prop("disabled", false);
        }
    });

    if ($('#EODSettlement').is(':checked')) {
        toggleRequired(0);
        toggleSettlementFreqState(false);
    }


    $('#EODSettlement').click(function () {
        if ($('#EODSettlement').is(':checked')) {
            //disable settlement frequency
            toggleSettlementFreqState(false);
            toggleRequired(0);
            handleFixedPart(0);
            //set time to 12:00AM
            $("#dateStartTime").val("00:00");
        } else {
            toggleSettlementFreqState(true);
            toggleRequired(selectedFrq);
            handleFixedPart(selectedFrq);
        }
    });


    function toggleSettlementFreqState(boolVal) {
        $("#FrequencyType").prop("required", boolVal);
        $('#FrequencyType').prop('disabled', !boolVal);

        //$("#datepicker3").prop("required", boolVal);
        //$("#datepicker3").prop('disabled', !boolVal);

        $("#dateStartTime").prop("required", boolVal);
        $("#dateStartTime").prop('readonly', !boolVal);
    }

    $(".default-mda").change(function () {
        if (this.checked) {
            $(".mdas").prop("disabled", true);
            $(".mdas").prop("checked", true);
            $("#rhList").empty();
            $("#rhList").prepend("<li class = 'rev-head-0'><label><input type='checkbox' value='0' class='rh' checked/> Apply to all Revenue Heads </label></li>");
        } else {
            $(".mdas").prop("disabled", false);
            $(".mdas").prop("checked", false);
            $("#rhList").empty();
        }
    });

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

    function onApplyAllChange(event) {
        $('.revChkbx').prop('checked', event.target.checked);
        $('.revChkbx').prop('disabled', event.target.checked);
    }


    $('#applyAll').change(function (event) {
        onApplyAllChange(event);
    });

    function onRevenueHeadCheckboxChange() {
    }

    $('.revChkbx').change(function () {
        onRevenueHeadCheckboxChange();
    });


    $('#FrequencyType').on('change', function () {
        selectedFrq = $(this).val();
        handleFixedPart(selectedFrq);
    });


    //return true if atleast on check box is checked
    function validateMonthly() {
        //select at least one
        //get all check boxs
        var checkboxes = document.getElementsByClassName('monthlyCheckBoxes');
        for (var i = 0; i < checkboxes.length; i++) {
            if (checkboxes[i].checked) {
                return true;
            }
        }
        $("#f_month").append('<span class="field-validation-error" style="color:#990808">Please, check at least one month</span>');
        return false;
    }


    //return true if atleast one check box is checked
    function validateWeekly() {
        //select at least one
        //get all check boxs
        var checkboxes = document.getElementsByClassName('weeklyCheckBoxes');
        for (var i = 0; i < checkboxes.length; i++) {
            if (checkboxes[i].checked)
            { return true; }
        }
        $("#f_day").append('<span class="field-validation-error" style="color:#990808">Please, check at least one day</span>');
        return false;
    }

    function toggleRequired(selectedValue) {
        switch (Number(selectedValue)) {
            case 1:
                $("#dailyfq").prop("required", true);
                $("#dailyfq").prop("min", 1);
                $("#monthlyfq").prop("required", false);
                $("#monthlyfq_week_select").prop("required", false);
                $("#monthlyfq_day_select").prop("required", false);
                $("#yearlyfq").prop("required", false);
                break;
            case 2:
                $("#dailyfq").prop("required", false);
                $("#dailyfq").prop("min", 0);
                //weekly
                $("#monthlyfq").prop("required", false);
                $("#monthlyfq_week_select").prop("required", false);
                $("#monthlyfq_day_select").prop("required", false);
                $("#yearlyfq").prop("required", false);
                break;
            case 3:
                $("#dailyfq").prop("required", false);
                $("#dailyfq").prop("min", 0);
                //month
                $("#monthlyfq_week_select").prop("required", true);
                $("#monthlyfq_day_select").prop("required", true);
                $("#yearlyfq").prop("required", false);
                break;
            case 4:
                $("#dailyfq").prop("required", false);
                $("#dailyfq").prop("min", 0);
                //yearly
                $("#monthlyfq_week_select").prop("required", true);
                $("#monthlyfq_day_select").prop("required", true);
                $("#yearlyfq").prop("required", true);
                $("#yearlyfq").prop("min", 1);
                break;
            default:
                $("#dailyfq").prop("required", false);
                $("#dailyfq").prop("min", 0);
                $("#weeklyfq").prop("required", false);
                $("#monthlyfq").prop("required", false);
                $("#monthlyfq_week_select").prop("required", false);
                $("#monthlyfq_day_select").prop("required", false);
                $("#yearlyfq").prop("required", false);
                break;
        }
    }


    function handleFixedPart(selectedValue) {
        switch (Number(selectedValue)) {
            case 1:
                $('#f_year').hide();
                $('#f_month').hide();
                $('#f_week').hide();
                $('#f_week_number').hide();
                $('#f_day_dropdown').hide();
                $('#f_day').hide();
                $('#f_day_number').show();
                break;
            case 2:
                $('#f_year').hide();
                $('#f_month').hide();
                $('#f_week').hide();
                $('#f_week_number').hide();
                $('#f_day_dropdown').hide();
                $('#f_day').show();
                $('#f_day_number').hide();
                break;
            case 3:
                $('#f_year').hide();
                $('#f_month').show();
                $('#f_week').show();
                $('#f_week_number').hide();
                $('#f_day_dropdown').show();
                $('#f_day').hide();
                $('#f_day_number').hide();
                break;
            case 4:
                $('#f_year').show();
                $('#f_month').show();
                $('#f_week').show();
                $('#f_week_number').hide();
                $('#f_day_dropdown').show();
                $('#f_day').hide();
                $('#f_day_number').hide();
                break;
            default:
                $('#f_year').hide();
                $('#f_month').hide();
                $('#f_week').hide();
                $('#f_week_number').hide();
                $('#f_day').hide();
                $('#f_day_number').hide();
                break;
        }
        toggleRequired(selectedValue);
    }


    $("#submitConfirmModalBtn").on('click', function (e) {
        $("#paymentChannelsList input[type='checkbox']").prop("disabled", false);
        confirmed = true;
        $("#formSubmit").submit();
    })


    $("#formSubmit").on('submit', function (e) {
        if (confirmed) { buildConstrainedRhAndMdaCollection();  return; }
        e.preventDefault();
        validateCheckBoxes();
        if (!formIsValid) { return; }
        showConfirmModal();
        //get the frequency selected
        switch (Number(selectedFrq)) {
            case 2:
                //for weekly
                //do check on check boxes
                if (validateWeekly()) {
                    return;
                }
                break;
            case 3:
            case 4:
                if (validateMonthly()) {
                    return;
                }
                break;
            default:
                return;
        }
        return false;
    });

    $("#myModal").on('hidden.bs.modal', function () {
        formIsValid = false;
    });

    function showConfirmModal() {
        $("#myModal").modal("show");
        $("#confirmModal .items").remove();
        $("#confirmProfileloader").show();
        let mdas = $(".mdas:checked");
        if ($(".default-mda:checked").length > 0) {
            mdas.push($(".default-mda:checked")[0]);
        }
        for (let i = 0; i < mdas.length; ++i) {
            let currentValue = mdas[i].value;
            let mdaRhs = $(".rev-head-" + currentValue + "").find("input[type=checkbox]:checked");
            if (mdaRhs.length < 1) { $("#mdaItem" + currentValue).attr("checked", false); removeRevHeads(currentValue, "#rhList li");; continue; }
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

    function getRevenueHeadsPerMda(filteredMdaList) {
        $("#profileloader").show();
        const url = "Ajax/RevenueheadsPerMda";
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

    function addRevHead(revHeads, mdaId) {
        revHeadCollection.set(mdaId, revHeads);
    }


    function buildRevHeadDropDown(revHeads, mdaId, selector, checkAllRh = false) {
        let allChecked = (SelectedRhAndMdas[mdaId] != null && SelectedRhAndMdas[mdaId][0] == 0) ? true : false;
        let listItems = "<li class = 'rev-head-" + mdaId + "' ><div class='divider'>" + $(".mdas[value='" + mdaId + "']")[0].parentElement.innerText + "</div></li>";
        if (allChecked) {
            listItems += "<li class = 'default-rev-head-" + mdaId + "'><label><input type='checkbox' value=" + 0 + " id='mda" + mdaId + "' checked />  All</label></li>";
        } else {
        listItems += "<li class = 'default-rev-head-" + mdaId + "'><label><input type='checkbox' value=" + 0 + " id='mda" + mdaId + "' />  All</label></li>";
        }
        for (let r = 0; r < revHeads.length; ++r) {
            if (SelectedRhAndMdas[mdaId] != null && SelectedRhAndMdas[mdaId].length > 0) {
                if (SelectedRhAndMdas[mdaId].find(function (val) { return (val == parseInt(revHeads[r].Id)) || (val == 0); }) != null) {
                    listItems += "<li class = 'rev-head-" + mdaId + "'><label><input type='checkbox' value=" + parseInt(revHeads[r].Id) + " name = 'RevenueHeadsSelected[]' class='rh' checked/> " + revHeads[r].Name + " | " + revHeads[r].Code + "</label></li>";
                } else {
                    listItems += "<li class = 'rev-head-" + mdaId + "'><label><input type='checkbox' value=" + parseInt(revHeads[r].Id) + " name = 'RevenueHeadsSelected[]' class='rh' /> " + revHeads[r].Name + " | " + revHeads[r].Code + "</label></li>";
                }
            } else {
            listItems += "<li class = 'rev-head-" + mdaId + "'><label><input type='checkbox' value=" + parseInt(revHeads[r].Id) + " name = 'RevenueHeadsSelected[]' class='rh' /> " + revHeads[r].Name + " | " + revHeads[r].Code + "</label></li>";
            }
        }
        $(selector).prepend(listItems);
        if (allChecked) { checkAll(".rev-head-" + mdaId + ""); }
        scrollToElement("#rhList","#rhList > li:nth-child(1)");
        $("#profileloader").hide();
        $("#mda" + mdaId + "").click(function () {
            if (this.checked) {
                checkAll(".rev-head-" + mdaId + "");
            } else { unCheckAll(".rev-head-" + mdaId + ""); }
        });
        $(".rh").off("change");
        $(".rh").change(function () { if (event.currentTarget.checked) { $(this).attr("checked", true); } });
        if (checkAllRh && !allChecked && SelectedRhAndMdas[mdaId] == null) { $("#mda" + mdaId + "").attr("checked", true); checkAll(".rev-head-" + mdaId + ""); }
        if (SelectedRhAndMdas[mdaId] != null){ delete (SelectedRhAndMdas[mdaId]); }
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
            //if (rhArr.length >= rh.length && $(".default-rev-head-" + mda + "").find("input[type='checkbox']").attr("checked")) {
            //    rhArr = [0];
            //}
            if (rhArr.length > 0) { selectedRhAndMdaCollection.set(mda, rhArr); }
        });
        $("#selectedRhAndMdas").val(mapToJson(selectedRhAndMdaCollection));
    }

    function filterRhIds(rhList) {
        let filteredRhList = [];
        for (let i = 0; i < rhList.length; ++i) {
            filteredRhList.push(parseInt(rhList[i].value));
        }
        return filteredRhList;
    }

    function mapToJson(mapObj) {
        let m = {};
        mapObj.forEach(function (rh, mda) {
            m[mda] = rh;
        });
        return JSON.stringify(m);
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

    function removeRevHeads(mdaId, selector) {
        $(selector).remove(".default-rev-head-" + mdaId + "");
        $(selector).remove(".rev-head-" + mdaId + "");
    }

    function validateCheckBoxes() {
        $(".validation-message").hide();
        if ($(".payment-providers:checked").length < 1) {
            $("#paymentProviderContentListValidationMessage").show();
            $(document).scrollTop(40);
            return;
        }

        if ($(".payment-channels:checked").length < 1) {
            $("#paymentChannelContentListValidationMessage").show();
            $(document).scrollTop(40);
            return;
        }

        if ($(".mdas:checked").length < 1) {
            $("#mdaContentListValidationMessage").show();
            return;
        }

        if ($(".rh:checked").length < 1) {
            $("#rhContentListValidationMessage").show();
            return;
        }

        formIsValid = true;
    }
});