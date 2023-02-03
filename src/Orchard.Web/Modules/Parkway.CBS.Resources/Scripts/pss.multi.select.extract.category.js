$(document).ready(function () {
    var extractSubcategoriesMap = new Map();
    var extractCategoryMultiSelectToggleState = true;
    var extractSubcategoriesPostbackObj = null;

    if (extractSubCategoriesPostback != null) {
        if ($("#selectedCategoriesAndSubCategories").val() != null && $("#selectedCategoriesAndSubCategories").val() != undefined && $("#selectedCategoriesAndSubCategories").val() != "") {
            extractSubcategoriesPostbackObj = JSON.parse($("#selectedCategoriesAndSubCategories").val());
            }
        $(extractSubCategoriesPostback).each(function (index, extractCategoryWithSubCategory) {
            extractSubcategoriesMap.set(parseInt(extractCategoryWithSubCategory.Id), extractCategoryWithSubCategory);
            buildMultiSelectDropdownForCategory(parseInt(extractCategoryWithSubCategory.Id));
        });
        extractSubcategoriesPostbackObj = null;
    }

    $("#extractCategoryMultiSelectDropdownToggle").click(function () {
        if (extractCategoryMultiSelectToggleState) {
            $("#extractCategoryMultiSelectDropdownMenu").slideUp("fast", "linear", function () {
                let availableCategories = $(".extract-category");
                if (availableCategories.length > 1) {
                    let labelText = availableCategories[0].nextSibling.data;
                    $("#extractCategoryMultiSelectDropdownToggle label").html("" + labelText + " and " + (availableCategories.length - 1) +" other");
                }
            });
        } else {
            $("#extractCategoryMultiSelectDropdownMenu").slideDown("fast", "linear", function () {
                $("#extractCategoryMultiSelectDropdownToggle label").html("Select a Category");
            });
        }
        extractCategoryMultiSelectToggleState = !extractCategoryMultiSelectToggleState;
    });

    $("input[type='checkbox'][name='SelectedCategories'].extract-category").change(function (e) {
        let categoryId = parseInt(e.currentTarget.value);
        if (e.currentTarget.checked) {
            if (extractSubcategoriesMap.has(categoryId)) {
                buildMultiSelectDropdownForCategory(categoryId);
            } else { getSubCategories(categoryId); }
        } else {
            if (extractSubcategoriesMap.get(categoryId).FreeForm) { toggleFreeForm(false); return; }
            removeMultiSelectDropdownForCategory(categoryId);
        }
    });

    $("#extractForm").submit(function (e) {
        e.preventDefault();
        $("#extractError").empty();
        compileSelectedCategories();
        if ($(".extract-category:checked").length < 1) {
            $("#extractError").html("<span class='field-validation-error tiny-caption' style = 'color:#ff0000'>category is required.</span >");
            $("html, body").animate({ scrollTop: $('#extractCategoryMultiSelectDropdownToggle').offset().top }, 500);
        }
        else { $("#extractForm").off("submit"); $("#extractForm").submit(); }
    });


    function getSubCategories(categoryId) {
        //event.preventDefault();
        $('#extractError').empty();
        disableAllCategories();
        $("#extractCategoriesloader").show();
        //do ajax call
        var url = 'x/get-extract-sub-categories';
        var requestData = { "categoryId": categoryId, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
        $.post(url, requestData, function (data) {
            if (!data.Error) {
                extractSubcategoriesMap.set(categoryId, data.ResponseObject);
                buildMultiSelectDropdownForCategory(categoryId);
            } else {
                $('#extractError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >' + data.ResponseObject + '</span > ');
            }

        }).fail(function () {
            $('#extractError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000">An error occurred try refreshing the page.</span > ');
        }).always(function () {
            enableAllCategories();
            $("#extractCategoriesloader").hide();
        });
    }


    function buildMultiSelectDropdownForCategory(categoryId) {
        let categoryObj = extractSubcategoriesMap.get(categoryId);
        let isPostback = (extractSubcategoriesPostbackObj != null && extractSubcategoriesPostbackObj[categoryId] != undefined) ? true : false;
        if (categoryObj.FreeForm) {
            toggleFreeForm(true);
            return;
        }
        let subCategoriesListMarkup = "";
        $(categoryObj.SubCategories).each(function (index, value) {
            if (isPostback) {
                if (extractSubcategoriesPostbackObj[categoryId].findIndex(function (val) { return val == value.Id; }) > -1) {
                    subCategoriesListMarkup += "<li class = 'sub-category-" + categoryId + "'><label class='form-check-label'><input type='checkbox' value='" + value.Id + "' name='SelectedSubCategories' class='form-check-input' checked />" + value.Name + "</label></li>";
                } else {
                    subCategoriesListMarkup += "<li class = 'sub-category-" + categoryId + "'><label class='form-check-label'><input type='checkbox' value='" + value.Id + "' name='SelectedSubCategories' class='form-check-input' />" + value.Name + "</label></li>";
                }
            } else {
                subCategoriesListMarkup += "<li class = 'sub-category-" + categoryId + "'><label class='form-check-label'><input type='checkbox' value='" + value.Id + "' name='SelectedSubCategories' class='form-check-input' />" + value.Name + "</label></li>";
            }
        });
        let subCategoryCollapsibleDropDownMarkup = "<div class='multi-select-collapsible-dropdown-container' id='multiSelectCollapsibleDropdownContainer" + categoryId + "'><div class='multi-select-dropdown-toggle' data-toggle='collapse' data-target='#multiSelectCollapseDiv" + categoryId + "'><label>" + categoryObj.Name + "</label><span class='multi-select-dropdown-toggle-inner-label' id='subCategoryInnerLabelCount" + categoryId + "'>"+categoryObj.SubCategories.length+" selected</span><span class='fa fa-caret-down multi-select-dropdown-caret'></span></div><div id='multiSelectCollapseDiv" + categoryId + "' class='multi-select-dropdown-collapsible-menu collapse show'><div class='multi-select-dropdown-collapsible-inner-menu'><div class='multi-select-collapsible-dropdown-menu-default-container'><ul><li class='sub-category-" + categoryId + "' ><label class='form-check-label'><input type='checkbox' value='0' class='form-check-input default-sub-category' />All("+categoryObj.SubCategories.length+")</label></li></ul></div><hr /><div class='multi-select-collapsible-dropdown-menu-list-container'><ul id='subCategoryList" + categoryId + "'></ul></div></div></div></div>";

        $("#subCategoryMultiSelectDropdownGroupContainer").prepend(subCategoryCollapsibleDropDownMarkup);
        $("#multiSelectCollapseDiv" + categoryId + " .multi-select-collapsible-dropdown-menu-list-container ul").append(subCategoriesListMarkup);
        $("#multiSelectCollapsibleDropdownContainer" + categoryId + " .default-sub-category").change(function (e) {
            if (e.currentTarget.checked) {
                checkAllForCategory(categoryId);
            } else { uncheckAllForCategory(categoryId); }
        });

        $(".sub-category-" + categoryId + " input[type='checkbox']").change(function (e) {
            updateSelectedItemsCountForCategory(categoryId);
            if (!e.currentTarget.checked) {
                if ($("#multiSelectCollapsibleDropdownContainer" + categoryId + " .default-sub-category").prop("checked")) {
                    $("#multiSelectCollapsibleDropdownContainer" + categoryId + " .default-sub-category").prop("checked", false);
                }
            }
        });

        //if (!isPostback) { checkAllForCategory(categoryId); }
        if (isPostback && (extractSubcategoriesPostbackObj[categoryId].findIndex(function (val) { return val == 0; }) > -1)) { checkAllForCategory(categoryId); }
        updateSelectedItemsCountForCategory(categoryId);
    }


    function removeMultiSelectDropdownForCategory(categoryId) { $("#multiSelectCollapsibleDropdownContainer" + categoryId + "").remove(); }

    function uncheckAllForCategory(categoryId) { $(".sub-category-" + categoryId + " input[type='checkbox']").prop("checked", false); }

    function checkAllForCategory(categoryId) { $(".sub-category-" + categoryId + " input[type='checkbox']").prop("checked", true); }

    function uncheckCategory(categoryId) { $("#extractCategoryCheck" + categoryId + "").prop("checked", false); }

    function disableAllCategories() { $(".extract-category").prop("disabled", true); }

    function enableAllCategories() { $(".extract-category").prop("disabled", false); }

    function toggleFreeForm(enable) {
        if (enable) { $("#reasonDiv").show(); $("#reasonDiv #reason").prop("required", true); }
        else { $("#reasonDiv").hide(); $("#reasonDiv #reason").prop("required", false); }
    }

    function updateSelectedItemsCountForCategory(categoryId) {
        let count = $("#multiSelectCollapseDiv" + categoryId + " .multi-select-collapsible-dropdown-menu-list-container ul .sub-category-" + categoryId + " input[type='checkbox']:checked").length;
        $("#subCategoryInnerLabelCount" + categoryId + "").html(""+count+" selected");
    }

    function mapToJson(mapObj) {
        let m = {};
        mapObj.forEach(function (rh, mda) {
            m[mda] = rh;
        });
        return JSON.stringify(m);
    }

    function compileSelectedCategories() {
        let compiledSelectedCategoriesAndSubCategoriesMap = new Map();
        let checkedCategories = $(".extract-category:checked");
        $(checkedCategories).each(function (index, checkedCategory) {
            let currentlyCheckedCategory = parseInt(checkedCategory.value);
            let checkedSubCategories = $(".sub-category-" + currentlyCheckedCategory + " input[type='checkbox']:checked");
            if (extractSubcategoriesMap.get(currentlyCheckedCategory) != undefined && extractSubcategoriesMap.get(currentlyCheckedCategory).FreeForm) { compiledSelectedCategoriesAndSubCategoriesMap.set(currentlyCheckedCategory, [0]); return; }
            if (checkedSubCategories == undefined || checkedSubCategories.length < 1) { removeMultiSelectDropdownForCategory(currentlyCheckedCategory); uncheckCategory(currentlyCheckedCategory); return; }
            compiledSelectedCategoriesAndSubCategoriesMap.set(currentlyCheckedCategory, []);
            $(checkedSubCategories).each(function (index, subCategory) {
                compiledSelectedCategoriesAndSubCategoriesMap.get(currentlyCheckedCategory).push(parseInt(subCategory.value));
            });
        })
        $("#selectedCategoriesAndSubCategories").val(mapToJson(compiledSelectedCategoriesAndSubCategoriesMap));
    }

});