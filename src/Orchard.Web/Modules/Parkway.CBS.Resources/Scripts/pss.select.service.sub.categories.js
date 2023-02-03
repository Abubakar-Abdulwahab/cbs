$(document).ready(function () {
    var token = $("input[name=__RequestVerificationToken]").val();
    var subCategories = new Map();
    var subSubCategories = new Map();

    $('input[type=radio][name=TaxPayerType]').change(function () {
        emptySubCategories();
        showCategoryInfo("", false);
        if (subCategories.has(this.value)) {
            toggleCategoryField(true);
            showCategoryLoader(true);
            buildsubCategoryDataList(subCategories.get(this.value));
        } else {
            fetchCategoriesForTaxEntityType(this.value);
        }
    });

    $("#subCategoryData").change(function () {
        showSubCategoryInfo("", false);
        let subCategoryId = $("#subCategories option[value='" + this.value + "']").attr('data-value');
        if (subCategoryId != "0") {
            if (subSubCategories.has(subCategoryId)) {
                toggleSubCategoryField(true);
                showSubCategoryLoader(true);
                buildsubSubsubCategoryDataList(subSubCategories.get(subCategoryId));
            } else {
                fetchSubCategoriesForTaxEntityTypeSubCategory(subCategoryId);
            }
        }
    });

    $("#serviceData").change(function () {
        let selectedService = $("#services option[value='" + $("#serviceData").val() + "']").attr('id');
        let taxPayerType = $('input[type=radio][name=TaxPayerType]:checked').val();
        if (selectedService == undefined) { return; }
        if (selectedService.toString() == escortServiceType) {
            showSubCategoryField(true);
            fetchCategoriesForTaxEntityType(taxPayerType);
            $("#subCategoryData").prop("required", true);
            $("#subSubCategoryData").prop("required", true);
        } else {
            showSubCategoryField(false);
            showSubSubCategoryField(false);
            $("#subCategoryData").prop("required", false);
            $("#subSubCategoryData").prop("required", false);
        }
    });

    $("#selectServiceForm").submit(function (e) {
        e.preventDefault();
        let selectedService = $("#services option[value='" + $("#serviceData").val() + "']").attr('id');
        let subCategoryId = $("#subCategories option[value='" + $("#subCategoryData").val() + "']").attr('data-value');
        let subSubCategoryId = $("#subSubCategories option[value='" + $("#subSubCategoryData").val() + "']").attr('data-value');
        if (selectedService == undefined) { return; }
        if (selectedService.toString() == escortServiceType) {
            if (subCategoryId == undefined) { return; }
            if (subSubCategories.has(subCategoryId) && subSubCategoryId == undefined) { return; }
            if (!subSubCategories.has(subCategoryId)) { fetchSubCategoriesForTaxEntityTypeSubCategory(subCategoryId); }
            if (subSubCategories.has(subCategoryId) && subSubCategoryId == undefined) { return; }
        }
        
        $("#subCategoryIdentifier").val(subCategoryId);
        $("#subSubCategoryIdentifier").val(subSubCategoryId);
        this.submit();
    });

    function showCategoryLoader(show) {
        if (show) {
            $("#subCategoryContainer #profileloader").show();
        } else {
            $("#subCategoryContainer #profileloader").hide();
        }
    }

    function showSubCategoryLoader(show) {
        if (show) {
            $("#subSubCategoryContainer #profileloader").show();
        } else {
            $("#subSubCategoryContainer #profileloader").hide();
        }
    }

    function toggleCategoryField(disable) {
        if (disable) {
            $("#subCategoryData").prop("disabled", true);
        } else {
            $("#subCategoryData").prop("disabled", false);
        }
    }

    function showSubSubCategoryField(show){
        if (show) {
            $("#subSubCategoryContainer").show();
        } else {
            $("#subSubCategoryContainer").hide();
        }
    }

    function showSubCategoryField(show) {
        if (show) {
            $("#subCategoryContainer").show();
        } else {
            $("#subCategoryContainer").hide();
        }
    }

    function toggleSubCategoryField(disable) {
        if (disable) {
            $("#subSubCategoryData").prop("disabled", true);
        } else {
            $("#subSubCategoryData").prop("disabled", false);
        }
    }

    function showCategoryInfo(msg,show) {
        if (show) {
            $("#subCategoryInfo").html(msg);
            $("#subCategoryInfo").show();
        } else {
            $("#subCategoryInfo").hide();
        }
    }

    function showSubCategoryInfo(msg, show) {
        if (show) {
            $("#subSubCategoryInfo").html(msg);
            $("#subSubCategoryInfo").show();
        } else {
            $("#subSubCategoryInfo").hide();
        }
    }

    function fetchCategoriesForTaxEntityType(taxPayerId) {
        let url = "/p/x/get-pss-sub-categories";
        let args = { "categoryId": taxPayerId, "__RequestVerificationToken": token };
        toggleCategoryField(true);
        showCategoryLoader(true);
        $.post(url, args, function (response) {
            if (!response.Error) {
                subCategories.set(taxPayerId, response.ResponseObject);
                buildsubCategoryDataList(response.ResponseObject);
            } else {
                toggleCategoryField(false);
                showCategoryLoader(false);
                showCategoryInfo(response.ResponseObject,true);
            }
        }).fail(function () {
            toggleCategoryField(false);
            showCategoryLoader(false);
            showCategoryInfo("An error occurred. Please try again later.", true);
        });
    }

    function fetchSubCategoriesForTaxEntityTypeSubCategory(subCategoryId) {
        let url = "/p/x/get-pss-sub-Sub-categories";
        let args = { "subCategoryId": subCategoryId, "__RequestVerificationToken": token };
        toggleSubCategoryField(true);
        showSubCategoryLoader(true);
        $.post(url, args, function (response) {
            if (!response.Error) {
                if (response.ResponseObject.length > 0) {
                    showSubSubCategoryField(true);
                } else { showSubSubCategoryField(false); return;}
                subSubCategories.set(subCategoryId, response.ResponseObject);
                buildsubSubsubCategoryDataList(response.ResponseObject);
            } else {
                toggleSubCategoryField(false);
                showSubCategoryLoader(false);
                showSubCategoryInfo(response.ResponseObject, true);
            }
        }).fail(function () {
            toggleSubCategoryField(false);
            showSubCategoryLoader(false);
            showSubCategoryInfo("An error occurred. Please try again later.", true);
        });
    }

    function buildsubCategoryDataList(categories) {
        $("#subCategories").empty();
        for (var category in categories) {
            $('<option data-value="' + categories[category].Id + '" value="' + categories[category].Name + '"></option>').appendTo($("#subCategories"));
        }
        toggleCategoryField(false);
        showCategoryLoader(false);
    }

    function buildsubSubsubCategoryDataList(categories) {
        $("#subSubCategories").empty();
        showSubSubCategoryField(true);
        for (var category in categories) {
            $('<option data-value="' + categories[category].Id + '" value="' + categories[category].Name + '"></option>').appendTo($("#subSubCategories"));
        }
        toggleSubCategoryField(false);
        showSubCategoryLoader(false);
    }

    function emptySubCategories() {
        $("#subCategoryData").val("");
        $("#subSubCategoryData").val("");
    }
});