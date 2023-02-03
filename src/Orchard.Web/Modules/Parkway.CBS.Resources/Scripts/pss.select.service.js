$(document).ready(function () {
    disableCategories();
    var servicePerCategory = new Map();


    function clearAllFieldSelection() {
        $("#serviceData").val("");
        $("#subCategoryData").val("");
        $("#subSubCategoryContainer").hide();
        $("#subSubCategoryData").val("");
        $("#subCategoryContainer").hide();
    }

    $("#clearSelectFieldBtn").click(function () {
        clearAllFieldSelection();
    });

    $("#clearSubCatSelectFieldBtn").click(function () {
        $("#subCategoryData").val("");
        $("#subSubCategoryContainer").hide();
    });

    $("#clearSubSubCatSelectFieldBtn").click(function () {
        $("#subSubCategoryData").val("");
    });

    function disableCategories() {
        if (loggedIn) {
            $(':radio:not(:checked)').attr('disabled', true);
        }
    }

    function toggleInputs(enable) {
        if (enable) {
            $("#proceedBtn").prop("disabled", false);
            $("#serviceData").prop("disabled", false);
            $("input[type=radio]").attr('disabled', false);
        } else {
            $("#proceedBtn").prop("disabled", true);
            $("#serviceData").prop("disabled", true);
            $("input[type=radio]").attr('disabled', true);
        }
    }


    $("#selectServiceForm").on('submit', function (e) {
        var inputValue = $('#serviceData').val();
        var serviceValueIdentifier = $('#services option[value="' + inputValue + '"]').attr('data-value');
        $('#serviceIdentifier').val(serviceValueIdentifier);
        $('#taxCategory').val($('input[name=TaxPayerType]:checked').val());
        toggleInputs(false);
        return true;
    });

    function fetchServicesPerCategory(taxPayerId) {
        $("#categoryMsg").hide();
        $("#categoryMsg").html('');
        $("#catloader").show();

        if (servicePerCategory.has(taxPayerId)) {
            buildServiceList(servicePerCategory.get(taxPayerId));
        } else {
            $.post("/p/x/get-service-per-category", { "categoryId": taxPayerId, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() }, function (response) {
                if (!response.Error) {
                    servicePerCategory.set(taxPayerId, response.ResponseObject);
                    buildServiceList(response.ResponseObject);
                    $("#catloader").hide();
                } else {
                    $("#categoryMsg").show();
                    $("#categoryMsg").html(response.ResponseObject);
                    $("#catloader").hide();
                }
            }).fail(function () {
                $("#categoryMsg").show();
                $("#categoryMsg").html("Sorry something went wrong while processing your request. Please try again later or contact admin.");
                $("#catloader").hide();
            });
        }

    }

    function buildServiceList(services) {
        $("#services").empty();
        clearAllFieldSelection();
        for (var service in services) {
            $('<option data-value="' + services[service].Id + '" value="' + services[service].Name + '" id="' + services[service].ServiceType + '" ></option>').appendTo($("#services"));
        }
        $("#catloader").hide();
    }


    $('input[type=radio][name=TaxPayerType]').change(function () {
        if (!loggedIn) {
            clearAllFieldSelection();
            fetchServicesPerCategory(this.value);
        }
    });

});